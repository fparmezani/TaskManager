FROM node:22-alpine AS build
WORKDIR /app
COPY frontend/taskmanager-web/package*.json ./
RUN npm install
COPY frontend/taskmanager-web/ ./
RUN npm run build

FROM nginx:1.27-alpine
COPY frontend/taskmanager-web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/taskmanager-web/browser /usr/share/nginx/html
EXPOSE 80
