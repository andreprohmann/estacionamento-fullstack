import axios from "axios";

export const api = axios.create({
  baseURL: "http://localhost:5000/api",
});

export type Veiculo = {
  id: number;
  placa: string;
  modelo?: string | null;
  vagaId?: number | null;
  vaga: Vaga | null;
};

export type Vaga = {
  id: number;
  numero: number;
  ocupada: boolean;
  veiculoAtual?: Veiculo | null;
};
