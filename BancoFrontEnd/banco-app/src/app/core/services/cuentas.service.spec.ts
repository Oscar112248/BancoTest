import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { API } from './api.config';
import { Observable } from 'rxjs';
import { Cliente } from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private url = API.base + API.clientes;

  constructor(private http: HttpClient) {}

  list(q = '', page = 1, size = 10): Observable<{items: Cliente[]; total: number}> {
    const params = new HttpParams().set('q', q).set('page', page).set('size', size);
    return this.http.get<{items: Cliente[]; total: number}>(this.url, { params });
  }

  get(id: number)       { return this.http.get<Cliente>(`${this.url}/${id}`); }
  create(body: Omit<Cliente,'id'>) { return this.http.post<Cliente>(this.url, body); }
  update(id: number, body: Partial<Cliente>) { return this.http.put<Cliente>(`${this.url}/${id}`, body); }
  delete(id: number)    { return this.http.delete<void>(`${this.url}/${id}`); }
}
