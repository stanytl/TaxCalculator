# Income Tax Calculator

## Overview
A UK income tax calculator built with C# and .NET 9. The solution includes:
- A RESTful API for tax calculation
- A simple HTML/JavaScript frontend for user input and displaying results

## Features
- Progressive tax calculation using configurable tax bands
- API endpoint for tax calculation
- Web UI for salary entry and results
- Unit tests for business logic

## How to Run
1. Clone the repository: git clone https://github.com/stanytl/TaxCalculator.git
2. Open the solution in Visual Studio 2022.
3. Build and run the `TaxCalculator.Api` project.
4. Open `TaxCalculator.Web\wwwroot\index.html` in your browser.
5. Enter a gross annual salary and click "Calc" to see results.

## Technologies & Libraries
- .NET 9, C# 13
- ASP.NET Core Web API
- MediatR (for CQRS and request handling)
- Entity Framework Core with SQLite (for data access)
- MSTest & Moq (for unit testing)

## Testing
Run tests using Visual Studio Test Explorer or: dotnet test


## Notes
- Tax bands are stored in the database and can be configured.
- The solution follows SOLID principles and uses dependency injection.
