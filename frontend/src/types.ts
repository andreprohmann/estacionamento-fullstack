export interface Vaga {
  id: number
  placa: string
  marca: string
  modelo: string
  ano: number
  checkInUtc: string
  checkOutUtc?: string | null
  minutosEstadia?: number | null
  valorCobrado?: number | null
  blocosDe30Min?: number | null
  valorPrevisto?: number | null
}

export interface VagaCreate {
  placa: string
  marca: string
  modelo: string
  ano: number
  checkIn?: string | null
}

export interface VagaUpdate {
  placa: string
  marca: string
  modelo: string
  ano: number
}
