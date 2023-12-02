using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.CommandPattern
{
    public class DeleteMultipleCommand<T> : ICommand<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly string[] _itemIds;

        public DeleteMultipleCommand(ApplicationDbContext db, string[] itemIds)
        {
            _db = db;
            _itemIds = itemIds;
        }

        public void Execute()
        {
            if(_itemIds != null && _itemIds.Any())
            {
                foreach(var itemId in _itemIds) 
                { 
                    var obj = _db.Set<T>().Find(Convert.ToInt32(itemId));
                    if (obj != null)
                    {
                        _db.Set<T>().Remove(obj);
                    }
                }
                _db.SaveChanges();
            }
        }
    }
}