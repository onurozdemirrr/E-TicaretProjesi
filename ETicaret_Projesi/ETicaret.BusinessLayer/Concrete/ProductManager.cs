using ETicaret.BusinessLayer.Abstract;
using ETicaret.DataAccessLayer.Abstract;
using ETicaret.DataAccessLayer.Concrete.EfCore;
using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;
        // Aşaıdaki gibi new'leyerek kodu yazdığımızda ilgili sınıfa bağımlı hale geliyoruz. Bu sorunu ortadan kaldırmak için bağımlılığı soyut lamamız gerekiyor. Burada da interface'lerden faydalanıyoruz. Yukarıdaki tanımlama ve aşağıdaki constructır ile DI'nı uygulamış aldık.
        //EfCoreProductRepository db = new EfCoreProductRepository();
        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public string ErrorMessage { get; set; }

        public bool Create(Product product)
        {
            // iş kurallarının denetiminin yapılacağı alan
            if (Validation(product))
            {
                _productRepository.Create(product);
                return true;
            }
            return false;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepository.CreateAsync(product);
            return product;
        }

        public void Delete(Product product)
        {
            _productRepository.Delete(product);
        }

        public async Task<List<Product>> GetAll()
        {
            return await _productRepository.GetAll();
        }

        public async Task<Product> GetById(int id)
        {
            return await _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public List<Product> GetPopularProducts()
        {
            throw new NotImplementedException();
        }

        public List<Product> GetProductByCategory(string categoryName, int page, int pageSize)
        {
            return _productRepository.GetProductByCategory(categoryName, page, pageSize);
        }

        public Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _productRepository.GetSearchResult(searchString);
        }

        public List<Product> GetTopFiveProducts()
        {
            throw new NotImplementedException();
        }

        public void Update(Product product)
        {
            _productRepository.Update(product);
        }

        public bool Update(Product product, int[] categoryIds)
        {
            if (Validation(product))
            {
                if (categoryIds.Length == 0)
                {
                    ErrorMessage += "Ürün için en az bir kategori seçmelisiniz.";
                    return false;
                }
                _productRepository.Update(product, categoryIds);
                return true;
            }
            return false;
        }

        public bool Validation(Product entity)
        {
            bool isValid = true;
            if (string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "Ürün Adı girilmeli\n";
                isValid = false;
            }
            if (string.IsNullOrEmpty(entity.Description))
            {
                ErrorMessage += "Ürün Açıklaması girilmeli\n";
                isValid = false;
            }
            if (entity.Price < 0)
            {
                ErrorMessage += "Ürün fiyatı negatif bir değer olamaz.\n";
                isValid = false;
            }
            return isValid;
        }
    }
}
