using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Security.Claims;
using Web_storebook.Models;
using Web_storebook.Models.ViewModel;

namespace Web_storebook.Controllers
{

    public class HomeController : Controller

    {
        private readonly BookStoreDbContext _bookStoreDbContext;

        public HomeController(BookStoreDbContext bookStoreDbContext)
        {
            _bookStoreDbContext = bookStoreDbContext;

        }

        public async Task<IActionResult> Index(bool viewAll = false)
        {
           
            var allBooks = await (from book in _bookStoreDbContext.Books
                                  orderby book.Author descending
                                  select book).ToListAsync();
            var booksToDisplay = viewAll ? allBooks : allBooks.Take(16).ToList();
            ViewBag.ViewAll = viewAll;

            var userInfoJson = HttpContext.Session.GetString("UserInfo");
			ViewBag.UserInfo = userInfoJson;  // Gán giá trị cho ViewBag

			if (userInfoJson != null)
            {
				Console.WriteLine(userInfoJson); // Log JSON để xem cấu trúc trả về từ Gmail
				var userInfo = JsonConvert.DeserializeObject<dynamic>(userInfoJson);
                ViewBag.UserName = userInfo.Name;
                //ViewBag.UserProfilePicture = userInfo.ProfilePicture;
                ViewBag.UserProfilePicture= userInfo.ProfilePicture ?? "/images/default-avatar.png";
            }
            return View(booksToDisplay);
        }

        public async Task<ActionResult> Details(string id)
        {
            var DetailBook = await _bookStoreDbContext.Books
                                .Include(book => book.Publisher) // Bao gồm thông tin nhà xuất bản
                                .Include(book => book.Category)   // Bao gồm thông tin danh mục
                                .FirstOrDefaultAsync(book => book.BookCode == id);
            var relationBook = await (from book in _bookStoreDbContext.Books
                                                     orderby book.Author descending
                                                     select book).Take(4).ToListAsync();
            return View(DetailBook);
        }



        public IActionResult Privacy()
        {
            return View();
        }

      
    }
}
