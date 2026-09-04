using System.Collections.Concurrent;
using TakeHome.Api.Models;

namespace TakeHome.Api.Repositories;

public class InMemoryShipmentRepository : IShipmentRepository
{
	private readonly ConcurrentDictionary<Guid, Shipment> _shipments = new();

	public Shipment Add(Shipment shipment)
	{
		_shipments[shipment.Id] = shipment;
		return shipment;
	}

	public IReadOnlyList<Shipment> GetAll() =>
		_shipments.Values.OrderByDescending(s => s.CreatedAt).ToList();

	public Shipment? GetById(Guid id) =>
		_shipments.TryGetValue(id, out var shipment) ? shipment : null;
}
