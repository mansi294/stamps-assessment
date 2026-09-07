using TakeHome.Api.Models;

namespace TakeHome.Api.Services;

/// <summary>
/// Calculates shipping rates based on package weight, dimensions, and destination zone.
/// </summary>
public class RateCalculator : IRateCalculator
{
// Dimensional weight divisor defined by the rate rules.
private const double DimensionalWeightDivisor = 166.0;

// Base rate tiers.
private const double MinimumWeightThreshold = 1.0;
private const double StandardWeightThreshold = 5.0;
private const double MaximumTierWeight = 20.0;

private const double RateUnderOnePound = 5.00;
private const double RateUpToFivePounds = 8.00;
private const double RateUpToTwentyPounds = 15.00;

// Incremental rate for packages above the 20 lb threshold.
private const double AdditionalRatePerPound = 0.50;

// Zone surcharge is applied per zone above Zone 1.
private const double ZoneSurchargePerZone = 0.75;

// Oversize threshold and surcharge.
private const double OversizeDimensionThreshold = 30.0;
private const double OversizeSurchargeAmount = 10.00;

/// <summary>
/// Calculates a shipping rate quote for the supplied package.
/// </summary>
/// <param name="request">
/// Package weight, dimensions, and destination zone used to calculate the quote.
/// </param>
/// <returns>
/// A <see cref="RateQuoteResponse"/> containing the billable weight,
/// applicable rates, surcharges, and final total.
/// </returns>
public RateQuoteResponse Calculate(RateQuoteRequest request)
{
    ArgumentNullException.ThrowIfNull(request);

    // Dimensional weight is calculated using the package's cubic volume.
    var dimensionalWeight = CalculateDimensionalWeight(request);

    // The billable weight is whichever is greater: actual or dimensional weight.
    var usedDimensionalWeight = dimensionalWeight > request.WeightLbs;
    var billableWeight = Math.Max(request.WeightLbs, dimensionalWeight);

    // Determine the base shipping rate from the billable weight.
    var baseRate = CalculateBaseRate(billableWeight);

    // Apply the destination-zone surcharge.
    var zoneSurcharge = CalculateZoneSurcharge(request.Zone);

    // Apply the oversize surcharge when any individual dimension exceeds 30 inches.
    var oversizeSurcharge = CalculateOversizeSurcharge(request);

    // The final shipping cost is the sum of the base rate and all applicable surcharges.
    var total = baseRate + zoneSurcharge + oversizeSurcharge;

    return new RateQuoteResponse(
        BillableWeightLbs: billableWeight,
        UsedDimensionalWeight: usedDimensionalWeight,
        BaseRate: baseRate,
        ZoneSurcharge: zoneSurcharge,
        OversizeSurcharge: oversizeSurcharge,
        Total: total);
}

/// <summary>
/// Calculates dimensional weight using the package volume and the
/// dimensional-weight divisor specified by the rate rules.
/// </summary>
private static double CalculateDimensionalWeight(RateQuoteRequest request)
{
    return (request.LengthIn * request.WidthIn * request.HeightIn)
           / DimensionalWeightDivisor;
}

/// <summary>
/// Determines the base shipping rate based on billable weight.
/// </summary>
private static double CalculateBaseRate(double billableWeight)
{
    if (billableWeight < MinimumWeightThreshold)
    {
        return RateUnderOnePound;
    }

    if (billableWeight <= StandardWeightThreshold)
    {
        return RateUpToFivePounds;
    }

    if (billableWeight <= MaximumTierWeight)
    {
        return RateUpToTwentyPounds;
    }

    // Packages above 20 lbs incur an additional charge for each pound over 20.
    var additionalWeight = billableWeight - MaximumTierWeight;

    return RateUpToTwentyPounds
           + (additionalWeight * AdditionalRatePerPound);
}

/// <summary>
/// Calculates the surcharge associated with the destination zone.
/// Zone 1 has no surcharge, with each subsequent zone adding $0.75.
/// </summary>
private static double CalculateZoneSurcharge(int zone)
{
    return (zone - 1) * ZoneSurchargePerZone;
}

/// <summary>
/// Determines whether the package qualifies for the oversize surcharge.
/// The surcharge applies when any single dimension is greater than 30 inches.
/// </summary>
private static double CalculateOversizeSurcharge(RateQuoteRequest request)
{
    var isOversized =
        request.LengthIn > OversizeDimensionThreshold ||
        request.WidthIn > OversizeDimensionThreshold ||
        request.HeightIn > OversizeDimensionThreshold;

    return isOversized ? OversizeSurchargeAmount : 0.00;
}

}
