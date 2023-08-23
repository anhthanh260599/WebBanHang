using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.Common
{
    public class Message
    {
        public IHtmlString NoRecord { get; } = new HtmlString("Không có bản ghi nào!");
        public IHtmlString RecordNotChecked { get; } = new HtmlString("Vui lòng chọn bản ghi");
        public IHtmlString ConfirmDelete { get; } = new HtmlString("Bạn có muốn xoá bản ghi này không?");

        public IHtmlString ConfirmDeleteSelected { get; } = new HtmlString("Bạn có thật sự muốn xoá các bản ghi này??");
    }
}