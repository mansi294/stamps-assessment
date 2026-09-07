import { useState } from 'react'
import type { FormEvent } from 'react'
import { createShipment, getRateQuote } from '../api/client'
import type { RateQuoteRequest, RateQuoteResponse, Shipment } from '../api/client'

interface QuoteFormProps {
  onShipmentCreated: (shipment: Shipment) => void
}

interface FormState {
  weightLbs: string
  lengthIn: string
  widthIn: string
  heightIn: string
  zone: string
}

const initialFormState: FormState = {
  weightLbs: '',
  lengthIn: '',
  widthIn: '',
  heightIn: '',
  zone: '1',
}

function toRequest(form: FormState): RateQuoteRequest {
  return {
    weightLbs: Number(form.weightLbs),
    lengthIn: Number(form.lengthIn),
    widthIn: Number(form.widthIn),
    heightIn: Number(form.heightIn),
    zone: Number(form.zone),
  }
}

export function QuoteForm({ onShipmentCreated }: QuoteFormProps) {
  const [form, setForm] = useState<FormState>(initialFormState)
  const [quote, setQuote] = useState<RateQuoteResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)

  function updateField(field: keyof FormState) {
    return (event: React.ChangeEvent<HTMLInputElement>) => {
      setForm((prev) => ({ ...prev, [field]: event.target.value }))
    }
  }

  async function handleGetQuote(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setIsSubmitting(true)
    try {
      const response = await getRateQuote(toRequest(form))
      setQuote(response)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to get quote')
      setQuote(null)
    } finally {
      setIsSubmitting(false)
    }
  }

  async function handleCreateShipment() {
    setError(null)
    setIsSubmitting(true)
    try {
      const shipment = await createShipment(toRequest(form))
      onShipmentCreated(shipment)
      setQuote(null)
      setForm(initialFormState)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create shipment')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="quote-form">
      <h2>Get a rate quote</h2>
      <form onSubmit={handleGetQuote}>
        <label>
          Weight (lbs)
          <input
            type="number"
            min="0.001"
            step="any"
            required
            value={form.weightLbs}
            onChange={updateField('weightLbs')}
          />
        </label>
        <label>
          Length (in)
          <input
            type="number"
            min="0.001"
            step="any"
            required
            value={form.lengthIn}
            onChange={updateField('lengthIn')}
          />
        </label>
        <label>
          Width (in)
          <input
            type="number"
            min="0.001"
            step="any"
            required
            value={form.widthIn}
            onChange={updateField('widthIn')}
          />
        </label>
        <label>
          Height (in)
          <input
            type="number"
            min="0.001"
            step="any"
            required
            value={form.heightIn}
            onChange={updateField('heightIn')}
          />
        </label>
        <label>
          Zone (1-8)
          <input
            type="number"
            min="1"
            max="8"
            required
            value={form.zone}
            onChange={updateField('zone')}
          />
        </label>

        <div className="quote-form-actions">
          <button type="submit" disabled={isSubmitting}>
            Get quote
          </button>
          <button type="button" disabled={isSubmitting} onClick={handleCreateShipment}>
            Create shipment
          </button>
        </div>
      </form>

      {error && <p className="error">{error}</p>}

      {quote && (
        <table className="quote-breakdown">
          <tbody>
            <tr>
              <td>Billable weight</td>
              <td>
                {quote.billableWeightLbs.toFixed(2)} lbs
                {quote.usedDimensionalWeight && ' (dimensional)'}
              </td>
            </tr>
            <tr>
              <td>Base rate</td>
              <td>${quote.baseRate.toFixed(2)}</td>
            </tr>
            <tr>
              <td>Zone surcharge</td>
              <td>${quote.zoneSurcharge.toFixed(2)}</td>
            </tr>
            <tr>
              <td>Oversize surcharge</td>
              <td>${quote.oversizeSurcharge.toFixed(2)}</td>
            </tr>
            <tr className="total">
              <td>Total</td>
              <td>${quote.total.toFixed(2)}</td>
            </tr>
          </tbody>
        </table>
      )}
    </div>
  )
}
