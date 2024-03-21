using API.ErrorResponse;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ErrorsController : BaseController
{
    private readonly StoreContext _context;
    public ErrorsController(StoreContext context)
    {
        _context = _context;
    }

    [HttpGet("authcheck")]
    [Authorize]
    public ActionResult<string> CheckAuthorization()
    {
        return "You are authorized";
    }
    [HttpGet("notfound")]
    public ActionResult NotFoundMethod()
    {
        var category = _context.Categories.Find(42);
        if (category == null) return NotFound(new ApiResponse(404));
        return Ok();
    }

    [HttpGet("servererror")]
    public ActionResult ServerErrorMethod()
    {
        var category = _context.Categories.Find(42);
        return Ok(category.ToString());
    }

    [HttpGet("badrequest")]
    public ActionResult BadRequestMethod()
    {
        return BadRequest(new ApiResponse(400));
    }

    [HttpGet("badrequest/{id}")]
    public ActionResult BadIdMethod(int id)
    {
        return Ok();
    }
}