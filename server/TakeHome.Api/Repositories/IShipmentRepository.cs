using TakeHome.Api.Models;

namespace TakeHome.Api.Repositories;

public interface IShipmentRepository
{
	Shipment Add(Shipment shipment);
	IReadOnlyList<Shipment> GetAll();
	Shipment? GetById(Guid id);
}
