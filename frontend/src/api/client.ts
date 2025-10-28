const API_BASE = import.meta.env.VITE_API_BASE || 'http://localhost:5000'

async function http<T>(url: string, options?: RequestInit): Promise<T> {
  const resp = await fetch(API_BASE + url, {
    headers: { 'Content-Type': 'application/json', ...(options?.headers || {}) },
    ...options,
  })
  if (!resp.ok) {
    let msg = `${resp.status} ${resp.statusText}`
    try { const j = await resp.json(); msg = j.message || msg } catch {}
    throw new Error(msg)
  }
  if (resp.status === 204) return undefined as T
  return resp.json() as Promise<T>
}

export const api = {
  listVagas: (abertas?: boolean) => http(`/api/vagas${abertas ? '?abertas=true' : ''}`),
  getVaga: (id: number) => http(`/api/vagas/${id}`),
  createVaga: (data: any) => http(`/api/vagas`, { method: 'POST', body: JSON.stringify(data) }),
  updateVaga: (id: number, data: any) => http(`/api/vagas/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  deleteVaga: (id: number) => http(`/api/vagas/${id}`, { method: 'DELETE' }),
  checkoutVaga: (id: number, data: any) => http(`/api/vagas/${id}/checkout`, { method: 'PUT', body: JSON.stringify(data) }),
}
