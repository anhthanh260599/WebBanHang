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
        private T _item;

        public DeleteCommand(ApplicationDbContext db, int itemId) 
        {
            _db = db;
            _itemId = itemId;
        }
        public void Execute()
        {
            _item = _db.Set<T>().Find(_itemId);
            if (_item != null)
            {
                _db.Set<T>().Remove(_item);
                _db.SaveChanges();
            }
        }

        public void Undo()
        {
            if(_item != null)
            {
                using (var undoContext = new ApplicationDbContext())
                {
                    undoContext.Set<T>().Add(_item);
                    undoContext.SaveChanges();
                }
            }
        }
    }
}