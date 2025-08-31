// src/app/features/clientes/clientes-list/clientes-list.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { API } from '../../../core/services/api.config';

export interface Cliente {
  codigoCliente: string;
  contraseniaHash: string;
  estado: boolean;
  personaId: number;
  nombre: string;
  genero: string;
  edad: number;
  identificacion: string;
  direccion: string;
  telefono: string;
  eliminado: boolean;
}
export interface ClienteResponse { data: Cliente[]; }

@Component({
  selector: 'app-clientes-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './clientes-list.component.html',
  styleUrls: ['./clientes-list.component.scss']
})
export class ClientesListComponent implements OnInit {
  items: Cliente[] = [];
  loading = true;

  // Modal state
  modalOpen = false;
modalMode: 'create' | 'edit' | 'delete' | null = null;
  current?: Cliente;

  // Form edición
  form!: FormGroup;

  constructor(
    private http: HttpClient,
    private router: Router,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.cargar();
    this.form = this.fb.group({
  codigoCliente: ['', Validators.required],
  contraseniaHash: ['', Validators.required],
  nombre: ['', Validators.required],
  genero: ['O', Validators.required],
  edad: [0, [Validators.required, Validators.min(0)]],
  identificacion: ['', Validators.required],
  direccion: [''],
  telefono: [''],
  estado: [true, Validators.required],

});
  }

  cargar(): void {
    this.loading = true;
    this.http.get<ClienteResponse>(`${API.base}${API.clientes.listar}`)
      .subscribe({
        next: (res) => { this.items = res.data ?? []; this.loading = false; },
        error: () => { this.loading = false; }
      });
  }

  nuevo(): void {
  this.current = undefined;     // no hay cliente seleccionado
  this.modalMode = 'create';    // modo crear
  this.modalOpen = true;

  this.form.reset();            // limpiar formulario
  this.form.patchValue({
    estado: true, // valor por defecto
    genero: 'M'
  });
      Object.keys(this.form.controls).forEach(k => this.form.get(k)?.enable());

}

guardarNuevo(): void {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  const body = this.form.getRawValue(); 
  this.http.post(`${API.base}${API.clientes.crear}`, body)
    .subscribe({
      next: () => this.cerrarModal(true),
      error: () => alert('No se pudo crear el cliente')
    });
}
  
  editar(c: Cliente): void {
    this.modalMode = 'edit';
    this.current = c;
    this.modalOpen = true;

    this.form.reset({
      codigoCliente: c.codigoCliente,
      contraseniaHash: c.contraseniaHash,
      nombre: c.nombre,
      genero: c.genero,
      edad: c.edad,
      identificacion: c.identificacion,
      direccion: c.direccion,
      telefono: c.telefono,
      estado: c.estado
    });

    const toDisable = ['codigoCliente','contraseniaHash','nombre','genero','identificacion'];
    toDisable.forEach(k => this.form.get(k)?.disable());
    ['edad','direccion','telefono'].forEach(k => this.form.get(k)?.enable());
  }

  guardarEdicion(): void {
    if (this.form.invalid || !this.current) { this.form.markAllAsTouched(); return; }
    const { telefono,direccion, edad } = this.form.getRawValue();
      const idCliente = this.current.codigoCliente; 

      this.http.put(`${API.base}${API.clientes.actualizar}/${idCliente}`, { telefono,direccion, edad })
        .subscribe({
          next: () => { alert('Cliente actualizado'); this.cerrarModal(true); },
          error: () => alert('No se pudo actualizar el cliente')
        });
  }

  eliminar(c: Cliente): void {
    this.current = c;
    this.modalMode = 'delete';
    this.modalOpen = true;
  }

  confirmarEliminar(): void {
    if (!this.current) return;
      const idCliente = this.current.codigoCliente; 
const estado = true;
   this.http.patch(`${API.base}${API.clientes.eliminar}/${idCliente}`, { estado  })
        .subscribe({
          next: () => { alert('Cliente eliminado'); this.cerrarModal(true); },
          error: () => alert('No se pudo actualizar el cliente')
        });
  }

  cerrarModal(refresh = false): void {
    this.modalOpen = false;
    this.modalMode = null;
    this.current = undefined;
    if (refresh) this.cargar();
  }
}
