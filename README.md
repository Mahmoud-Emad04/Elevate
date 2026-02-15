# Elevate API

Elevate is a comprehensive Order Management System API built with .NET 10. It provides functionalities for managing products, orders, customers, and payments, serving as a robust backend for e-commerce applications.

## 🚀 Features

- **Product Management**: Create, update, and retrieve products with stock tracking.
- **Order Management**: Place orders, track status, and view history.
- **Customer Management**: specialized endpoints for customer data.
- **Authentication**: Secure JWT-based authentication with Role-Based Access Control (Admin/Customer).
- **Payments**: Integrated Stripe payment processing for secure transactions.
- **Invoicing**: Automatic invoice generation for orders.
- **Email Notifications**: Integration with SMTP services for sending transactional emails.
- **Documentation**: Interactive API documentation via Scalar.

## 🛠️ Tech Stack

- **Framework**: [.NET 10](https://dotnet.microsoft.com/)
- **Database**: Entity Framework Core (In-Memory provider for development)
- **Validation**: FluentValidation
- **Payments**: Stripe.net
- **Documentation**: Scalar.AspNetCore
- **Testing**: xUnit (implied by solution structure)

## 🏁 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Configuration

The application uses `appsettings.json` for configuration. For local development, key settings include:

- **JWT Settings**: defined in `Extensions/DependencyInjection.cs` (defaults provided for dev).
- **Stripe**: Requires specific keys for payment processing at `Stripe:SecretKey`.
- **MailSettings**: Configured for Ethereal Email (for testing).

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Mahmoud-Emad04/Elevate.git
   cd Elevate
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project Elevate.Api
   ```

The API will start at `https://localhost:7228` (or the port defined in your `launchSettings.json`).

## 📖 Usage

### API Documentation

Once the application is running, access the interactive API reference at:
👉 **[https://localhost:7228/scalar/v1](https://localhost:7228/scalar/v1)**

### Default Users (Seed Data)

The application seeds an in-memory database with the following accounts for testing:

| Role     | Username | Password      |
|----------|----------|---------------|
| **Admin**    | `admin`    | `Admin123!`     |
| **Customer** | `customer` | `Customer123!`  |

## 📂 Project Structure

- **Elevate.Api**: Main Web API project.
  - `Controllers`: API endpoints.
  - `Services`: Business logic.
  - `Repositories`: Data access patterns.
  - `Models`: Domain entities.
  - `Dtos`: Data Transfer Objects.
  - `Validation`: FluentValidation rules.
- **Elevate.Api.Tests**: Unit and integration tests.
