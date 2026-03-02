using Marketplace.Messaging.Outbox;
using Marketplace.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payments.Infrastructure.Persistence;

namespace Payments.Infrastructure.Outbox;

public sealed class OutboxPublisherWorker : BackgroundService
{
    private const int MaxAttempts = 20;

    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxPublisherWorker> _log;
    private readonly RabbitMqOptions _rmq;

    public OutboxPublisherWorker(IServiceProvider sp, IOptions<RabbitMqOptions> rmq, ILogger<OutboxPublisherWorker> log)
    {
        _sp = sp;
        _log = log;
        _rmq = rmq.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("Payments OutboxPublisherWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
                var now = DateTimeOffset.UtcNow;

                var batch = await db.OutboxMessages
                    .Where(x => x.Status == OutboxMessageStatus.Pending)
                    .Where(x => x.AttemptCount < MaxAttempts)
                    .Where(x => x.NextAttemptAt == null || x.NextAttemptAt <= now)
                    .OrderBy(x => x.OccurredAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                using var publisher = new RabbitMqPublisher(_rmq);

                foreach (var msg in batch)
                {
                    try
                    {
                        publisher.Publish(msg.Exchange, msg.RoutingKey, msg.PayloadJson, msg.Id);

                        msg.Status = OutboxMessageStatus.Published;
                        msg.PublishedAt = DateTimeOffset.UtcNow;
                        msg.LastError = null;
                        msg.NextAttemptAt = null;
                    }
                    catch (Exception ex)
                    {
                        msg.AttemptCount++;
                        msg.LastError = ex.Message;

                        if (msg.AttemptCount >= MaxAttempts)
                        {
                            msg.Status = OutboxMessageStatus.Failed;
                        }
                        else
                        {
                            msg.Status = OutboxMessageStatus.Pending;
                            msg.NextAttemptAt = DateTimeOffset.UtcNow.Add(Backoff(msg.AttemptCount));
                        }

                        _log.LogError(ex, "Failed to publish payment outbox message {Id} (attempt {Attempt})", msg.Id, msg.AttemptCount);
                    }
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Payments OutboxPublisherWorker loop error.");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private static TimeSpan Backoff(int attempt)
    {
        var cappedAttempt = Math.Min(attempt, 10);
        var seconds = Math.Min(60, Math.Pow(2, cappedAttempt));
        return TimeSpan.FromSeconds(seconds);
    }
}
