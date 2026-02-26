using System;
using System.ComponentModel.DataAnnotations;

namespace Gestor.Finanzas.Models.ViewModels
{
    public class PresupuestoViewModel
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        [Required(ErrorMessage = "Selecciona una categoría.")]
        [Display(Name = "Categoría")]
        public int categoria_id { get; set; }

        [Required(ErrorMessage = "El monto objetivo es obligatorio.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El monto debe ser mayor a 0.")]
        [Display(Name = "Monto objetivo")]
        [DataType(DataType.Currency)]
        public decimal monto_objetivo { get; set; }

        [Display(Name = "Monto gastado")]
        [DataType(DataType.Currency)]
        public decimal monto_gastado { get; set; } = 0;

        [Display(Name = "Fecha de creación")]
        public DateTime? fecha_creacion { get; set; }

        // ── Propiedades calculadas ──
        public decimal Disponible => monto_objetivo - monto_gastado;

        public double Porcentaje => monto_objetivo > 0
            ? Math.Min(100, (double)(monto_gastado / monto_objetivo) * 100) : 0;

        public bool Excedido => monto_gastado > monto_objetivo;
    }
}