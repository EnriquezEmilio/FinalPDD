using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class Pago
    {
        public int id { get; set; } 
        public string nombre { get; set; }
        public float monto { get; set; }
        public bool pagado { get; set; }
        public string metodo { get; set; }
        public Usuario user { get; set; }
        public int num_usr { get; set; }                   

        public Pago() { }

        public Pago(string nombre, float monto)
        {                      
            this.nombre = nombre;
            this.monto = monto;
            this.pagado = false;            
        }
        public Pago(Usuario user, string nombre, float monto, bool pagado, string metodo)
        {
            this.user = user;
            this.nombre = nombre;
            this.monto = monto;
            this.pagado = pagado;
            this.metodo = metodo;           
        }

        public string[] toArray()
        {
            return new string[] { this.id.ToString(), this.nombre, this.monto.ToString() };
        }
        public string[] toArray2()
        {
            return new string[] { this.id.ToString(), this.nombre, this.monto.ToString(), this.metodo.ToString(), this.user.num_usr.ToString() };
        }

    }
}
