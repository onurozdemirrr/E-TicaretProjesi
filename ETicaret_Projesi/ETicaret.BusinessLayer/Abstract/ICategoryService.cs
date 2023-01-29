using ETicaret.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Abstract
{
    public interface ICategoryService
    {
        Task<Category> GetById(int id);
        Task<List<Category>> GetAll();
        void Delete(Category entity);
        void Update(Category entity);
        void Create(Category entity);
        Task<Category> CreateAsync(Category entity);
        Category GetByIdWithProducts(int categoryId);
        void DeleteFromCategory(int productId, int categoryId);
    }
}
