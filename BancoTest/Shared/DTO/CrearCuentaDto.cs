namespace Shared.DTO
{
    public sealed record CrearCuentaDto(
       long ClienteIdPersona, string NumeroCuenta, string TipoCuenta, decimal SaldoInicial);
}
