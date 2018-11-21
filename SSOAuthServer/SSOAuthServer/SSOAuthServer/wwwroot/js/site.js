$(document).on("click.app.logout", ".app-logout", function (e) {
    e.preventDefault();
    $('#logoutForm').submit();
});