using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Abstract
{
    public interface IProductService: IValidator<Product>
    {
        Task<Product> GetById(int id);
        Task<List<Product>> GetAll();
        void Delete(Product product);
        void Update(Product product);
        bool Create(Product product);
        Task<Product> CreateAsync(Product product);
        List<Product> GetProductByCategory(string categoryName, int page, int pageSize);
        List<Product> GetTopFiveProducts();
        List<Product> GetPopularProducts();
        List<Product> GetSearchResult(string searchString);
        Product GetProductDetails(string url);
        int GetCountByCategory(string category);
        List<Product> GetHomePageProducts();

        Product GetByIdWithCategories(int id);
        bool Update(Product product, int[] categoryIds);
    }
}
