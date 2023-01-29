using ETicaret.BusinessLayer.Abstract;
using ETicaret.BusinessLayer.Concrete;
using ETicaret.DataAccessLayer.Abstract;
using ETicaret.DataAccessLayer.Concrete.EfCore;
using ETicaret.WebUI.EmailServices;
using ETicaret.WebUI.Identity;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(option=> option.UseSqlServer("Server=DESKTOP-QQB8DP7; Database=DbETicaretCore; Integrated Security=true;"));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

// Identity ile ilgili �zelliklerin konfig�rsayonunu a�a��daki gibi yapabiliriz.
builder.Services.Configure<IdentityOptions>(options => 
{ 
    // password ile ilgili i�lemler
    options.Password.RequireDigit= true;    // Say�sal de�er olabilir.
    options.Password.RequireLowercase= true;
    options.Password.RequireUppercase= true;
    options.Password.RequiredLength= 5;

    // Lockout -- 5 defa yanl�� girildi�inde hesap kilitlenecek.
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromMinutes(5); // kilitlendikten sonra bekleyece�i s�re
    options.Lockout.AllowedForNewUsers= true;   // Yeni kullan�c�ya izin verme i�lemi
    
    options.User.RequireUniqueEmail= true;  // Ayn� email adresi bir defa kullan�labilir.
    options.SignIn.RequireConfirmedEmail= false; // Email onay mesaj�, true ise mail g�nderilecek.
    options.SignIn.RequireConfirmedPhoneNumber= false;
});

builder.Services.ConfigureApplicationCookie(options => 
{ 
    options.LoginPath = "/account/login";   
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath= "/admin/accessdenied";  // sadece giri� yapanlar�n yapabildikleri i�lemler.
    options.SlidingExpiration = true;       // i�lem yap�lmad���nda 20 dakika sonra kullan�c� logout olur. default 20 dakika
    options.ExpireTimeSpan= TimeSpan.FromMinutes(30);    // i�lem yap�lmama s�resi

    options.Cookie = new CookieBuilder
        { HttpOnly = true, Name = ".ETicaret.Security.Cookie" };
        
});

// IoC : Incersion of Control metodu.
// Dependense Injection i�in yapt���m�z tan�mlamalar.
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderService, OrderManager>();

builder.Services.AddScoped<IProductRepository, EfCoreProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
builder.Services.AddScoped<ICartRepository, EfCoreCartRepository>();
builder.Services.AddScoped<IOrderRepository, EfCoreOrderRepository>();

// Mail i�in tan�mlama
builder.Services.AddScoped<IEmailSender, EmailSender>(m => new EmailSender("smtp.office365.com", 587, "onur5234@windowslive.com", "Onur6340.", true));

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    MyInitialData.Seed();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // static dosyalara eri�ebilmek i�in bu satr�n mutlaka eklenmesi gerekiyor.static dosyalar�m�z da wwwroot alt�nda bulunan dosyalar�m�z oluyor. wwwroot alt�ndaki dosyalara eri�ebilmek i�in �rne�in images klas�r�n�n alt�ndaki foto�raflara eri�mek istiyorsam // ~/images/ornek.jpg
            // ~/css/css_dosyas�n�n_ismi
app.UseAuthentication();


app.UseRouting();

app.UseAuthorization();

//app.MapRazorPages();
//app.MapDefaultControllerRoute();

// Sipari� i�lemleri
app.MapControllerRoute(
    name: "orders",
    pattern: "orders",
    defaults: new { controller = "cart", action = "GetOrders" }
    );

// Sepet ��lemleri
app.MapControllerRoute(
    name: "cart",
    pattern: "cart",
    defaults: new { controller = "cart", action = "Index" }
    );

app.MapControllerRoute(
    name: "completeshopping",
    pattern: "completeshopping",
    defaults: new { controller = "cart", action = "CompleteShopping" }
    );



// Admin user i�in
app.MapControllerRoute(
    name: "adminusercreate",
    pattern: "/admin/user/create",
    defaults: new { controller = "admin", action = "UserCreate" }
    );


app.MapControllerRoute(
    name: "adminuserlist",
    pattern: "/admin/user/list",
    defaults: new { controller = "admin", action = "UserList" }
    );

app.MapControllerRoute(
    name: "adminuseredit",
    pattern: "/admin/user/{id?}",
    defaults: new { controller = "admin", action = "UserEdit" }
    );

app.MapControllerRoute(
    name: "adminrolelist",
    pattern: "/admin/role/list",
    defaults: new { controller = "admin", action = "RoleList" }
    );

app.MapControllerRoute(
    name: "rolecreate",
    pattern: "/admin/role/create",
    defaults: new { controller = "admin", action = "RoleCreate" }
    );

app.MapControllerRoute(
    name: "adminroleedit",
    pattern: "/admin/role/{id?}",
    defaults: new { controller = "admin", action = "RoleEdit" }
    );



app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "account", action = "register" }
    );

app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "account", action = "login" }
    );

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "account", action = "logout" }
    );


// Admin category create
app.MapControllerRoute(
    name: "admincategorycreate",
    pattern: "admin/categories/create",
    defaults: new { controller = "admin", action = "createcategory" }
    );

//Admin category edit
app.MapControllerRoute(
    name: "admincategoryedit",
    pattern: "admin/category/{id}",
    defaults: new { controller = "admin", action = "editcategory" }
    );

//Admin category list
app.MapControllerRoute(
    name: "admincategorylist",
    pattern: "admin/categories",
    defaults: new { controller = "admin", action = "categorylist" }
    );



// Admin product create
app.MapControllerRoute(
    name: "adminproductcreate",
    pattern: "admin/products/create",
    defaults: new { controller = "admin", action = "createproduct" }
    );

//Admin product edit
app.MapControllerRoute(
    name: "adminproductedit",
    pattern: "admin/products/{id}",
    defaults: new { controller = "admin", action = "editproduct" }
    );

//Admin product list
app.MapControllerRoute(
    name: "adminproductlist",
    pattern: "admin/products",
    defaults: new { controller = "admin", action = "productlist" }
    );


//Search
app.MapControllerRoute(
    name: "search",
    pattern: "search",
    defaults: new { controller = "shop", action = "search" }
    );



//localhost/about
app.MapControllerRoute(
    name: "about",
    pattern: "about",
    defaults: new { controller = "home", action = "about" }
    );

//localhost/contact
app.MapControllerRoute(
    name: "contact",
    pattern: "contact",
    defaults: new { controller = "home", action = "contact" }
    );

//domain/products
app.MapControllerRoute(
    name: "products",
    pattern: "products/{category?}",
    defaults: new { controller="shop", action="list"}
    );

// A�a��daki link route'u �r�n ayr�nt�s�n� listelemek i�in kullan�lacak.
// domain/samsung-s6
app.MapControllerRoute(
    name: "productdetails",
    pattern: "{url}",
    defaults: new { controller = "shop", action = "details" }
    );


// domain/Home/Index/5
app.MapControllerRoute(
    name:"default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
