function bootBoxReturn() {
    $(document).ready(function () {
        $('.js-return').on('click', function () {
            var btn = $(this);
            var id = btn.data('id');
            var url = btn.data('url');
            var book = btn.data('book');
            var entitytype = btn.data('entity-type');

            bootbox.confirm({
                message: "Are you sure that you need to return this " + entitytype + " '" + book + "' ?",
                buttons: {
                    confirm: {
                        label: 'Yes',
                        className: 'btn-success'
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

                                    //// تحديث الحالة
                                    //var statusBadge = btn.closest('tr').find('.status-badge');
                                    //statusBadge.html('<span class="badge bg-success text-dark fs-6 status-badge"><i class="bi bi-check-circle me-1"></i>Returned</span>');

                                    //// تحديث الأزرار
                                    //var actionsCell = btn.closest('td');
                                    //actionsCell.html(`
                                    //    <a asp-controller="Borrowings" asp-action="BorrowDetails" asp-route-id="@borrowing.Id" class="btn btn-outline-primary btn-sm">
                                    //        <i class="bi bi-eye me-1"></i> Details
                                    //    </a>
                                    //    <a href="javascript:;"
                                    //           class="btn btn-outline-danger btn-sm js-delete"
                                    //           data-id="@borrowing.Id"
                                    //           data-url="/Borrowings/DeleteBorrow/"
                                    //           data-name="@borrowing.Book.Title"
                                    //           data-entity-type="Borrow">
                                    //        <i class="bi bi-trash me-1"></i> Delete
                                    //    </a>
                                    //`);


                                    bootbox.alert({
                                        title: "Success!",
                                        message: response.message,
                                        className: 'bootbox-alert-success',

                                        callback: function () {
                                            // Reload Page
                                            location.reload();
                                        }
                                    });
                                }
                                else {
                                    bootbox.alert({
                                        title: "Cannot Return",
                                        message: response.message,
                                        className: 'bootbox-alert-warning'
                                    });
                                }
                            },
                            error: function () {
                                alert('An error occurred while Returning ' + entitytype);
                            }
                        });
                    }
                }
            });
        });
    });
}