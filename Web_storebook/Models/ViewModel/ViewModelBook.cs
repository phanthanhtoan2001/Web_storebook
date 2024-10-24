using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web_storebook.Models.ViewModel
{
    public class ViewModelBook
    {
        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Isbn { get; set; } = null!;

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public string? Description { get; set; }

        public DateOnly PublicationDate { get; set; }

        public int StockQuantity { get; set; }

        public int? PublisherId { get; set; }

        public int? CategoryId { get; set; }

        public string? Language { get; set; }

        public int? Pages { get; set; }

        public decimal? Weight { get; set; }

        public string? Dimension { get; set; }

        public string? CoverType { get; set; }

        public string? ImageBook {  get; set; } 

        public List<SelectListItem> PublisherList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();




    }
}
