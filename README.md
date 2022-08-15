# About #
### This project was created to try out many different technologies and <b>StandardApi</b> approach with <b>.NET 6</b>. Planning to use: ###
* Serilog
* Kibana/Seq for logging platform
* Docker contenerization
* EntityFramework Core
* Postgres SQL
* CQRS with MediatR 
* JWT Authorization
* Roles, Claims
* Unit tests
* Integrated tests
* Automapper
* FluentValidation
* Swagger
* Redis for cache
* Pagination
* HealthChecks
* Filtering
* CI/CD with GitHub actions
# How to run # 
Create 2 files in the main directory with content to run Postgres in docker-compose. Schema: filename-content
* `postgres_password.txt-<YOUR_PASSWORD>`
* `postgres_user.txt-<YOUR_USERNAME>`

Afterwards run in cmd $docker-compose up