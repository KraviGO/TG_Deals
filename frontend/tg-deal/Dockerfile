# Этап сборки React/Vite приложения.
FROM node:20-alpine AS build

WORKDIR /app

# Сначала копируем package-файлы, чтобы кешировать npm install.
COPY package.json package-lock.json ./

# Устанавливаем зависимости строго по package-lock.
RUN npm ci

# Затем копируем исходники приложения.
COPY . .

# Базовый URL API подставляется на этапе сборки frontend.
ARG VITE_API_GATEWAY_URL
ENV VITE_API_GATEWAY_URL=$VITE_API_GATEWAY_URL

# Username Telegram-бота нужен только для подсказки пользователю в интерфейсе.
ARG VITE_TELEGRAM_BOT_USERNAME
ENV VITE_TELEGRAM_BOT_USERNAME=$VITE_TELEGRAM_BOT_USERNAME

# Собираем production-версию.
RUN npm run build

# Этап раздачи готовой статики через nginx.
FROM nginx:alpine

# Подключаем nginx-конфиг для SPA fallback и кеширования ассетов.
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Копируем собранные файлы из build-этапа.
COPY --from=build /app/dist /usr/share/nginx/html

# В контейнере nginx слушает порт 80.
EXPOSE 80

# Запускаем nginx в foreground-режиме.
CMD ["nginx", "-g", "daemon off;"]
