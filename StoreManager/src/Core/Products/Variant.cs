using System.Collections.Generic;

namespace Core.Products;

public class Variant
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<VariantValue> VariantValues;
}