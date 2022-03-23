namespace Infrastructure.Products.Models;

public class ProductVariant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Product Product { get; set; }
    public string Sku { get; set; }
    public decimal Price { get; set; }
}