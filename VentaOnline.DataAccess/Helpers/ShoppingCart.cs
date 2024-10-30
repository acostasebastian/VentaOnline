using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaOnline.Data;
using VentaOnline.Models;

namespace VentaOnline.DataAccess.Helpers
{
    public class ShoppingCart
    {
        //private readonly ApplicationDbContext _db;
        ////private ApplicationDbContext db = new ApplicationDbContext();

        //public ShoppingCart(ApplicationDbContext db)
        //{
        //    _db = db;
        //}

        //public string ShoppingCartId { get; set; }



        //public const string CartSessionKey = "cartId";


        //public static ShoppingCart GetCart(DefaultHttpContext context)
        ////public ShoppingCart GetCart(DefaultHttpContext context)

        //{
        //    var cart = new ShoppingCart();

        //  //  cart.ShoppingCartId = cart.GetCartId(context);

        //    return cart;
        //}

        //---------------------

        //private readonly ISession _session;

        //public ShoppingCart(IHttpContextAccessor httpContextAccessor)
        //{
        //    _session = httpContextAccessor.HttpContext.Session;
        //}

        //private List<Carrito> _items = new List<Carrito>();

        //public void AddToCart(int productId, int quantity)
        //{
        //    var item = _items.FirstOrDefault(i => i.ProductoId == productId);
        //    if (item == null)
        //    {
        //        _items.Add(new Carrito { ProductoId = productId, Cantidad = quantity, FechaCreacion = DateTime.Now, CartId = "Prueba" });
        //    }
        //    else
        //    {
        //        item.Cantidad += quantity;
        //    }
        //}

        //public void RemoveItem(int productId)
        //{
        //    var item = _items.FirstOrDefault(i => i.ProductoId == productId);
        //    if (item != null)
        //    {
        //        _items.Remove(item);
        //    }
        //}

        //public List<Carrito> GetItems() => _items;

        ////public decimal GetTotal() => _items.Sum(i => i.Quantity * GetProductPrice(i.ProductId)); // Implementa GetProductPrice según tu lógica
        //public decimal GetTotal() => _items.Sum(i => i.Cantidad * 8); // Implementa GetProductPrice según tu lógica

        //public class CartItem
        //{
        //    public int ProductId { get; set; }
        //    public int Quantity { get; set; }
        //}
    }
}

   

