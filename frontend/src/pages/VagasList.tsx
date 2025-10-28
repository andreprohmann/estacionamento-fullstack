import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/client'
import type { Vaga } from '../types'

function formatMoney(v?: number | null) {
  if (v == null) return '-'
  return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
}

function formatDateTimeUtc(iso?: string | null) {
  if (!iso) return '-'
  const d = new Date(iso)
  return d.toLocaleString('pt-BR')
}

export default function VagasList() {
  const [vagas, setVagas] = useState<Vaga[]>([])
  const [loading, setLoading] = useState(true)
  const [abertas, setAbertas] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const load = async () => {
    setLoading(true); setError(null)
    try {
      const data = await api.listVagas(abertas)
      setVagas(data)
    } catch (e: any) {
      setError(e.message)
    } finally { setLoading(false) }
  }

  useEffect(() => { load() }, [abertas])

  return (
    <div className="card">
      <div style={{display:'flex', justifyContent:'space-between', alignItems:'center', marginBottom: '.8rem'}}>
        <h2>Vagas {abertas ? '(abertas)' : '(todas)'}</h2>
      </div>
      {loading && <p>Carregando...</p>}
      {error && <p style={{color:'crimson'}}>Erro: {error}</p>}
      {!loading && !error && (
        <table className="table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Placa</th>
              <th>Marca/Modelo</th>
              <th>CheckIn</th>
              <th>CheckOut</th>
              <th>Blocos</th>
              <th>Valor</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            {vagas.map(v => (
              <tr key={v.id}>
                <td>{v.id}</td>
                <td>{v.placa}</td>
                <td>{v.marca} / {v.modelo} ({v.ano})</td>
                <td>{formatDateTimeUtc(v.checkInUtc)}</td>
                <td>{formatDateTimeUtc(v.checkOutUtc || null)}</td>
                <td>{v.blocosDe30Min ?? '-'}</td>
                <td>{formatMoney(v.valorCobrado ?? v.valorPrevisto)}</td>
                <td className="actions">
                  {!v.checkOutUtc && <Link className="btn" to={`/vagas/${v.id}/editar`}>Editar</Link>}
                  {!v.checkOutUtc && <Link className="btn" to={`/vagas/${v.id}/checkout`}>Checkout</Link>}
                  <button className="btn" onClick={async()=>{ if(confirm('Excluir registro?')) { await api.deleteVaga(v.id); load(); } }}>Excluir</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}
