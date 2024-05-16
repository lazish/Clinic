using Clinic.Helper;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDbContext _context;
        public CategoryController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("getCategories")]
        public Result GetCategories()
        {
            var categories = _context.Categories.ToList();
            if (categories != null)
            {
                List<CategoryFront> categoriesFront = new List<CategoryFront>();

                foreach (var item in categories)
                {
                    var doctors = _context.Doctors.Where(doctor => doctor.CategoryId == item.Id).ToList();
                    int count = 0;
                    if (doctors != null)
                    {
                        count = doctors.Count();
                    }
                    categoriesFront.Add(new CategoryFront() { Id = item.Id, Name = item.Name, DoctorCount = count });
                }
                return new Result() { Res = categoriesFront.OrderBy(item => item.Name) };
            }
            return new Result() { Errors = new List<string>() { "კატეგორია არ მოიძებნა" } };
        }

        [Authorize]
        [HttpPost("add")]
        public Result Add([FromBody] Category category)
        {
            if (category.Name.Trim().Length > 0)
            {
                var categoryName = _context.Categories.FirstOrDefault(item => item.Name == category.Name);
                if (categoryName != null)
                {
                    return new Result() { Errors = new List<string>() { "კატეგორია ამ სახელით უკვე არსებობს" } };
                }

                _context.Categories.Add(category);
                _context.SaveChanges();
                return new Result() { Res = category };
            }
            return new Result() { Errors = new List<string>() { "კატეგორიის სახელი აუცილებებლია" } };

        }

        [Authorize(Roles = "admin")]
        [HttpPost("delete")]
        public Result Delete([FromBody] int id)
        {
            var category = _context.Categories.Where(item => item.Id == id).FirstOrDefault();
            if (category != null)
            {
                var doctor = _context.Doctors.Where(item => item.CategoryId == id).FirstOrDefault();
                if (doctor == null)
                {
                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                    return new Result() { Res = true };
                }
                return new Result() { Errors = new List<string>() { "მითითებული კატეგორიის წაშლა შეუძლებელია" } };

            }
            return new Result() { Errors = new List<string>() { "კატეგორია ვერ მოიძებნა" } };

        }

        [Authorize(Roles = "admin")]
        [HttpPost("update")]
        public Result Update([FromBody] Category updatedCategory)
        {
            var category = _context.Categories.Where(item => item.Id == updatedCategory.Id).FirstOrDefault();
            if (category != null)
            {
                category.Name = updatedCategory.Name;
                _context.Categories.Update(category);
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "კატეგორია ვერ მოიძებნა" } };

        }
    }
}
