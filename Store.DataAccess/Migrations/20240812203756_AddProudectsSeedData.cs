using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Store.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProudectsSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "George Orwell", "A dystopian novel set in a totalitarian society under constant surveillance, exploring the dangers of oppressive government control.", "978-0-452-28423-4", 99.0, 90.0, 80.0, 85.0, "1984" },
                    { 2, "Harper Lee", "A novel about the serious issues of rape and racial inequality in the Deep South, seen through the eyes of young Scout Finch.", "978-0-06-112008-4", 99.0, 90.0, 80.0, 85.0, "To Kill a Mockingbird" },
                    { 3, "Jane Austen", "A classic romance novel that critiques the societal norms and expectations of marriage in early 19th century England.", "978-0-19-953556-9", 99.0, 90.0, 80.0, 85.0, "Pride and Prejudice" },
                    { 4, "F. Scott Fitzgerald", "A novel that explores themes of decadence, idealism, and the American Dream in 1920s America.", "978-0-7432-7356-5", 99.0, 90.0, 80.0, 85.0, "The Great Gatsby" },
                    { 5, "J.D. Salinger", "A novel about the experiences of a disillusioned teenager, Holden Caulfield, as he navigates life in New York City.", "978-0-316-76948-0", 99.0, 90.0, 80.0, 85.0, "The Catcher in the Rye" },
                    { 6, "J.R.R. Tolkien", "A fantasy novel that follows the adventure of Bilbo Baggins, a hobbit who embarks on an epic quest with a group of dwarves.", "978-0-618-00221-3", 99.0, 90.0, 80.0, 85.0, "The Hobbit" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
