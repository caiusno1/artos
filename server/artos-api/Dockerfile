FROM node:12 as builder
WORKDIR /usr/src/api-server
COPY package*.json ./
RUN npm i
RUN npm ci
COPY . .
RUN npx tsc

FROM node:12
WORKDIR /usr/src/api-server
COPY --from=builder /usr/src/api-server/build ./
COPY package*.json ./
RUN npm install

EXPOSE 8000
CMD ["node","index.js"]