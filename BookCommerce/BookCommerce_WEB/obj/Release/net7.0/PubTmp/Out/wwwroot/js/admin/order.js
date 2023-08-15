var dataTable;
$(document).ready(function () {
    var url = window.location.search;

    if (url.includes("inprocess"))
        loadDataTable("inprocess");
    else if (url.includes("completed"))
        loadDataTable("completed");
    else if (url.includes("paymentpending"))
        loadDataTable("paymentpending");
    else if (url.includes("approved"))
        loadDataTable("approved");
    else if (url.includes("cancelled"))
        loadDataTable("cancelled");
    else
        loadDataTable("all");
});

function loadDataTable(status) {
    dataTable = $('#tblOrder').DataTable({
        "ajax": { "url":"/Admin/Order/GetAll?status=" + status },
        "columns": [
            {"data": "orderHeaderId", "width": "5%"},
            {"data": "name", "width": "25%"},
            {"data": "phoneNumber", "width": "20%"},
            {"data": "applicationUser.email", "width": "20%"},
            {"data": "orderStatus", "width": "15%"},
            {"data": "orderTotal", "width": "15%"},
            {
                "data": "orderHeaderId",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Order/Details?orderHeaderId=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-search"></i>
                            </a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
    });
}


connectionNotification.on("TriggerNotification", () => {
    var cookieArr = document.cookie.split(";");
    var role = cookieArr[0].split("=");

    if (role[1] == "Admin" || role[1] == "Employee") {
        dataTable.ajax.reload();
        toastr.success(`New order has been placed.`);
    }
});

connectionNotification.start();