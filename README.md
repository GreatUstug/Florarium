# Florarium

## Запуск в Docker

Требуется Docker Desktop с Docker Compose.

```powershell
docker compose up --build
```

После запуска доступны:

- Swagger: http://localhost:8080/swagger
- Проверка API и PostgreSQL: http://localhost:8080/health

Для остановки контейнеров выполните:

```powershell
docker compose down
```
