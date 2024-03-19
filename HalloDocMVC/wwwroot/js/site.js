$(document).ready(function () {
    $(".t-tab > .position-relative >  .btn > .rounded").click(function () {
        $(".t-tab > .position-relative >  .btn > .rounded").removeClass("activeTab");
        $(this).addClass("activeTab");
    });    
    $(".t-filter > .btn").click(function () {
        $(".t-filter > .btn").removeClass("activeFilter");
        $(this).addClass("activeFilter");
    });
});