using System;
using System.ComponentModel.DataAnnotations;

namespace Gestor.Finanzas.Models.ViewModels
{
    public class TransaccionViewModel
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        [Required(ErrorMessage = "Selecciona el tipo de transacción.")]
        [Display(Name = "Tipo")]
        public int tipo_id { get; set; }

        [Display(Name = "Categoría")]
        public int? categoria_id { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, 9999999.99, ErrorMessage = "El monto debe ser mayor a 0.")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        public decimal monto { get; set; }

        [Display(Name = "Fecha")]
        public DateTime fecha_transaccion { get; set; }

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }
    }
}