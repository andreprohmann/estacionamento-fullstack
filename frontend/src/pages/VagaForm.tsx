import { FormEvent, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { api } from '../api/client'
import type { Vaga, VagaCreate, VagaUpdate } from '../types'

export default function VagaForm() {
  const { id } = useParams()
  const editing = !!id
  const navigate = useNavigate()

  const [form, setForm] = useState<VagaCreate|VagaUpdate>({ placa: '', marca: '', modelo: '', ano: new Date().getFullYear() })
  const [checkIn, setCheckIn] = useState<string>('')
  const [loading, setLoading] = useState<boolean>(editing)
  const [error, setError] = useState<string|undefined>()

  useEffect(() => {
    if (editing) {
      (async () => {
        try {
          const data: Vaga = await api.getVaga(Number(id))
          setForm({ placa: data.placa, marca: data.marca, modelo: data.modelo, ano: data.ano })
        } catch (e:any){ setError(e.message) } finally { setLoading(false) }
      })()
    }
  }, [id])

  async function onSubmit(e: FormEvent) {
    e.preventDefault()
    setError(undefined)
    try {
      if (editing) {
        await api.updateVaga(Number(id), form)
      } else {
        const payload: VagaCreate = { ...(form as VagaCreate) }
        if (checkIn) {
          const local = new Date(checkIn)
          const tzOffsetMin = local.getTimezoneOffset()
          const sign = tzOffsetMin > 0 ? '-' : '+'
          const hh = String(Math.floor(Math.abs(tzOffsetMin)/60)).padStart(2,'0')
          const mm = String(Math.abs(tzOffsetMin)%60).padStart(2,'0')
          const isoLocal = `${checkIn}:00${sign}${hh}:${mm}`
          payload.checkIn = isoLocal
        }
        await api.createVaga(payload)
      }
      navigate('/vagas')
    } catch (e:any){ setError(e.message) }
  }

  function set<K extends keyof (VagaCreate|VagaUpdate)>(key: K, value: any){
    setForm(prev => ({ ...prev as any, [key]: value }))
  }

  if (loading) return <div className="card"><p>Carregando...</p></div>

  return (
    <div className="card">
      <h2>{editing ? 'Editar vaga' : 'Nova vaga (check-in)'}</h2>
      {error && <p style={{color:'crimson'}}>Erro: {error}</p>}
      <form onSubmit={onSubmit}>
        <label>
          <span>Placa</span>
          <input className="input" value={form.placa} onChange={e=>set('placa', e.target.value.toUpperCase())} required maxLength={10} />
        </label>
        <label>
          <span>Marca</span>
          <input className="input" value={form.marca} onChange={e=>set('marca', e.target.value)} required maxLength={60} />
        </label>
        <label>
          <span>Modelo</span>
          <input className="input" value={form.modelo} onChange={e=>set('modelo', e.target.value)} required maxLength={60} />
        </label>
        <label>
          <span>Ano</span>
          <input className="input" type="number" value={form.ano} onChange={e=>set('ano', Number(e.target.value))} required min={1950} max={2100} />
        </label>
        {!editing && (
          <label>
            <span>Check-in (opcional)</span>
            <input className="input" type="datetime-local" value={checkIn} onChange={e=>setCheckIn(e.target.value)} />
          </label>
        )}
        <div className="actions">
          <button className="btn primary" type="submit">Salvar</button>
          <button className="btn" onClick={()=>navigate('/vagas')} type="button">Cancelar</button>
        </div>
      </form>
    </div>
  )
}
