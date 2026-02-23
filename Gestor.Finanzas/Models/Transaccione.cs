namespace Gestor.Finanzas.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Transaccione
    {
        public int id { get; set; }

        public int? usuario_id { get; set; }

        public int? categoria_id { get; set; }

        public decimal monto { get; set; }

        [Column(TypeName = "date")]
        public DateTime fecha_transaccion { get; set; }

        [StringLength(255)]
        public string descripcion { get; set; }

        public virtual Categoria Categoria { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
