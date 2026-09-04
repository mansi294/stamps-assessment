# Package Rate Quote — Business Rules

Implement `IRateCalculator.Calculate(RateQuoteRequest)` so it returns a `RateQuoteResponse`
matching the rules below exactly, including the tier boundaries.

## Inputs

- `WeightLbs` — actual weight of the package, in pounds
- `LengthIn`, `WidthIn`, `HeightIn` — package dimensions, in inches
- `Zone` — destination zone, an integer from 1 to 8 inclusive

## 1. Dimensional weight

Carriers bill large-but-light packages by their _dimensional weight_, not their actual weight.

```
dimWeightLbs = (LengthIn * WidthIn * HeightIn) / 166
```

The **billable weight** is whichever is larger: `WeightLbs` or `dimWeightLbs`. If the dimensional
weight is used (i.e. it's strictly greater than the actual weight), set
`UsedDimensionalWeight = true` on the response.

## 2. Base rate by weight tier

Base rate is determined by the **billable weight** (from step 1), using these tiers:

| Billable weight        | Base rate                          |
|-------------------------|-------------------------------------|
| `< 1 lb`                | `$5.00` flat                       |
| `>= 1 lb` and `<= 5 lb` | `$8.00` flat                       |
| `> 5 lb` and `<= 20 lb` | `$15.00` flat                      |
| `> 20 lb`               | `$15.00 + $0.50` per lb over 20    |

Boundaries are inclusive on the upper end of each tier as shown — e.g. a billable weight of
exactly `5.0` lb is `$8.00` (not `$15.00`); exactly `20.0` lb is `$15.00` (not the over-20 formula).

## 3. Zone surcharge

```
zoneSurcharge = (Zone - 1) * $0.75
```

Zone 1 has no surcharge. Zone 8 adds `7 * $0.75 = $5.25`.

## 4. Oversize surcharge

If **any single dimension** (`LengthIn`, `WidthIn`, or `HeightIn`) is **strictly greater than 30
inches**, add a flat `$10.00` oversize surcharge. Otherwise it's `$0.00`. This applies regardless
of weight or zone, and stacks with everything else.

## 5. Total

```
Total = BaseRate + ZoneSurcharge + OversizeSurcharge
```

## Output shape

Return a `RateQuoteResponse` with:

- `BillableWeightLbs` — the weight actually used for the base-rate lookup (step 1)
- `UsedDimensionalWeight` — `true` if dimensional weight exceeded actual weight
- `BaseRate` — from step 2
- `ZoneSurcharge` — from step 3
- `OversizeSurcharge` — from step 4
- `Total` — from step 5

## Validation

`Zone` must be between 1 and 8 inclusive; `WeightLbs`, `LengthIn`, `WidthIn`, `HeightIn` must all
be greater than 0. Invalid input should result in a `400 Bad Request` from the API — the exact
validation mechanism is up to you (data annotations, manual checks, etc.), but the failing tests
in `RateCalculatorTests.cs` describe the expected behavior.
