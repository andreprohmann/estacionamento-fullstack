
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { api } from '../api/client'
import type { Vaga } from '../types'

export default function VagaCheckout() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [vaga, setVaga] = useState<Vaga | null>(null)
  const [checkOut, setCheckOut] = useState<string>('')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    (async () => {
      try {
        const v = await api.getVaga(Number(id))
        setVaga(v)
        const now = new Date()
        const local = new Date(now.getTime() - now.getTimezoneOffset()*60000)
        setCheckOut(local.toISOString().slice(0,16))
      } catch (e:any){ setError(e.message) }
    })()
  }, [id])

  async function doCheckout(){
    try {
      const tzOffsetMin = new Date().getTimezoneOffset()
      const sign = tzOffsetMin > 0 ? '-' : '+'
      const hh = String(Math.floor(Math.abs(tzOffsetMin)/60)).padStart(2,'0')
      const mm = String(Math.abs(tzOffsetMin)%60).padStart(2,'0')
      const isoLocal = `${checkOut}:00${sign}${hh}:${mm}`
      await api.checkoutVaga(Number(id), { checkOut: isoLocal })
      navigate('/vagas')
    } catch (e:any){ setError(e.message) }
  }

  if (!vaga) return <div className="card"><p>Carregando...</p>{error && <p style={{color:'crimson'}}>Erro: {error}</p>}</div>

  return (
    <div className="card">
      <h2>Checkout da vaga #{vaga.id} - {vaga.placa}</h2>
      {error && <p style={{color:'crimson'}}>Erro: {error}</p>}
      <p><b>Entrada:</b> {new Date(vaga.checkInUtc).toLocaleString('pt-BR')}</p>
      <p><b>Valor previsto até agora:</b> {(vaga.valorPrevisto ?? 0).toLocaleString('pt-BR', {style:'currency', currency:'BRL'})}</p>
      <label>
        <span>Data/hora de saída</span>
        <input className="input" type="datetime-local" value={checkOut} onChange={e=>setCheckOut(e.target.value)} />
      </label>
      <div className="actions" style={{marginTop:'.8rem'}}>
        <button className="btn primary" onClick={doCheckout}>Confirmar checkout</button>
        <button className="btn" onClick={()=>navigate('/vagas')}>Cancelar</button>
      </div>
    </div>
  )
}
