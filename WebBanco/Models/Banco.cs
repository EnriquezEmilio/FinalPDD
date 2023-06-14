using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO.Pipes;

namespace WebBanco.Models
{
    public class Banco
    {        
        private Usuario usuarioLogueado;
        private WebBanco.Data.MyContext contexto;
        public Banco()
        {
            inicializarAtributos();
        }
        private void inicializarAtributos()
        {
            try
            {                
                //contexto = new MyContext();
                contexto.usuarios.Include(u => u.Caja).Include(u => u.misPagos).Include(u => u.misPlazosFijos).Include(u => u.misTarjetas).Load();
                contexto.cajas.Include(c => c.misMovimientos).Include(c => c.users).Load();
                contexto.movimientos.Load();
                contexto.pagos.Load();
                contexto.tarjetas.Load();
                contexto.plazoFijos.Load();
                              
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void cerrar()
        {
            contexto.Dispose();
        }
        // USUARIO
        public bool agregarUsuario(int Dni, string Nombre, string Apellido, string Email, string Password, bool EsADM)
        {
            bool esValido = true;
            foreach (Usuario u in contexto.usuarios)
            {
                if (u.email == Email || u.dni == Dni)
                    esValido = false;
            }
            if (esValido)
                try
                {
                    if (Password.Length < 4)
                        return false;
                    else
                    {
                        Usuario nuevo = new Usuario {dni = Dni, nombre = Nombre, apellido = Apellido, email = Email, password = Password, esADM = EsADM };

                        contexto.usuarios.Add(nuevo);
                        contexto.SaveChanges();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            else return false;
        }
        public bool eliminarUsuario(int id)
        {
            try
            {
                Usuario u = buscarUsuarioId(id);
                if (u != null && u.misPagos.Where(p => p.pagado == false).FirstOrDefault() == null && u.misPlazosFijos.Where(pf => pf.pagado == false).FirstOrDefault() == null && u.misTarjetas.Where(t => t.consumos != 0).FirstOrDefault() == null)               
                    {
                        contexto.usuarios.Remove(u);
                        contexto.SaveChanges();
                        return true;
                    }
                return false;                
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool modificarUsuario(int id, string email, string password)
        {
            Usuario u = buscarUsuarioId(id);            
                if (u != null)
                {   
                    u.email = email;
                    u.password = password;                    
                    contexto.usuarios.Update(u);
                    contexto.SaveChanges();
                    return true;                    
                }        
               
            return false; ;
        }       
        public bool esADM()
        {
            return usuarioLogueado.esADM;
        }       
        
        public List<Usuario> obtenerUsuarios()
        {
            return contexto.usuarios.ToList();
        }

        public bool iniciarSesion(string email, string password)
        {
            try
            {
                var query = from Usuario in contexto.usuarios
                            where Usuario.email == email 
                            select Usuario;
                Usuario acceso = query.FirstOrDefault();

                if (acceso != null)
                {
                    if (acceso.bloqueado) return false;

                    if (acceso.password != password)
                    {
                        acceso.errorInicio();
                        contexto.usuarios.Update(acceso);
                        contexto.SaveChanges();
                        return false;
                    }
                    acceso.intentosFallidos = 0;
                    usuarioLogueado = acceso;
                    contexto.usuarios.Update(acceso);
                    contexto.SaveChanges();
                    return true;
                }
                return false;
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string mostrarEmail()
        {
            return usuarioLogueado.email;
        }
        public int mostrarID()
        {
            return usuarioLogueado.num_usr;
        }
        public bool cerrarSesion()
        {
          this.usuarioLogueado = null;
          return true;
        }
        public Usuario buscarUsuarioId(int id)
        {
            return contexto.usuarios.FirstOrDefault(u => u.num_usr == id);
        }
        
        public List<Movimiento> obtenerMovimientosBanco()
        {
            return contexto.movimientos.ToList();
        }
        public List<Tarjeta> obtenerTarjetasBanco()
        {
            return contexto.tarjetas.ToList();
        }
        
        public List<CajaDeAhorro> obtenerCajasDeAhorro()
        {
            return usuarioLogueado.Caja.ToList();
        }
        public List<CajaDeAhorro> obtenerCajasBanco()
        {
            return contexto.cajas.ToList();
        }

        // CAJA DE AHORRO

        public bool crearCajaDeAhorro()
        {
            try
            {
                string cbu = crearCBU();            
                
                if (usuarioLogueado != null )
                {
                    CajaDeAhorro caja = new CajaDeAhorro(cbu, 0);                    
                    usuarioLogueado.Caja.Add(caja);
                    caja.users.Add(usuarioLogueado);
                    contexto.cajas.Add(caja);
                    contexto.usuarios.Update(usuarioLogueado);                    
                    contexto.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }             

        public bool borrarCajaAhorro(int id)
        {
            try
            {
                CajaDeAhorro caja = buscarCajaId(id);
                    if (caja != null && caja.saldo == 0)
                    {                    
                        foreach(Usuario u in caja.users)
                        {
                            u.Caja.Remove(caja);                           
                        }
                        contexto.cajas.Remove(caja);
                        contexto.SaveChanges();
                        return true;                    
                    }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool agregarTitular(int caja, int usr)
        {
            try
            {
                CajaDeAhorro c = buscarCajaId(caja);
                Usuario u = buscarUsuarioId(usr);
                if (c != null && u != null)
                {
                    if (!c.users.Contains(u) && c.users.Count < 3)
                    {
                        c.users.Add(u);
                        u.Caja.Add(c);
                        contexto.cajas.Update(c);
                        contexto.usuarios.Update(u);
                        contexto.SaveChanges();
                        return true;
                    }
                    return false;
                }
                return false;                
            }
            catch (Exception ex)
            {
                return false;
            }            
        }
        public bool eliminarTitular(int caja, int usr)
        {
            try
            {
                CajaDeAhorro c = buscarCajaId(caja);
                Usuario u = buscarUsuarioId(usr);
                if (c != null && u != null)
                {
                    if (c.users.Count >= 1 && c.users.Contains(u))
                    {
                        c.users.Remove(u);
                        u.Caja.Remove(c);
                        contexto.usuarios.Update(u);
                        contexto.cajas.Update(c);
                        contexto.SaveChanges();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }           
        }
        public bool pagarConCbu(int idPago, int idCaja, int idUsuario)
        {
            try
            {
                Pago p = buscarPagoId(idPago);
                CajaDeAhorro c = buscarCajaId(idCaja);
                Usuario u = buscarUsuarioId(idUsuario);
                
                if (c.users.Contains(u) && c.saldo >= p.monto)
                {
                    c.saldo -= p.monto;
                    this.modificarPago(idPago);
                    this.agregarMovimiento(c, "Pago con Cbu", p.monto);
                    p.metodo = "Cbu";
                    contexto.cajas.Update(c);
                    contexto.pagos.Update(p);                    
                    contexto.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public CajaDeAhorro buscarCajaId(int id)
        {
            return contexto.cajas.FirstOrDefault(c => c.id == id);
        }
        public float obtenerSaldoCaja(int id)
        {
            foreach (CajaDeAhorro c in contexto.cajas)
            {
                if (c.id == id) return c.saldo;
            }
            return 0;
        }
        public bool depositar(int id, float monto)
        {
            try
            {
                CajaDeAhorro caja = buscarCajaId(id);
                if (caja != null)
                {
                    caja.saldo += monto;
                    this.agregarMovimiento(caja, "Deposito", monto);
                    contexto.cajas.Update(caja);
                    contexto.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool transferir(int idOrigen, int idDestino, float monto)
        {
            try
            {
                CajaDeAhorro c1 = buscarCajaId(idOrigen);
                CajaDeAhorro c2 = contexto.cajas.FirstOrDefault(c => c.id == idDestino);

                if (c1.saldo >= monto)
                {

                    c1.saldo -= monto;
                    this.agregarMovimiento(c1, "Transferencia Realizada", monto);
                    c2.saldo += monto;
                    this.agregarMovimiento(c2, "Transferencia Recibida", monto);
                    contexto.cajas.Update(c1);
                    contexto.cajas.Update(c2);
                    contexto.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool retirar(int id, float monto)
        {
            try
            {
                CajaDeAhorro caja = buscarCajaId(id);

                if (caja != null)
                {
                    if (caja.saldo >= monto)
                    {
                        caja.saldo -= monto;                        
                        this.agregarMovimiento(caja, "Retiro", monto);
                        contexto.cajas.Update(caja);
                        contexto.SaveChanges();
                        return true;
                    }
                    else
                        return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string crearCBU()
        {
            string cbu = "0";
            Random num = new Random();
            for (int i = 0; i < 5; i++)
            {
                cbu += num.Next(0, 9999);
            }
            while (cbu.Length < 10)
            {
                cbu = "0" + cbu;
            }
            return cbu;
        }

        //PAGOS
        public bool agregarPago(string nombre, float monto, bool pagado, string metodo)
        {
            try
            {
                if (usuarioLogueado != null)
                {
                    Pago pago = new Pago(usuarioLogueado, nombre, monto, pagado, metodo);
                    pago.metodo = "";
                    pago.pagado = false;
                    pago.user = usuarioLogueado;
                    usuarioLogueado.misPagos.Add(pago);
                    contexto.pagos.Add(pago);
                    contexto.usuarios.Update(usuarioLogueado);                    
                    contexto.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool bajaPago(int id)
        {
            try
            {
                Pago p = buscarPagoId(id);
                {
                    if (p != null && p.pagado == true)
                    {
                        p.user.misPagos.Remove(p);
                        contexto.pagos.Remove(p);
                        contexto.SaveChanges();
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool modificarPago(int id)
        {
            Pago p = buscarPagoId(id);
            if (p != null)
            {
                p.pagado = true;
                contexto.pagos.Update(p);                
                contexto.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Pago> obtenerPagos()
        {
            return usuarioLogueado.misPagos.ToList();
        }
        public List<Pago> obtenerPagosBanco()
        {
            return contexto.pagos.ToList();
        }
        
        public Pago buscarPagoId(int id)
        {
            return contexto.pagos.FirstOrDefault(p => p.id == id);
        }
        
        // PLAZO FIJO
        
        public void pagarPlazoFijo()
        {
            foreach(PlazoFijo pf in contexto.plazoFijos)
            {
                if (pf.pagado == false && pf.fechaFin < DateTime.Now)
                {
                    pagarUnPf(pf);
                }  
            }
        }
        private void pagarUnPf(PlazoFijo pf)
        {            
            pf.pagado = true;
            CajaDeAhorro caja = pf.user.Caja.FirstOrDefault();
            caja.saldo += pf.monto + (pf.tasa / 365 * (pf.fechaFin - pf.fechaIni).Days) * pf.monto;
            contexto.plazoFijos.Update(pf);
            contexto.cajas.Update(caja);
            contexto.SaveChanges();
        }
        public bool crearPlazoFijo(int idCaja,int idUsuario, float monto)
        {
            try
            {                
                bool pagado = false;
                CajaDeAhorro caja = buscarCajaId(idCaja);
                Usuario u = buscarUsuarioId(idUsuario);
                {
                    if(caja.users.Contains(u))
                    {
                        if(monto >= 1000 && caja.saldo > monto )
                        {
                            caja.saldo -= monto;
                            PlazoFijo plazo = new PlazoFijo(u, monto, 90, pagado );
                            this.agregarMovimiento(caja, "Alta Plazo Fijo", monto);
                            u.misPlazosFijos.Add(plazo);
                            plazo.user = u;
                            contexto.plazoFijos.Add(plazo);
                            contexto.cajas.Update(caja);                            
                            contexto.usuarios.Update(u);
                            contexto.SaveChanges();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        } 
        public List<PlazoFijo> obtenerPlazosFijos()
        {
            return usuarioLogueado.misPlazosFijos.ToList();
        }
        public List<PlazoFijo> obtenerPlazosBanco()
        {
            return contexto.plazoFijos.ToList();
        }

        // TARJETA

        public bool crearTarjeta()
        {
            try
            {
                string numero = crearNumeroTarjeta();
                string codigoV = crearCodigoV();               
                if (usuarioLogueado != null)
                {
                    Tarjeta tarjeta = new Tarjeta(usuarioLogueado,numero, codigoV, 30000, 0);
                    tarjeta.user = usuarioLogueado;
                    usuarioLogueado.misTarjetas.Add(tarjeta);
                    contexto.tarjetas.Add(tarjeta);
                    contexto.usuarios.Update(usuarioLogueado);
                    contexto.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            }

        public bool borrarTarjeta(int id)
        {
            try
            {
                Tarjeta t = buscarTarjetaId(id);
                if (t != null && t.consumos != 0)
                {
                    contexto.tarjetas.Remove(t);
                    t.user.misTarjetas.Remove(t);
                    contexto.tarjetas.Update(t);
                    contexto.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool modificarTarjeta(int id, float limite)
        {
            try
            {
                Tarjeta t = buscarTarjetaId(id);
                
                    if (t != null && limite > t.consumos )
                    {
                        t.limite = limite;                        
                        contexto.tarjetas.Update(t);
                        contexto.SaveChanges();
                        return true;
                    }                
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string crearNumeroTarjeta()
        {
            string nt = "#";
            Random num = new Random();
            for (int i = 0; i < 4; i++)
            {
                nt += num.Next(0, 9999);
            }
            while (nt.Length < 10)
            {
                nt = "#" + nt;
            }
            return nt;
        }
        private string crearCodigoV()
        {
            string cvv = "#";
            Random num = new Random();
            for (int i = 0; i < 3; i++)
            {
                cvv += num.Next(0, 9);
            }
            while (cvv.Length < 4)
            {
                cvv = "#" + cvv;
            }
            return cvv;
        }
        public Tarjeta buscarTarjetaId(int id)
        {
            return contexto.tarjetas.FirstOrDefault(t => t.id == id);
        }
        
        public List<Tarjeta> obtenerTarjetas()
        {
            return usuarioLogueado.misTarjetas.ToList();
        }
        
        public bool pagarConTarjeta(int idPago, int idTarjeta, int idUsuario)
        {
            try
            {
                Pago p = buscarPagoId(idPago);
                Tarjeta t = buscarTarjetaId(idTarjeta);
                Usuario u = buscarUsuarioId(idUsuario);    
                
                    if (p != null && t != null && u.misTarjetas.Contains(t))
                    {
                            if (t.limite - t.consumos >= p.monto)
                            {
                                t.consumos += p.monto;
                                this.modificarPago(idPago);                                                           
                                p.metodo = "Tarjeta";                               
                                contexto.tarjetas.Update(t);                                
                                contexto.pagos.Update(p);                                
                                contexto.usuarios.Update(u);
                                contexto.SaveChanges();
                                return true;
                            }
                        return false;
                       
                    } return false;               

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        //MOVIMIENTOS
        public bool agregarMovimiento(CajaDeAhorro caja, string detalle, float monto)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                Movimiento nuevo = new Movimiento(caja, detalle, monto);
                contexto.movimientos.Add(nuevo);
                caja.misMovimientos.Add(nuevo);
                contexto.cajas.Update(caja);
                contexto.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<Movimiento> obtenerMovimientos(int id)
        {
            return contexto.movimientos.Where(m => m.id == id).ToList();
        }
        public List<Movimiento> buscarMovimientoPorDetalle(int idCaja, string detalle)
        {
            return contexto.movimientos.Where(m => m.detalle == detalle && m.id == idCaja).ToList();
        } 
    }
}

