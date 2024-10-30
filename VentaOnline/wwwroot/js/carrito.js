//$(document).ready(function () {
//    cargarDataTable();
//});


//function addCarrito(id) {
//    var c = $('#CantidadComprar-' + id).val();
//    var url = '@Url.Action("AddToCart", "ShoppingCart")';
//    location.href = url + "?id=" + id + "&CantidadComprar=" + c;

//    console.log('entro')
//}

//$(document).ready(function () {


//    $(".add-to-cart-btn").click(function () {
//        var id = $(this).data("id");
//        addCarrito(id);
//        console.log('ID: ' + id)
//    });
//});



///----


$(document).ready(function () {
   // obtenerCantidad();
})
function obtenerCantidad() {
    jQuery.ajax({
        url: '@Url.Action("CantidadCarrito", "Tienda")',
        type: "GET",
        data: null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $(".contador-carrito").text(data.respuesta);
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {

        },
    });

}
$(document).on('click', '.btn-agregar-carrito', function (event) {
    console.log('entro aca: ' )
    var request = {
        oCarrito: {

            oProducto: { IdProducto: $(this).data("idproducto") }
             
        }
           
         
    } 
    

    jQuery.ajax({
        url: '@Url.Action("InsertarCarrito", "Home")',
        type: "POST",
        data: JSON.stringify(request),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var actual = parseInt($(".contador-carrito").text());
            if (data.respuesta != 0) {
                actual = actual + 1;
                $(".contador-carrito").text(actual);
                $('#toast-carrito').toast('show');
            }
        },
        error: function (error) {
            console.log(error)
        },
        beforeSend: function () {

        },
    });

});