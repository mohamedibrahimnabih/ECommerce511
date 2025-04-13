using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var products = _productRepository.Get(includes: [e => e.Category]);

            return Ok(products.Adapt<IEnumerable<ProductResponse>>());
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var product = _productRepository.GetOne(e => e.Id == id);

            if (product != null)
            {
                return Ok(product.Adapt<ProductResponse>());
            }

            return NotFound();
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] ProductRequest productRequest, CancellationToken cancellationToken)
        {
            if (productRequest.File != null && productRequest.File.Length > 0)
            {
                // Save img in images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productRequest.File.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                //if(!System.IO.File.Exists(filePath))
                //{
                //    System.IO.File.Create(filePath);
                //}

                using (var stream = System.IO.File.Create(filePath))
                {
                    await productRequest.File.CopyToAsync(stream);
                }

                // Save img name in db
                Product product = productRequest.Adapt<Product>();
                product.MainImg = fileName;
                var productInDb = await _productRepository.CreateAsync(product, cancellationToken);
                await _productRepository.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = productInDb.Id }, productRequest);
            }

            ModelStateDictionary keyValuePairs = new();
            keyValuePairs.AddModelError("File", "The file is not found");
            return BadRequest(keyValuePairs);
        }


        [HttpPut("{id}")]
        public IActionResult Edit([FromRoute] int id, [FromForm] ProductRequest productRequest)
        {
            var productInDb = _productRepository.GetOne(e => e.Id == id, tracked: false);
            Product product = productRequest.Adapt<Product>();

            if (productInDb != null && productRequest.File != null && productRequest.File.Length > 0)
            {
                // Save img in images
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productRequest.File.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    productRequest.File.CopyTo(stream);
                }

                // Delete old img from images
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "images", productInDb.MainImg);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save img name in db
                product.MainImg = fileName;
            }
            else
                product.MainImg = productInDb!.MainImg;

            if (product != null)
            {
                product.Id = id;
                _productRepository.Edit(product);

                return NoContent();
            }

            ModelStateDictionary keyValuePairs = new();
            keyValuePairs.AddModelError("File", "The file is not found");
            return BadRequest(keyValuePairs);
        }

        [HttpDelete("DeleteImg/{id}")]
        public async Task<IActionResult> DeleteImg(int id)
        {
            var product = _productRepository.GetOne(e => e.Id == id);

            if (product != null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete img name in db
                product.MainImg = null;
                await _productRepository.CommitAsync();

                return RedirectToAction("Edit", new { id });
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetOne(e => e.Id == id);

            if (product != null)
            {
                // Delete old img from wwwroot
                if (product.MainImg != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Delete img name in db
                _productRepository.Delete(product);

                return NoContent();
            }

            return NotFound();
        }
    }
}
