var dataTable;

$(document).ready(function () {
    cargarDataTable();
});


function cargarDataTable() {
    dataTable = $("#tblTamanios").DataTable({
        "ajax": {
            "url": "/admin/tamanios/GetAll",
            "type": "POST",
            "datatype": "json"
        },


        "processing": true,
        "serverSide": true,
        "pageLength": 10,
        "filter": true,
        "data": null,
        "responsive": true,

        "columns": [  

            { "data": "id", "autoWidth": true },
            { "data": "nombre", "autoWidth": true },            
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Tamanios/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-edit"></i> Editar
                                </a>
                                &nbsp;
                                <a onclick=Delete("/Admin/Tamanios/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                                <i class="far fa-trash-alt"></i> Borrar  
                                </a>                                
                               
                          </div>
                         `;
                },
                /*"width": "40%"*/
                "autoWidth": true
            }
        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 de 0 de un total de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },

        "width": "100%"
    });
}

function Delete(url) { /*ESTE METODO ES EL QUE SE LLAMA DESDE EL BOTON DE BORRAR DEL DATATABLE (QUE ESTÁ MÁS ARRIBA)*/
    swal({
        title: "¿Está seguro de borrar?",
        text: "¡Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        cancelButtonText: "Cancelar",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, ¡borrar!",
        closeOnconfirm: true
    }, function () {
        $.ajax({
            type: 'DELETE', /*esto es la llamada al metodo que está en el controller*/
            url: url,
            success: function (data) {
                if (data.success) {
                   // toastr.success(data.message);

                    swal({
                        position: "top-end",
                        type: "success",
                        title: data.message,
                        showConfirmButton: false,
                        timer: 1500
                    });

                 
                    dataTable.ajax.reload();               
                }
                else {
                    // toastr.error(data.message);                  
                    swal({
                        type: "error",
                        title: "Error",                        
                        text: data.message
                       
                    });                    
                }
            }
        });       

    });
}


