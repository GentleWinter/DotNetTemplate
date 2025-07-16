# .NET API Template

This is a .NET 9 API template project that provides a solid foundation for building RESTful APIs. It includes features like user authentication, client management, and Stripe integration.

## About the Project

This project is a .NET 9 API template that can be used as a starting point for building RESTful APIs. It comes with a number of features out of the box, including:

*   User authentication with JWT
*   Client management
*   Stripe integration for payments
*   A clean, layered architecture
*   Dependency injection
*   Database migrations with Entity Framework Core

## Getting Started

To get started with this project, you'll need to have the following installed:

*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [PostgreSQL](https://www.postgresql.org/download/)
*   [Docker](https://www.docker.com/products/docker-desktop) (optional)

### Installation

1.  Clone the repository:

    ```bash
    git clone https://github.com/your-username/DotNetTemplate.git
    ```

2.  Navigate to the project directory:

    ```bash
    cd DotNetTemplate
    ```

3.  Install the dependencies:

    ```bash
    dotnet restore
    ```

4.  Set up the database:

    *   Make sure you have a PostgreSQL database running.
    *   Update the connection string in `Template.Api/appsettings.json`.
    *   Run the database migrations:

        ```bash
        dotnet ef database update -p Template.Infra/Template.Infra.csproj -s Template.Api/Template.Api.csproj
        ```

5.  Run the application:

    ```bash
    dotnet run --project Template.Api/Template.Api.csproj
    ```

The API will be available at `http://localhost:5000`.

## Technologies

*   [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0)
*   [ASP.NET Core 9](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-9.0)
*   [Entity Framework Core 9](https://docs.microsoft.com/en-us/ef/core/)
*   [PostgreSQL](https://www.postgresql.org/)
*   [Stripe.net](https://github.com/stripe/stripe-net)
*   [AutoMapper](https://automapper.org/)
*   [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)

## Project Structure

The project is divided into four layers:

*   **Template.Api:** The presentation layer, which contains the API controllers and other API-specific configurations.
*   **Template.Application:** The application layer, which contains the business logic of the application.
*   **Template.Domain:** The domain layer, which contains the domain entities and DTOs.
*   **Template.Infra:** The infrastructure layer, which contains the database context, repositories, and other infrastructure-related code.

## API Endpoints

The following API endpoints are available:

*   **`POST /api/client`**: Creates a new client.
*   **`GET /api/client`**: Returns a list of all clients.
*   **`GET /api/client/{id}`**: Returns a specific client.
*   **`PUT /api/client/{id}`**: Updates a specific client.
*   **`DELETE /api/client/{id}`**: Deletes a specific client.
*   **`POST /api/login`**: Authenticates a user and returns a JWT.
*   **`POST /api/stripe-webhook`**: Handles Stripe webhooks.

For more information about the API endpoints, you can view the Swagger documentation at `http://localhost:5000/swagger`.
