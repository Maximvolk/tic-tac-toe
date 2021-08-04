// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('td').click(function() {
    if ($(this).hasClass('cross') || $(this).hasClass('circle'))
        return;

    $(this).addClass('cross');

    $.post('/home/move?userCoordinate=' + this.id, function(data, status, xhr) {
            if (data['result'] == 1) {
                $('#modalContent').text("You won!");
                $('#resultModal').modal('show');
            }
            else if (data['result'] == 2) {
                $('#' + data['computerCoordinate']).addClass('circle');
                $('#modalContent').text("You lost!");
                $('#resultModal').modal('show');
            }
            else if (data['result'] == 3) {
                $('#modalContent').text("Draw!");
                $('#resultModal').modal('show');
            }
            else
                $('#' + data['computerCoordinate']).addClass('circle');
        }
    );
});

$('#closeModal').click(function() {
    // document.cookie = "userId=;expires=" + new Date(0).toUTCString()
    location.href = "/home";
});
