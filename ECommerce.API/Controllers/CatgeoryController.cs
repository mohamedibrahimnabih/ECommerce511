using ECommerce.API.Models;
using ECommerce.API.Repositories.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public IActionResult GetAll()
        {
            var categories = _categoryRepository.Get();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        //[Route("{id}")]
        public IActionResult GetOne([FromRoute] int id)
        {
            var category = _categoryRepository.GetOne(e => e.Id == id);

            if (category != null)
                return Ok(category);

            return NotFound();
        }

        [HttpPost("")]
        public IActionResult Create([FromBody] Category category)
        {
            _categoryRepository.Create(category);
            _categoryRepository.Commit();

            //CookieOptions cookieOptions = new CookieOptions()
            //{
            //    Expires = DateTime.Now.AddSeconds(10),
            //    Secure = true
            //};
            //Response.Cookies.Append("notifaction", "Created Category Successfuly", cookieOptions);

            //return Create();
            //return Created($"{Request.Scheme}://{Request.Host}/api/Catgeory/{category.Id}", category);
            return CreatedAtAction(nameof(GetOne), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Category category)
        {
            var categoryInDB = _categoryRepository.GetOne(e => e.Id == id, tracked: false);

            if(categoryInDB != null)
            {
                category.Id = categoryInDB.Id;
                _categoryRepository.Edit(category);
                _categoryRepository.Commit();

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
                _categoryRepository.Commit();

                return NoContent();
            }

            return NotFound();
        }
    }
}
