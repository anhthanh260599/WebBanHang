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
        public static IHtmlString Brand { get; } = new HtmlString("FourC");
        public static IHtmlString PleaseLogin { get; } = new HtmlString("Vui lòng đăng nhập");
        public static IHtmlString SuccessDeleted { get; } = new HtmlString("Xoá thành công!");
        public static IHtmlString FailDeleted { get; } = new HtmlString("Xoá thất bại!");
        public static IHtmlString SuccessDeletedSelected { get; } = new HtmlString("Các mục đã được xoá thành công!");
        public static IHtmlString NoMessage { get; } = new HtmlString("");
        public static IHtmlString SuccessSaveChange { get; } = new HtmlString("Cập nhật thành công");
        public static IHtmlString FailureSaveChange { get; } = new HtmlString("Cập nhật thất bại, vui lòng kiểm tra lại thông tin");
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
        public static IHtmlString WrongUserNameOrPassword { get; } = new HtmlString("Sai tên tài khoản hoặc mật khẩu");
        public static IHtmlString ConfirmDeleteItemInWishList { get; } = new HtmlString("Bạn có thật sự muốn xoá sản phẩm này khỏi yêu thích?");
        public static IHtmlString SuccessSubscribe { get; } = new HtmlString("Subscribe thành công! Chúng tôi sẽ gửi email cho bạn trong tương lai");
        public static IHtmlString FailureSubscribe { get; } = new HtmlString("Email này đã subscribe, vui lòng sử dụng Email khác!");
        public static IHtmlString SuccessReview { get; } = new HtmlString("Gửi đánh giá thành công, đánh giá của bạn sẽ hiển thị nếu được duyệt");
        public static IHtmlString PleaseDontUseBadWord { get; } = new HtmlString("Vui lòng không sử dụng từ ngữ tiêu cực");
        public static IHtmlString SuccessAddWishList { get; } = new HtmlString("Thêm vào danh sách yêu thích thành công");
        public static IHtmlString OutTime { get; } = new HtmlString("Chưa đến giờ mua hàng, vui lòng quay lại từ 6:00 - 22:00");

        public static IHtmlString ChooseTypePayment { get; } = new HtmlString("Vui lòng chọn phương thức thanh toán");

    }
}