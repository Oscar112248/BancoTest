import { Injectable } from '@angular/core';

export type ToastLevel = 'success' | 'error' | 'info' | 'warn';

export interface Toast {
  id: number;
  text: string;
  level: ToastLevel;
  timeoutMs?: number;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private _subs = new Set<(toasts: Toast[]) => void>();
  private _toasts: Toast[] = [];
  private _nextId = 1;

  subscribe(cb: (toasts: Toast[]) => void): () => void {
    this._subs.add(cb);
    cb(this._toasts);
    return () => this._subs.delete(cb);
  }

  private _emit() { for (const s of this._subs) s(this._toasts); }

  show(text: string, level: ToastLevel = 'info', timeoutMs = 3500) {
    const id = this._nextId++;
    const toast: Toast = { id, text, level, timeoutMs };
    this._toasts = [...this._toasts, toast];
    this._emit();
    if (timeoutMs && timeoutMs > 0) {
      setTimeout(() => this.dismiss(id), timeoutMs);
    }
  }

  success(text: string, ms?: number) { this.show(text, 'success', ms ?? 3000); }
  error(text: string, ms?: number)   { this.show(text, 'error',   ms ?? 5000); }
  info(text: string, ms?: number)    { this.show(text, 'info',    ms ?? 3500); }
  warn(text: string, ms?: number)    { this.show(text, 'warn',    ms ?? 4000); }

  dismiss(id: number) {
    this._toasts = this._toasts.filter(t => t.id !== id);
    this._emit();
  }

  clear() {
    this._toasts = [];
    this._emit();
  }
}
