using System;
using System.ComponentModel.DataAnnotations;

namespace Gestor.Finanzas.Models.ViewModels
{
    public class MetaViewModel
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre del Objetivo")]
        public string nombre { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "El monto objetivo es obligatorio.")]
        [Range(1, 99999999.99, ErrorMessage = "El monto objetivo debe ser mayor a 0.")]
        [Display(Name = "Monto Objetivo")]
        [DataType(DataType.Currency)]
        public decimal monto_objetivo { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "El monto actual no puede ser negativo.")]
        [Display(Name = "Monto Actual (Guardado)")]
        [DataType(DataType.Currency)]
        public decimal? monto_actual { get; set; }

        [Display(Name = "Fecha Estimada")]
        [DataType(DataType.Date)]
        public DateTime? fecha_estimada { get; set; }

        [StringLength(50)]
        [Display(Name = "Ícono")]
        public string icono { get; set; }
    }
}
