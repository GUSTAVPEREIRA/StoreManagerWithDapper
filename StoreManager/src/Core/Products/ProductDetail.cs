namespace Core.Products;

public class ProductDetail
{
    public int Id { get; set; }
    public ProductVariant Variant { get; set; }
    public VariantValue VariantValue { get; set; }
}