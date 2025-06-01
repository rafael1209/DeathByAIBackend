# 💸 DeathByAIBackend

![Project Banner](https://github.com/user-attachments/assets/49b4cf8e-0e34-4607-a902-14678045a94a)

**Death by AI Co-Founder** is an educational web platform built by the team **Diamond Studio** to teach school students and young adults the fundamentals of personal finance.
We make learning simple, fun, and engaging through interactive lessons, games, and real-life simulations.

---

## 🏁 Built at GreenHack 2025

This project was created in just **24 hours** as part of the [**GreenHack 2025**](https://greenhack.eu/) hackathon — a European innovation event focused on sustainability, technology, and social impact.

Despite the limited time, we built a fully working MVP featuring:

* ✅ AI-assisted personalized finance education
* ✅ Google OAuth user authentication
* ✅ Fully functional backend with MongoDB and JWT
* ✅ Interactive and responsive frontend using Nuxt3
* ✅ Deployment via Docker & Nginx
* ✅ Team collaboration and version control on GitHub

We are proud to have created a real, working product in just one day — ready to grow beyond the hackathon.

---

## 📚 Project Overview

**Death by AI Co-Founder** uses artificial intelligence to personalize financial education and adapt the experience to each user's level.
It combines practical scenarios with AI-driven feedback to make complex financial concepts accessible and engaging.

### ✨ Features

* Gamified personal finance lessons
* Interactive real-world simulations
* AI-driven personalization of content and learning curve
* User progress tracking and recommendations

---

## 🛠️ Tech Stack

* **Backend:** ASP.NET Core, MongoDB, OpenAI (ChatGPT), Google OAuth
* **Frontend:** Nuxt3, TailwindCSS, JavaScript
* **Deployment:** Docker & GitHub Container Registry

---

## 🚀 Setup & Installation

### `appsettings.json`

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

### `docker-compose.yml`

```yaml
version: '3.8'

services:
  death-by-ai-back:
    image: ghcr.io/rafael1209/deathbyaibackend:master
    container_name: death-by-ai-back
    expose:
      - "8080"
    environment:
      Jwt:SecretKey: "###########"
      Jwt:Issuer: "###########"
      MongoDb:ConnectionString: "###########"
      MongoDb:DatabaseName: "###########"
      Google:ClientId: "###########"
      Google:ClientSecret: "###########"
      Google:RedirectUri: "###########"
      ChatGpt:ConnectionString: "###########"
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

---

## 🔗 Links

* 🌐 Live App (Frontend): [greenhack-red.vercel.app](https://greenhack-red.vercel.app/)
* 💻 Backend Repo: [github.com/rafael1209/DeathByAIBackend](https://github.com/rafael1209/DeathByAIBackend)
* 🎨 Frontend Repo: [github.com/CkutlsGit/greenhack](https://github.com/CkutlsGit/greenhack)
* 🧠 Project Page (InnoPower): [submissions.innopower.me/project-detail/death-by-ai-co-founder\_t8](https://submissions.innopower.me/project-detail/death-by-ai-co-founder_t8)

---

## 🧩 Team – Diamond Studio

Meet the creators behind **Death by AI Co-Founder**:

| Name                | Role                         | GitHub                                       |
| ------------------- | ---------------------------- | -------------------------------------------- |
| **Danil Tkachenko** | Team Lead, Backend Developer | [@danilt2000](https://github.com/danilt2000) |
| **Rafael Chasman**  | Backend Developer            | [@rafael1209](https://github.com/rafael1209) |
| **CkutlsGit**       | Frontend Developer           | [@CkutlsGit](https://github.com/CkutlsGit)   |
| **FUpir**           | UI/UX Designer               | [@FUpir](https://github.com/FUpir)           |

We are **Diamond Studio**, a passionate and innovative team focused on creating engaging educational tools powered by modern technologies and AI.
