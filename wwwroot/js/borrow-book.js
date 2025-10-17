$(document).ready(function () {
    // Borrow Book Button Click Event
    $(".borrowBtn").click(function () {
        var btn = $(this);
        var bookId = btn.data("id");
        var bookBorrowUrl = btn.data("url");

        $.ajax({
            url: bookBorrowUrl + bookId,
            type: 'POST',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (data) {
                if (data.success) {
                    // Success message with bootbox
                    bootbox.alert({
                        title: "✅ Success!",
                        message: data.message,
                        className: 'bootbox-alert-success',
                        callback: function () {
                            // Update available copies in both places
                            $(".available-copies").text(data.newCopies);
                            //$(".available-copies-counter").text(data.newCopies);

                            // Update availability badge and button if no copies left
                            if (data.newCopies <= 0) {
                                var availabilityBadge = $(".badge");
                                availabilityBadge.removeClass('bg-success').addClass('bg-danger')
                                    .html('<i class="bi bi-x-circle me-1"></i>Not Available');

                                btn.replaceWith(
                                    '<button class="btn btn-outline-secondary w-100 btn-lg" disabled>' +
                                    '<i class="bi bi-clock me-2"></i>Currently Unavailable</button>'
                                );
                            }
                        }
                    });
                }
                else {
                    // Error message with bootbox
                    bootbox.alert({
                        title: "⚠️ Cannot Borrow",
                        message: data.message,
                        className: 'bootbox-alert-warning'
                    });
                }
            },
            error: function () {
                // Network error message
                bootbox.alert({
                    title: "❌ Error",
                    message: "Error while borrowing book. Please try again.",
                    className: 'bootbox-alert-danger'
                });
            }
        });
    });
});