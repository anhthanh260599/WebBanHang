using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.MySingleton
{
    public class StoreSingleton
    {
        private static StoreSingleton _instance;
        private static readonly object _lockObject = new object();

        private StoreSingleton()
        {
            // Khởi tạo
            //Id = 0;
        }

        public static StoreSingleton Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new StoreSingleton();
                    }
                    return _instance;
                }
            }
        }

        public int Id { get; set; }
    }
}