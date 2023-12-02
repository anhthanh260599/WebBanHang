using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models.Repository;

namespace WebBanHangOnline.Models.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<News> NewsRepository { get; }
        IRepository<Posts> PostsRepository { get; }
        // Thêm các IRepository khác nếu cần
        void UnitOfWorkSaveChanges();
        ApplicationDbContext DbContext { get; } 

    }
}
