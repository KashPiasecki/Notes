# About #
### This project was created to try out <b> .NET 6 StandardApi</b> clean architecture approach. Technologies/frameworks: ###
* Serilog <span style="color:lightgreen"> ✓</span> 
* Kibana logging platform <span style="color:lightgreen"> ✓</span> 
* Docker contenerization <span style="color:lightgreen"> ✓</span> 
* EntityFramework Core <span style="color:lightgreen"> ✓</span> 
* Postgres SQL <span style="color:lightgreen"> ✓</span> 
* CQRS with MediatR <span style="color:lightgreen"> ✓</span> 
* JWT Role Authorization <span style="color:lightgreen"> ✓</span> 
* Unit tests
* Integrated tests <span style="color:lightgreen"> ✓</span> 
* Automapper <span style="color:lightgreen"> ✓</span> 
* FluentValidation <span style="color:lightgreen"> ✓</span> 
* Swagger Documentation <span style="color:lightgreen"> ✓</span> 
* Redis for cache <span style="color:lightgreen"> ✓</span>
* Pagination <span style="color:lightgreen"> ✓</span>
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
3. In [Kibana](http://localhost:5601/app/home) set index pattern to `notes.api-*` with @timestamp
4. Try out:
    * [Swagger](http://localhost:2100/documentation/index.html)
    * [Kibana](http://localhost:5601/app/home)
5. First user created will be user with admin role