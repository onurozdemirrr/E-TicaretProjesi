using ETicaret.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Concrete.EfCore
{
    public class MyInitialData
    {
        private static Category[] Categories = new Category[5]
        {
            new Category() { Name="Telefon", Url="telefon" },
            new Category() { Name="Bilgisayar", Url="bilgisayar" },
            new Category() { Name="Elektronik", Url="elektronik" },
            new Category() { Name="Giyim", Url="giyim" },
            new Category() { Name="Beyaz Eşya", Url="beyaz-esya" }
        };

        private static Product[] Products = new Product[]
        {
            new Product() { Name="Samsung S6", Url="samsung-s6", Description="Kameralı Android telefon 8 gb", ImageUrl="samsungS6.jpg", Price=100, IsApproved=true},
            new Product() { Name="Samsung S7", Url="samsung-s7", Description="Kameralı Android telefon 16 gb", ImageUrl="samsungS7.jpg", Price=200, IsApproved=false},
            new Product() { Name="Samsung S8", Url="samsung-s8", Description="Kameralı Android telefon 128 gb", ImageUrl="samsungS8.jpg", Price=800, IsApproved=true},
            new Product() { Name="Iphone11", Url="iphone-11", Description="Kameralı IOS telefon 64 gb", ImageUrl="iphone11.jpg", Price=900, IsApproved=true},
            new Product() { Name="Iphone14", Url="iphone-14", Description="Kameralı IOS telefon 128 gb", ImageUrl="iphone-14.jfif", Price=1000, IsApproved=true},
            new Product() { Name="Asus", Url="asus", Description="Laptop", ImageUrl="asus.jpg", Price=12000, IsApproved=true},
            new Product() { Name="Toshiba", Url="toshiba", Description="Gamer Laptop", ImageUrl="toshiba.jpg", Price=25100, IsApproved=true},
            new Product() { Name="Lg TV", Url="lg-tv", Description="Full Hd Televizyon", ImageUrl="lgtv.jpg", Price=12000, IsApproved=true},
            new Product() { Name="Takım Elbise", Url="takim-elbise", Description="Slimfit italyan", ImageUrl="takim-elbise.jpg", Price=2000, IsApproved=true},
            new Product() { Name="Arçelik Buzdolabı", Url="arcelik-buzdolabi", Description="A++", ImageUrl="arcelik_buzdolabi.png", Price=100, IsApproved=true},
        };

        private static ProductCategory[] productCategories = new ProductCategory[]
        {
            new ProductCategory() {Product=Products[0], Category=Categories[0]},
            new ProductCategory() {Product=Products[1], Category=Categories[0]},
            new ProductCategory() {Product=Products[2], Category=Categories[0]},
            new ProductCategory() {Product=Products[3], Category=Categories[0]},
            new ProductCategory() {Product=Products[4], Category=Categories[0]},
            new ProductCategory() {Product=Products[5], Category=Categories[1]},
            new ProductCategory() {Product=Products[6], Category=Categories[1]},
            new ProductCategory() {Product=Products[7], Category=Categories[2]},

            new ProductCategory() {Product=Products[0], Category=Categories[2]},
            new ProductCategory() {Product=Products[1], Category=Categories[2]},
            new ProductCategory() {Product=Products[2], Category=Categories[2]},
            new ProductCategory() {Product=Products[3], Category=Categories[2]},
            new ProductCategory() {Product=Products[4], Category=Categories[2]},
            new ProductCategory() {Product=Products[5], Category=Categories[2]},
            new ProductCategory() {Product=Products[6], Category=Categories[2]},            

            new ProductCategory() {Product=Products[8], Category=Categories[3]},
            new ProductCategory() {Product=Products[9], Category=Categories[4]}
        };

        public static void Seed()
        {
            ETicaretContext context = new ETicaretContext();
            if (context.Database.GetPendingMigrations().Count()==0)
            {
                if (context.Categories.Count() == 0)
                {
                    foreach (var cat in Categories)
                    {
                        context.Categories.Add(cat);
                    }
                }
                if (context.Products.Count() == 0)
                {
                    foreach (var prod in Products)
                    {
                        context.Products.Add(prod);
                    }
                    foreach (var prodCat in productCategories)
                    {
                        context.Add(prodCat);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
