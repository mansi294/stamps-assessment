using Microsoft.AspNetCore.Mvc;
using TakeHome.Api.Models;
using TakeHome.Api.Repositories;
using TakeHome.Api.Services;

namespace TakeHome.Api.Controllers;

[ApiController]
[Route("api/shipments")]
public class ShipmentsController : ControllerBase
{
	private readonly IRateCalculator _rateCalculator;
	private readonly IShipmentRepository _shipmentRepository;

	public ShipmentsController(IRateCalculator rateCalculator, IShipmentRepository shipmentRepository)
	{
		_rateCalculator = rateCalculator;
		_shipmentRepository = shipmentRepository;
	}

	[HttpPost]
	public ActionResult<Shipment> Create([FromBody] RateQuoteRequest request)
	{
		var quote = _rateCalculator.Calculate(request);
		var shipment = new Shipment(Guid.NewGuid(), DateTimeOffset.UtcNow, request, quote);
		_shipmentRepository.Add(shipment);
		return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
	}

	[HttpGet]
	public ActionResult<IReadOnlyList<Shipment>> GetAll()
	{
		return Ok(_shipmentRepository.GetAll());
	}

	[HttpGet("{id:guid}")]
	public ActionResult<Shipment> GetById(Guid id)
	{
		var shipment = _shipmentRepository.GetById(id);
		return shipment is null ? NotFound() : Ok(shipment);
	}
}
