using FluentMigrator;

namespace Infrastructure.Migrations
{
    [Migration(1)]
    public class CreateRoleTable : Migration
    {
        public override void Up()
        {
            Create.Table("roles")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("name").AsString(size: 100).NotNullable()
                .WithColumn("is_admin").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        public override void Down()
        {
            Delete.Table("roles");
        }
    }
}