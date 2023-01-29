﻿using ETicaret.DataAccessLayer.Abstract;
using ETicaret.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ETicaret.DataAccessLayer.Concrete.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ETicaretContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id)
        {
            using (var context = new ETicaretContext())
            {
                var result = context.Products
                .Where(p => p.Id == id)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .FirstOrDefault();
                return result;
            }

        }

        public int GetCountByCategory(string category)
        {
            using (var context = new ETicaretContext())
            {
                var products = context.Products.Where(x => x.IsApproved).AsQueryable();

                if (!string.IsNullOrEmpty(category))
                {
                    products = products.Include(x => x.ProductCategories).ThenInclude(x => x.Category).Where(x => x.ProductCategories.Any(y => y.Category.Url == category));
                }

                return products.Count();
            }
        }

        public List<Product> GetHomePageProducts()
        {
            using (var context = new ETicaretContext())
            {
                return context.Products.Where(x => x.IsHome == true && x.IsApproved == true).ToList();
            }
        }

        public List<Product> GetPopularProducts()
        {
            using (var context = new ETicaretContext())
            {
                return context.Products.ToList();
            }
        }

        public List<Product> GetProductByCategory(string categoryName, int page, int pageSize)
        {
            using (var context = new ETicaretContext())
            {
                var products = context.Products.Where(x => x.IsApproved).AsQueryable();

                if (!string.IsNullOrEmpty(categoryName))
                {
                    products = products.Include(x => x.ProductCategories).ThenInclude(x => x.Category).Where(x => x.ProductCategories.Any(y => y.Category.Url == categoryName));
                }

                // Skip(X) Örneğin X,5 ise sorgu sonucu ilk 5 kaydı almadan diğerlerini alacak.
                // Take(Y) Örn Y=3 ise o 3 veriyi bana getirir.
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public Product GetProductDetails(string url)
        {
            using (var context = new ETicaretContext())
            {
                return context.Products.Where(p => p.Url == url).Include(i => i.ProductCategories).ThenInclude(c => c.Category).FirstOrDefault();
            }
        }

        public List<Product> GetSearchResult(string searchString)
        {

            // Kullanıcı bir ürünü aramak için search'ü kullandığında, girmiş olduğu string değeri 2 farklı noktada arayacağız. 1. Product içindeki name alanında, 2. Product içindeki detail alanında.
            // Diğer bir koşul da ürünün onaylanmış olması (IsApproved)
            using (var context = new ETicaretContext())
            {
                IQueryable<Product> products;
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = context.Products.Where(x => x.IsApproved && (x.Name.Contains(searchString.ToLower().Trim()) || x.Description.Contains(searchString.ToLower().Trim()))).AsQueryable();
                }
                else
                {
                    products = context.Products.Where(x => x.IsApproved).AsQueryable();
                }
                
                return products.ToList();
            }
            

        }

        public List<Product> GetTopFiveProducts()
        {
            throw new NotImplementedException();
        }

        public void Update(Product product, int[] categoryIds)
        {
            using (var context = new ETicaretContext())
            {
                var entity = context.Products
                    .Include(i => i.ProductCategories)
                    .FirstOrDefault(p => p.Id == product.Id);

                if (entity != null)
                {
                    entity.Name = product.Name;
                    entity.Description = product.Description;
                    entity.Price = product.Price;
                    entity.Url = product.Url;
                    entity.ImageUrl = product.ImageUrl;
                    entity.IsApproved = product.IsApproved;
                    entity.IsHome = product.IsHome;

                    entity.ProductCategories = categoryIds.Select(catId => new ProductCategory()
                    {
                        ProductId = product.Id,
                        CategoryId = catId,
                    }).ToList();
                    context.SaveChanges();

                }
            }
        }
    }
}
