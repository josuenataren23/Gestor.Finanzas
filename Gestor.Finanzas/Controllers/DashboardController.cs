using Gestor.Finanzas.Models;
using Gestor.Finanzas.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Gestor.Finanzas.Controllers
{
    public class DashboardController : BaseController
    {
        private GestorFinanzasModel db = new GestorFinanzasModel();

        // 1 = GASTO | 2 = INGRESO  (según tu tabla TipoTransaccion)
        private const int TIPO_GASTO = 1;
        private const int TIPO_INGRESO = 2;

        public ActionResult Index()
        {
            var userId = UsuarioActualId;
            var ahora = DateTime.Now;
            var inicioMes = new DateTime(ahora.Year, ahora.Month, 1);
            var mesAnteriorInicio = inicioMes.AddMonths(-1);
            var mesAnteriorFin = inicioMes.AddDays(-1);

            var transacciones = db.Transacciones
                .Include("Categoria")
                .Include("TipoTransaccion")
                .Where(t => t.usuario_id == userId)
                .ToList();

            if (!transacciones.Any())
            {
                ViewBag.EmptyState = new EmptyStateViewModel
                {
                    Title = "Tu viaje financiero comienza aquí",
                    Description = "Aún no tienes movimientos registrados. Comienza agregando tu primera transacción.",
                    ActionText = "Registrar mi primera transacción",
                    ActionUrl = Url.Action("Create", "Transacciones"),
                    IconClass = "fa-regular fa-credit-card"
                };
                return View();
            }

            var totalIngresos = transacciones
                .Where(t => t.tipo_id == TIPO_INGRESO)
                .Sum(t => t.monto);

            var totalGastos = transacciones
                .Where(t => t.tipo_id == TIPO_GASTO)
                .Sum(t => t.monto);

            var balanceTotal = totalIngresos - totalGastos;

            var gastosMes = transacciones
                .Where(t => t.tipo_id == TIPO_GASTO && t.fecha_transaccion >= inicioMes)
                .Sum(t => t.monto);

            var gastosMesAnterior = transacciones
                .Where(t => t.tipo_id == TIPO_GASTO
                         && t.fecha_transaccion >= mesAnteriorInicio
                         && t.fecha_transaccion <= mesAnteriorFin)
                .Sum(t => t.monto);

            var variacionGastos = gastosMesAnterior > 0
                ? Math.Round(((gastosMes - gastosMesAnterior) / gastosMesAnterior) * 100, 1)
                : 0m;

            var metas = db.Metas.Where(m => m.usuario_id == userId).ToList();
            var totalAhorro = metas.Sum(m => m.monto_actual ?? 0);
            var metaDestacada = metas
                .Where(m => m.monto_objetivo > 0)
                .OrderByDescending(m => (m.monto_actual ?? 0) / m.monto_objetivo)
                .FirstOrDefault();

            // Tendencia semanal últimos 7 días (solo gastos)
            var hace7Dias = ahora.Date.AddDays(-6);
            var tendenciaDias = new string[7];
            var tendenciaValores = new decimal[7];
            for (int i = 0; i < 7; i++)
            {
                var dia = hace7Dias.AddDays(i);
                tendenciaDias[i] = dia.ToString("ddd").ToUpper();
                tendenciaValores[i] = transacciones
                    .Where(t => t.tipo_id == TIPO_GASTO && t.fecha_transaccion.Date == dia)
                    .Sum(t => t.monto);
            }

            // Gastos por categoría mes actual
            var gastosPorCat = transacciones
                .Where(t => t.tipo_id == TIPO_GASTO && t.fecha_transaccion >= inicioMes)
                .GroupBy(t => t.Categoria != null ? t.Categoria.nombre : "Otros")
                .Select(g => new { Nombre = g.Key, Total = g.Sum(t => t.monto) })
                .OrderByDescending(g => g.Total)
                .Take(4).ToList();

            var ultimasTx = transacciones
                .OrderByDescending(t => t.fecha_transaccion)
                .Take(5).ToList();

            ViewBag.BalanceTotal = balanceTotal;
            ViewBag.TotalIngresos = totalIngresos;
            ViewBag.GastosMes = gastosMes;
            ViewBag.VariacionGastos = variacionGastos;
            ViewBag.TotalAhorro = totalAhorro;
            ViewBag.MetaDestacada = metaDestacada;
            ViewBag.TendenciaDias = tendenciaDias;
            ViewBag.TendenciaGastos = tendenciaValores;
            ViewBag.CategoriaLabels = gastosPorCat.Select(g => g.Nombre).ToArray();
            ViewBag.CategoriaTotales = gastosPorCat.Select(g => g.Total).ToArray();
            ViewBag.TotalGastosCategorias = gastosPorCat.Sum(g => g.Total);
            ViewBag.UltimasTransacciones = ultimasTx;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}