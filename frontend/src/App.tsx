import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import VagasPage from "./pages/VagasPage";
import VeiculosPage from "./pages/VeiculosPage";

export default function App() {
  return (
    <BrowserRouter>
      <nav style={{ display: "flex", gap: 12, padding: 12, borderBottom: "1px solid #ddd" }}>
        <Link to="/vagas">Vagas</Link>
        <Link to="/veiculos">Veículos</Link>
      </nav>
      <Routes>
        <Route path="/" element={<VagasPage />} />
        <Route path="/vagas" element={<VagasPage />} />
        <Route path="/veiculos" element={<VeiculosPage />} />
      </Routes>
    </BrowserRouter>
  );
}
