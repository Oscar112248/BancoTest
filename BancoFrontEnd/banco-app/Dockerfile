FROM node:20-alpine AS build

WORKDIR /app

COPY package*.json ./

RUN npm install -g @angular/cli
RUN npm install --legacy-peer-deps

COPY . /app/

RUN npm run build -- --configuration=production --project=banco-app

RUN ls /app/dist/project=banco-app

FROM nginx:stable-alpine
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*

COPY --from=build /app/dist/project=banco-app/browser .

RUN apk add --no-cache tzdata
ENV TZ=America/Guayaquil
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY ./default.conf /etc/nginx/conf.d/default.conf


EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
