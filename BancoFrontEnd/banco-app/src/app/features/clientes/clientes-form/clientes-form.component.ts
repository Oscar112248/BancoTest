import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';  // 👈 importa FormGroup
import { Cliente } from '../../../core/models/cliente.model';
import { ClientesService } from '../../../core/services/clientes.service';

@Component({
  selector: 'app-clientes-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],   
  templateUrl: './clientes-form.component.html',
  styleUrls: ['./clientes-form.component.scss']
})
export class ClientesFormComponent {
  @Input() model!: Cliente;
  @Output() ok = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  form!: FormGroup;   

  constructor(private fb: FormBuilder, private api: ClientesService) {}

  ngOnInit() {
    this.form = this.fb.group({
      cedula: ['', [Validators.required, Validators.minLength(10)]],
      nombres: ['', Validators.required],
      apellidos: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      activo: [true]
    });

    if (this.model) {
      this.form.patchValue(this.model);
    }
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = { ...this.model, ...this.form.value } as Cliente;
    const req = v.id ? this.api.update(v.id, v) : this.api.create(v as any);
    req.subscribe(() => this.ok.emit());
  }
}
