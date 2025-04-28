# Product Catalog API

## Overview

This is a RESTful API for managing a product catalog, built with ASP.NET Core. The API allows users to perform CRUD operations on products by using the Command Query Responsibility Segregation (CQRS) pattern combined with MediatR for cleaner architecture and better separation of concerns.

## Features

- Retrieve a list of products
- Retrieve a specific product by ID
- Add a new product
- Implemented logging using Serilog
- Asynchronous methods for enhanced performance
- Caching for frequently accessed data for improved response times

## Prerequisites

- .NET 8 SDK or higher
- SQL Server (local instance or accessible server)
- (Optional) Visual Studio 2022 or later for development

## Setup Instructions

### 1. Clone the Repository

Clone this repository to your local machine using:

   ```bash
   git clone https://github.com/phil-dev-0315/ProductCatalogApi.git
   cd ProductCatalogApi
```
### 2. Restore Dependencies

Restore the project dependencies using the following command:

```bash
   dotnet restore
```
### 3. Configure the Database

- Ensure your SQL Server is running and accessible.
- Create a database for the project, e.g., ProductCatalog.

### 4. Update appsettings.Development.json

Update your connection string in src/appsettings.Development.json to match your SQL Server configuration:

```json
  {
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductCatalog;User Id=your_user;Password=your_password;"
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```
### 5. Run Migrations

To create the necessary tables in your database, run the following command in the project directory:

  ```bash
  dotnet ef database update
```
### 6. Build and Run the Application

#### Using Visual Studio

  * Open the solution in Visual Studio.
  * Set the `ProductCatalogApi` project as the startup project.
  * Press **F5** to build and run the application.

#### Using Command Line
  ```bash
  dotnet build
  dotnet run
  ```
Navigate to http://localhost:5000/swagger in your browser to view the Swagger UI and test the API endpoints.
You can also use Postman or other alternatives to test the api locally.

## Architectural Design Decisions

  * **Use of Entity Framework Core**: Chosen to simplify data access with robust ORM capabilities, making it easier to manage and query databases.

  * **CQRS Pattern**: Utilized to separate data modification (commands) and data retrieval (queries), enhancing clear responsibility and potential scalability.

  * **Service Layer**: Maintained to encapsulate business logic, ensuring reusable services and a clear separation from data access logic.
    
  * **Repository Pattern**: Implemented to abstract the data access layer, enabling easier changes to the data source without impacting business logic.
  
## Unit Testing

### Overview

The unit test project utilizes [xUnit](https://xunit.net/) as the testing framework, coupled with [Moq](https://github.com/moq/moq4) for mocking dependencies to facilitate isolated testing of components. This ensures robust verification of the application's functionality and promotes better code maintainability.

### Coverage

Test cases are implemented across various areas, including:
- **Service Layer Logic**: Tests for business logic related to product retrieval, creation, updating, and deletion.
- **Controller Actions**: Ensuring that API endpoints handle requests and responses correctly.

### Setup Instructions

#### 1. Navigate to Test Project Directory

Use the command line to navigate to the test project directory:

   ```bash
   cd ProductCatalogApi.Tests
   ```
### 2. Restore Dependencies

Restore the project dependencies using the following command:

```bash
   dotnet restore
```

### 3. Build the Test Project

Before running your tests, ensure that the solution (including your main project) builds correctly:

   ```bash
   dotnet build
   ```

### 3 Running Tests

Unit tests can be executed using the command line or through an IDE:

* **Command Line**: Run all tests in the project with the following command:

   ```bash
   dotnet test
   ```

* **Visual Studio**: Open Visual Studio and ensure your solution is loaded.
   
  * Open **Test Explorer** from the menu (Test -> Test Explorer).
  * Click **Run All** to execute all tests visible in the Test Explorer.

### Sample Test Cases

Examples of test cases included in the test project:

* `GetAllProducts_ReturnsOkResult`: Tests that all products are retrieved successfully.
* `GetProductByIdAsync_ExistingId_ReturnsOkResult`: Tests that retrieving a product by an existing ID returns the correct product.
* `AddProductAsync_ValidProduct_ReturnsCreatedResult`: Tests that adding a valid product returns a created status.
* `AddProductAsync_InvalidProduct_ReturnsBadRequest`: Tests that invalid product data returns a bad request response.
