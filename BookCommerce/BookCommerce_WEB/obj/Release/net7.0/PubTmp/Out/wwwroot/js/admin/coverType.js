var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    try {
        dataTable = $('#tblCoverType').DataTable({
            "ajax": {
                "url": "/Admin/CoverType/GetAll"
            },
            "columns": [
                { "data": "coverTypeId", "width": "15%" },
                { "data": "name", "width": "15%" },
                {
                    "data": "coverTypeId",
                    "render": function (data) {
                        return `
                        <div class="w-20 btn-group" role="group">
                            <a href="/Admin/CoverType/Upsert?coverTypeId=${data}" class="btn btn-primary mx-2">
                                <i class="bi bi-pencil-square"></i> &nbspUpdate
                            </a>
                             <a onClick="return Delete('/Admin/CoverType/Delete/${data}')" class="btn btn-danger mx-2">
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