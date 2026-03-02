# Marketplace MVP Docker Compose

Сборка локального окружения, отражающего текущее состояние сервисов:

## Хостинг
- Postgres `marketplace-postgres` (5432)
- RabbitMQ `marketplace-rabbitmq` (5672/15672)
- Identity `/swagger` (5001)
- Publishers `/swagger` (5002)
- ChannelCatalog `/swagger` (5003)
- Deals `/swagger` (5004)
- Payments `/swagger` (5005)

## Шаги запуска
```bash
cp .env.example .env
docker compose up -d --build
```

## Проверки
- `docker compose ps`
- Swagger каждого сервиса (см. порты выше)
- `GET http://localhost:<port>/health`
- RabbitMQ UI: http://localhost:15672 (guest/guest)

## Авто миграции
В каждом Host вставлена Dev-логика: при старте в `Development` выполняется `db.Database.Migrate()`. Это позволяет compose поднимать сервисы без внешнего миграционного шага.

## Структура файлов
- `docker-compose.yml` — описывает сервисы, порты, переменные окружения.
- `.env.example` — шаблон переменных, копируй в `.env`.
