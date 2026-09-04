import type { Shipment } from '../api/client'

interface ShipmentsListProps {
  shipments: Shipment[]
}

export function ShipmentsList({ shipments }: ShipmentsListProps) {
  return (
    <div className="shipments-list">
      <h2>Shipments</h2>
      {shipments.length === 0 ? (
        <p>No shipments yet.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Created</th>
              <th>Weight (lbs)</th>
              <th>Dimensions (in)</th>
              <th>Zone</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            {shipments.map((shipment) => (
              <tr key={shipment.id}>
                <td>{new Date(shipment.createdAt).toLocaleString()}</td>
                <td>{shipment.request.weightLbs}</td>
                <td>
                  {shipment.request.lengthIn} x {shipment.request.widthIn} x{' '}
                  {shipment.request.heightIn}
                </td>
                <td>{shipment.request.zone}</td>
                <td>${shipment.quote.total.toFixed(2)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
