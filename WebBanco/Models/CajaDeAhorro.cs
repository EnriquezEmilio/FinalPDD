using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class CajaDeAhorro
    {
        public int id { get; set; }
        public string Cbu { get; set; }
        public float saldo { get; set; }
        public List<Movimiento> misMovimientos { get; set; } = new List<Movimiento>();
        public ICollection<Usuario> users { get; } = new List<Usuario>();
        public List<UsuarioCaja> UserCaja { get; set; } = new List<UsuarioCaja>(); 
        
        public CajaDeAhorro() { }
        
       
        public CajaDeAhorro(string Cbu, float saldo)
        {
            this.Cbu = Cbu;
            this.saldo = saldo;
        }
        public CajaDeAhorro(int id, string cbu, float saldo)
        {
            this.id = id;
            this.Cbu = cbu;
            this.saldo = saldo;
            users = new List<Usuario>();
            misMovimientos = new List<Movimiento>();
        } 

        public string[] toArray()
        {
            return new string[] { this.id.ToString(), this.Cbu, this.saldo.ToString() };
        }        
    }
}
