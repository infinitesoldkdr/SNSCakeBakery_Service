namespace SNSCakeBakery_Service.DTOs.Orders
{
    // ---------------------------------------------
    // Create Order
    // ---------------------------------------------
    public class CreateOrderDto
    {
        public string UserId { get; set; }
        public string CakeType { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // New field
        public string DeliveryAddressId { get; set; }
    }

    // ---------------------------------------------
    // Update Order
    // ---------------------------------------------
    public class UpdateOrderDto
    {
        public string OrderId { get; set; }
        public string CakeType { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public string DeliveryAddressId { get; set; }
    }

    // ---------------------------------------------
    // Order Response (sent back to client)
    // ---------------------------------------------
    public class OrderResponseDto
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public string CakeType { get; set; }
        public int Quantity { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime OrderDate { get; set; }

        // Address info
        public string DeliveryAddressId { get; set; }
        public AddressSummaryDto DeliveryAddress { get; set; }
    }

    // ---------------------------------------------
    // Lightweight address representation included with orders
    // ---------------------------------------------
    public class AddressSummaryDto
    {
        public string AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
