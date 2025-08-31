using Banco.Application.Exceptions;
using Banco.Domain.Exceptions;
using Banco.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BancoTest.Excepciones
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error no controlado al procesar la solicitud.");

            var (statusCode, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                LimiteDiarioExcedidoException => (StatusCodes.Status400BadRequest, "Error de límite Diario"),
                ValorMovimientoInvalidoException => (StatusCodes.Status401Unauthorized, "Valor Inválido"),
                RepositoryException => (StatusCodes.Status500InternalServerError, "Error en acceso a datos"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };

            var details = new ProblemDetails
            {
                Title = title,
                Status = statusCode,
                Detail = exception?.Message

            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(details, cancellationToken);
            return true;
        }
    }
}
