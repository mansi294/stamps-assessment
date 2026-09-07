using System.ComponentModel.DataAnnotations;
using TakeHome.Api.Validation;

namespace TakeHome.Api.Models;

public record RateQuoteRequest(
	[GreaterThanZero] double WeightLbs,
	[GreaterThanZero] double LengthIn,
	[GreaterThanZero] double WidthIn,
	[GreaterThanZero] double HeightIn,
	[Range(1, 8)] int Zone
);

public record RateQuoteResponse(
	double BillableWeightLbs,
	bool UsedDimensionalWeight,
	double BaseRate,
	double ZoneSurcharge,
	double OversizeSurcharge,
	double Total
);
