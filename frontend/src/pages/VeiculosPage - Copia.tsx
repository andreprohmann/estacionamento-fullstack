import { useEffect, useState } from "react";
import { api, Veiculo, Vaga } from "../api/api";

export default function VeiculosPage() {
  const [veiculos, setVeiculos] = useState<Veiculo[]>([]);
  const [vagas, setVagas] = useState<Vaga[]>([]);
  const [placa, setPlaca] = useState("");
  const [modelo, setModelo] = useState("");
  const [editId, setEditId] = useState<number | null>(null);
  const [vagaEscolhida, setVagaEscolhida] = useState<number | null>(null);

  const load = async () => {
    const [v1, v2] = await Promise.all([
      api.get<Veiculo[]>("/veiculos"),
      api.get<Vaga[]>("/vagas"),
    ]);
    setVeiculos(v1.data);
    setVagas(v2.data);
  };

  useEffect(() => { load(); }, []);

  const save = async () => {
    const payload = { placa, modelo };
    if (editId) {
      await api.put(`/veiculos/${editId}`, payload);
    } else {
      await api.post(`/veiculos`, payload);
    }
    setPlaca(""); setModelo(""); setEditId(null);
    await load();
  };

  const remove = async (id: number) => {
    if (!confirm("Excluir este veículo?")) return;
    await api.delete(`/veiculos/${id}`);
    await load();
  };

  const ocupar = async (id: number) => {
    if (!vagaEscolhida) { alert("Escolha uma vaga livre."); return; }
    await api.post(`/veiculos/${id}/ocupar/${vagaEscolhida}`);
    setVagaEscolhida(null);
    await load();
  };

  const liberar = async (id: number) => {
    await api.post(`/veiculos/${id}/liberar`);
    await load();
  };

  const vagasLivres = vagas.filter(v => !v.ocupada);

  return (
    <div style={{ padding: 16 }}>
      <h2>Veículos</h2>

      <div style={{ marginBottom: 16 }}>
        <input
          placeholder="Placa (ABC1D23)"
          value={placa}
          onChange={(e) => setPlaca(e.target.value)}
          style={{ textTransform: "uppercase" }}
        />
        <input
          placeholder="Modelo"
          value={modelo}
          onChange={(e) => setModelo(e.target.value)}
        />
        <button onClick={save}>{editId ? "Atualizar" : "Cadastrar"}</button>
        {editId && <button onClick={() => { setEditId(null); setPlaca(""); setModelo(""); }}>Cancelar</button>}
      </div>

      <div style={{ marginBottom: 16 }}>
        <label>Vaga para ocupar: </label>
        <select value={vagaEscolhida ?? ""} onChange={(e) => setVagaEscolhida(e.target.value ? parseInt(e.target.value) : null)}>
          <option value="">-- selecione vaga livre --</option>
          {vagasLivres.map(v => <option key={v.id} value={v.id}>#{v.numero}</option>)}
        </select>
      </div>

      <table border={1} cellPadding={6} style={{ borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Placa</th>
            <th>Modelo</th>
            <th>Vaga</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {veiculos.map(v => (
            <tr key={v.id}>
              <td>{v.id}</td>
              <td>{v.placa}</td>
              <td>{v.modelo ?? "-"}</td>
              <td>{v.vaga?.numero ?? "-"}</td>
              <td>
                <button onClick={() => { setEditId(v.id); setPlaca(v.placa); setModelo(v.modelo ?? ""); }}>Editar</button>
                <button onClick={() => remove(v.id)} disabled={!!v.vagaId}>Excluir</button>
                {!v.vagaId
                  ? <button onClick={() => ocupar(v.id)} disabled={!vagaEscolhida}>Ocupar</button>
                  : <button onClick={() => liberar(v.id)}>Liberar</button>}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
