namespace Infrastructure.Products.Models;

public class VariantValue
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Variant Variant { get; set; }
}