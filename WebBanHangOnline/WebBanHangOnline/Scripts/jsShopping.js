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
            data: { id: id, quantity : quantity },
            success: function (rs) {
                if (rs.success) {
                    $('#checkout_items').html(rs.Count)
                    alert(rs.message)
                }
            }
        });
    })

    $('body').on('click', '.btnDeleteCartItem', function (e) { // sử dụng hàm xoá cart item
        e.preventDefault();
        var id = $(this).data('id');
        var confirmMessage = confirm("Bạn thật sự muốn xoá sản phẩm này không?");
        if (confirmMessage == true) {
            $.ajax({
                url: '/shoppingcart/DeleteCartItem',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.success) {

                        //Cập nhật lại Cart Count
                        $('#checkout_items').html(rs.Count)

                        // Xoá dòng 
                        $('#trow_' + id).remove();

                        // Cập nhật lại STT
                        $('.stt-cell').each(function (index) {
                            $(this).text(index + 1);
                        });
                        LoadCart();

                        //location.reload();
                    }
                }
            });
        }
    });

    $('body').on('click', '.btnDeleteAllCart', function (e) { // sử dụng hàm xoá toàn bộ cart
        e.preventDefault();
        var confirmMessage = confirm("Bạn thật sự muốn xoá toàn bộ giỏ hàng không?");
        if (confirmMessage == true) {
            DeleteAll();
            ShowCount();
            LoadCart();
        }

    });

    $('body').on('click', '.btnUpdateCartItem', function (e) { // Sử dụng hàm update cart item
        e.preventDefault();
        var id = $(this).data("id")
        var quantity = $('#soLuong_product_' + id).val();
        UpdateQuantityCartItem(id,quantity);
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
                
            }
        }
    })
}

function UpdateQuantityCartItem(id, quantity) { //Hàm dùng để update số lượng item trong giỏ hàng
    $.ajax({
        url: '/shoppingcart/UpdateQuantityCartItem',
        type: 'POST',
        data: {id: id, quantity: quantity},
        success: function (rs) {
            if (rs.success) {
                LoadCart();
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

