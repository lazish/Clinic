using Clinic.Helper;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly MyDbContext _context;

        public BookController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("addBook")]
        public Result AddBook([FromBody] Book book)
        {
            book.Date = book.Date.ToLocalTime();
            Console.WriteLine(book.Date);

            var res = validationHelper.ValidateBook(book, _context);
            if (res.Errors.Count > 0)
            {
                return res;
            }
            else
            {

                _context.Books.Add(book);
                _context.SaveChanges();
                return new Result() { Res = book };
            }
        }

        [Authorize]
        [HttpPost("updateBook")]
        public Result UpdateBook([FromBody] Book newBook)
        {
            var book = _context.Books.Where(item => item.Id == newBook.Id).FirstOrDefault();
            if (book != null)
            {
                book.Date = newBook.Date;
                book.Description = newBook.Description;
                _context.Books.Update(book);
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "მსგავსი ჯავშანი ვერ მოიძებნა" } };


        }

        [Authorize]
        [HttpPost("removeBook")]
        public Result RemoveBook([FromBody] int bookId)
        {
            var book = _context.Books.Where(item => item.Id == bookId).FirstOrDefault();
            if (book != null)
            {
                _context.Books.Remove(book);
                _context.SaveChanges();
                return new Result() { Res = true };
            }
            return new Result() { Errors = new List<string>() { "მსგავსი ჯავშანი ვერ მოიძებნა" } };


        }

        [HttpGet("getDoctorBookedDays")]
        public Result GetDoctorBookedDays(int id = 6)
        {
            Console.WriteLine(id);
            var bookedDays = _context.Books.Where(item => item.DoctorId == id).ToList();
            if (bookedDays == null || bookedDays.Count == 0)
            {
                return new Result() { Errors = new List<string>() { "ჩანაწერი ვერ მოიძებნა" } };
            }
            return new Result() { Res = bookedDays };
        }

        [HttpGet("getClientBookedDays")]
        public Result GetClientBookedDays(int id = 6)
        {
            Console.WriteLine(id);
            var bookedDays = _context.Books.Where(item => item.UserId == id).ToList();
            if (bookedDays == null || bookedDays.Count == 0)
            {
                return new Result() { Errors = new List<string>() { "ჩანაწერი ვერ მოიძებნა" } };
            }
            return new Result() { Res = bookedDays };
        }

    }
}

