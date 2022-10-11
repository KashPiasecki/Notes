# Status #
[![.NET](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml/badge.svg?branch=main)](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml)

[![Scheduled Tests Run](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml/badge.svg)](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml)

# About #
### This project was created to try out <b> .NET 6 StandardApi</b> clean architecture approach. Technologies/frameworks: ###
* Serilog ✅
* Kibana logging platform ✅
* Docker contenerization ✅
* EntityFramework Core ✅
* Postgres SQL ✅
* CQRS with MediatR ✅
* JWT Role Authorization ✅
* Unit tests
* Integrated tests ✅
* Automapper ✅
* FluentValidation ✅
* Swagger Documentation ✅
* Redis for cache ✅
* Pagination ✅
* HealthChecks ✅
* Filtering ✅
* CI with GitHub actions ✅

# How to run # 

1. Create file .env in the main folder and fill the file with secret values:
    ```
    db_user=<USERNAME>
    db_password=<PASSWORD>
    jwt_secret=<SECRET_64_CHAR>
    ```
2. Run `docker-compose up` to spin up
3. In [Kibana](http://localhost:5601/app/home) set index pattern to `notes.api-*` with @timestamp
4. Try out:
    * [Swagger](http://localhost:2100/documentation/index.html)
    * [Kibana](http://localhost:5601/app/home)
5. First user created will be user with admin role