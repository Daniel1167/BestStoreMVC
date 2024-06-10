
using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models
{   

    // This class allows user to submit the product details. (It's used to create new products but also to update products)
    // THis class is named DTO, meaning data transfer object 
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; } 
    }
}
