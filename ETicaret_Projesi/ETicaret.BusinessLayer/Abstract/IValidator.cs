using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaret.BusinessLayer.Abstract
{
    public interface IValidator<T>
    {
        // Generic bir yapı oluşturuyorum çünkü hem product için hem de category için Validation işlemlerinde kullanabilirim.
        string ErrorMessage { get; set; }   // hata mesajlarını burada tutacağım.
        bool Validation(T entity);  // Gönderilen entity'i validate etsin ve sonuç başarılı mı değil mi geriye true ya da false olarak göndersin.
    }
}
