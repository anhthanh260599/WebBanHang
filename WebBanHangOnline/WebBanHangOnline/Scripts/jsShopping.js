$(document).ready(function () {
    ShowCount();
    LoadCart();

    $('body').on('click', '.btnAddToCart', function (e) { // sử dụng hàm thêm vào giỏ hàng
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;
        var textQuantity = $('#quantity_value').text();
        if (textQuantity != '') {
            quantity = parseInt(textQuantity);
        }

        $.ajax({
            url: '/shoppingcart/AddToCart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                if (rs.success) {
                    $('#checkout_items').html(rs.Count);
                    //alert(rs.message)
                    toastr.options = {
                        positionClass: 'toast-bottom-right', // Đặt vị trí ở góc phải dưới
                        closeButton: true, // Hiển thị nút X để tắt
                    };
                    toastr.success(rs.message);
                    //Cập nhật lại LOCAL STORAGE
                    $.ajax({
                        url: '/shoppingcart/GetCartJson',
                        type: 'GET',
                        success: function (cartData) {
                             //Kiểm tra xem có dữ liệu trong giỏ hàng hay không
                            if (cartData.Items.length !== 0) {
                                // Lưu dữ liệu vào Local Storage
                                localStorage.setItem('cart', JSON.stringify(cartData));
                                let dataSessionCart = JSON.parse(localStorage.getItem("cart"));
                                if (dataSessionCart != null) {
                                    //lap qua cac item
                                    dataSessionCart.Items.forEach((element) => {
                                        if (element.ProductStoreID != 0) {
                                            let storeIDSession = element.ProductStoreID;
                                            updateStoreSingleton(storeIDSession);
                                            localStorage.setItem("storeID", storeIDSession);
                                            console.log(storeIDSession)
                                            return;
                                        }
                                    })
                                }
                            }
                        }
                    });
                }
                else {
                    toastr.options = {
                        positionClass: 'toast-bottom-right', // Đặt vị trí ở góc phải dưới
                        closeButton: true, // Hiển thị nút X để tắt
                    };
                    toastr.error(rs.message);
                }
            }
        });
    })

        $('body').on('click', '.btnDeleteCartItem', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            Swal.fire({
                title: 'Xác nhận',
                text: 'Bạn thật sự muốn xoá sản phẩm này không?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Xóa'
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/shoppingcart/DeleteCartItem',
                        type: 'POST',
                        data: { id: id },
                        success: function (rs) {
                            if (rs.success) {
                                // Cập nhật lại Cart Count
                                $('#checkout_items').html(rs.Count)

                                // Xoá dòng 
                                $('#trow_' + id).remove();

                                // Cập nhật lại STT
                                $('.stt-cell').each(function (index) {
                                    $(this).text(index + 1);
                                });
                                LoadCart();

                                // Cập nhật LOCAL STORAGE của cart
                                $.ajax({
                                    url: '/shoppingcart/GetCartJson',
                                    type: 'GET',
                                    success: function (cartData) {
                                        console.log(cartData);
                                        // Kiểm tra xem có dữ liệu trong giỏ hàng hay không
                                        if (cartData.Items.length !== 0) {
                                            // Lưu dữ liệu vào Local Storage
                                            localStorage.setItem('cart', JSON.stringify(cartData));
                                        } else {
                                            localStorage.setItem('cart', null);
                                        }
                                    }
                                });

                                Swal.fire(
                                    'Xóa thành công!',
                                    '',
                                    'success'
                                );
                            }
                        }
                    });
                }
            });
        });

    //$('body').on('click', '.btnDeleteCartItem', function (e) { // sử dụng hàm xoá cart item
    //    e.preventDefault();
    //    var id = $(this).data('id');
    //    var confirmMessage = confirm("Bạn thật sự muốn xoá sản phẩm này không?");
    //    if (confirmMessage == true) {
    //        $.ajax({
    //            url: '/shoppingcart/DeleteCartItem',
    //            type: 'POST',
    //            data: { id: id },
    //            success: function (rs) {
    //                if (rs.success) {

    //                    //Cập nhật lại Cart Count
    //                    $('#checkout_items').html(rs.Count)

    //                    // Xoá dòng
    //                    $('#trow_' + id).remove();

    //                    // Cập nhật lại STT
    //                    $('.stt-cell').each(function (index) {
    //                        $(this).text(index + 1);
    //                    });
    //                    LoadCart();
    //                    //location.reload();

    //                    // Cập nhật LOCAL STORAGE của cart
    //                    $.ajax({
    //                        url: '/shoppingcart/GetCartJson',
    //                        type: 'GET',
    //                        success: function (cartData) {
    //                            console.log(cartData);
    //                            //Kiểm tra xem có dữ liệu trong giỏ hàng hay không
    //                            if (cartData.Items.length !== 0) {
    //                                // Lưu dữ liệu vào Local Storage
    //                                localStorage.setItem('cart', JSON.stringify(cartData));
    //                            } else {
    //                                localStorage.setItem('cart', null);
    //                            }
    //                        }
    //                    });
    //                }
    //            }
    //        });
    //    }
    //});

    $('body').on('click', '.btnDeleteAllCart', function (e) {
        e.preventDefault();

        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn thật sự muốn xoá toàn bộ giỏ hàng không?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Xóa'
        }).then((result) => {
            if (result.isConfirmed) {
                DeleteAll();
                ShowCount();
                LoadCart();
                Swal.fire(
                    'Xóa thành công!',
                    '',
                    'success'
                ).then(() => {
                    location.reload();
                })
            }
        });
    });

    //$('body').on('click', '.btnDeleteAllCart', function (e) { // sử dụng hàm xoá toàn bộ cart
    //    e.preventDefault();
    //    var confirmMessage = confirm("Bạn thật sự muốn xoá toàn bộ giỏ hàng không?");
    //    if (confirmMessage == true) {
    //        DeleteAll();
    //        ShowCount();
    //        LoadCart();
    //        location.reload();
    //    }

    //});

    $('body').on('click', '.btnUpdateCartItem', function (e) { // Sử dụng hàm update cart item
        e.preventDefault();
        var id = $(this).data("id")
        var quantity = $('#soLuong_product_' + id).val();
        UpdateQuantityCartItem(id, quantity);
    });

})

function ShowCount() { //Hàm dùng để lưu Session Cart
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'POST',
        success: function (rs) {
            $('#checkout_items').html(rs.Count)
        }
    })
}

function DeleteAll() { //Hàm dùng để xoá toàn bộ giỏ hàng
    $.ajax({
        url: '/shoppingcart/DeleteAllCart',
        type: 'POST',
        success: function (rs) {
            if (rs.success) {
                LoadCart();
                ShowCount();
                // Xoá toàn bộ LOCAL STORAGE của cart sau khi xoá giỏ hàng
                localStorage.removeItem('cart');
            }
        }
    })
}

function UpdateQuantityCartItem(id, quantity) { //Hàm dùng để update số lượng item trong giỏ hàng
    $.ajax({
        url: '/shoppingcart/UpdateQuantityCartItem',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            if (rs.success) {
                LoadCart();

                // Cập nhật LOCAL STORAGE của cart sau khi cập nhật số lượng
                $.ajax({
                    url: '/shoppingcart/GetCartJson',
                    type: 'GET',
                    success: function (cartData) {
                        console.log(cartData);
                        //Kiểm tra xem có dữ liệu trong giỏ hàng hay không
                        if (cartData.Items.length !== 0) {
                            // Lưu dữ liệu vào Local Storage
                            localStorage.setItem('cart', JSON.stringify(cartData));
                        } else {
                            localStorage.removeItem('cart');
                        }
                    }
                });

            }
        }
    })
}

function LoadCart() { //Hàm dùng để load Cart
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'POST',
        success: function (rs) {
            $('#load_data').html(rs)
        }
    })
}

