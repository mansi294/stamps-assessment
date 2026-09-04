import { useEffect, useState } from 'react'
import { listShipments } from './api/client'
import type { Shipment } from './api/client'
import { QuoteForm } from './components/QuoteForm'
import { ShipmentsList } from './components/ShipmentsList'
import './App.css'

function App() {
  const [shipments, setShipments] = useState<Shipment[]>([])

  useEffect(() => {
    listShipments()
      .then(setShipments)
      .catch(() => setShipments([]))
  }, [])

  function handleShipmentCreated(shipment: Shipment) {
    setShipments((prev) => [shipment, ...prev])
  }

  return (
    <main className="app">
      <h1>Package Rate Quote</h1>
      <QuoteForm onShipmentCreated={handleShipmentCreated} />
      <ShipmentsList shipments={shipments} />
    </main>
  )
}

export default App
