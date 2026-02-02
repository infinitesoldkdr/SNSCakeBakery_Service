public class Product 
{
    public int ProductId { get; set; }
    public int ProductTypeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // EF will use this to understand the 1-to-Many relationship
    public List<ProductImage> Images { get; set; } = new List<ProductImage>();
}