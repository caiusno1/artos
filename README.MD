## Welcome to ARTOS [![Unity Builds](https://github.com/caiusno1/artos/actions/workflows/build-artos-toj_apps.yml/badge.svg)](https://github.com/caiusno1/artos/actions/workflows/build-artos-toj_apps.yml)
THE Augmented Reality Toolsuite for Online Studies (at least if you love bayesian data analysis).

### Installation Requirements
- Docker
- create an .env (docker-compose environment variables) with the following variables:
  - DB_PASSWD
  - DB_NAME
  - DB_TYPE (only tested with mysql)
  - DB_PORT
  - DB_USER (only tested with root)
  - MODE (preview, development, production)
  - DOMAIN
- in artos-api/config/ you need to create *config.json* with content 
  ```json
  {
    "passkey":"<API-KEY>",
    "jwt-secret":"<The secrete to be used by JWT>",
    "adminPassword":"<The admin passwort that you wish to use>"
  }
  ```
  you need to replace the rectangular brackets for sure!
- in the *docker-up.sh* you have to add the password you want to use for the database (but that is only required for development)

### Thinks you need to understand to use this toolsuite
- a bit of python programming knowlegde is needed (If you speak german I can recommend: https://www.youtube.com/watch?v=DiidEp6DqCo&list=PLNmsVeXQZj7q0ao69AIogD94oBgp3E9Zs)
- a basic understanding of bayesian data analysis (If you want to dig deeper I can recommend: Kruschke, J. (2014). Doing Bayesian data analysis: A tutorial with R, JAGS, and Stan.)
- a bit of knowlegde how to use jupyter could be helpful
