using Clinic.Helper;
using Clinic.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Clinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        public AuthController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public Result Register([FromBody] User user)
        {

            if (user != null)
            {
                List<string> errors = validationHelper.Validateuser(user);
                user.CreatedAt = DateTime.Now;
                bool usernameAlreadyExists = false;

                usernameAlreadyExists = _context.Users.Where(u => u.Email == user.Email).FirstOrDefault() != null;

                if (!usernameAlreadyExists)
                {

                    if (errors.Count == 0)
                    {
                        _context.Add(user);
                        _context.SaveChanges();

                        if (user.Role == "doctor")
                        {
                            _context.Doctors.Add(new Doctor { UserID = user.Id, Rating = 0, Views = 0, CategoryId = user.CategoryId });
                            _context.SaveChanges();
                        }
                        return new Result() { Res = user };
                    }
                    return new Result() { Errors = errors };
                }
                return new Result() { Errors = new List<string>() { "მომხმარებლის სახელი უკვე არსებობს!" } };

            }
            return new Result() { Errors = new List<string>() { "მონაცემები არასწორია" } };
        }



        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public Result Login([FromBody] LoginRequest request)
        {
            string username = request.Email;
            string password = request.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new Result() { Errors = new List<string> { "მომხმარებლის სახელი და პაროლი აუცილებელია!" } };
            }
            else
            {
                var user = _context.Users.FirstOrDefault(user => user.Email == username && user.Password == password);

                if (user != null)
                {
                    user.Token = CreateJwt(user);
                    return new Result()
                    {
                        Res = new JwtAuthResponse
                        {
                            Token = user.Token,
                        }
                    };
                }
                return new Result() { Errors = new List<string> { "მომხმარებლის სახელი ან პაროლი არასწორია!" } };
            }
        }


        public class Mail
        {
            public List<string> EmailTo { get; set; }
            public string EmailFromId { get; set; }
            public string EmailFromPassword { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsBodyHtml { get; set; }
            public List<string> Attachments { get; set; }
        }

        [HttpPost("sendMail")]
        public bool SendMail([FromBody] Mail mail)
        {
            var mailTo = mail.EmailTo[0];
            var code = Guid.NewGuid().ToString("N").Substring(0, 8);
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(Constants.EMAIL_FROM));
            email.To.Add(MailboxAddress.Parse(mailTo));
            //email.Subject = mail.Subject;
            email.Body = new TextPart(mail.IsBodyHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = $"Your unique code is: {code}"
            };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.mail.yahoo.com", 587,SecureSocketOptions.Auto);
            smtp.Authenticate(Constants.EMAIL_FROM, Constants.MAIL_PASSWORD);
            smtp.Send(email);
            smtp.Disconnect(true);

            var emailConfirm = _context.EmailConfirm.FirstOrDefault(item => item.Email == mailTo);
            if (emailConfirm != null)
            {
                emailConfirm.ValidDate = DateTime.Now.AddMinutes(30);
                emailConfirm.Code = code;
                _context.EmailConfirm.Update(emailConfirm);
            }
            else
            {
                _context.EmailConfirm.Add(new EmailConfirm { Code = code, Email = mailTo, ValidDate = DateTime.Now.AddMinutes(30) });
            }
            _context.SaveChanges();
            return true;
        }
        public class EmailConfirmRequest
        {
            public string Email { get; set; }
            public string Code { get; set; }
        }

        [HttpPost("emailConfirm")]
        public ActionResult<Result> EmailConfirm([FromBody] EmailConfirmRequest request)
        {
            var email = request.Email;
            var code = request.Code;
            var result = _context.EmailConfirm.FirstOrDefault(item => item.Email == email && item.Code == code);

            if (result != null)
            {
                if (result.ValidDate < DateTime.Now)
                {
                    return BadRequest(new Result() { Errors = new List<string>() { "კოდი ვადაგასულია" } });
                }

                return Ok(new Result() { Res = true });
            }

            return BadRequest(new Result() { Errors = new List<string>() { "კოდი არასწორია" } });
        }

        public class ChangePasswordRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }

        [HttpPost("changePassword")]
        public Result ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var email = request.Email;
            var password = request.Password;
            var confirmPassword = request.ConfirmPassword;

            var passwordErrors = validationHelper.isPasswordValid(password);

            if (password != confirmPassword)
            {
                return new Result() { Errors = new List<string>() { "პაროლი არ ემთხვევა" } };
            }

            if (passwordErrors.Count > 0)
            {
                return new Result() { Errors = new List<string>() { "პაროლი არ ემთხვევა" } };
            }
            else
            {
                var user = _context.Users.FirstOrDefault(item => item.Email == email);

                if (user != null)
                {
                    user.Password = password;
                    _context.SaveChanges();
                    return new Result() { Res = true };
                }

                return new Result() { Errors = new List<string>() { "მომხმარებელი ვერ მოიძებნა" } };
            }
        }

        [Authorize]
        [HttpPost("twoFactory")]
        public Result Twofactory([FromBody] bool status)
        {
            Claim? claimId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("id", StringComparison.InvariantCultureIgnoreCase));
            if (claimId != null)
            {
                var user = _context.Users.FirstOrDefault(item => item.Id == Convert.ToInt32(claimId.Value));
                if (user != null)
                {
                    user.TwoFactory = status;
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    user.Token = CreateJwt(user);
                    return new Result()
                    {
                        Res = new JwtAuthResponse
                        {
                            Token = user.Token,
                        }
                    };
                }
                else
                {
                    return new Result() { Errors = new List<string>() { "მომხმარებელი ვერ მოიძებნა" } };
                }

            }
            return new Result() { Errors = new List<string>() { "მომხმარებელი ვერ მოიძებნა" } };

        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);

            var identity = new ClaimsIdentity(new Claim[]
            {
        new Claim("id", user.Id.ToString()),
        new Claim("twoFactory", user.TwoFactory == null ? "False": user.TwoFactory.ToString()),
        new Claim(ClaimTypes.Role,user.Role),
        new Claim("lastname",$"{user.Lastname}"),
        new Claim("firstname",$"{user.Firstname}"),
        new Claim("ID",$"{user.IdentityNumber}"),
        new Claim(ClaimTypes.Email,$"{user.Email}"),
            }); ;

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(Constants.JWT_TOKEN_VALIDITY_MINS),
                SigningCredentials = credentials,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
    }
}

