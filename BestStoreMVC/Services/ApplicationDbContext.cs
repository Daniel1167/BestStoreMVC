using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services
{    // This class allows us to connect to the database using Entity Framework.
    // This class is a 'service' because it is used by other classes.
    // This class extends the DbContext class of Entity Framework.
    // This class must be added to the service container of our application (Program.cs)
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        // This property allows us to create a table called 'Products' in the database
        public DbSet<Product> Products { get; set; } // this will be name of the table in the database
    }
}
