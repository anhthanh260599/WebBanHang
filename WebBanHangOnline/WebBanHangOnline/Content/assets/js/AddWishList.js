jQuery(document).ready(function ($) {
    initFavorite();

    function initFavorite() {
        if ($('.favorite').length) {
            var favs = $('.favorite');

            favs.each(function () {
                var fav = $(this);
                var active = false;
                if (fav.hasClass('active')) {
                    active = true;
                }

                fav.on('click', function () {
                    var id = $(this).data('id');

                    if (active) {
                        fav.removeClass('active');
                        active = false;
                        DeleteWishList(id);
                    }
                    else {
                        fav.addClass('active');
                        active = true;
                        AddWishList(id);
                    }
                });
            });
        }
    }

    function AddWishList(id) {
        $.ajax({
            url: '/WishList/PostWishList',
            type: 'POST',
            data: { ProductId: id },
            success: function (res) {
                if (res.success == false) {
                    alert(res.message)
                }
            }
        })
    }

    function DeleteWishList(id) {
        $.ajax({
            url: '/WishList/PostDeleteWishList',
            type: 'POST',
            data: { ProductId: id },
            success: function (res) {
                if (res.success) {
                    alert(res.message)
                }
            }
        })
    }
});

