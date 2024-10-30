using Microsoft.AspNetCore.Mvc;
using VentaOnline.Data;
using VentaOnline.DataAccess.Helpers;
using VentaOnline.Models;

namespace VentaOnline.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShoppingCartController : Controller
    {
        ////private ApplicationDbContext db = new ApplicationDbContext();
        private readonly ApplicationDbContext _db;

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
        }

        //// GET: ShoppingCart
        //public ActionResult Index()
        //{

        //    var cart = ShoppingCart.GetCart(this.HttpContext as DefaultHttpContext);
        //   // var cart = ShoppingCart.GetCart((DefaultHttpContext)this.HttpContext);
        //    //var cart = ShoppingCart.GetCart();

        //    //var viewModel = new ShoppingCartViewModel
        //    //{
        //    //    CartItems = cart.GetCartItems(),
        //    //    CartTotal = cart.GetTotal()
        //    //};

        //    //return View(viewModel);
        //    return View();
        //}

        //----------------------



       // public void AddToCart(Producto producto)
        public void AddToCart(int id)
        {
            //var cartItem = _db.Carrito.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductoId == producto.ProductoId);
            //var cartItem = _db.Carrito.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductoId == id);

            //if (cartItem == null)
            //{
            //    cartItem = new Carrito
            //    {
            //        ProductoId = producto.ProductoId,
            //        CartId = ShoppingCartId,
            //        Cantidad = producto.CantidadComprar == 0 ? 1 : producto.CantidadComprar,
            //        FechaCreacion = DateTime.Now
            //    };
            //    _db.Carrito.Add(cartItem);
            //}
            //else
            //{
            //    cartItem.Cantidad = cartItem.Cantidad + (producto.CantidadComprar == 0 ? 1 : producto.CantidadComprar);
            //}

            _db.SaveChanges();
        }


        //private readonly ShoppingCart _shoppingCart;

        //public ShoppingCartController(ShoppingCart shoppingCart)
        //{
        //    _shoppingCart = shoppingCart;
        //}

        public IActionResult Index()
        {
            //return View(_shoppingCart.GetItems());

            return View();
        }

       
        }
}
