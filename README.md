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

1. Create file .env in the main folder.
2. Fill the file with secret values:
    ```
    db_user=<USERNAME>
    db_password=<PASSWORD>
    jwt_secret=<SECRET_64_CHAR>
    ```
3. Run docker-compose up to spin up the db and service
4. Try out on 
    ```
    http://localhost:2100/documentation/index.html
    ```