public class ProductDisplayDto
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ProductTypeName { get; set; }
    public string MainImageUrl { get; set; } // Fully qualified URL for the <img> tag
    public List<string> GalleryUrls { get; set; } = new();
}