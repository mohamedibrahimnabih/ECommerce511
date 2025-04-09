using ECommerce.API.DTOs.Request;
using ECommerce.API.DTOs.Response;
using ECommerce.API.Models;
using ECommerce.API.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatgeoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CatgeoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("")]
        [Authorize]
        public IActionResult GetAll()
        {
            var categories = _categoryRepository.Get(includes: [e=>e.Products]);

            //List<CategoryResponse> categoryResponse = new();
            //foreach (var item in categories)
            //{
            //    categoryResponse.Add(new()
            //    {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Description = item.Description,
            //        Status = item.Status
            //    });
            //}

            return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
        }

        [HttpGet("{id}")]
        //[Route("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var category = _categoryRepository.GetOne(e => e.Id == id);

            if (category != null)
            {
                return Ok(category.Adapt<CategoryResponse>());
            }

            return NotFound();
        }

        [HttpPost("")]
        public IActionResult Create([FromBody] CategoryRequest category)
        {
            var categoryInDb = _categoryRepository.Create(new Category()
            {
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            });

            //CookieOptions cookieOptions = new CookieOptions()
            //{
            //    Expires = DateTime.Now.AddSeconds(10),
            //    Secure = true
            //};
            //Response.Cookies.Append("notifaction", "Created Category Successfuly", cookieOptions);

            //return Create();
            //return Created($"{Request.Scheme}://{Request.Host}/api/Catgeory/{category.Id}", category);
            return CreatedAtAction(nameof(GetOne), new { id = categoryInDb.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] CategoryRequest category)
        {
            var categoryInDB = _categoryRepository.GetOne(e => e.Id == id, tracked: false);

            if(categoryInDB != null)
            {
                _categoryRepository.Edit(new Category()
                {
                    Id = id,
                    Name = category.Name,
                    Description = category.Description,
                    Status = category.Status
                });

                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Reomve([FromRoute] int id)
        {
            var categoryInDB = _categoryRepository.GetOne(e => e.Id == id);

            if (categoryInDB != null)
            {
                _categoryRepository.Delete(categoryInDB);

                return NoContent();
            }

            return NotFound();
        }
    }
}
