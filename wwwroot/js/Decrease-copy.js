$(document).ready(function () {
    // Decrease
    $(".decrease-copy-btn").click(function () {
        var btn = $(this);
        var id = btn.data("id");
        var url = btn.data("url");

        $.ajax({
            url: url + id,
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (data) {
                if (data.success) {
                    var row = btn.closest("tr");
                    row.find(".available-copies").text(data.newCopies);
                }
                else {
                    // alert(data.message);
                    bootbox.alert({
                        title: "Can not decrease",
                        message: data.message,
                        className: 'bootbox-alert-warning'
                    });
                    var row = btn.closest("tr");
                    row.find(".available-copies").text(data.newCopies);
                }
            },
            error: function () {
                alert("❌ Error while decreasing copies.");
            }
        });
    });
});