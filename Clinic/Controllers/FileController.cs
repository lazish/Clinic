using Clinic.Helper;
using Clinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly MyDbContext _context;
        public FileController(MyDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("uploadImage"), DisableRequestSizeLimit]
        public Result UploadProductImage([FromForm] UploadFile file)
        {
            string? url = SaveFile(file, Constants.IMAGES_FOLDER_NAME);
            if (url != null)
            {
                return new Result() { Res = url };
            }
            return new Result() { Errors = new List<string>() { "file not saved" } };
        }

        private string? SaveFile(UploadFile file, string path)
        {
            try
            {
                var foldername = Path.Combine("Resources", path);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), foldername);
                string? url = null;
                if (file.File.Length > 0)
                {
                    var fileName = DateTime.Now.ToBinary() + ContentDispositionHeaderValue.Parse(file.File.ContentDisposition).FileName.Trim().ToString();
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(foldername, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.File.CopyTo(stream);
                        url = fullPath;
                    }
                    var user = _context.Users.FirstOrDefault(item => item.Id == file.UserId);
                    if (user != null)
                    {
                        if (path == Constants.IMAGES_FOLDER_NAME)
                        {
                            user.ImageUrl = dbPath;
                        }
                        else
                        {
                            user.CV = dbPath;
                        }
                        _context.Users.Update(user);
                        _context.SaveChanges();
                    }

                }
                _context.SaveChanges();

                return url;
            }
            catch
            {
                return null;
            }
        }



        [Authorize(Roles = "admin")]
        [HttpPost("uploadDocument"), DisableRequestSizeLimit]

        public Result UploadProductDocument([FromForm] UploadFile file)
        {
            string? url = SaveFile(file, Constants.DOCUMENTS_FOLDER_NAME);
            if (url != null)
            {
                return new Result() { Res = url };
            }
            return new Result() { Errors = new List<string>() { "file not saved" } };
        }

    }
}
