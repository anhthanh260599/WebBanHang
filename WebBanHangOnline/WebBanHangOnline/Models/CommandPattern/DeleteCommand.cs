using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.CommandPattern
{
    public class DeleteCommand<T> : ICommand<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly int _itemId;

        public DeleteCommand(ApplicationDbContext db, int itemId) 
        {
            _db = db;
            _itemId = itemId;
        }
        public void Execute()
        {
            var item = _db.Set<T>().Find(_itemId);
            if (item != null)
            {
                _db.Set<T>().Remove(item);
                _db.SaveChanges();
            }
        }
    }
}