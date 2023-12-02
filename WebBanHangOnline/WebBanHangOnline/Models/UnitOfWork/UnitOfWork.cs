using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;
using WebBanHangOnline.Models.Repository;


namespace WebBanHangOnline.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;
        private IRepository<News> _newsRepository;
        private IRepository<Posts> _postsRepository;
        private IUnitOfWork _unitOfWork;

        public UnitOfWork()
        {
            db = new ApplicationDbContext();
        }

        public IRepository<News> NewsRepository
        {
            get
            {
                if(_newsRepository == null)
                {
                    _newsRepository = new GenericRepository<News>(this);
                }
                return _newsRepository;
            }
        }

        public IRepository<Posts> PostsRepository
        {
            get
            {
                if(_postsRepository == null)
                {
                    _postsRepository = new GenericRepository<Posts>(this);
                }
                return _postsRepository;
            }
        }

        public void SaveChanges()
        {
            db.SaveChanges();
        }
    }
}