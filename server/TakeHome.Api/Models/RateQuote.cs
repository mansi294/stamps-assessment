using System.ComponentModel.DataAnnotations;

namespace TakeHome.Api.Models;

public record RateQuoteRequest(
	[Range(0.001, double.MaxValue)] double WeightLbs,
	[Range(0.001, double.MaxValue)] double LengthIn,
	[Range(0.001, double.MaxValue)] double WidthIn,
	[Range(0.001, double.MaxValue)] double HeightIn,
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
