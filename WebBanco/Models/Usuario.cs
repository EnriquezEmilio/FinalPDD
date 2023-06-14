using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class Usuario
    {                

        public int num_usr { get; set; }
        public int dni { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int intentosFallidos { get; set; }
        public bool bloqueado { get; set; }
        public bool esADM { get; set; }
        
        public ICollection<CajaDeAhorro> Caja { get; } = new List<CajaDeAhorro>();
        public List<UsuarioCaja> UserCaja { get; set; } = new List<UsuarioCaja>();
        public List<PlazoFijo> misPlazosFijos { get; } = new List<PlazoFijo>();
        public List<Tarjeta> misTarjetas { get; } = new List<Tarjeta>();
        public List<Pago> misPagos { get; } = new List<Pago>();     


        public Usuario() {}
                

        public Usuario(int dni, string nombre, string apellido, string email, string password, int intentos, bool bloqueado, bool esADM)
        {
            this.dni = dni;
            this.nombre = nombre;
            this.apellido = apellido;
            this.email = email;
            this.password = password;
            this.intentosFallidos = intentos;
            this.bloqueado = bloqueado;
            this.esADM = esADM;
            this.misPlazosFijos = new List<PlazoFijo>();
            this.misPagos = new List<Pago>();
            this.misTarjetas = new List<Tarjeta>();
            this.Caja = new List<CajaDeAhorro>();
        }       
        

        public void errorInicio()
        {
            this.intentosFallidos++;
            if (this.intentosFallidos == 3)
            {
                this.bloqueado = true;
            }
        }  
       
        public string[] toArray()
        {
            return new string[] { num_usr.ToString(),dni.ToString(), nombre, apellido, email, password, esADM.ToString() };
        }      
        
    }
}
