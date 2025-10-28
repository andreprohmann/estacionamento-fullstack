import { Link, Route, Routes, Navigate } from 'react-router-dom'
import VagasList from './pages/VagasList'
import VagaForm from './pages/VagaForm'
import VagaCheckout from './pages/VagaCheckout'

export default function App() {
  return (
    <div className="container">
      <nav className="navbar">
        <h1>Estacionamento</h1>
        <div>
          <Link to="/vagas" className="btn">Vagas</Link>
          <Link to="/vagas/nova" className="btn primary">Nova</Link>
        </div>
      </nav>
      <Routes>
        <Route path="/" element={<Navigate to="/vagas" />} />
        <Route path="/vagas" element={<VagasList />} />
        <Route path="/vagas/nova" element={<VagaForm />} />
        <Route path="/vagas/:id/editar" element={<VagaForm />} />
        <Route path="/vagas/:id/checkout" element={<VagaCheckout />} />
      </Routes>
    </div>
  )
}
