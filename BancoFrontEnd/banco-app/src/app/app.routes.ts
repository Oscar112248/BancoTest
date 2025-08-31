import { Routes } from '@angular/router';
import { ClientesListComponent } from './features/clientes/clientes-list/clientes-list.component';
import { CuentasListComponent } from './features/cuentas/cuentas-list/cuentas-list.component';
import { MovimientosListComponent } from './features/movimientos/movimientos-list/movimientos-list.component';
import { ReportesComponent } from './features/reportes/reportes/reportes.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'clientes' },
  { path: 'clientes', component: ClientesListComponent },
  { path: 'cuentas', component: CuentasListComponent },
  { path: 'movimientos', component: MovimientosListComponent },
  { path: 'reportes', component: ReportesComponent },
  { path: '**', redirectTo: 'clientes' }
];
