<p align="center">
  <img src="src/assets/apps/default.png" width="100" alt="Logo" >
</p>

<h1 align="center">Trackster</h1>

<p align="center">
<a href="https://github.com/Citr0sCo/trackster/actions/workflows/build.yml"><img src="https://github.com/Citr0sCo/trackster/actions/workflows/build.yml/badge.svg" alt="Build"></a>
<a href="https://github.com/Citr0sCo/trackster/actions/workflows/deploy.yml"><img src="https://github.com/Citr0sCo/trackster/actions/workflows/deploy.yml/badge.svg" alt="Publish Docker image"></a>
<a href="https://hub.docker.com/r/citr0s/trackster"><img src="https://img.shields.io/docker/image-size/citr0s/trackster" alt="Docker Image Size"></a>
<a href="https://hub.docker.com/r/citr0s/trackster"><img src="https://img.shields.io/docker/pulls/citr0s/trackster" alt="Docker pulls"></a>
<a href="https://hub.docker.com/r/citr0s/trackster"><img src="https://img.shields.io/docker/v/citr0s/trackster?sort=semver" alt="Docker version"></a>
</p>

---

<h4 align="center">Trackster is a simple app that allows you to track your Plex watching habits.</h4>

---

> [!WARNING]
> The app is still actively in development. Only latest release is available.
> Once stable, a release tag will be made available.

---

<p align="center">
  <img src="src/assets/apps/example.png" width="100%" alt="Apps" >
</p>

---

## 🛠️ Installation

> [!NOTE]
> To run this application, you'll need [Docker](https://docs.docker.com/engine/install/) with [docker-compose](https://docs.docker.com/compose/install/).

Start off by showing some ❤️ and give this repo a star. Then from your command line:

```bash
# Create a new directory
> mkdir home-app
> cd home-app

# Create docker-compose.yml and copy the example contents into it
> touch docker-compose.yml
> nano docker-compose.yml
```

### docker-compose.yml

```yml
services:
  trackster:
    image: citr0s/trackster
    ports:
      - '83:80'
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
    volumes:
      - ./assets:/web-api/app/assets

```

---

## 💡 Feature request?

For any feedback, help or feature requests, please [open a new issue](https://github.com/citr0s/trackster/issues/new/choose).
Before you do, please read [the wiki](https://github.com/citr0s/trackster/wiki). The question you have might be answered over there.