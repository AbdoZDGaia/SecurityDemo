using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationApp.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    NameIdentifier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Mobile = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true),
                    Roles = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "UserId", "Email", "FirstName", "LastName", "Mobile", "NameIdentifier", "Password", "Provider", "Roles", "UserName" },
                values: new object[] { 1, "oldtimeprogrammer@yahoo.com", "Abdulrahman", "Seliem", "+201111012606", null, "P@ssw0rd", "Cookies", "Admin,User", "AbdoZ" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");
        }
    }
}
