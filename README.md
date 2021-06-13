# payment-gateway
Simple payment gateway application built in .NET 5

## Technologies used
* .NET v5 for the payment gateway

* MySQL for the database

* Docker-compose to run both database and app as docker containers

* Bank API runs in a separate port using the wiremock library. Port can be configured by
  changing the variable BankApi__Port in docker-compose.yml
## Running instructions
Docker and docker-compose are required in order to run this app successfully

### Running with docker-compose
The following commands need to be run:
```
docker-compose build
docker-compose up -d
dotnet ef database update --project .\src\PaymentGateway.Infrastructure\PaymentGateway.Infrastructure.csproj --startup-project .\src\PaymentGateway.Api\PaymentGateway.Api.csproj
```

If you prefer to run with local .NET, you can also do that by opening the project in your IDE and clicking Run.
This assumes that the database in running in docker-compose though

## Developed features

The following features were implemented
* Login as a shopper
* Create and get specific card
* Validate card
* Create payment
* Get single payment
* Get payments paginated

## Extra points
* Global exception handler
* Logging
* Containerization (using docker-compose and docker)
* CI for pull requests (using Github actions)
* Policy-based authorization with JWT (once logged in, shopper id goes in the token claims and is used for all requests)
* Mock server and API client (for bank service)

## Improvement points
As we can be talking about a large amount of events, we could consider switching the payment storage from a SQL database to document-store (for example elasticsearch).
By doing this, we would treat every payment as an event (single write, multiple reads) and could take advantage of all the features of ES

In order to check the performance of the application, monitoring could be added, so we would add metrics to measure memory allocation, request duration and others

Performance testing could also be an improvement point on this, to ensure not only the API's resilience to concurrent requests but also a high load of them at the same time

Depending on the results of performance tests, we could also consider rate limiting

