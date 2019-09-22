$(function () {
    $('.chatbox-top').click(function () {
        $(this).closest('.chatbox').toggleClass('chatbox-min');
    });
});