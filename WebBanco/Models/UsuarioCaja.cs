using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanco.Models
{
    public class UsuarioCaja
    {
        public int id { get; set; }        
        public Usuario user { get; set; }
        public CajaDeAhorro caja { get; set; }
        public int num_usr { get; set; }
        public UsuarioCaja() { }
        public UsuarioCaja(int caja, int usuario)
        {
            id = caja;
            num_usr = usuario;
        }
    }
}
