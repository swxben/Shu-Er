// jQuery version, targets all <a> with rel="external" or with href starting with "http://":
$(function () {
    $('a[rel="external"], a[href^="http://"]').click(function () {
        window.open(this.href);
        return false;
    });
});


