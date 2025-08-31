import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from './core/services/toast.service';

@Component({
  standalone: true,
  selector: 'app-toast-container',
  imports: [CommonModule],
  styles: [`
    .toast-wrap {
      position: fixed;
      right: 16px;
      bottom: 16px;
      display: flex;
      flex-direction: column;
      gap: 8px;
      z-index: 9999;
      pointer-events: none; /* permite clics a elementos detrás excepto en tarjetas */
    }
    .toast {
      min-width: 260px;
      max-width: 420px;
      padding: 10px 12px;
      border-radius: 8px;
      box-shadow: 0 8px 24px rgba(0,0,0,.18);
      color: #0b141a;
      background: #fff;
      border-left: 4px solid #ccc;
      display: flex;
      align-items: flex-start;
      gap: 8px;
      pointer-events: auto; /* clic habilitado en tarjeta */
      animation: slideIn .18s ease-out;
      font-size: 14px;
      line-height: 1.25rem;
    }
    .toast--success { border-left-color: #2e7d32; }
    .toast--error   { border-left-color: #c62828; }
    .toast--info    { border-left-color: #1275bb; }
    .toast--warn    { border-left-color: #ef6c00; }

    .toast .title {
      font-weight: 600;
      margin-right: 6px;
      text-transform: capitalize;
    }
    .toast .msg { white-space: pre-wrap; }
    .toast button.close {
      margin-left: auto;
      border: none;
      background: transparent;
      cursor: pointer;
      font-size: 18px;
      line-height: 18px;
      padding: 0 2px;
      color: #555;
    }
    .toast button.close:hover { color: #000; }

    @keyframes slideIn {
      from { transform: translateY(10px); opacity: 0; }
      to   { transform: translateY(0);    opacity: 1; }
    }
  `],
  template: `
    <div class="toast-wrap" *ngIf="toasts.length">
      <div class="toast"
           *ngFor="let t of toasts"
           [class.toast--success]="t.level==='success'"
           [class.toast--error]="t.level==='error'"
           [class.toast--info]="t.level==='info'"
           [class.toast--warn]="t.level==='warn'">
        <span class="title">{{ t.level }}</span>
        <span class="msg">{{ t.text }}</span>
        <button class="close" (click)="dismiss(t.id)" aria-label="Cerrar">×</button>
      </div>
    </div>
  `
})
export class ToastContainerComponent implements OnDestroy {
  toasts: Toast[] = [];
  private unsub?: () => void;

  constructor(private toast: ToastService) {
    this.unsub = this.toast.subscribe(ts => this.toasts = ts);
  }

  dismiss(id: number) { this.toast.dismiss(id); }

  ngOnDestroy(): void { this.unsub?.(); }
}
