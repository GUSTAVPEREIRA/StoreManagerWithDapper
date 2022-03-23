using System.Collections.Generic;

namespace Infrastructure.Products.Models;

public class Variant
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<VariantValue> VariantValues;
}