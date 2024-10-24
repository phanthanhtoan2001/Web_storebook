using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Newtonsoft.Json;
using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web_storebook.Models.ViewModel;
using Web_storebook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BCrypt.Net; // Thêm thư viện BCrypt
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization; // Đảm bảo rằng bạn đã thêm không gian tên này



namespace Web_storebook.Controllers
{
    public class LoginController : Controller
    {

        private readonly BookStoreDbContext _bookStoreDbContext;
        private readonly IConfiguration _configuration;

        public LoginController(BookStoreDbContext bookStoreDbContext, IConfiguration configuration)
        {
            _bookStoreDbContext = bookStoreDbContext;
            _configuration = configuration;
        }

        public IActionResult Signin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("Callback") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> Callback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                // Lấy thông tin user từ các claim
                var claims = result.Principal.Claims.ToList();
                var userInfo = new
                {
                    Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    ProfilePicture = claims.FirstOrDefault(c => c.Type == "urn:google:profile")?.Value

                    //ProfilePicture = claims.FirstOrDefault(c => c.Type == "picture")?.Value

                };

                // Lưu thông tin user vào Session
                HttpContext.Session.SetString("UserInfo", JsonConvert.SerializeObject(userInfo));

                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Error");

        }


        public async Task<IActionResult> Logout()
        {
            // Xóa session trước
            HttpContext.Session.Clear();

            // Đăng xuất khỏi hệ thống xác thực bằng Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            // Điều hướng về trang chủ sau khi đăng xuất thành công
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> SignInForm()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignInForm([FromBody] ViewModelCustomer viewModelCustomer)
        {
            if (ModelState.IsValid)
            {
                Customer customer = new Customer
                {
                    FirstName = viewModelCustomer.FirstName,
                    LastName = viewModelCustomer.LastName,
                    Email = viewModelCustomer.Email,
                    Phone = viewModelCustomer.Phone,
                    BillingAddress = viewModelCustomer.BillingAddress,
                    ShippingAddress = viewModelCustomer.ShippingAddress,
                    MembershipLevel = viewModelCustomer.MembershipLevel,
                    DateOfBirth = viewModelCustomer.DateOfBirth,
                    RegistrationDate = viewModelCustomer.RegistrationDate,
                };
                await _bookStoreDbContext.AddAsync(customer);
                await _bookStoreDbContext.SaveChangesAsync();

                return Json(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập!", redirectUrl = Url.Action("SignInForm", "Login") });
            }
            else
            {
                return Json(new { success = false, message = "Thông tin khách hàng không hợp lệ" });
            }

        }

        public async Task<IActionResult> Account()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Account([FromBody] LoginViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            try
            {
                // Kiểm tra thông tin đăng nhập
                var user = _bookStoreDbContext.Users.SingleOrDefault(u => u.Username == model.Username);

                // Nếu không tìm thấy người dùng hoặc mật khẩu không đúng
                if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
                {
                    return Unauthorized();
                }

                // Tạo claims cho token
                var claims = new[]
                {
                       new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                       new Claim("role", GetUserRole(user.UserId).ToString())
                };


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                HttpContext.Response.Cookies.Append("jwtToken", new JwtSecurityTokenHandler().WriteToken(token));

                // Xác định URL để chuyển hướng
                var redirectUrl = GetUserRole(user.UserId) == "Admin" ? Url.Action("Index", "Admin") : Url.Action("Index", "Home");
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo,
                    RedirectUrl = redirectUrl

                });
            }
            catch (Exception ex)
            {
                // Log the exception to your logging framework
                Console.WriteLine(ex.Message); // Consider using a logger
                return StatusCode(500, "Internal server error");
            }
        }



        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        private string GetUserRole(int userId)
        {
            var role = (from ur in _bookStoreDbContext.UserRoles
                        join r in _bookStoreDbContext.Roles on ur.RoleId equals r.RoleId
                        where ur.UserId == userId
                        select r.RoleName).FirstOrDefault();

            return role ?? "User"; // Mặc định trả về vai trò "User" nếu không tìm thấy
        }



        /////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng đã tồn tại chưa
                var existingUser = _bookStoreDbContext.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }

                // Kiểm tra mật khẩu
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "Passwords do not match.");
                    return View(model);
                }

                // Hash mật khẩu trước khi lưu vào DB
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Tạo đối tượng người dùng mới
                var newUser = new User
                {
                    Username = model.Username,
                    PasswordHash = passwordHash,
                    // Thêm các trường khác nếu cần
                    RegistrationDate = DateTime.Now
                };

                // Lưu người dùng vào cơ sở dữ liệu
                await _bookStoreDbContext.Users.AddAsync(newUser);
                await _bookStoreDbContext.SaveChangesAsync();

                // Redirect đến trang đăng nhập hoặc trang chính sau khi đăng ký thành công
                return RedirectToAction("Login");
            }

            return View(model);
        }


    }
}
