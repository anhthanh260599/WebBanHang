using PayPal.Api;
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
        private List<T> _items;

        public DeleteMultipleCommand(ApplicationDbContext db, string[] itemIds)
        {
            _db = db;
            _itemIds = itemIds;
        }

        public void Execute()
        {
            _items = new List<T>();

            if (_itemIds != null && _itemIds.Any())
            {
                foreach (var itemId in _itemIds)
                {
                    var obj = _db.Set<T>().Find(Convert.ToInt32(itemId));
                    if (obj != null)
                    {
                        _items.Add(obj);
                        _db.Set<T>().Remove(obj);
                    }
                }
                _db.SaveChanges();
            }
        }

        public void Undo()
        {
            if (_items != null && _items.Any())
            {
                using (var undoContext = new ApplicationDbContext())
                {
                    foreach (var item in _items)
                    {
                        undoContext.Set<T>().Add(item);
                    }
                    undoContext.SaveChanges();
                }
            }
        }
    }
}