using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Models.Common
{
    public class Message
    {
        public static IHtmlString NoMessage { get; } = new HtmlString("");
        public static IHtmlString SuccessSaveChange { get; } = new HtmlString("Thay đổi thành công");
        public static IHtmlString FailureSaveChange { get; } = new HtmlString("Thay đổi thất bại, vui lòng kiểm tra lại thông tin");
        public static IHtmlString NoRecord { get; } = new HtmlString("Không có bản ghi nào!");
        public static IHtmlString RecordNotChecked { get; } = new HtmlString("Vui lòng chọn bản ghi");
        public static IHtmlString ConfirmDelete { get; } = new HtmlString("Bạn có muốn xoá bản ghi này không?");
        public static IHtmlString ConfirmDeleteSelected { get; } = new HtmlString("Bạn có thật sự muốn xoá các bản ghi này??");
        public static IHtmlString ConfirmDeleteProductImage { get; } = new HtmlString("Bạn có muốn xoá ảnh này không?");
        public static IHtmlString SuccessAddToCart { get; } = new HtmlString("Thêm vào giỏ hàng thành công");
        public static IHtmlString NoItemInCart { get; } = new HtmlString("Chưa có sản phẩm nào trong giỏ hàng");
        public static IHtmlString ConfirmDeleteItemInCart { get; } = new HtmlString("Bạn có thật sự muốn xoá sản phẩm này khỏi giỏ hàng?");
        public static IHtmlString PleaseCheckYourOrderInformationAgain { get; } = new HtmlString("Vui lòng kiểm tra lại thật kỹ thông tin đơn hàng");
        public static IHtmlString RequiredCustomerName { get; } = new HtmlString("Vui lòng nhập tên khách hàng");
        public static IHtmlString RequiredPhone { get; } = new HtmlString("Vui lòng nhập số điện thoại");
        public static IHtmlString RequiredEmail { get; } = new HtmlString("Vui lòng nhập Email");
        public static IHtmlString RequiredAddress { get; } = new HtmlString("Vui lòng nhập địa chỉ");
        public static IHtmlString InvalidEmail { get; } = new HtmlString("Email không hợp lệ");
        public static IHtmlString OrderSuccess { get; } = new HtmlString("Đặt hàng thành công");
        public static IHtmlString OrderFailure { get; } = new HtmlString("Đặt hàng thất bại");
        public static IHtmlString SuccessLogin { get; } = new HtmlString("Đăng nhập thành công");


    }
}