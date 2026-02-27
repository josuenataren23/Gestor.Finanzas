using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Gestor.Finanzas.Controllers
{
    public class ReportesController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();

        // 1 = GASTO | 2 = INGRESO
        private const int TIPO_GASTO = 1;
        private const int TIPO_INGRESO = 2;

        // =============================================
        // GET: /Reportes?periodo=mes|3meses|anio
        // =============================================
        public ActionResult Index(string periodo = "mes")
        {
            int userId = UsuarioActualId;

            var todasLasTx = db.Transacciones
                .Include("Categoria")
                .Include("TipoTransaccion")
                .Where(t => t.usuario_id == userId)
                .ToList();

            // ── Empty state ──────────────────────────
            if (todasLasTx.Count < 5)
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Necesitamos más datos",
                    Description = "Registra al menos 5 transacciones para generar tus primeros reportes visuales.",
                    ActionText = "Ir a transacciones",
                    ActionUrl = Url.Action("Index", "Transacciones"),
                    IconClass = "fa-solid fa-chart-simple"
                };
                return View();
            }

            // ── Rango según período ───────────────────
            DateTime hoy = DateTime.Today;
            DateTime inicio;

            if (periodo == "3meses")
            {
                var hace2 = hoy.AddMonths(-2);
                inicio = new DateTime(hace2.Year, hace2.Month, 1);
            }
            else if (periodo == "anio")
            {
                inicio = new DateTime(hoy.Year, 1, 1);
            }
            else
            {
                inicio = new DateTime(hoy.Year, hoy.Month, 1); // mes actual
            }

            var txPeriodo = todasLasTx
                .Where(t => t.fecha_transaccion >= inicio)
                .ToList();

            ViewBag.PeriodoActual = periodo;

            // ═══════════════════════════════════════════
            // 1. INGRESOS VS GASTOS por mes
            //    Siempre muestra los últimos 6 meses para
            //    que la gráfica de barras tenga contexto
            // ═══════════════════════════════════════════
            var meses = Enumerable.Range(0, 6)
                .Select(i => hoy.AddMonths(-5 + i))
                .Select(d => new DateTime(d.Year, d.Month, 1))
                .ToList();

            var ingVsGast = meses.Select(m => new
            {
                label = m.ToString("MMM", new CultureInfo("es-MX")).ToUpper(),
                ingresos = todasLasTx
                    .Where(t => t.fecha_transaccion.Year == m.Year
                             && t.fecha_transaccion.Month == m.Month
                             && t.tipo_id == TIPO_INGRESO)
                    .Sum(t => (decimal?)t.monto) ?? 0m,
                gastos = todasLasTx
                    .Where(t => t.fecha_transaccion.Year == m.Year
                             && t.fecha_transaccion.Month == m.Month
                             && t.tipo_id == TIPO_GASTO)
                    .Sum(t => (decimal?)t.monto) ?? 0m
            }).ToList();

            ViewBag.ChartLabels = JsonConvert.SerializeObject(ingVsGast.Select(x => x.label));
            ViewBag.ChartIngresos = JsonConvert.SerializeObject(ingVsGast.Select(x => x.ingresos));
            ViewBag.ChartGastos = JsonConvert.SerializeObject(ingVsGast.Select(x => x.gastos));

            // ═══════════════════════════════════════════
            // 2. FRECUENCIA DE GASTOS por día de semana
            //    (sobre el período seleccionado)
            // ═══════════════════════════════════════════
            // Orden: LUN, MAR, MIÉ, JUE, VIE, SÁB, DOM
            var ordenDias = new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday,
                                    DayOfWeek.Sunday };
            var labDias = new[] { "LUN", "MAR", "MIÉ", "JUE", "VIE", "SÁB", "DOM" };

            var gastosDelPeriodo = txPeriodo.Where(t => t.tipo_id == TIPO_GASTO).ToList();
            var freqDias = ordenDias
                .Select(d => gastosDelPeriodo
                    .Where(t => t.fecha_transaccion.DayOfWeek == d)
                    .Sum(t => (decimal?)t.monto) ?? 0m)
                .ToList();

            // Normalizar a escala 0-100 para el radar
            decimal maxFreq = freqDias.Any() ? freqDias.Max() : 1m;
            var freqNorm = freqDias
                .Select(v => maxFreq > 0 ? Math.Round((double)(v / maxFreq) * 100, 1) : 0)
                .ToList();

            ViewBag.RadarLabels = JsonConvert.SerializeObject(labDias);
            ViewBag.RadarData = JsonConvert.SerializeObject(freqNorm);

            // ═══════════════════════════════════════════
            // 3. GASTOS RECURRENTES
            //    Top 5 categorías con más transacciones
            // ═══════════════════════════════════════════
            var recurrentes = gastosDelPeriodo
                .Where(t => t.Categoria != null)
                .GroupBy(t => new { t.categoria_id, t.Categoria.nombre })
                .Select(g => new RecurrenteViewModel
                {
                    Categoria = g.Key.nombre,
                    Veces = g.Count(),
                    TotalGasto = g.Sum(t => t.monto)
                })
                .OrderByDescending(r => r.Veces)
                .Take(5)
                .ToList();

            ViewBag.Recurrentes = recurrentes;

            // ═══════════════════════════════════════════
            // 4. RESUMEN rápido del período
            // ═══════════════════════════════════════════
            ViewBag.TotalIngresos = txPeriodo.Where(t => t.tipo_id == TIPO_INGRESO).Sum(t => (decimal?)t.monto) ?? 0m;
            ViewBag.TotalGastos = txPeriodo.Where(t => t.tipo_id == TIPO_GASTO).Sum(t => (decimal?)t.monto) ?? 0m;
            ViewBag.TotalTx = txPeriodo.Count;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ViewModel auxiliar para recurrentes
    public class RecurrenteViewModel
    {
        public string Categoria { get; set; }
        public int Veces { get; set; }
        public decimal TotalGasto { get; set; }
    }
}