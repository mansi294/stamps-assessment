namespace TakeHome.Api.Models;

public record Shipment(
	Guid Id,
	DateTimeOffset CreatedAt,
	RateQuoteRequest Request,
	RateQuoteResponse Quote
);
