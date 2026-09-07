# C# Full-Stack Take-Home Exercise

Thanks for taking the time to work on this! It's a small, self-contained **package rate quote
service** — an API plus a minimal client — themed loosely on shipping rates (not real business
logic, just a fun domain to model).

**Expected time: around 3 hours.** We're not looking for a polished product — focus your time on
correct, well-tested logic over UI polish. AI usage is encourage.

## What's here

- `server/TakeHome.Api` — ASP.NET Core Web API (.NET 10)
- `server/TakeHome.Api.Tests` — xUnit tests (some are already failing — see below)
- `client/` — Vite + React + TypeScript client
- `docs/RATE_RULES.md` — **the spec you're implementing against.** Read this first.

## Your task

Implement `IRateCalculator` in
[`server/TakeHome.Api/Services/RateCalculator.cs`](server/TakeHome.Api/Services/RateCalculator.cs)
according to the rules in [`docs/RATE_RULES.md`](docs/RATE_RULES.md).

The test suite in
[`server/TakeHome.Api.Tests/RateCalculatorTests.cs`](server/TakeHome.Api.Tests/RateCalculatorTests.cs)
describes the expected behavior — it currently fails because `RateCalculator` throws
`NotImplementedException`. Your goal is to make it pass, paying close attention to the tier
boundary cases.

The API and client around it (`RatesController`, `ShipmentsController`, the in-memory repository,
the React form/list) are already wired up and working — you shouldn't need to change them, but
feel free to if you want to demonstrate more (e.g. extra validation, extra tests, UI tweaks).

## Running it

### Server

```bash
cd server
dotnet test                                                    # currently failing — that's expected
dotnet run --project TakeHome.Api --urls http://localhost:5013
```

### Client

```bash
cd client
npm install
npm run dev            # http://localhost:5173 — talks to the API at http://localhost:5013
```

## Submitting

Push your work to a repo of your own (fork or new repo) and send us the link, Please include a short note on any
assumptions or trade-offs you made, and anything you'd do differently with more time.

Good luck — reach out if anything in the setup is unclear (that's a setup bug on our end, not a
test of your patience).

## Implementation Notes

### What I Implemented

* Implemented `IRateCalculator` in `server/TakeHome.Api/Services/RateCalculator.cs` according to the provided rate rules, including dimensional weight, billable weight, base-rate tiers, zone surcharges, oversize surcharges, and total calculation.
* Added strict weight and dimension validation in `server/TakeHome.Api/Models/RateQuoteRequest.cs` using a custom `GreaterThanZero` validation attribute.
* Kept the existing `[Range(1, 8)]` validation for the `Zone` field.
* Added an explicit `Microsoft.OpenApi` package reference in `server/TakeHome.Api/TakeHome.Api.csproj` to resolve the reported high-severity transitive dependency vulnerability.
* Verified the API and client end-to-end, including rate quoting, shipment creation, and displaying created shipments in the client.

### Testing
* All existing xUnit tests pass: 19/19.
* Tests cover the rate-tier boundary cases, including weights at 1 lb, 5 lb, and 20 lb, as well as dimensional weight, zone, and oversize boundaries.
* Verified API validation returns HTTP 400 for invalid input.
* Verified valid rate quote calculations through the API.
* Verified shipment creation and shipment listing through the client.
* Verified the client production build completes successfully with `npm run build`.

### Assumptions and Trade-offs
* Kept the existing application structure and interfaces rather than introducing additional abstractions that were not required by the exercise.
* Kept the rate calculation logic in `RateCalculator` and used named constants for the business-rule values to keep the implementation readable and maintainable.
* Used the greater of actual and dimensional weight without rounding the billable weight before applying the rate tiers, following the provided rules.
* Kept the in-memory shipment repository since persistence was outside the scope of the exercise.
