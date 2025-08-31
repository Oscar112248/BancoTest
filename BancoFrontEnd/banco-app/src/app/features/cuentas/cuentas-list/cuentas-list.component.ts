import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { API } from '../../../core/services/api.config';

export interface Cuenta {
  cuentaId: number;
  clienteIdPersona: number;
  numeroCuenta: string;
  tipoCuenta: number;
  saldoInicial: number;
  estado: boolean;
  fechaApertura: Date | string;
}
export interface CuentaResponse { data: Cuenta[]; }

@Component({
  selector: 'app-cuentas-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cuentas-list.component.html',
  styleUrls: ['./cuentas-list.component.scss']
})
export class CuentasListComponent implements OnInit {

  items: Cuenta[] = [];
  loading = true;

  modalOpen = false;
  modalMode: 'create' | 'edit' | 'delete' | null = null;
  current?: Cuenta;

  form!: FormGroup;

  constructor(
    private http: HttpClient,
    private router: Router,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    // Inicializa el form
    this.form = this.fb.group({
      clienteIdPersona: [null, [Validators.required]],
      numeroCuenta: ['', [Validators.required, Validators.minLength(5)]],
      tipoCuenta: [1, [Validators.required]],
      saldoInicial: [0, [Validators.required, Validators.min(0)]],
      estado: [false],
      // input type="datetime-local" entrega string "yyyy-MM-ddTHH:mm"
      fechaApertura: [this.toLocalInputValue(new Date()), [Validators.required]]
    });

    this.cargar();
  }

  // ------------------ Utilidades ------------------
  trackById = (_: number, item: Cuenta) => item.cuentaId;

  /** Convierte Date a string compatible con input datetime-local (sin zona) */
  private toLocalInputValue(d: Date): string {
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  }

  /** Convierte valor del control (string datetime-local o Date) a ISO string */
  private toIsoFromForm(value: string | Date): string {
    if (!value) return new Date().toISOString();
    // Si viene como string "yyyy-MM-ddTHH:mm"
    if (typeof value === 'string') return new Date(value).toISOString();
    return new Date(value).toISOString();
  }

  /** Habilita todos los controles del form */
  private enableAll(): void {
    Object.keys(this.form.controls).forEach(k => this.form.get(k)?.enable());
  }

  /** Deshabilita campos no editables en modo edición */
  private disableNonEditableOnEdit(): void {
    // Suponiendo que numeroCuenta no se debe cambiar
    this.form.get('numeroCuenta')?.disable();
  }

  // ------------------ Data ------------------
  cargar(): void {
    this.loading = true;
    this.http.get<CuentaResponse>(`${API.base}${API.cuentas.listar}`)
      .subscribe({
        next: (res) => {
          this.items = res?.data ?? [];
          this.loading = false;
        },
        error: () => { this.loading = false; }
      });
  }

nuevo(): void {
  this.current = undefined;
  this.modalMode = 'create';
  this.modalOpen = true;

  this.form.reset({
    clienteIdPersona: null,          // requerido
    numeroCuenta: '',                // requerido minLength 5
    tipoCuenta: 1,                   // requerido
    saldoInicial: 0,                 // min(0)
    estado: false,
    fechaApertura: this.toLocalInputValue(new Date()) // requerido
  });

  this.enableAll();

  // fuerza reevaluación de validadores (por si el datetime-local no emite evento)
  this.form.updateValueAndValidity({ onlySelf: false, emitEvent: true });
}

  guardarNuevo(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const body = {
      clienteIdPersona: Number(raw.clienteIdPersona),
      numeroCuenta: String(raw.numeroCuenta),
      tipoCuenta: raw.tipoCuenta == 1 ? "Ahorro":"Corriente",
      saldoInicial: Number(raw.saldoInicial)
    };
console.log(body);
    this.http.post(`${API.base}${API.cuentas.crear}`, body)
      .subscribe({
        next: () => this.cerrarModal(true),
        error: () => alert('No se pudo crear la cuenta')
      });
  }

  // ------------------ Editar ------------------
  editar(c: Cuenta): void {
    this.current = c;
    this.modalMode = 'edit';
    this.modalOpen = true;

    this.form.reset({
      clienteIdPersona: c.clienteIdPersona,
      numeroCuenta: c.numeroCuenta,
      tipoCuenta: c.tipoCuenta,
      saldoInicial: c.saldoInicial,
      estado: c.estado,
      // Normaliza fecha al input
      fechaApertura: this.toLocalInputValue(new Date(c.fechaApertura))
    });

    this.enableAll();
    this.disableNonEditableOnEdit();
  }

  guardarEdicion(): void {
    if (!this.current) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const body = {
      
      // numeroCuenta: String(raw.numeroCuenta || this.current.numeroCuenta),
      tipoCuenta:  raw.tipoCuenta === 1 ? "Ahorro":"Corriente",
      eliminaCuenta: !!raw.estado,
      estado: !!raw.estado,
      clienteIdPersona: raw.clienteIdPersona
    };
console.log(body);
    // Ajusta según tu API (por id)
    this.http.put(`${API.base}${API.cuentas.actualizar}/${this.current.numeroCuenta}`, body)
      .subscribe({
        next: () => this.cerrarModal(true),
        error: () => alert('No se pudo actualizar la cuenta')
      });
  }

  // ------------------ Eliminar ------------------
  eliminar(c: Cuenta): void {
    this.current = c;
    this.modalMode = 'delete';
    this.modalOpen = true;
  }

  confirmarEliminar(): void {
    if (!this.current) return;

    // Ajusta según tu API (por id)
    this.http.delete(`${API.base}${API.cuentas.eliminar}/${this.current.cuentaId}`)
      .subscribe({
        next: () => this.cerrarModal(true),
        error: () => alert('No se pudo eliminar la cuenta')
      });
  }

  // ------------------ Modal ------------------
  cerrarModal(refresh = false): void {
    this.modalOpen = false;
    this.modalMode = null;
    this.current = undefined;
    if (refresh) this.cargar();
  }
}
