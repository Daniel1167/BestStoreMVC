using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace BestStoreMVC.Controllers
{
    // This controller allows us to perform CRUD operations on 'Products'

    // Here we need to read the list of products from the database, and we have to pass this list to view.
    // For this we need 'ApplicationDbContext.cs' that we already added to the service container.
    // And to request it from the service container we need to create the constructor of the ProductsController class.

    // The ProductsController should be available only to 'Administrator'
    [Route("/Admin/[controller]/[action]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {   
       
        private readonly IWebHostEnvironment environment; // we use this field to obtain a full path of 'wwwroot' folder
        private readonly ApplicationDbContext context; // we use this field to read the products from the database

        private readonly int pageSize = 5;


        // As second parameter of this constructor, we need an object of IWebHostEnvironment that allows us to obtain a full path of the 'wwwroot' folder
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment evironmet)
        { 
            this.context = context;
            this.environment = evironmet;

        }

       

        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = context.Products;

           // query = query.OrderByDescending(p => p.Id);

            // search functionality
            if(search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            //sort functionality
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);

                }
            } 
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query.OrderByDescending(p => p.Category);
                }
            }    
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //pagination functionality
            if(pageIndex < 1)
            {
                pageIndex = 1;
            }
            // we need the total number of products
            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }


        // This method will be accessible using 'http post' method, so we have to decorate it with the appropriate attribute
        // In this method is required an object of type ProductDto, which is the submitted data
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            // we need to validate image file manually because in ProductDto the image file is considered as optional
            if(productDto.ImageFile == null)
            {   // We will add and error to the ModelState which is related to the 'ImageFile' argument that is a property of ProductDto 
                // And the error message will the second argument
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if(!ModelState.IsValid)
            {
                return View(productDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            // we need the full path where we will save the image 
            string ImageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(ImageFullPath))
            {
                // this statement allows us to save the received image to the above path
                productDto.ImageFile.CopyTo(stream);
            }

            // Save the new product in the database (In the database we can add objects of type Product)
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Products.Add(product); // We add 'product' to the table 'Products'
            context.SaveChanges(); 

            // Redirects the user to the 'Index' action to the ProductsController that allows us to display a list of 'Products'
            return RedirectToAction("Index", "Products");
        }

        // this action requires a parameter call id which is the product id and it is added to the URL
        public IActionResult Edit(int id)
        {
            // we need to read the product having this 'id' from the database
            var product = context.Products.Find(id);

            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // If we found a product then we will create an object of type productDto using data of 'product'

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            //return the View with this object
            return View(productDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            // read the product having this 'id' parameter from the database
            var product = context.Products.Find(id);

            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // verify if submitted data is valid or not
            if(!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            // update the product in the database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }


    }
}
