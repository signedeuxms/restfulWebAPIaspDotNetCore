var dataTable;
var allColumn = [];

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        //"processing": true,
        //bserverside": true,
        //"jquery.support.cors": true,
        "orderfixed": {
            "post": [[0, 'asc'], [1, 'asc'], [2, 'asc']]
        },
        "ajax": {
            //"processing": true,
            //"serverSide": true,
            "url": "/nationalParks/GetAllNationalPark",
            "type": "GET",
            "datatype": "json",
            //"datasrc": "",
            //var data = table.ajax.params();
            //"contenttype": "application/json",
            "success": function (value) {
                console.log(' \n\n data.length => ' + value.data.length);

                console.log('\n\n ALL \n');
                for (var i = 0; i < value.data.length; i++) {
                    var park = value.data[i];
                    var my_item = {};                   
                    console.log('\n');
                    for (const [key, val] of Object.entries(park)) {
                        console.log(' => ' + key + ": " + val);    
                        my_item.title = key;
                        my_item.data = key;
                    }
                    allColumn.push(park);
                }
            },
            "error": function (error) {
                alert('\n error get data => ' + error);
            }
        },
        "datasrc": "data",
        //"paging": true,
        //"ordering": true,
        "columns": [
            //{ "data": "name", "width": "50%" },
            {
                "data": "name", // can be null or undefined
                "width": "50%",
                "defaultContent": "name=nothing"
            },
            //{ "data": "state", "width": "20%" },
            {
                "data": "state", // can be null or undefined
                "width": "20%",
                "defaultContent": "state=nothing"
            },
            {
                "data": "id",
                "render": function (data) {
                    console.log('\n\n nationalparks \n\n');
                    console.log(data);
                    return `<div class="text-center">
                                <a href="/nationalParks/Upsert/${data}" 
                                    class='btn btn-success text-white'
                                    style='cursor:pointer;'>
                                    <i class='far fa-edit'></i>
                                </a>
                                &nbsp;
                                <a onclick=Delete("/nationalParks/Delete/${data}") 
                                    class='btn btn-danger text-white'
                                    style='cursor:pointer;'>
                                    <i class='far fa-trash-alt'></i>
                                </a>
                            </div>
                            `;
                },
                "width": "30%",
                "defaultContent": "id=nothing"
            },
        ]
        /*"error": function (error) {
            console.log('\n\n fail => ', error);
        }*/
    });
}