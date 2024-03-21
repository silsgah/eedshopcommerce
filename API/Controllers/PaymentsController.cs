using AutoMapper;
using Infrastructure.Services;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using API.DTO;
using API.ErrorResponse;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    public class PaymentsController : BaseController
    {
        private readonly PaymentService _paymentService;
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        public PaymentsController(PaymentService paymentService, StoreContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _paymentService = paymentService;

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> PaymentIntentAsync()
        {
            var basket = await ExtractBasket(User.Identity.Name);

            if (basket == null) return NotFound(new ApiResponse(404));

            var intent = await _paymentService.PaymentIntentAsync(basket);

            if (intent == null) return BadRequest(new ApiResponse(400, "Problem creating the payment intent"));

            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                basket.PaymentIntentId =  intent.Id;
            }
            else
            {
                basket.PaymentIntentId = basket.PaymentIntentId;
            }
            if (string.IsNullOrEmpty(basket.ClientSecret))
            {
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                basket.ClientSecret = basket.ClientSecret;
            }
            _context.Update(basket);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest(new ApiResponse(400, "Problem updating basket with intent"));

            return _mapper.Map<Basket, BasketDto>(basket);
        }

        private async Task<Basket> ExtractBasket(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                Response.Cookies.Delete("clientId");
                return null;
            }
            return await _context.Basket
                        .Include(b => b.Items)
                        .ThenInclude(i => i.Course)
                        .OrderBy(i => i.Id)
                        .FirstOrDefaultAsync(x => x.ClientId == clientId);

        }
    }
}