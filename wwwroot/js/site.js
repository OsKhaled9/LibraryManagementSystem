function bootBoxDelete() {
    $(document).ready(function () {
        $('.js-delete').on('click', function () {
            var btn = $(this);
            var id = btn.data('id');
            var url = btn.data('url');
            var name = btn.data('name');
            var entitytype = btn.data('entity-type');

            bootbox.confirm({
                message: "Are you sure that you need to delete this " + entitytype + " '" + name + "' ?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-danger'
                    },
                    cancel: {
                        label: 'No',
                        className: 'btn-secondary'
                    }
                },
                callback: function (result) {
                    if (result) {
                        $.ajax({
                            url: url + id,
                            type: 'POST',
                            data: {
                                '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                            },
                            success: function (response) {
                                if (response.success) {
                                    btn.closest('tr').fadeOut(500, function () {
                                        $(this).remove();
                                    });

                                    bootbox.alert({
                                        title: "Success!",
                                        message: response.message,
                                        className: 'bootbox-alert-success'
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        title: "Cannot Delete",
                                        message: response.message,
                                        className: 'bootbox-alert-warning'
                                    });
                                }
                                
                                var alertBox = $('#alert');
                                alertBox.removeClass("d-none").fadeIn(500);

                                setTimeout(function () {
                                    alertBox.fadeOut(500, function () {
                                        $(this).addClass("d-none");
                                    });
                                }, 5000);
                            },
                            error: function () {
                                alert('An error occurred while deleting ' + entitytype);
                            }
                        });
                    }
                }
            });
        });
    });
}