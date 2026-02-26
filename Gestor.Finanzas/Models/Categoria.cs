namespace Gestor.Finanzas.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Categoria
    {
        public int id { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public virtual ICollection<Presupuesto>  Presupuestos  { get; set; }
        public virtual ICollection<Transaccione> Transacciones { get; set; }
    }
}