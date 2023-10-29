using Exam.Models;
using Exam.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
		[Authorize]
		[Route("/Products")]
        public IActionResult Index()
		{
			var products = _productService.GetProducts();
            return View(products.Select(c => ProductViewModel.FromProductModel(c)).ToList());
		}
		[Authorize]
		[Route("/Checkout")]
		public IActionResult Checkout()
		{
			return View(api.OrdersController.Cart);
		}
		[Authorize]
        [Route("/OrderPlaced")]
        public IActionResult OrderPlaced()
		{
			return View();
		}

    }
}
