namespace Gestor.Finanzas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Presupuestos")]
    public partial class Presupuesto
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        public int? categoria_id { get; set; }

        public decimal monto_objetivo { get; set; }

        public decimal? monto_gastado { get; set; }

        public DateTime? fecha_creacion { get; set; }

        public virtual Categoria Categoria { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
