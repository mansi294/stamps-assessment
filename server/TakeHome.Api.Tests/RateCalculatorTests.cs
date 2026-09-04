using TakeHome.Api.Models;
using TakeHome.Api.Services;

namespace TakeHome.Api.Tests;

public class RateCalculatorTests
{
	private readonly IRateCalculator _calculator = new RateCalculator();

	private static RateQuoteRequest Request(
		double weightLbs, double lengthIn = 5, double widthIn = 5, double heightIn = 5, int zone = 1) =>
		new(weightLbs, lengthIn, widthIn, heightIn, zone);

	[Theory]
	[InlineData(0.5, 5.00)]
	[InlineData(0.999, 5.00)]
	[InlineData(1.0, 8.00)]
	[InlineData(3.0, 8.00)]
	[InlineData(5.0, 8.00)]
	[InlineData(5.01, 15.00)]
	[InlineData(20.0, 15.00)]
	public void Calculate_BaseRate_MatchesWeightTier(double weightLbs, double expectedBaseRate)
	{
		var result = _calculator.Calculate(Request(weightLbs, lengthIn: 1, widthIn: 1, heightIn: 1));

		Assert.Equal(expectedBaseRate, result.BaseRate, precision: 2);
	}

	[Fact]
	public void Calculate_OverTwentyPounds_AddsFiftyCentsPerPoundOverTwenty()
	{
		var result = _calculator.Calculate(Request(25.0, lengthIn: 1, widthIn: 1, heightIn: 1));

		Assert.Equal(17.50, result.BaseRate, precision: 2);
	}

	[Fact]
	public void Calculate_DimensionalWeightExceedsActual_UsesDimensionalWeightForBaseRate()
	{
		// dim weight = (20*20*20)/166 = 48.19 lbs, actual weight only 2 lbs
		var result = _calculator.Calculate(Request(2.0, lengthIn: 20, widthIn: 20, heightIn: 20, zone: 1));

		Assert.True(result.UsedDimensionalWeight);
		Assert.Equal(48.19, result.BillableWeightLbs, precision: 2);
		Assert.Equal(15.00 + 0.50 * (48.19 - 20), result.BaseRate, precision: 2);
	}

	[Fact]
	public void Calculate_ActualWeightExceedsDimensionalWeight_DoesNotUseDimensionalWeight()
	{
		// dim weight = (5*5*5)/166 = 0.75 lbs, actual weight 10 lbs
		var result = _calculator.Calculate(Request(10.0, lengthIn: 5, widthIn: 5, heightIn: 5, zone: 1));

		Assert.False(result.UsedDimensionalWeight);
		Assert.Equal(10.0, result.BillableWeightLbs, precision: 2);
		Assert.Equal(15.00, result.BaseRate, precision: 2);
	}

	[Fact]
	public void Calculate_DimensionalWeightEqualToActual_DoesNotCountAsUsed()
	{
		// dim weight = (166*1*1)/166 = 1.0 lb, exactly equal to actual weight
		var result = _calculator.Calculate(Request(1.0, lengthIn: 166, widthIn: 1, heightIn: 1, zone: 1));

		Assert.False(result.UsedDimensionalWeight);
		Assert.Equal(1.0, result.BillableWeightLbs, precision: 2);
	}

	[Theory]
	[InlineData(1, 0.00)]
	[InlineData(2, 0.75)]
	[InlineData(8, 5.25)]
	public void Calculate_ZoneSurcharge_MatchesZone(int zone, double expectedSurcharge)
	{
		var result = _calculator.Calculate(Request(1.0, lengthIn: 1, widthIn: 1, heightIn: 1, zone: zone));

		Assert.Equal(expectedSurcharge, result.ZoneSurcharge, precision: 2);
	}

	[Fact]
	public void Calculate_NoDimensionOverThirtyInches_NoOversizeSurcharge()
	{
		var result = _calculator.Calculate(Request(1.0, lengthIn: 30, widthIn: 30, heightIn: 30));

		Assert.Equal(0.00, result.OversizeSurcharge, precision: 2);
	}

	[Theory]
	[InlineData(31, 5, 5)]
	[InlineData(5, 31, 5)]
	[InlineData(5, 5, 31)]
	public void Calculate_AnyDimensionOverThirtyInches_AddsOversizeSurcharge(
		double lengthIn, double widthIn, double heightIn)
	{
		var result = _calculator.Calculate(Request(1.0, lengthIn, widthIn, heightIn));

		Assert.Equal(10.00, result.OversizeSurcharge, precision: 2);
	}

	[Fact]
	public void Calculate_Total_SumsBaseZoneAndOversize()
	{
		// billable weight 25 lbs (actual, dims small) -> base 17.50; zone 4 -> 2.25; dimension 32in -> oversize 10.00
		var result = _calculator.Calculate(Request(25.0, lengthIn: 32, widthIn: 5, heightIn: 5, zone: 4));

		Assert.Equal(17.50, result.BaseRate, precision: 2);
		Assert.Equal(2.25, result.ZoneSurcharge, precision: 2);
		Assert.Equal(10.00, result.OversizeSurcharge, precision: 2);
		Assert.Equal(29.75, result.Total, precision: 2);
	}
}
