using Exam.Models;
using Exam.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Web.Providers.Entities;

namespace Exam.Controllers.api
{
    [ApiController]
    public class OrdersController : ControllerBase
    {
        /// <summary>
        /// NOT THE RIGHT WAY - Temporarily doing this for demo purposes.
        /// </summary>
        public static List<CartItem> Cart = new List<CartItem>();

        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public OrdersController(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }

        [HttpPost]
        [Route("api/Orders/AddToCart")]
        public IActionResult AddToCart(ProductIDViewModel model)
        {
            var product = _productService.GetProducts().Where(c => c.Id == model.productId).FirstOrDefault();
            if (product != null)
            {
                var inCart = Cart.Where(c=>c.productId == model.productId).FirstOrDefault();
                if (inCart != null)
                {
                    if (product.Quantity >= (inCart.quantity + 1))
                    {
                        inCart.quantity++;
                    }
                    else
                        return Ok("Out of stock.");
                }
                else
                {
                    if (product.Quantity > 0)
                    {
                        Cart.Add(new CartItem
                        {
                            productId = model.productId,
                            quantity = 1,
                            product = product
                        });
                    }
                    else
                        return Ok("Out of stock.");
                }
            }
            return Ok("Item Added");
        }


        [HttpPost]
        [Route("api/Orders/GetCartCount")]
        public IActionResult GetCartCount()
        {
            return Ok(Cart.Sum(c => c.quantity));
        }


        [HttpPost]
        [Route("api/Orders/RemoveFromCart")]
        public IActionResult RemoveFromCart(ProductIDViewModel model)
        {
            var inCart = Cart.Where(c => c.productId == model.productId).FirstOrDefault();
            if (inCart != null)
            {
                Cart = Cart.Where(c => c.productId != model.productId).ToList();
            }
            return Ok();
        }


        [HttpPost]
        [Route("api/Orders/PlaceOrder")]
        public IActionResult PlaceOrder()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int res = _orderService.SaveOrder(UserId, Cart.Select(c => new OrderItem { Price = c.product.Price, Quantity = c.quantity, ProductId = c.productId }).ToList());
            if(res > 0)
            {
                Cart = new List<CartItem>();
            }
            return Ok();
        }
    }

    public class ProductIDViewModel
    {
        public int productId { get; set; }
    }

    public class CartItem
    {
        public int productId { get; set; }
        public int quantity { get; set; }
        public Product product { get; set; }
    }
}
