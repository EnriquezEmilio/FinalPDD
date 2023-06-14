using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class Tarjeta
    {
        public int id { get; set; }        
        public string numero { get; set; }
        public string codigoV { get; set; }
        public float limite { get; set; }
        public float consumos { get; set; }
        public Usuario user { get; set; }
        public int num_usr { get; set; }

        public Tarjeta() { }
        public Tarjeta(Usuario user, string Numero, string CodigoV, float Limite, float Consumos)
        {
            this.user = user;
            this.numero = Numero;
            this.codigoV = CodigoV;
            this.limite = Limite;
            this.consumos = Consumos; 
        } 
        
        public string[] toArray()
        {
            return new string[] { this.id.ToString(), this.numero.ToString(), this.codigoV.ToString(), this.limite.ToString(), this.consumos.ToString(), this.user.num_usr.ToString() };
        }

    }
}
