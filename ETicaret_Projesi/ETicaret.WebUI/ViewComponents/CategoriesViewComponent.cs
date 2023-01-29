using ETicaret.BusinessLayer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ETicaret.WebUI.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        private ICategoryService _categoryService;
        public CategoriesViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Burada ilgili kodların olması gerekiyor. Örneğin veritabanından istenen verilerin alınması gibi.
            if (RouteData.Values["category"] != null)
            {
                ViewBag.SelectCategory = RouteData?.Values["category"];
            }
            return View(await _categoryService.GetAll());
        }
    }
}

/*
 ViewComponent nedir?
PartialView'de bir data kullanılacaksa bu data kesinlikle Controller'dan gelmek zorunda. Başka Yolu yok.
* ComponentView ise Contoller'ı devreden çıkarır ve kendisine ait olan (yani bu class CategoriesViewComponent) sınıfına gider verisini alır ve bu datayı View'inde kullanır.
* Öncellikle cskısmını yazın(yani bu dosya). WebUI katmanı içinde ViewComponents isimli bir klasör oluşturun.(Bu isim zorunlu değil) İçinde de bir class oluşturuyoruz. Class'a bir isim veriyoruz(Categories) ismin sonuna da (ViewComponent) ekliyoruz.(yani CategoriesViewComponent sınıfı).
* Daha sonra bu sınıfı ViewComponent'ten türetiyoruz. Yani bu sınıfın miras almasını sağlıyoruz. (using Microsoft.AspNetCore.Mvc; altında bulunan bir sınıftır.)
* ViewComponent'in tetiklenmesi için de aşağıdaki metodu barındırması gerekiyor. Bu bir kuraldır.
            

        public IViewComponentResult Invoke()
        {
            // Burada ilgili kodların olması gerekiyor. Örneğin veritabanından istenen verilerin alınması gibi.
            return View(_categoryService.GetAll());
        }

* ilgili View nerede barındırılacak/konumlandırılacak?
    - 2 farklı yerde konumlandırılabilir.
        1 - Views klasörü altında ilgili Controller klasörünün altında Components klasörü olmalı. Bu klasör altında da bu sınıf ismi ile aynı ada sahip bir klasör daha oluşturmalıyız. İlgili view bu klasörün içinde olmalı.
        View/Home/Components/Categoris/Default.cshtml

        2 - Shared klasörü altında Component altında ve Class ismi oluşturulan klasörün içinde barındırmamız gerekiyor.
        View/Shared/Components/Categoris/Default.cshtml

* View ismi ne olmalı?
    - Eğer farklı bir isim vermeyeceksek Default ismini kullanabiliriz. Bu durumda bunu return View() kısmında paremetre olarak vermemize gerek yok.
    - Farklı bir isim vereceksek bu durumda bu ismi Invoke() metodu içindeki View içinde belirtmemiz gerekecek.

* ViewComponent'i kullanmak istediğimizde de ilgili View içinde aşağıdaki gibi kullanabiliriz.

    @await Component.InvokeAsync("Categories"); ya da
    @Component Invoke("Categories"); 
 
*/