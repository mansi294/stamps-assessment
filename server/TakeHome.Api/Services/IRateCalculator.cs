using TakeHome.Api.Models;

namespace TakeHome.Api.Services;

public interface IRateCalculator
{
	RateQuoteResponse Calculate(RateQuoteRequest request);
}
