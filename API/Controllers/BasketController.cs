using API.DTO;
using API.ErrorResponse;
using AutoMapper;
using Entity;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseController
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        public BasketController(StoreContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await ExtractBasket(GetClientId());

            if (basket == null) return NotFound(new ApiResponse(404));

            var basketResponse = _mapper.Map<Basket, BasketDto>(basket);

            return basketResponse;
        }


        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(Guid courseId)
        {
            var basket = await ExtractBasket(GetClientId());

            if (basket == null) basket = CreateBasket();

            var course = await _context.Courses.FindAsync(courseId);

            if (course == null) return NotFound(new ApiResponse(404));

            basket.AddCourseItem(course);

            var basketResponse = _mapper.Map<Basket, BasketDto>(basket);
            try
            {
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return basketResponse;
            }catch(Exception ex)
            {
                string error = ex.InnerException.InnerException.Message;
            }
            return BadRequest(new ApiResponse(400, "Problem saving item to the Basket"));
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(Guid courseId)
        {
            var basket = await ExtractBasket(GetClientId());

            if (basket == null) return NotFound();

            basket.RemoveCourseItem(courseId);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(new ApiResponse(400, "Problem removing item from the basket"));
        }
        private Basket CreateBasket()
        {
            var clientId = User.Identity?.Name;
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = Guid.NewGuid().ToString();
                var options = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(10) };
                Response.Cookies.Append("clientId", clientId, options);
            }
            var basket = new Basket { ClientId = clientId };
            _context.Basket.Add(basket);
            return basket;
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
        private string GetClientId()
        {
            return User.Identity?.Name ?? Request.Cookies["clientId"];
        }
    }
}
