using ETicaret.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ETicaret.WebUI.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        //[Display(Name = "Ürün Adı", Prompt ="Ürün Adını giriniz.")]
        //[Required(ErrorMessage = "Ürün Adı zorunlu bir alandır, boş geçilemez.")]
        //[StringLength(100, MinimumLength = 5, ErrorMessage = "Ürün Adı 5-100 karakter arasında olmalıdır.")]
        public string Name { get; set; }

        //[Display(Name = "Ürün Fiyatı")]
        //[Required(ErrorMessage = "Ürün Fiyatı zorunlu bir alandır, boş geçilemez.")]
        //[Range(0, 100000, ErrorMessage ="Ürün Fiyatı için 1-100.000 arasında bir değer girmelisiniz.")]
        public double? Price { get; set; }

        //[Display(Name = "Ürün Açıklaması", Prompt ="Ürün Açıklamasını giriniz.")]
        //[Required(ErrorMessage = "Ürün Açıklaması zorunlu bir alandır, boş geçilemez.")]
        //[StringLength(1000, MinimumLength = 5, ErrorMessage = "Ürün Açıklaması 5-1000 karakter arasında olmalıdır.")]
        public string Description { get; set; }

        [Display(Name ="Ürün Fotoğrafı", Prompt ="Ürün Fotoğrafını yükleyiniz.")]
        [Required(ErrorMessage = "Ürün Fotoğrafı zorunlu bir alandır, boş geçilemez.")]
        public string ImageUrl { get; set; }

        [Display(Name="Ürün Linki", Prompt ="Ürün linkini giriniz.")]
        [Required(ErrorMessage = "Ürün Linki zorunlu bir alandır, boş geçilemez.")]
        public string Url { get; set; }

        [Display(Name="Onaylı mı")]
        public bool IsApproved { get; set; }

        [Display(Name="Anasayfada gösterilsin mi")]
        public bool IsHome { get; set; }

        public List<Category> SelectedCategories { get; set; }
        public ProductModel()
        {
            SelectedCategories= new List<Category>();
        }
    }
}
