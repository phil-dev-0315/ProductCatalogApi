# Product Catalog API

## Overview

This is a simple RESTful API for managing a product catalog, built with ASP.NET Core. The API allows users to perform CRUD operations on products. Originally designed to be containerized using Docker, it has been adjusted to run without Docker for local development.

## Features

- Retrieve a list of products
- Retrieve a specific product by ID
- Add a new product
- Built-in Swagger UI for easy API testing

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
### 2. Configure the Database

- Ensure your SQL Server is running and accessible.
- Create a database for the project, e.g., ProductCatalog.

### 3. Update appsettings.Development.json

Update your connection string in src/appsettings.Development.json to match your SQL Server configuration:

```json
  {
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProductCatalog;User Id=your_user;Password=your_password;"
  }
}
```
### 4. Run Migrations

To create the necessary tables in your database, run the following command in the project directory:

  ```bash
  dotnet ef database update
```
### 5. Build and Run the Application

#### Using Visual Studio

  * Open the solution in Visual Studio.
  * Set the ProductCatalogApi project as the startup project.
  * Press F5 to build and run the application.

#### Using Command Line
  ```bash
  dotnet build
  dotnet run
  ```
Navigate to http://localhost:5000/swagger in your browser to view the Swagger UI and test the API endpoints.
You can also use Postman or other alternatives to test the api locally.

## Design Decisions

  * **Use of Entity Framework Core**: Chosen to simplify data access with robust ORM capabilities, making it easier to manage and query databases.

  * **Swagger UI Implementation**: Provides a user-friendly interface to explore and test API endpoints, enhancing developer experience.

  * **Removal of Docker**: The project initially included Docker support for containerized deployment but has been simplified for local machine development without Docker to facilitate easier setup and debugging.
  
### Unit Testing 
  
  #### The unit test project utilizes [xUnit] as the testing framework, coupled with [Moq(for mocking dependencies)] to facilitate isolated testing of components.

  * **Coverage**: Test cases are implemented across various areas, including:
    * Service layer logic (e.g., product retrieval, creation).
    * Controller actions, ensuring that endpoints are handling requests as expected.

  * **Running Tests**: Unit tests can be executed using the command line or through the IDE:
    * **Command Line**: Navigate to the test project directory and run:
      ```bash
      dotnet test
      ```

* **Visual Studio**: Use the Test Explorer to run all tests and view results directly within the IDE.
