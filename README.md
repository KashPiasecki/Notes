# Status #
![.Net](https://camo.githubusercontent.com/fa75219e71963a85f42da1649a890785d4e34c2fbb1a0136cc72098ca5df9e7a/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f56657273696f6e2d2e4e4554253230362e302d696e666f726d6174696f6e616c3f7374796c653d666c6174266c6f676f3d646f746e6574)
![Docker](https://camo.githubusercontent.com/b6cf3f3cd72b2b3af1f9656f7902845bb0c7adbbff83dbd52d57de9da8b912c7/68747470733a2f2f696d672e736869656c64732e696f2f62616467652f4275696c745f576974682d446f636b65722d696e666f726d6174696f6e616c3f7374796c653d666c6174266c6f676f3d646f636b6572)
[![.NET](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml/badge.svg?branch=main)](https://github.com/KashPiasecki/Notes/actions/workflows/ci.yaml)
[![Scheduled Tests Run](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml/badge.svg)](https://github.com/KashPiasecki/Notes/actions/workflows/periodical.yaml)
![CodeCoverage](https://img.shields.io/badge/CodeCoverage-92%25-green)

# About #
### This project was created to try out <b>.NET 6 StandardApi</b> clean architecture approach with various technologies. Technologies/frameworks: ###
* Serilog with Kibana logging platform ✅
* Docker contenerization ✅
* Postgres SQL with EntityFramework Core ORM ✅
* CQRS with MediatR ✅
* Pagination ✅
* Filtering ✅
* HealthChecks ✅
* FluentValidation ✅
* JWT Role Authorization ✅
* Unit tests with Any library ✅
* Integrated tests with AutoFixture library ✅
* Code coverage tool on pull requests ✅
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

* Normally I would add `.env` file to gitignore but for testing simplicity these secrets are already filled
* First user created will have admin role

Testing Pull Request