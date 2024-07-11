# Project Name: ASP.NET 8 WebAPI with JWT Authentication and Authorization

## Overview

This repository contains the implementation of authentication and authorization using JSON Web Tokens (JWT) in an ASP.NET 8 WebAPI project. The goal of this project is to demonstrate how to secure an API by implementing JWT-based authentication and authorization.

## Features

- **User Registration**: Allows new users to register with the API.
- **User Login**: Authenticates users and generates a JWT for authorized access.
- **JWT Validation**: Validates the JWT for subsequent API requests to ensure secure access.
- **Role-Based Authorization**: Implements role-based access control to restrict access to certain API endpoints based on user roles.

## Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or any other database you prefer

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/Perigrino/-Jwt_Auth_AspNet8_WebAPI.git
   cd -Jwt_Auth_AspNet8_WebAPI
   ```

2. **Set Up the Database**

   - Create a new database in your SQL Server instance.
   - Update the connection string in the `appsettings.json` file to point to your database.

3. **Run Migrations**

   ```bash
   dotnet ef database update
   ```

4. **Build and Run the Project**

   ```bash
   dotnet build
   dotnet run
   ```

### Usage

- **Register a New User**

  Send a `POST` request to `/api/auth/register` with the following JSON payload:

  ```json
  {
      "firstName": "John",
      "lastName": "Doe",
      "email" : "johnDoe@email.com"
      "username": "newuser",
      "password": "password123",
      "securityStamp": "b2d5e1c2-1b9a-4bfa-a1a2-9c8b0e2fabb5"
  }
  ```

- **Login**

  Send a `POST` request to `/api/auth/login` with the following JSON payload:

  ```json
  {
      "username": "newuser",
      "password": "password123"
  }
  ```

  The response will contain a JWT token.

- **Access Protected Endpoints**

  Include the JWT token in the `Authorization` header of your requests to protected endpoints:

  ```http
  Authorization: Bearer <your_jwt_token>
  ```

## Project Structure

- **Controllers**: Contains the API controllers for authentication and other endpoints.
- **Models**: Contains the data models for the application.
- **Services**: Contains the services for handling business logic.
- **Helpers**: Contains helper classes for JWT generation and validation.

## Contributing

Contributions are welcome! Please fork this repository and submit pull requests with your changes.


## Contact

For any questions or feedback, please open an issue or contact me at [chasebruce1992@gmail.com](mailto:chasebruce1992@gmail.com).

---
