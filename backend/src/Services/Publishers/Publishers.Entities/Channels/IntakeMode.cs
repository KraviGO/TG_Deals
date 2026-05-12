namespace Publishers.Entities.Channels;

/// <summary>
/// Режим приема сделок по каналу.
/// </summary>
public enum IntakeMode
{
    /// <summary>Сделки принимаются и публикуются автоматически.</summary>
    ActiveAuto = 1,

    /// <summary>Паблишер вручную принимает или отклоняет сделки.</summary>
    ActiveManual = 2,

    /// <summary>Канал не принимает новые сделки.</summary>
    Paused = 3
}
