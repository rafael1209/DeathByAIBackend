# DeathByAIBackend

`appsettings.json`
```json
{
  "Google": {
    "ClientId": "#################################",
    "ClientSecret": "#######################################",
    "RedirectUri": "##########################"
  },
  "MongoDb": {
    "ConnectionString": "mongodb://###############",
    "DatabaseName": "death-by-ai-bd"
  },
  "Jwt": {
    "SecretKey": "#############################",
    "Issuer": "mr.rafaello"
  },
  "ChatGpt": {
    "ConnectionString": "sk-##############################################"
  }
}
```

`docker-compose.yml`
```yml
version: '3.8'

services:
  death-by-ai-back:
    image: ghcr.io/rafael1209/deathbyaibackend:master
    container_name: death-by-ai-back
    expose:
      - "8080"    
    environment:
      Jwt:SecretKey: "###########"
      Jwt:Issuer: ""###########""
      MongoDb:ConnectionString: ""###########""
      MongoDb:DatabaseName: ""###########""
      Google:ClientId: ""###########""
      Google:ClientSecret: ""###########""
      Google:RedirectUri: ""###########""
      ChatGpt:ConnectionString: ""###########""
    networks:
      - app-network
      - mongo-network
  
  mongodb:
    image: mongo:latest
    container_name: mongodb
    environment:
      MONGO_INITDB_ROOT_USERNAME: #######
      MONGO_INITDB_ROOT_PASSWORD: #######
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - ./mongo-data:/data/db
    networks:
      - mongo-network
  
  nginx:
    image: nginx:alpine
    container_name: nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx-conf:/etc/nginx/conf.d
    networks:
      - app-network
      
networks:
  app-network:
    driver: bridge
  mongo-network:
    driver: bridge
```
