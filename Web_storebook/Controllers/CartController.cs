using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web_storebook.Models.DTO;
using Web_storebook.Models;


[Route("cart")] 

public class CartController : Controller
{
    private readonly BookStoreDbContext _bookStoreDbContext;
    private readonly CartService _cartService;

    public CartController(BookStoreDbContext bookStoreDbContext, CartService cartService)
    {
        _bookStoreDbContext = bookStoreDbContext;
        _cartService = cartService;
    }

    public IActionResult Cart()
    {
        var cartItems = _cartService.GetCartItems(); // Lấy danh sách các sản phẩm trong giỏ

        return View(cartItems); // Truyền dữ liệu giỏ hàng sang view
    }

    [HttpGet]
    [Route("count")]
    public IActionResult GetCartItemCount()
    {
        int count = _cartService.GetCartItemCount(); // Calculate the number of items in the cart
        return Ok(new { count }); // Return the count as a JSON object
    }

    [HttpPost]
    [Route("addcart/{bookCode}")]
    public IActionResult AddToCart([FromRoute] string bookCode)
    {
        var book = _bookStoreDbContext.Books
            .Where(p => p.BookCode == bookCode)
            .FirstOrDefault();

        if (book == null)
            return NotFound("Không có sản phẩm");

        _cartService.AddToCart(book); // Thêm vào giỏ hàng

        return Ok(new { message = "Sản phẩm đã được thêm vào giỏ hàng!" });
    }
    [HttpPost]
    [Route("removecart/{bookcode}", Name = "removecart")]
    public IActionResult RemoveCart(string bookcode)
    {
        _cartService.RemoveFromCart(bookcode);
        return RedirectToAction(nameof(Cart));
    }

    [Route("updatecart", Name = "updatecart")]
    [HttpPost]
    public IActionResult UpdateCart([FromForm] string productid, [FromForm] int quantity)
    {
        _cartService.UpdateCart(productid, quantity); // Cập nhật số lượng sản phẩm

        return RedirectToAction(nameof(Cart));
    }

   

    [Route("checkout", Name = "checkout")]
    public IActionResult CheckOut()
    {
        var cartItems = _cartService.GetCartItems(); // Kiểm tra giỏ hàng trước khi thanh toán
        if (!cartItems.Any())
        {
            ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
            return RedirectToAction(nameof(Cart));
        }

        // Xử lý thanh toán
        return View(cartItems);
    }
}
