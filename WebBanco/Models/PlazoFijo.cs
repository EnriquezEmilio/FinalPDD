using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class PlazoFijo
    {
        public int id { get; set; }
        public float monto { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public float tasa { get; set; }
        public bool pagado { get; set; }
        public Usuario user { get; set; }
        public int num_usr { get; set; }

        public PlazoFijo() { }
       
        public PlazoFijo(Usuario user, float monto, float tasa, bool pagado)
        {       
            this.user = user;            
            this.monto = monto;
            this.fechaIni = DateTime.Now;
            this.fechaFin = fechaIni.AddMonths(3);
            this.tasa = tasa;
            this.pagado = pagado;
        }        
        public string[] toArray()
        {
            return new string[] { this.id.ToString(), this.monto.ToString(), this.fechaIni.ToString(), this.fechaFin.ToString(), this.tasa.ToString(), this.pagado.ToString(), this.user.num_usr.ToString() };
        }

    }
}
