var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    try {
        dataTable = $('#tblCategory').DataTable({
            "ajax": {
                "url": "/Admin/Category/GetAll"
            },
            "columns": [
                { "data": "categoryId", "width": "15%" },
                { "data": "name", "width": "15%" },
                { "data": "displayOrder", "width": "15%" },
                {
                    "data": "categoryId",
                    "render": function (data) {
                        return `
                        <div class="w-20 btn-group" role="group">
                            <a href="/Admin/Category/Upsert?categoryId=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> &nbspUpdate
                            </a>
                             <a onClick="return Delete('/Admin/Category/Delete/${data}')" class="btn btn-danger mx-2">
                                <i class="bi bi-trash"></i> &nbspDelete
                            </a>
                        </div>
                    `
                    },
                    "width": "10%"
                }
            ]
        });
    }
    catch (err) { }
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success == true) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}