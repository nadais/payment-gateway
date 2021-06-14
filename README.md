# Payment Gateway
Simple payment gateway application built in .NET 5

## Technologies used
* .NET 5 for the payment gateway backend

* MySQL for the database

* Docker-compose to run both database and app as docker containers

* Bank API runs in a separate port using the wiremock library. Port can be configured by
  changing the variable BankApi__Port in docker-compose.yml
## Running instructions
Docker and docker-compose are required in order to run this app successfully

### Running with docker compose
The following commands need to be run:
```
docker compose build
docker compose up -d
dotnet ef database update --project .\src\PaymentGateway.Infrastructure\PaymentGateway.Infrastructure.csproj --startup-project .\src\PaymentGateway.Api\PaymentGateway.Api.csproj
```


Once this is running, you can find the API swagger page in your browser by navigating to http://localhost:5001/swagger.

You can also ensure that the API is connected to the database by triggering the healthcheck in http://localhost:5001/health. A response with "Healthy" should be obtained

While the database has to run in docker compose, you can opt for running the .NET app locally (via dotnet run or your editor of choice).
If you opt for this, the port used will be 7001 instead of 5001. All other links still apply

Note: Pay attention to use http in your URLs for the docker compose version (meaning http://localhost:5001), as https will not redirect to it

## Execution flow

1. Login as a shopper (/shoppers/{id}/login). You can use a generator of GUID like https://www.uuidgenerator.net to get a valid shopper id
2. Once the token is obtained, copy it into the authorize section in swagger page, in the format `Bearer [your_token]`
   
Now you're authenticated! Explore at your will!

### Bank Service
The bank service runs on a separate port (localhost:12345) without swagger environment. You can use POSTman
or any other REST client of your choice to test requests to it
* Endpoint: POST /transfer
* Body: `{"fromCard": [Card(same structure as create payment card object)], "currency": "string", "amount":"decimal", "toAccountId": "Guid" }`

The response will be successful as long as the request has the properties currency and amount
## Developed features

The following features were implemented
* Login as a shopper
* Get card by id and card number
* Create payment
* Get single payment (filtered per shopper)
* Get payments paginated (filtered per shopper)

## Development considerations and decisions
* Decided not to include Luhn's algorithm for checking credit card as fluent validation's credit card checker
already does the same, which would make the code redundant
* The shopper id is included in the token, as this enforces that not only payments can only be created for
a specific shopper, but also that a shopper cannot have access to payments of a different shopper, allowing for possible multi-tenancy
* Although payments are specific per shopper, cards are not. If the same merchant uses the same card on two shoppers, the card is only created once.
This ensures not only less data repetition but also an easier path to a GDPR request, as all payments done for all shoppers by the same merchant would point to the same card

## Extra points
* Global exception handler
* Logging
* Containerization (using docker)
* CI for pull requests (using Github actions)
* Policy-based authorization with JWT (once logged in, shopper id goes in the token claims and is used for all requests)
* Mock server and API client (for bank service)
* Health checks
* Concurrency and double payment checks (in case the user send the same request with the same timestamp twice).
Checks for both currently in progress or already processed

## Improvement points
As we can be talking about a large amount of events, we could consider switching the payment storage from a SQL database to document-store (for example elasticsearch).
By doing this, we would treat every payment as an event (single write, multiple reads) and could take advantage of all the features of ES

In order to check the performance of the application, monitoring could be added, so we would add metrics to measure memory allocation, request duration and others

Performance testing could also be an improvement point on this, to ensure not only the API's resilience to concurrent requests but also a high load of them at the same time

Depending on the results of performance tests, we could also consider rate limiting

