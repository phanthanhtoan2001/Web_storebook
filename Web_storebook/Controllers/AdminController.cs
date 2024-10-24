using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_storebook.Models;
using Web_storebook.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Web_storebook.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookStoreDbContext _bookStoreDbContext;

        
        public AdminController(BookStoreDbContext bookStoreDbContext)
        {
            _bookStoreDbContext = bookStoreDbContext;

        }
        public async Task<IActionResult> Index()
        {
            var allBooks = await (from book in _bookStoreDbContext.Books
                                  orderby book.BookCode descending
                                  select book).ToListAsync();
            return View(allBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string key)
        { 

            var searchbook = await (from book in _bookStoreDbContext.Books
                                    where (book.Author.Contains(key) || book.Title.Contains(key) 
                                    || book.BookCode.Contains(key))
                                    orderby book.BookCode descending
                                    select book).ToListAsync();
            return PartialView("_BookListPartial", searchbook); // Trả về partial view

        }
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(string bookcode)
        {
            var book = await _bookStoreDbContext.Books
              .FirstOrDefaultAsync(b => b.BookCode == bookcode);

            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            _bookStoreDbContext.Books.Remove(book);
            await _bookStoreDbContext.SaveChangesAsync();

            return Ok(new { message = "Book deleted successfully" });
        }


        public async Task<IActionResult> EditBook(string bookcode)
        {
            var book = await _bookStoreDbContext.Books
                .FirstOrDefaultAsync(b => b.BookCode == bookcode);

            if (book == null)
            {
                return NotFound();
            }

            return PartialView("_EditBook",book);
        }


        [HttpPut]
        public async Task<IActionResult> EditBook([FromBody] Book updatedBook)
        {
            var book = await _bookStoreDbContext.Books
                .FirstOrDefaultAsync(b => b.BookCode == updatedBook.BookCode);

            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            book.Title = updatedBook.Title;
            book.Author = updatedBook.Author;
            book.Price = updatedBook.Price;


            _bookStoreDbContext.Books.Update(book);
            await _bookStoreDbContext.SaveChangesAsync();

            return Ok(new { message = "Book updated successfully", updatedBook = book });
        }

        public IActionResult AddBook()
        {
            var publishers = _bookStoreDbContext.Publishers.ToList();

            var categories = _bookStoreDbContext.Categories.ToList();

            var viewModel = new ViewModelBook
            {
                PublisherList = publishers.Select(p => new SelectListItem
                {
                    Value = p.PublisherId.ToString(),
                    Text = p.PublisherName
                }).ToList(),

                CategoryList = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] ViewModelBook viewModelBook)
        {
            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    Author = viewModelBook.Author,
                    Title = viewModelBook.Title,
                    Isbn = viewModelBook.Isbn,
                    Price = viewModelBook.Price,
                    DiscountPrice = viewModelBook.DiscountPrice,
                    PublicationDate = viewModelBook.PublicationDate,
                    StockQuantity = viewModelBook.StockQuantity,
                    Language = viewModelBook.Language,
                    Pages = viewModelBook.Pages,
                    Weight = viewModelBook.Weight,
                    Dimension = viewModelBook.Dimension,
                    CoverType = viewModelBook.CoverType,
                    PublisherId = viewModelBook.PublisherId,
                    CategoryId = viewModelBook.CategoryId,
                    Description = viewModelBook.Description,
                    ImageBook = viewModelBook.ImageBook,

                };
                await _bookStoreDbContext.AddAsync(book);
                await _bookStoreDbContext.SaveChangesAsync();

                return Json(new { success = true, message = "Sách đã được thêm thành công", redirectUrl = Url.Action("Index") });
            }
            else
            {
                return Json(new { success = false, message = "Thông tin sách không hợp lệ" });
            }
        }

        

    }
}
