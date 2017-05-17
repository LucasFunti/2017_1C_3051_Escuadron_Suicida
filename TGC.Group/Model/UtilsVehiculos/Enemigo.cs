using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.UtilsVehiculos
{
    class Enemigo : Vehiculo
    {
        private int TiempoRetardo = 1;
        private int contadorDeCiclos = 0;
        private bool adelante = false;
        public Enemigo(TwistedMetal env, TgcMesh mesh) : base(mesh, env)
        {

            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!
        }
        public override void Update()
        {
            contadorDeCiclos++;
            if (contadorDeCiclos> TiempoRetardo)
            {
                contadorDeCiclos = 0;
                adelante = true;
            }
            
            base.Update();
            contadorDeCiclos++;
            adelante = false;
        }

        public override bool moverAdelante()
        {
            return adelante;
        }
    }
}
