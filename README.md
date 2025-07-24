# Example Team and Players API
This is an old example project that I had as a private repository and I decided to make it public.
This serves as a nice example for a clean API implementation for whoever wants to learn how to do this. 

<ins>Build status</ins>: ![Build Status](https://github.com/alex-pricope/example-sports-api/actions/workflows/ci.yml/badge.svg)

## Example web api to manage a team of players
The solution should allow us to manage the players for a team
- Get single player data (name, height, age)
- Update a single player
- Get all players from a team
- Add a new player to a team
- Remove a player from a team

## Description 
My solution consists of a _web api_ having two controllers:
* A `Team` endpoint that allows us to manage teams
  * Add a new (empty) team
  * Update team details
  * Get a team, with or without players
* A `Player` endpoint that allows us to manage the players
  * Add a new player to a team
  * Update individual player properties
  * Get a player (with the team details)
  * Delete a player
* A `Health` endpoint to be used for a possible Kubernetes alive check
 
Also, the `TeamController` expose a `DELETE` endpoint to help clean data. 

## The API
I designed the API to be self-explanatory and intuitive and has swagger enabled. \
Each endpoint describes exactly what status codes return and what are the types of request/response.\
<ins>Swagger endpoints</ins>
![image](https://github.com/user-attachments/assets/6a1d872e-558f-487e-a6d4-a5c0e48a7941)

## Project structure
The project structure is quite simple, it has a couple of extra libraries
* Domain - where I added all the interfaces (abstractions), models, entities, etc
* Services - where I added the implementation for two services the API uses. These are here to avoid writing too much code inside the controllers and keep them basic. 
  * `TeamManagementService` - encapsulates the logic for managing teams
  * `PlayerManagementService` - encapsulates the logic for managing players
* Data - where I added the implementation for repositories: `TeamRepository` / `PlayerRepository` (repositories that deal with data)
* Tests - I have 2 projects, one for `unit tests` and one for `integration tests`

The API also has:
* An error middleware where we can map errors and status codes. I also use this for a generic catch-all that returns `500`, so there is no need to handle different error types in the controllers.
* A logger
* Versioning
* Separation of endpoints (team and players)
* Integration tests for all endpoints
* Good documentation, swagger docs
* Good DTO separation

## Data store
I used `EF core` with `localdb SqlLite` - this is great for a rapid start without the need for a more complicated setup.\
When running the API first time, a new `localdatabase.db` will be created together with the tables - this can be seen in the terminal.\
There are two table entities
* A `Team` that has multiple `Player` children
* A `Player` that has one parent `Team`
* Both entities have their properties and `auto-incremented Id` (long): _PlayerEntity_ / _TeamEntity_

The _ApplicationDbContext_ is responsible for set up the two entities and exposing the `DbSets`.\
Two repositories manage the CRUD operations: _PlayerRepository_ / _TeamRepository_\
 
## Libraries
I used the following libraries
* NUnit / Moq / FluentAssertions
* EF Core / SqlLite

## Testing
There are two types of tests present:
* Unit tests in _MyCompany.Services.Tests_ 
* Integration tests in _MyCompany.Integration.Tests_ - this contains integration tests for the endpoints
  * These tests need the API to run on `5054` (check the below command)

<ins>How to run the integration tests locally</ins>:
```bash
# First Window - run the API on 5054. Navigate to the MyCompany.Api folder
dotnet run --urls "http://localhost:5054"
# Second window - execute the tests. Navigate to the MyCompany.Integration.Tests folder
dotnet test
```

## CI
This repo has a `Github Actions` that will run the unit tests and integration for each push to `main`. This can easily be extended to automate more steps. 
See the pipeline at: https://github.com/alex-pricope/example-sports-api/actions
![image](https://github.com/user-attachments/assets/763a7290-d8c1-428e-b77a-8b5b4d160dcc)


## Things to consider
* This is just an example on how to implement an API so it is very limited
* The repository is not concurrent - to do this would mean much extra work: retries, handling specific exceptions
* Not all code is unit tested
