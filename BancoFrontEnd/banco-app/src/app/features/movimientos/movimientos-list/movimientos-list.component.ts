import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { API } from '../../../core/services/api.config';
import { ToastService } from '../../../core/services/toast.service';

export interface MovimientoCuentas {
  numeroCuenta: string;
  tipoCuenta: string;          // viene como texto del DTO
  saldoInicial: number;
  anulado: boolean;
  movimientoNeto: number;
  tipoMovimiento: string;
}

// Si tu API responde con un objeto { data: MovimientoCuentasDto[] }
export interface MovimientoCuentasResponse {
  data: MovimientoCuentas[];
}

@Component({
  selector: 'app-movimientos-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  styles: [`
    .toolbar {
      display: flex;
      gap: .5rem;
      align-items: center;
      margin-bottom: .75rem;
      flex-wrap: wrap;
    }
    .toolbar input[type="text"], .toolbar select, .toolbar input[type="number"] {
      padding: .35rem .5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
    }
    .tbl {
      width: 100%;
      border-collapse: collapse;
      background: #fff;
    }
    .tbl th, .tbl td {
      border: 1px solid #eee;
      padding: .5rem .6rem;
      text-align: left;
      vertical-align: middle;
      font-size: 14px;
    }
    .tbl thead th {
      background: #f6f7f8;
      position: sticky;
      top: 0;
      z-index: 1;
    }
    .actions {
      display: flex;
      gap: .5rem;
    }
    .actions button {
      padding: .35rem .6rem;
      border: 1px solid #ccc;
      background: #f9f9f9;
      border-radius: 4px;
      cursor: pointer;
    }
    .actions button:disabled {
      opacity: .6;
      cursor: not-allowed;
    }
    .badge {
      display: inline-block;
      padding: .15rem .5rem;
      border-radius: 999px;
      font-size: 12px;
      line-height: 1;
    }
    .badge--ok { background: #e6f7ed; color: #137a3b; border: 1px solid #bfe7cd; }
    .badge--off { background: #fdecea; color: #b71c1c; border: 1px solid #f5c6c4; }

    /* Modal simple */
    .modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0,0,0,.45);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }
    .modal {
      width: min(560px, 95vw);
      background: #fff;
      border-radius: 10px;
      padding: 1rem;
      box-shadow: 0 8px 28px rgba(0,0,0,.25);
    }
    .modal h4 { margin: 0 0 .75rem 0; }
    .grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: .75rem;
    }
    .grid label {
      display: flex;
      flex-direction: column;
      gap: .3rem;
      font-size: 14px;
    }
    .grid input, .grid select {
      padding: .45rem .55rem;
      border: 1px solid #ddd;
      border-radius: 6px;
      font-size: 14px;
    }
    .error { color: #c62828; font-size: 12px; }
    .modal-actions {
      margin-top: 1rem;
      display: flex;
      justify-content: flex-end;
      gap: .5rem;
    }
    .modal-actions button {
      padding: .45rem .8rem;
      border-radius: 6px;
      border: 1px solid #ccc;
      background: #fafafa;
      cursor: pointer;
    }
    .modal-actions button[type="submit"] {
      border-color: #1275bb;
      background: #1275bb;
      color: #fff;
    }

    .table-wrap {
      max-height: 60vh;
      overflow: auto;
      border: 1px solid #eee;
      border-radius: 8px;
    }
  `],
  template: `
  <h3>Cuentas</h3>

  <!-- Toolbar con buscador y filtros -->
  <div class="toolbar">
    <input type="text"
           placeholder="Buscar por número o persona…"
           [(ngModel)]="filtroTexto"
           (input)="aplicarFiltro()"
           style="min-width: 260px;">
    
    <button type="button" (click)="recargar()">Actualizar</button>
    <span style="margin-left:auto; font-size:12px; opacity:.8">Mostrando {{itemsFiltrados.length}} / {{items.length}}</span>
  </div>

  <div *ngIf="loading">Cargando…</div>

  <div class="table-wrap" *ngIf="!loading">
    <table class="tbl">
      <thead>
        <tr>
          <th>Numero Cuenta</th>
          <th>Tipo Cuenta</th>
          <th>Saldo Inicial</th>
          <th>Movimiento Neto</th>
          <th>Tipo Movimiento</th>
          <th>Estado</th>

          <th style="width:220px">Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let c of itemsFiltrados; trackBy: trackById">
          <td>{{ c.numeroCuenta }}</td>
          <td>{{ c.tipoCuenta }}</td>
          <td>{{ c.saldoInicial | number:'1.2-2' }}</td>
          <td>{{ c.movimientoNeto }}</td>
          <td>{{ c.tipoMovimiento}}</td>

          <td>
            <span class="badge" [class.badge--ok]="!c.anulado" [class.badge--off]="c.anulado">
              {{ !c.anulado ? 'Activo' : 'Inactivo' }}
            </span>
          </td>
          <td class="actions">
            <button type="button" (click)="abrirMovimiento('deposito', c)">Depositar</button>
            <button type="button" [disabled]="c.anulado" (click)="abrirMovimiento('retiro', c)">Retirar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>

  <!-- Modal Movimiento (Depósito/Retiro) -->
  <div class="modal-backdrop" *ngIf="modalOpen && modalMode==='mov'">
    <div class="modal">
      <h4>{{ movTipo === 'deposito' ? 'Depositar' : 'Retirar' }} – {{ current?.numeroCuenta }}</h4>

      <form [formGroup]="movForm" (ngSubmit)="confirmarMovimiento()">
        <div class="grid">
          <label>
            Monto
            <input type="number" step="0.01" min="0.01" formControlName="valor" inputmode="decimal" />
          </label>
          <div class="error" *ngIf="movForm.get('valor')?.touched && movForm.get('valor')?.invalid">
            Ingrese un monto válido (> 0)
          </div>

          <label>
            Fecha
            <input type="datetime-local" formControlName="fecha" />
          </label>
        </div>

        <div class="modal-actions">
          <button type="button" (click)="cerrarModal()">Cancelar</button>
          <button type="submit" [disabled]="movForm.invalid || processing">
            {{ processing ? 'Procesando…' : (movTipo === 'deposito' ? 'Confirmar depósito' : 'Confirmar retiro') }}
          </button>
        </div>
      </form>
    </div>
  </div>
  `
})
export class MovimientosListComponent implements OnInit {

  items: MovimientoCuentas[] = [];
  itemsFiltrados: MovimientoCuentas[] = [];
  loading = true;

  // ---- Filtros ----
  filtroTexto = '';
  filtroTipo: 0 | 1 | 2 = 0;        // 0=Todos
  filtroEstado: -1 | 0 | 1 = -1;    // -1=Todos, 1=Activo, 0=Inactivo
  filtroSaldoMin?: number;
  filtroSaldoMax?: number;

  // ---- Modal movimiento ----
  modalOpen = false;
  modalMode: 'mov' | null = null;
  current?: MovimientoCuentas;
  movTipo: 'deposito' | 'retiro' = 'deposito';
  processing = false;

  // ---- Inyecciones / services ----
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);

  // Form del movimiento (sólo lo que exige tu API)
  movForm: FormGroup = this.fb.group({
    valor: [null, [Validators.required, Validators.min(0.01)]],
    // input datetime-local entrega "yyyy-MM-ddTHH:mm" (sin zona). Lo convertimos a ISO UTC al enviar.
    fecha: [this.toLocalInputValue(new Date()), [Validators.required]]
  });

  ngOnInit(): void {
    this.cargar();
  }

  // ------------------ Data ------------------
  cargar(): void {
    this.loading = true;
    this.http.get<MovimientoCuentasResponse>(`${API.base}${API.movimientos.listar}`)
      .subscribe({
        next: (res) => {
          console.log(res?.data )
          this.items = res?.data ?? [];
          this.refrescarFiltro();
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
  }

  async recargar() { this.cargar(); }

  // ------------------ Filtros ------------------
  aplicarFiltro(): void { this.refrescarFiltro(); }

 private refrescarFiltro(): void {
  const q = (this.filtroTexto || '').toLowerCase().trim();

  // si no hay filtro, clona todos
  if (!q) {
    this.itemsFiltrados = this.items.slice();
    return;
  }

  // filtra por número de cuenta o tipo de cuenta (texto)
  this.itemsFiltrados = this.items.filter(c =>
    (c.numeroCuenta ?? '').toLowerCase().includes(q) ||
    (c.tipoCuenta ?? '').toLowerCase().includes(q)
  );
}

  // ------------------ Movimientos ------------------
  abrirMovimiento(tipo: 'deposito' | 'retiro', c: MovimientoCuentas): void {
    this.current = c;
    this.movTipo = tipo;
    this.movForm.reset({
      valor: null,
      fecha: this.toLocalInputValue(new Date())
    });
    this.processing = false;
    this.modalMode = 'mov';
    this.modalOpen = true;
  }

  confirmarMovimiento(): void {
    if (!this.current || this.movForm.invalid || this.processing) return;
    this.processing = true;

    const raw = this.movForm.getRawValue();
    const body = {
      // tus endpoints requieren este shape exacto:
      // { "numeroCuenta": "string", "valor": number, "fecha": "ISO8601" }
      numeroCuenta: this.current.numeroCuenta,
      valor: Number(raw.valor),
      fecha: this.localToIsoUtc(raw.fecha) // convertimos a ISO-UTC con sufijo Z
    };

    const url = this.movTipo === 'deposito'
      ? `${API.base}/movimientos/depositos`
      : `${API.base}/movimientos/retiros`;

    this.http.post(url, body).subscribe({
      next: () => {
        alert(`${this.movTipo === 'deposito' ? 'Depósito' : 'Retiro'} realizado correctamente.`);
        // Si deseas, recarga la lista (o actualiza sólo la fila)
        this.cerrarModal(true);
      },
       error: (err) => {
        new ToastService().error("sssss");
    alert(this.mapError(err));   // << usamos tu mapError mejorado
    this.processing = false;
  }
    });
  }

  cerrarModal(refresh = false): void {
    this.modalOpen = false;
    this.modalMode = null;
    this.current = undefined;
    this.processing = false;
    if (refresh) this.cargar();
  }

  trackById = (_: number, item: MovimientoCuentas) => item.numeroCuenta;

  /** A <input type="datetime-local> */
  private toLocalInputValue(d: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }

  /** Convierte "yyyy-MM-ddTHH:mm" (local) a ISO-UTC con Z */
  private localToIsoUtc(localStr: string | Date): string {
    if (localStr instanceof Date) return localStr.toISOString();
    // new Date('yyyy-MM-ddTHH:mm') lo interpreta en zona local; toISOString() lo normaliza a UTC con "Z"
    return new Date(localStr).toISOString();
  }

private mapError(e: any): string {
  if (!e) return 'Error desconocido';

  const status = e?.status as number | undefined;
  const data = e?.error ?? e;

  // Texto plano
  if (typeof data === 'string') return data.trim();

  // ProblemDetails (ASP.NET Core)
  if (data?.detail || data?.title) {
    return data.detail || data.title;
  }

  // ValidationProblemDetails
  if (data?.errors && typeof data.errors === 'object') {
    const msgs: string[] = [];
    for (const k of Object.keys(data.errors)) {
      const arr = data.errors[k];
      if (Array.isArray(arr)) {
        msgs.push(...arr);
      }
    }
    if (msgs.length) return msgs.join('\n');
  }

  // Mensaje directo
  if (data?.message) return data.message;
  if (data?.error) return data.error;

  // Array de strings
  if (Array.isArray(data) && data.every(x => typeof x === 'string')) {
    return data.join('\n');
  }

  // Por status code
  if (status) {
    switch (status) {
      case 0: return 'No hay conexión con el servidor.';
      case 400: return 'Solicitud inválida (400).';
      case 401: return 'No autorizado (401).';
      case 403: return 'Prohibido (403).';
      case 404: return 'No encontrado (404).';
      case 409: return 'Conflicto (409).';
      case 422: return 'Entidad no procesable (422).';
      case 500: return 'Error interno del servidor (500).';
      default: return `Error HTTP ${status}.`;
    }
  }

  // Fallback
  if (e?.message) return e.message;
  return 'Se produjo un error inesperado.';
}

}
