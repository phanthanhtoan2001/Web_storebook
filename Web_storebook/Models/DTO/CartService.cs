using Newtonsoft.Json;

namespace Web_storebook.Models.DTO
{
    public class CartService
    {
        private readonly BookStoreDbContext _dbContext;
        private readonly ISession _session;

        public CartService(BookStoreDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _session = httpContextAccessor.HttpContext.Session; // Sử dụng session để lưu trữ giỏ hàng
        }

        public void AddToCart(Book book)
        {
            var cart = GetCart(); // Lấy giỏ hàng hiện tại từ session
            var cartItem = cart.FirstOrDefault(item => item.BookCode == book.BookCode);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
            }
            else
            {
                cart.Add(new CartItem { BookCode = book.BookCode, Book = book, Quantity = 1 }); // Thêm sản phẩm mới vào giỏ hàng
            }

            SaveCart(cart); // Lưu giỏ hàng vào session
        }
        public List<CartItem> GetCartItems()
        {
            return GetCart(); // Lấy danh sách các sản phẩm trong giỏ hàng
        }

        private List<CartItem> GetCart()
        {
            var cartJson = _session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }
        public int GetCartItemCount()
        {
            var cart = GetCart(); // Retrieve the current cart from session
            return cart.Sum(item => item.Quantity); // Sum the quantities of all cart items
        }
        private void SaveCart(List<CartItem> cart)
        {
            if (_session == null)
            {
                Console.WriteLine("Session is null.");
                return;
            }
            Console.WriteLine("Saving Cart: " + JsonConvert.SerializeObject(cart));

            _session.SetString("Cart", JsonConvert.SerializeObject(cart));

        }

        public void RemoveFromCart(string bookcode)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.BookCode.ToString() == bookcode);

            if (cartItem != null)
            {
                cart.Remove(cartItem); // Xóa sản phẩm khỏi giỏ hàng
                SaveCart(cart);
            }
        }
        public void UpdateCart(string bookcode, int quantity)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.BookCode.ToString() == bookcode);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity; // Cập nhật số lượng sản phẩm
                SaveCart(cart);
            }
        }
        public class CartItem
        {
            public string BookCode { get; set; }
            public Book Book { get; set; }
            public int Quantity { get; set; }
        }

    }
}