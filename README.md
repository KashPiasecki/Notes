# About #
### This project was created to try out <b> .NET 6 StandardApi</b> clean architecture approach. Technologies/frameworks: ###
* Serilog
* Kibana logging platform
* Docker contenerization
* EntityFramework Core
* Postgres SQL
* CQRS with MediatR 
* JWT Role Authorization
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

1. Create file .env in the main folder and fill the file with secret values:
    ```
    db_user=<USERNAME>
    db_password=<PASSWORD>
    jwt_secret=<SECRET_64_CHAR>
    ```
2. Run `docker-compose up` to spin up
3. [Kibana](http://localhost:5601/app/home) set index pattern to `notes.api-*` with @timestamp
4. Try out:
    * [Swagger](http://localhost:2100/documentation/index.html)
    * [Kibana](http://localhost:5601/app/home)