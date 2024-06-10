using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BestStoreMVC.Models
{   // This class is a domain model or entity model that allows us to create a table in a database, 
    // to read and write data from database.
    public class Product
    {
        // Add different properties that correspond to the different colors of the products table
        // that we will create in a database

        // We can annotate these properties to limit their length in the database.
       
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Precision(16, 2)]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ImageFileName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
