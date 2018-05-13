var FLAG_SUCCESS = 0;
var FLAG_ERROR = 1;

$(function () {
    $('#side-menu').metisMenu();
});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        topOffset = 55;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse')
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse')
        }

        height = (this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    })
})

var showBootstrapDialog = function (flag, title, message) {
    switch (flag) {
        case FLAG_SUCCESS:
            // success message
            BootstrapDialog.show({
                title: title,
                message: message,
                buttons: [{
                    label: 'Đóng',
                    cssClass: 'btn-success',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }]
            });
            break;

        case FLAG_ERROR:
            // error message
            BootstrapDialog.show({
                title: title,
                message: message,
                buttons: [{
                    label: 'Đóng',
                    cssClass: 'btn-danger',
                    action: function (dialogItself) {
                        dialogItself.close();
                    }
                }]
            });
            break;
    }
};

/**
* Number.prototype.format(n, x, s, c)
* 
* param integer n: length of decimal
* param integer x: length of whole part
* param mixed   s: sections delimiter
* param mixed   c: decimal delimiter
**/
Number.prototype.format = function (n, x, s, c) {
    var re = '\\d(?=(\\d{' + (x || 3) + '})+' + (n > 0 ? '\\D' : '$') + ')',
        num = this.toFixed(Math.max(0, ~~n));
    return (c ? num.replace('.', c) : num).replace(new RegExp(re, 'g'), '$&' + (s || ','));
};

/**
* Function to format date
*/
function formatDateVN(date) {
    var yyyy = date.getFullYear();
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    return dd + "-" + mm + "-" + yyyy;
}

function formatDateTimeVN(date) {
    var yyyy = date.getFullYear();
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var hh = date.getHours();
    var mi = date.getMinutes();
    if (dd < 10) {
        dd = "0" + dd;
    }
    if (mm < 10) {
        mm = "0" + mm;
    }
    if (hh < 10) {
        hh = "0" + hh;
    }
    if (mi < 10) {
        mi = "0" + mi;
    }
    return dd + "-" + mm + "-" + yyyy + " " + hh + ":" + mi;
}
