// src/app/core/services/clientes.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { API } from './api.config';
import { Observable } from 'rxjs';
import { Cliente, ClienteResponse} from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private base = API.base;

  constructor(private http: HttpClient) {}
list(): Observable<ClienteResponse[]> {
    this.http.get<ClienteResponse>('http://localhost:5297/api/clientes/ConsultaClientes')
  .subscribe(res => {
    if (res.isSuccess) {
      console.log('Clientes:', res.data);
    }
  });
      return this.http.get<ClienteResponse[]>(`${API.base}${API.clientes.listar}`);

  }


  consultaPorId(id: number): Observable<Cliente> {
    return this.http.get<Cliente>(`${this.base}${API.clientes.consulta}/${id}`);
  }

  create(body: Omit<Cliente,'id'>): Observable<Cliente> {
    return this.http.post<Cliente>(`${this.base}${API.clientes.crear}`, body);
  }

  update(id: number, body: Partial<Cliente>): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.base}${API.clientes.actualizar}/${id}`, body);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}${API.clientes.eliminar}/${id}`);
  }
}

