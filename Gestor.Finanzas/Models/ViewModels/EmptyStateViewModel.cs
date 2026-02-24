using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gestor.Finanzas.Models.ViewModels
{
    public class EmptyStateViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionText { get; set; }
        public string ActionUrl { get; set; }
        public string IconClass { get; set; }
    }
}