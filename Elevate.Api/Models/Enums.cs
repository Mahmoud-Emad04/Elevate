namespace Elevate.Api.Models;

public enum PaymentMethod
{
    CreditCard,
    PayPal
}

public enum OrderStatus
{
    Pending,
    Processing,
    Completed,
    Cancelled
}

public enum UserRole
{
    Admin,
    Customer
}
