import { useEffect, useState } from "react";
import { api, Vaga } from "../api/api";

export default function VagasPage() {
  const [vagas, setVagas] = useState<Vaga[]>([]);
  const [numero, setNumero] = useState<number>(0);
  const [editId, setEditId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [msg, setMsg] = useState<string | null>(null);

  const load = async () => {
    const { data } = await api.get<Vaga[]>("/vagas");
    setVagas(data);
  };

  useEffect(() => { load(); }, []);

  const save = async () => {
    setLoading(true);
    try {
      if (editId) {
        await api.put(`/vagas/${editId}`, { id: editId, numero });
        setMsg("Vaga atualizada.");
      } else {
        await api.post("/vagas", { numero });
        setMsg("Vaga criada.");
      }
      setNumero(0);
      setEditId(null);
      await load();
    } catch (e: any) {
      setMsg(e.response?.data ?? "Erro ao salvar.");
    } finally {
      setLoading(false);
    }
  };

  const remove = async (id: number) => {
    if (!confirm("Excluir esta vaga?")) return;
    try {
      await api.delete(`/vagas/${id}`);
      await load();
    } catch (e: any) {
      alert(e.response?.data ?? "Erro ao excluir.");
    }
  };

  return (
    <div style={{ padding: 16 }}>
      <h2>Vagas</h2>

      <div style={{ marginBottom: 16 }}>
        <input
          type="number"
          placeholder="Número da vaga"
          value={numero || ""}
          onChange={(e) => setNumero(parseInt(e.target.value || "0"))}
        />
        <button disabled={loading || !numero} onClick={save}>
          {editId ? "Atualizar" : "Criar"}
        </button>
        {editId && <button onClick={() => { setEditId(null); setNumero(0); }}>Cancelar</button>}
        {msg && <p>{msg}</p>}
      </div>

      <table border={1} cellPadding={6} style={{ borderCollapse: "collapse" }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Número</th>
            <th>Ocupada?</th>
            <th>Placa atual</th>
            <th>Ações</th>
          </tr>
        </thead>
        <tbody>
          {vagas.map(v => (
            <tr key={v.id}>
              <td>{v.id}</td>
              <td>{v.numero}</td>
              <td>{v.ocupada ? "Sim" : "Não"}</td>
              <td>{v.veiculoAtual?.placa ?? "-"}</td>
              <td>
                <button onClick={() => { setEditId(v.id); setNumero(v.numero); }}>Editar</button>
                <button onClick={() => remove(v.id)} disabled={v.ocupada}>Excluir</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
