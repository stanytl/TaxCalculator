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

1. Clone the repository: `git clone https://github.com/stanytl/TaxCalculator.git`
2. Open the solution in Visual Studio 2022.
3. Build the solution.
4. Start the API project:
    - Right-click `TaxCalculator.Api` and select __Debug > Start New Instance__ or press __F5__ to run with debugging.
5. Start the web project:
    - Right-click `TaxCalculator.Web` and select __Debug > Start New Instance__ to launch the web frontend in your browser.
6. Use the web UI to enter a gross annual salary and click "Calc" to see results.

**Note:**  
To run both projects simultaneously, use __Start New Instance__ for each. If you attempt to start a second project using __View in Browser__ while another is already running in debug mode, Visual Studio will prompt "Do you wish to stop debugging?"—this can be avoided by starting a new instance for each project.

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
