namespace Gestor.Finanzas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Meta
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        public decimal monto_objetivo { get; set; }

        public decimal? monto_actual { get; set; }

        public DateTime? fecha_creacion { get; set; }

        public DateTime? fecha_cumplimiento { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
