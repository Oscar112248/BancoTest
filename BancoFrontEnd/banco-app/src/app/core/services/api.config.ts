import { environment } from '../../../environments/environment';

export const API = {
  base: environment.apiBaseUrl,
  clientes: {    
   listar:   '/clientes/ConsultaClientes',             
    consulta: '/clientes/ConsultaCliente',    
    crear:    '/clientes',
    actualizar:'/clientes/ActualizaCliente',
    eliminar: '/clientes/EliminarCliente'
  }
  ,cuentas: {    
   listar:   '/cuentas/ConsultaCuentas',             
    consulta: '/clientes/ConsultaCliente',    
    crear:    '/cuentas/CrearCuenta',
    actualizar:'/cuentas/ActualizaCuenta',
    eliminar: '/clientes/EliminarCliente'
  }
  ,movimientos: {    
   listar:   '/movimientos/ConsultaMovimientos',
    estadoCuenta:   '/reportes/EstadoCuenta'             
  }
};