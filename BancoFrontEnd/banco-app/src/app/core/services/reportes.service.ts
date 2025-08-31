import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API } from './api.config';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private base = API.base;

  constructor(private http: HttpClient) {}

  estadoCuentaPorUsuario(cliente: string, desde: string, hasta: string): Observable<Blob> {
    const params = new HttpParams()
      .set('cliente', cliente) 
      .set('desde', desde)       
      .set('hasta', hasta);      

    return this.http.get(`${this.base}${API.movimientos.estadoCuenta}`, {
      params,
      responseType: 'blob'
    });
  }
}
