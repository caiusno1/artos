FROM node:12 as builder
WORKDIR /usr/src/api-server
COPY package*.json ./
RUN npm install
COPY . .
RUN npm install typescript -g
RUN tsc

FROM node:12
WORKDIR /usr/src/api-server
COPY --from=builder /usr/src/api-server/build ./
COPY package*.json ./
RUN npm install

EXPOSE 8000
CMD ["node","index.js"]