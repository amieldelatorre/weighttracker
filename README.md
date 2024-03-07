# WeightTracker

## Running locally
```bash
docker compose -f ./compose-local.yml up --build -d
docker compose -f ./compose-local.yml down
```

## Improvements
- [ ] Add a not found html file
- [ ] Suppress Entity Framework Logs query logging
- [ ] Use Terraform to deploy (not going to be within pipeline to keep deployment hidden)
- [ ] Separate the backend implementation to a Layered Architecture. With the following a Application Presentation Layer, Application / Business Access Layer, and a Data Access Layer.
  - Felt that the controllers were doing too much work and were expanding in responsibility. It should have only been in charge of presenting the data back to the client (frontend).
  - To have a clear separation of concern where one layer is in charge of presenting the data back to the client, another with the business logic of what is allowed and what isn't, and finally a layer for just data access