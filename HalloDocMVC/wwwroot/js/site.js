$(document).ready(function () {
    $(".t-tab > .position-relative >  .btn > .rounded").click(function () {
        $(".t-tab > .position-relative >  .btn > .rounded").removeClass("activeTab");
        $(this).addClass("activeTab");
    });

    
});