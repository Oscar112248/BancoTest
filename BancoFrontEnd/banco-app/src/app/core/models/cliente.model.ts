export interface Cliente { id:number; cedula:string; nombres:string; apellidos:string; email:string; activo:boolean; }

export interface Cuenta { id:number; numero:string; clienteId:number; saldoInicial:number; estado:boolean; }

export type TipoMovimiento = 'C'|'D';
export interface Movimiento { id:number; cuentaId:number; fecha:string; tipo:TipoMovimiento; valor:number; anulado:boolean; movimientoNeto:number; }
export interface ClienteResponse {
  isSuccess: boolean;
  message: string;
  data: Cliente[];
  statusCode: number;
}

export interface Cliente {
  codigoCliente: string;
  contraseniaHash: string;
  estado: boolean;
  personaId: number;
  nombre: string;
  genero: string; // puedes restringir a 'M' | 'F' | 'O' si quieres
  edad: number;
  identificacion: string;
  direccion: string;
  telefono: string;
  eliminado: boolean;
}