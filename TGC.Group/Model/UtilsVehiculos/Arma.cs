using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.UtilsVehiculos
{
    class Arma : Vehiculo
    {
        private int TiempoRetardo = 1;
        private int contadorDeCiclos = 0;
        private bool adelante = false;
        public Arma(TgcMesh Mesh, TwistedMetal env, string sonido, float aceleracion) : base(Mesh, env)
        {
            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!
            
            this.setVelocidadMaxima(150);
            this.setVelocidadMinima(0);
            this.setConstanteDeAsceleracionX(aceleracion);
            //string sonido = env.MediaDir + "MySounds\\MachineGun.wav";

            base.setSonido(sonido);
            base.setSonidoMotor(sonido);
            base.setSonidoArma(sonido);
            base.setSonidoColision(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoItem(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoSalto(env.MediaDir + "MySounds\\Crash4.wav");
        }
        public override void Update()
        {
            base.Update();
            
        }

        public override void Render()
        {
            base.Render();

        }

        public override bool moverAdelante()
        {
            return true;
        }
    }
}
