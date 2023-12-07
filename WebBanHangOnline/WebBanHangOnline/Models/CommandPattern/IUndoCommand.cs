using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHangOnline.Models.CommandPattern
{
    public interface IUndoCommand
    {
        void Undo();
    }
}
