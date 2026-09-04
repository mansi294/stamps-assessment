const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5013'

export interface RateQuoteRequest {
  weightLbs: number
  lengthIn: number
  widthIn: number
  heightIn: number
  zone: number
}

export interface RateQuoteResponse {
  billableWeightLbs: number
  usedDimensionalWeight: boolean
  baseRate: number
  zoneSurcharge: number
  oversizeSurcharge: number
  total: number
}

export interface Shipment {
  id: string
  createdAt: string
  request: RateQuoteRequest
  quote: RateQuoteResponse
}

async function parseJsonOrThrow<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const body = await response.text()
    throw new Error(`Request failed (${response.status}): ${body || response.statusText}`)
  }
  return response.json() as Promise<T>
}

export function getRateQuote(request: RateQuoteRequest): Promise<RateQuoteResponse> {
  return fetch(`${API_BASE_URL}/api/rates/quote`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  }).then((response) => parseJsonOrThrow<RateQuoteResponse>(response))
}

export function createShipment(request: RateQuoteRequest): Promise<Shipment> {
  return fetch(`${API_BASE_URL}/api/shipments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  }).then((response) => parseJsonOrThrow<Shipment>(response))
}

export function listShipments(): Promise<Shipment[]> {
  return fetch(`${API_BASE_URL}/api/shipments`).then((response) =>
    parseJsonOrThrow<Shipment[]>(response),
  )
}
