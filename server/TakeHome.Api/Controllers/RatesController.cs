using Microsoft.AspNetCore.Mvc;
using TakeHome.Api.Models;
using TakeHome.Api.Services;

namespace TakeHome.Api.Controllers;

[ApiController]
[Route("api/rates")]
public class RatesController : ControllerBase
{
	private readonly IRateCalculator _rateCalculator;

	public RatesController(IRateCalculator rateCalculator)
	{
		_rateCalculator = rateCalculator;
	}

	[HttpPost("quote")]
	public ActionResult<RateQuoteResponse> Quote([FromBody] RateQuoteRequest request)
	{
		var response = _rateCalculator.Calculate(request);
		return Ok(response);
	}
}
