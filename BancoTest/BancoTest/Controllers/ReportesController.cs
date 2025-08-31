using Banco.Infrastructure.Persistence.Context;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.DTO;
using System.Reflection.Metadata;

namespace BancoTest.Controllers
{
    [ApiController]
    [Route("api/reportes")]

    public class ReportesController : ControllerBase
    {
        private readonly BANCO_DbContext _db;
        public ReportesController(BANCO_DbContext db) => _db = db;

        [HttpGet("EstadoCuenta")]
        public async Task<IActionResult> EstadoCuenta(
        [FromQuery] string cliente,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        CancellationToken ct)
        {
            var rows = await (
                from c in _db.Cuenta
                join p in _db.Personas on c.ClienteIdPersona equals p.PersonaId
                join m in _db.Movimientos.Where(x => !x.Anulado) on c.CuentaId equals m.CuentaId into movs
                from m in movs.DefaultIfEmpty()
                where p.Nombre == cliente
                group new { c, p, m } by new
                {
                    c.CuentaId,
                    c.NumeroCuenta,
                    c.TipoCuenta,
                    c.SaldoInicial,
                    c.Estado,
                    p.Nombre,
                    m.Fecha,
                    m.Anulado,
                   m.MovimientoNeto
                }
        into g
                select new EstadoCuentaDto(
                    g.Key.Fecha != null ? g.Key.Fecha : DateTime.MinValue, 
                    g.Key.Nombre,
                    g.Key.NumeroCuenta,
                    g.Key.TipoCuenta,
                    g.Key.SaldoInicial,
                    !g.Key.Estado, 
                    g.Key.MovimientoNeto ?? 0m,
                    (decimal)(g.Key.SaldoInicial + g.Sum(x => x.m != null ? x.m.MovimientoNeto : 0m)))

            ).AsNoTracking().ToListAsync(ct);

            var data = new List<EstadoCuentaDto>(rows.Count);
            foreach (var grp in rows.GroupBy(r => r.NumeroCuenta))
            {
                decimal acumulado = 0m;
                var saldoBase = grp.First().SaldoInicial;

                foreach (var r in grp.OrderBy(r => r.Fecha))
                {
                    acumulado += r.Movimiento;
                    data.Add(new EstadoCuentaDto(
                        r.Fecha,
                        r.Cliente,
                        r.NumeroCuenta,
                        r.Tipo,
                        r.SaldoInicial,
                        r.Estado,
                        r.Movimiento,
                        r.SaldoDisponible
                    ));
                }
            }

         

            using var ms = new MemoryStream();
            using (var writer = new PdfWriter(ms))
            using (var pdf = new PdfDocument(writer))
            using (iText.Layout.Document doc = new iText.Layout.Document(pdf))
            {
                doc.Add(new Paragraph("Estado de Cuenta por Usuario")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold().SetFontSize(16));

                doc.Add(new Paragraph($"Usuario: {cliente}")
                    .SetFontSize(10).SetMarginBottom(10));

                var table = new iText.Layout.Element.Table(new float[] { 12, 24, 14, 12, 12, 10, 12, 14 })
                            .UseAllAvailableWidth();

                void H(string t) => table.AddHeaderCell(new Cell().Add(new Paragraph(t).SetBold()));

                H("Fecha"); H("Cliente"); H("Número"); H("Tipo"); H("Saldo Inicial"); H("Estado"); H("Movimiento"); H("Saldo Disp.");

                foreach (var r in data)
                {
                    table.AddCell(r.Fecha.ToString("dd/MM/yyyy"));
                    table.AddCell(r.Cliente);
                    table.AddCell(r.NumeroCuenta);
                    table.AddCell(r.Tipo);
                    table.AddCell(r.SaldoInicial.ToString("0.##"));
                    table.AddCell(r.Estado ? "Activo" : "Inactivo");
                    table.AddCell(r.Movimiento.ToString("0.##"));
                    table.AddCell(r.SaldoDisponible.ToString("0.##"));
                }

                if (data.Count == 0)
                    doc.Add(new Paragraph("Sin movimientos en el rango seleccionado.").SetItalic());
                else
                    doc.Add(table);
            }

            var bytes = ms.ToArray();
            return File(bytes, "application/pdf", "estado_cuenta.pdf");
        }
    }
}
