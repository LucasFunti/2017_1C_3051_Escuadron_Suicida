using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.UtilsVehiculos
{
    class ControladorDeVehiculos
    {
        private VehiculoPrincipal autoPrincipal;
        private TwistedMetal env;
        private readonly List<Vehiculo> listaDeVehiculos = new List<Vehiculo>();

        public ControladorDeVehiculos(TwistedMetal env)
        {
            this.env = env;
        }

        public void crearAutoPrincipal()
        {
            autoPrincipal = new VehiculoPrincipal(this.env);
        }
        public VehiculoPrincipal getAutoPrincipal()
        {
            return autoPrincipal;
        }
        public void crearEnemigo()
        {
          //  autoPrincipal = new Enemigo(this.env);
        }
        
    }
}
