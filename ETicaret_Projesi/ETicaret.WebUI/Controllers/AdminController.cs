using ETicaret.BusinessLayer.Abstract;
using ETicaret.Entities;
using ETicaret.WebUI.Identity;
using ETicaret.WebUI.Models;
using ETicaret.WebUI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using Product = ETicaret.Entities.Product;

namespace ETicaret.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    // Role: Admin
    // Standart
    // Customer

    public class AdminController : Controller
    {
        private IProductService _productService;
        private ICategoryService _categoryService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;

        public AdminController(IProductService productService, ICategoryService categoryService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonMembers = new List<User>();

            foreach (var user in _userManager.Users.ToList())
            {
                //IsInRoleAsync(user, role.Name) metodu bir user nesnesi alır, role ismini alır ve veritabanından kontrol eder. Eğer ilgili role user'a atanmış ise true değerini döndürür. Atanmamış ise false değerini döndürür.

                //1. yol
                //if (await _userManager.IsInRoleAsync(user, role.Name))
                //{
                //    members.Add(user);
                //}
                //else
                //{
                //    nonMembers.Add(user);
                //}

                //Aşağıdaki satırda tanımladığım list değişkeninin referansını değiştiriyorum. IsInRoleAsync metodundan gelen True/False değerine göre list değişkeninin referansı değişiyor. True ise list değişkeni members'ın referansına sahip oluyor. false ise nonMembers referansına sahip oluyor.
                //list.add(user); satırı ile de user ilgili listin içine eklenmiş oluyor.
                //2.yol
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);     // Ternery Operatörü ?:

            }
            var model = new RoleDetails()
            {
                Members = members,
                NonMembers = nonMembers,
                Role = role
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            ModelState.Remove("IdsToAddRole");
            ModelState.Remove("IdsRemoveFromRole");
            if (ModelState.IsValid)
            {
                // Role eklenecek kullanıcılar
                foreach (var userId in model.IdsToAddRole ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }

                // User'ı Role'den silmek için 
                foreach (var userId in model.IdsRemoveFromRole ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                }
            }

            return Redirect("/admin/role/" + model.RoleId);
        }

        [HttpPost]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("RoleList");
        }

        // Erişim olmadığı durumlarda kullanılan AccessDenied Action'ı
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // User işlemleri
        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }

        public IActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCreate(UserDetailsModel model)
        {
            ModelState.Remove("SelectedRoles");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    PasswordHash = model.Password
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{model.UserName} isimli kullanıcı kaydedildi.",
                        AlertType = "success"
                    };
                    TempData["message"] = JsonConvert.SerializeObject(msg);
                    return Redirect("/admin/user/list");
                }
                else
                {
                    var msg = new AlertMessage()
                    {
                        Message = $"{model.UserName} isimli kullanıcı kaydedilemedi.",
                        AlertType = "danger"
                    };
                    TempData["message"] = JsonConvert.SerializeObject(msg);
                    return View(model);
                }
                
                
            }
            return View(model);
        }

        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(x => x.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles,
                });
            }
            return Redirect("/admin/user/list");
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserDetailsModel model, string[] selectedRoles)
        {
            ModelState.Remove("SelectedRoles");
            // Değişiklikleri alıp güncelleme işlemi yap.
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.EmailConfirmed = model.EmailConfirmed;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);     //Identity'nin içerisindeki metotlardan. User nesnesini verince, ilgili user'a ait Role'leri getiriyor.

                        selectedRoles = selectedRoles ?? new string[] { };
                        await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles).ToArray<string>());
                        // Except ile selectedRoles içinden kullanıcının sahip olduğu roller çıkarılıyor ve kalanlar veritabanına ekleniyor. Yani yeni eklenen Roller bu kullanıcı için ilgili tabloya eklenmiş oluyor.

                        await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles).ToArray<string>());
                        // Burada da yukarıdaki işlemin tam tersi bir işlem yapılıyor. user rolleri içerisinden selectedRole'ler çıkarılıyor ve kalanlar siliniyor.

                        return Redirect("/admin/user/list");
                    }
                }
                return Redirect("/admin/user/list");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UserList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("UserList");
        }

        public async Task<IActionResult> ProductList()
        {
            var productListViewModel = new ProductListViewModel()
            {
                Products = await _productService.GetAll()
            };
            return View(productListViewModel);
        }


        [HttpGet]
        public IActionResult CreateProduct()
        {
            // Boş bir form gönderecek.
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductModel model, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                // Formu dolrduktan sonra burada da kayıt işlemini yapacağız.
                // Boş product nesnesi oluşturulacak. (Entity'deki Product clasında)
                Product product = new Product();
                // Parametredeki model nesnesi içindeki veriler Product nesnesi içine aktarılır.
                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Url = model.Url;
                //product.ImageUrl = model.ImageUrl;
                // Verileri alan product nesnesi veritabanına kaydedilmek için ilgilimetoda parametre olarak verilir.

                if (file != null)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var randomName = string.Format($"{Guid.NewGuid()} {extension}");
                    product.ImageUrl = randomName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", randomName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                if (_productService.Create(product))
                {
                    CreateMessage("Kayıt eklendi", "success");
                    return RedirectToAction("ProductList");
                }
                CreateMessage(_productService.ErrorMessage, "danger");
                return View(model);
            }

            return View(model);


        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            // İlgili ürünü Edit sayfasına göndereceğiz.
            if (id == null)
            {
                return NotFound();
            }
            Product entity = _productService.GetByIdWithCategories((int)id);
            if (entity == null)
            {
                return NotFound();
            }
            ProductModel model = new ProductModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Url = entity.Url,
                ImageUrl = entity.ImageUrl,
                Price = entity.Price,
                IsApproved = entity.IsApproved,
                IsHome = entity.IsHome,

                SelectedCategories = entity.ProductCategories.Select(c => c.Category).ToList()
            };
            ViewBag.Categories = await _categoryService.GetAll();

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, int[] categoryIds, IFormFile? file)
        {
            // 3- Fotoğrafı yakalamak için parametreye IFormFile fle ekliyoruz.

            // Validation işlemi yapıldıktan sonra kurala uymayan veri olup olmadığının kontrolünü yapıyoruz ModelState.Isvalid ile yapıyoruz.
            // hata mesajlarını da ilgili cshtml asp-validation-for ile ekrana yazdırıyoruz.
            if (ModelState.IsValid)
            {
                // Formu doldurduktan sonra burada da kayıt işlemini gerçekleştireceğiz.
                //Veritabanından ilgili kayıt alınır
                Product product = await _productService.GetById(model.Id);
                // Model içindeki veriler güncellenen veriler olduğu için veritabanından gelen Product nesnesi içine yerleştirilir.
                if (product == null)
                {
                    return NotFound();
                }
                product.Name = model.Name;
                product.Description = model.Description;
                product.Url = model.Url;
                product.Price = model.Price;
                //product.ImageUrl = model.ImageUrl;
                product.IsApproved = model.IsApproved;
                product.IsHome = model.IsHome;
                // Değiştirilen Product nesnesi update metodu ile veritabanına gönderilir ve güncelleme işi biter

                if (file != null)
                {
                    //4- parametredeki file null gelmediyse bu durumda dosyaya eşsiz bir isim ile wwwroot/images altına kaydetmemiz gerekecek.
                    // Aşağıdaki kod ile dosyanın uzantısını alıyoruz.
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    // Aşağıda dosyayı kaydetmek için kullanacağımız isme eşsiz bir isim yaratıyoruz. Guid'ten faydalanıyoruz.
                    var randomName = string.Format($"{Guid.NewGuid()} {extension}");
                    // Product içindeki alana oluşturduğumuz dosya adını yazdırıyoruz.
                    product.ImageUrl = randomName;

                    // Dosyanın kaydedileceği path'i ve dosya adını veriyoruz.
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", randomName);

                    // Aşağıdaki satırda da dosya adı ve yolu verilen yere fotoğrafı kaydediyoruz.
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }

                if (_productService.Update(product, categoryIds))
                {
                    CreateMessage("Ürün güncellendi", "success");
                    return RedirectToAction("ProductList");
                }
                CreateMessage(_productService.ErrorMessage, "danger");
            }
            ViewBag.Categories = await _categoryService.GetAll();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Id'si gönderilen ürünü sileceğiz.
            // Parametredeki Id kullanılarak veritabanından ilgili Product nesnesi alınır.
            // Product nesnesi Delete metoduna parametre olarak verilerek silme işlemi veritabanından yapılır.
            Product product = await _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            _productService.Delete(product);

            var msg = new AlertMessage()
            {
                Message = $"{product.Name} isimli ürün silindi.",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("ProductList");

        }


        // Kategori işlemleri
        public async Task<IActionResult> CategoryList()
        {
            var categoryListVievModel = new CategoryListViewModel()
            {
                Categories = await _categoryService.GetAll()
            };
            return View(categoryListVievModel);
        }

        [HttpGet]
        public IActionResult Editcategory(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category entity = _categoryService.GetByIdWithProducts((int)id);
            if (entity == null)
            {
                return NotFound();
            }
            CategoryModel model = new CategoryModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p => p.Product).ToList()

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryModel model)
        {
            if (ModelState.IsValid)
            {
                Category category = await _categoryService.GetById(model.Id);
                if (category == null)
                {
                    return NotFound();
                }
                category.Name = model.Name;
                category.Url = model.Url;

                _categoryService.Update(category);

                var msg = new AlertMessage()
                {
                    Message = $"{category.Name} isimli kategori güncellendi.",
                    AlertType = "success"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);

                return RedirectToAction("CategoryList");
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult CreateCategory()
        {
            // Boş bir form gönderecek.
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryModel model)
        {
            ModelState.Remove("Products");
            if (ModelState.IsValid)
            {
                Category entity = new Category()
                {
                    Name = model.Name,
                    Url = model.Url
                };

                await _categoryService.CreateAsync(entity);

                var msg = new AlertMessage()
                {
                    Message = $"{entity.Name} isimli kategori kaydedildi.",
                    AlertType = "success"
                };
                TempData["message"] = JsonConvert.SerializeObject(msg);
                return RedirectToAction("CategoryList");
            }
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            Category category = await _categoryService.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            _categoryService.Delete(category);

            var msg = new AlertMessage()
            {
                Message = $"{category.Name} isimli kategori silindi.",
                AlertType = "danger"
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);

            return RedirectToAction("CategoryList");

        }

        public IActionResult DeleteFromCategory(int productId, int categoryId)
        {
            _categoryService.DeleteFromCategory(productId, categoryId);
            return Redirect("/admin/category/" + categoryId);
        }

        private void CreateMessage(string message, string alertType)
        {
            var msg = new AlertMessage()
            {
                Message = message,
                AlertType = alertType
            };
            TempData["message"] = JsonConvert.SerializeObject(msg);
        }
    }
}
