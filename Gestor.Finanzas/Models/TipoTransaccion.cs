using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gestor.Finanzas.Models
{
    [Table("TipoTransaccion")]
    public class TipoTransaccion
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string nombre { get; set; }

        // Navegación
        public virtual ICollection<Transaccione> Transacciones { get; set; }
    }
}