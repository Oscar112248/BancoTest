import { Component } from '@angular/core';
import { ReportService } from '../../../core/services/reportes.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reportes',
    standalone: true,

  imports: [CommonModule, FormsModule],   // 👈 agrega FormsModule aquí
  templateUrl: './reportes.component.html',
  styleUrl: './reportes.component.scss'
})
export class ReportesComponent {
  cliente = '';
  desde = '';
  hasta = '';
  cargando = false;
  error = '';

    constructor(private report: ReportService) {}
  generar() {
    this.error = '';
    if (!this.cliente || !this.desde || !this.hasta) return;

    this.cargando = true;
    this.report
      .estadoCuentaPorUsuario(this.cliente, this.desde, this.hasta) // ← TU método del service
      .subscribe({
        next: (pdfBlob) => this.descargar(pdfBlob, 'estado_cuenta.pdf'),
        error: () => (this.error = 'No se pudo generar el PDF.'),
        complete: () => (this.cargando = false),
      });
  }

   private descargar(blob: Blob, nombre: string) {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = nombre;
    a.click();
    URL.revokeObjectURL(url);
  }

}
