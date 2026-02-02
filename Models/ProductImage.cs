// ProductImage.cs
public class ProductImage
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public int ImageTypeId { get; set; }
    public string StorageKey { get; set; } // e.g., "cakes/vanilla-01.jpg"
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime UploadedAt { get; set; }
}