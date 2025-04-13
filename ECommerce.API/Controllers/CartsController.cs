using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderItemRepository _orderItemRepository;

        public CartsController(ICartRepository orderRepository, UserManager<ApplicationUser> userManager, IOrderItemRepository orderItemRepository, IOrderRepository orderRepo)
        {
            _cartRepository = orderRepository;
            _userManager = userManager;
            _orderItemRepository = orderItemRepository;
            _orderRepo = orderRepo;
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Create([FromRoute] int id, [FromQuery] int count, CancellationToken cancellationToken)
        {
            var appUserId = _userManager.GetUserId(User);

            if (appUserId is not null)
            {
                await _cartRepository.CreateAsync(new Cart()
                {
                    ProductId = id,
                    Count = count,
                    ApplicationUserId = appUserId
                }, cancellationToken);

                await _cartRepository.CommitAsync();

                return Created();
            }

            return NotFound();
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var appUserId = _userManager.GetUserId(User);

            if (appUserId is not null)
            {
                var carts = _cartRepository.Get(e => e.ApplicationUserId == appUserId, includes: [e=>e.Product, e=>e.ApplicationUser]);

                return Ok(new CartWithTotalResponse()
                {
                    Carts = carts.Select(e => e.Product).Adapt<IEnumerable<CartResponse>>(),
                    TotalPrice = carts.Sum(e => e.Product.Price * e.Count)
                });
            }

            return NotFound();
        }

        [HttpGet("Pay")]
        public IActionResult Pay()
        {
            var userId = _userManager.GetUserId(User);
            var cart = _cartRepository.Get(e => e.ApplicationUserId == userId, includes: [e => e.Product, e => e.ApplicationUser]);

            var order = new Order();
            order.ApplicationUserId = userId;
            order.OrderDate = DateTime.Now;
            order.OrderTotal = (double)cart.Sum(e => e.Product.Price * e.Count);

            _orderRepo.CreateAsync(order);
            _orderRepo.CommitAsync();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
            };

            foreach (var item in cart)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Count,
                    }
                );
            }

            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            _orderRepo.CommitAsync();

            List<OrderItem> orderItems = [];
            foreach (var item in cart)
            {
                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    Count = item.Count,
                    Price = (double)item.Product.Price,
                    ProductId = item.ProductId,
                };

                orderItems.Add(orderItem);
            }
            _orderItemRepository.CreateRange(orderItems);
            _orderRepo.CommitAsync();

            return Ok(session.Url);
        }
    }
}
