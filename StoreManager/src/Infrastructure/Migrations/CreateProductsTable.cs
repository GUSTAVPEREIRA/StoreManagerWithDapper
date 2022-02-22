using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(3)]
public class CreateProductsTable : Migration
{
    public override void Up()
    {
        Create.Table("categories")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(size: 200).NotNullable();


        Create.Table("products")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(500).NotNullable()
            .WithColumn("description").AsString()
            .WithColumn("category_id").AsInt32()
            .ForeignKey("categories", "id").NotNullable();

        Create.Table("variants")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(500).NotNullable();

        Create.Table("variant_values")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(500).NotNullable()
            .WithColumn("variant_id").AsInt32()
            .ForeignKey("variants", "id").NotNullable();


        Create.Table("product_variants")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("name").AsString(500).NotNullable()
            .WithColumn("sku").AsString(100).NotNullable()
            .WithColumn("product_id").AsInt32()
            .ForeignKey("products", "id").NotNullable();

        Create.Table("product_details")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("product_variant_id").AsInt32()
            .ForeignKey("product_variants", "id").NotNullable()
            .WithColumn("variant_value_id").AsInt32()
            .ForeignKey("variant_values", "id").NotNullable();

        Create.UniqueConstraint("ProductVariantIdAndVariantValueId")
            .OnTable("product_details")
            .Columns("product_variant_id", "variant_value_id");
    }

    public override void Down()
    {
        Delete.Table("categories");
        Delete.Table("products");
        Delete.Table("variants");
        Delete.Table("variant_values");
        Delete.Table("product_variants");
        Delete.Table("product_details");
    }
}