using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebBanco.Models
{
    public class Movimiento
    {
        public int idm { get; set; }       
        public string detalle { get; set; }
        public float monto { get; set; }
        public DateTime fecha { get; set; }
        public CajaDeAhorro caja { get; set; }
        public int id { get; set; }

        public Movimiento() { }
        public Movimiento(int idm, int idc, string detalle, float monto, DateTime fecha)
        {
            this.idm = idm;
            this.id = idc;
            this.detalle = detalle;
            this.monto = monto;
            this.fecha = fecha;
        }
        public Movimiento(CajaDeAhorro Caja, string Detalle, float Monto)
        {
            this.caja = Caja;
            this.detalle = Detalle;
            this.monto = Monto;
            this.fecha = DateTime.Now;
        }

        public string[] toArray()
        {
            return new string[] { this.detalle, this.monto.ToString(), this.fecha.ToString(), this.caja.id.ToString() };
        }
    }
}
