//import { Toast } from "../lib/bootstrap/dist/js/bootstrap.bundle";

var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/trails/GetAllTrail",
            "type": "GET",
            "datatype": "json",
            "complete": function (data) {
                console.log(data);
                console.log(data['responseJSON']);
            }
        },
        "columns": [
            //{ "data": "nationalPark.name", "width": "25%" },
            {
                "data": "nationalPark.name", // can be null or undefined
                "width": "25%",
                "defaultContent": "name is undefined/null"
            },
            //{ "data": "name", "width": "20%" },
            {
                "data": "name", // can be null or undefined
                "width": "20%",
                "defaultContent": "state is undefined/null"
            },
            //{ "data": "distance", "width": "15%" },
            {
                "data": "distance", // can be null or undefined
                "width": "15%",
                "defaultContent": "distance is undefined/null"
            },
            //{ "data": "elevation", "width": "15%" },
            {
                "data": "elevation", // can be null or undefined
                "width": "15%",
                "defaultContent": "elevation is undefined/null"
            },
            {
                "data": "id",
                "render": function (data) {
                    console.log('\n\n nationalparks \n\n');
                    console.log(data);
                    return `<div class="text-center">
                                <a href="/trails/UpdateInsert/${data}" 
                                    class='btn btn-success text-white'
                                    style='cursor:pointer;'>
                                    <i class='far fa-edit'></i>
                                </a>
                                &nbsp;
                                <a onclick=Delete("/trails/Delete/${data}") 
                                    class='btn btn-danger text-white'
                                    style='cursor:pointer;'>
                                    <i class='far fa-trash-alt'></i>
                                </a>
                            </div>
                            `;
                },
                "width": "30%",
                "defaultContent": "data.id is undefined/null"
            },
        ]
    });
}


function Delete(url) {
    swal({
        title: "Are you sure you want to delete?",
        text: "You will be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}