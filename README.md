# Status #
[![.NET](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml/badge.svg?branch=main)](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml)
[![Scheduled Tests Run](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml/badge.svg)](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml)
![CodeCoverage](https://img.shields.io/badge/CodeCoverage-96%25-green)

# About #
### This project was created to try out <b> .NET 6 StandardApi</b> clean architecture approach. Technologies/frameworks: ###
* Serilog with Kibana logging platform ✅
* Docker contenerization ✅
* Postgres SQL with EntityFramework Core ORM✅
* CQRS with MediatR ✅
* Pagination ✅
* Filtering ✅
* HealthChecks ✅
* FluentValidation ✅
* JWT Role Authorization ✅
* Unit tests with Any library ✅
* Integrated tests with AutoFixture library✅
* Automapper for DTO mappings ✅
* Swagger Documentation ✅
* Redis for cache✅
* CI with GitHub actions ✅

# How to run # 

1. Run `docker-compose up` to spin up
2. In [Kibana](http://localhost:5601/app/home) set index pattern to `notes.api-*` with @timestamp
3. Try out:
    * [Swagger](http://localhost:2100/documentation/index.html)
    * [Kibana](http://localhost:5601/app/home)

# Notes From Author # 

* Normally I would add `.env` file to gitignore but for running simplicity I filled these secrets
* First user created will have admin role