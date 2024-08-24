using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Store.Models;
using Microsoft.AspNetCore.Identity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Store.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base (options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "1984",
                    Author = "George Orwell",
                    Description = "A dystopian novel set in a totalitarian society under constant surveillance, exploring the dangers of oppressive government control.",
                    ISBN = "978-0-452-28423-4",
                    ListPrice = 99,
                    Price = 90,
                    Price50 =85 ,
                    Price100 = 80,
                    CategoryId = 1,
					ImageUrl = ""
				},
                new Product
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Description = "A novel about the serious issues of rape and racial inequality in the Deep South, seen through the eyes of young Scout Finch.",
                    ISBN = "978-0-06-112008-4",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 1,
					ImageUrl = ""
				},
                new Product
                {
                    Id = 3,
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    Description = "A classic romance novel that critiques the societal norms and expectations of marriage in early 19th century England.",
                    ISBN = "978-0-19-953556-9",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 1,
					ImageUrl = ""
				}, new Product
                {
                    Id = 4,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Description = "A novel that explores themes of decadence, idealism, and the American Dream in 1920s America.",
                    ISBN = "978-0-7432-7356-5",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 2,
					ImageUrl = ""
				},
                new Product
                {
                    Id = 5,
                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    Description = "A novel about the experiences of a disillusioned teenager, Holden Caulfield, as he navigates life in New York City.",
                    ISBN = "978-0-316-76948-0",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 2,
					ImageUrl = ""
				},
                new Product
                {
                    Id = 6,
                    Title = "The Hobbit",
                    Author = "J.R.R. Tolkien",
                    Description = "A fantasy novel that follows the adventure of Bilbo Baggins, a hobbit who embarks on an epic quest with a group of dwarves.",
                    ISBN = "978-0-618-00221-3",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 3,
					ImageUrl=""
				});

            modelBuilder.Entity<Company>().HasData(
              new Company
              {
                  Id = 1,
                  Name = "Tech Solutions",
                  City = "New York",
                  StreetAddress = "123 Innovation Drive",
                  PostalCode = "10001",
                  State = "NY",
                  PhoneNumber = "555-1234"
              },
              new Company
              {
                  Id = 2,
                  Name = "Green Earth",
                  City = "San Francisco",
                  StreetAddress = "456 Market Street",
                  PostalCode = "94105",
                  State = "CA",
                  PhoneNumber = "555-5678"
              },
              new Company
              {
                  Id = 3,
                  Name = "Blue Ocean",
                  City = "Miami",
                  StreetAddress = "789 Ocean Drive",
                  PostalCode = "33101",
                  State = "FL",
                  PhoneNumber = "555-9012"
              }
          );
        }
    }
}
