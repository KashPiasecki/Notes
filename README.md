# About #
### This project was created to try out many different technologies and new <b>StandardApi</b> approach with <b>.NET 6</b>. Planning to use: ###
* Serilog
* Kibana/Seq for logging mechanism
* Docker contenerization
* EntityFramework Core
* Postgres SQL
* JWT Authorization
* Roles,Claims
* Docker contenerization
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
Need to create 2 files in the main directory with content to run Postgres in docker-compose. Schema: filename-content
* `postgres_password.txt-<YOUR_PASSWORD>`
* `postgres_user.txt-<YOUR_USERNAME>`

Afterwards run $docker-compose up