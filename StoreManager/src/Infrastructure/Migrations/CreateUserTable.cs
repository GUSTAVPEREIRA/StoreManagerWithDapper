using FluentMigrator;

namespace Infrastructure.Migrations
{
    [Migration(2)]
    public class CreateUserTable : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsInt32().PrimaryKey().Identity()
                .WithColumn("full_name").AsString(255).NotNullable()
                .WithColumn("user_id").AsInt32().ForeignKey("roles", "id").NotNullable();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}