using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model.UtilsVehiculos
{
    class Arma : Vehiculo
    {
    //   private Boolean doblaD = false;
     //   private Boolean doblaI = false;
        private int tiempoDeVida = 0;

        public Arma(TgcMesh Mesh, TwistedMetal env, string sonido, float aceleracion, float orientacion) : base(Mesh, env)
        {
            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!
            
            this.setVelocidadMaxima(150);
            this.setVelocidadMinima(0);
            this.setConstanteDeAsceleracionX(aceleracion);
            this.orientacion = orientacion;
            //this.getMesh().rotateY(orientacion);
            //this.getMesh().Rotation = me.Rotation;
            //this.matrixRotacion = me.Transform;
            //var m = Matrix.Scaling(scale3) * me.matrixRotacion * Matrix.Translation(NuevaPosicion);
            //this.getMesh().Transform = MT;
            //this.getMesh().rotateY(orientacion);
            //this.getMesh().Position = NuevaPosicion;
            /*var sentido = 1f;
            if (this.getVelocidadX() < 0)
                sentido *= -1;

            sentido = sentido * this.getVelocidadRotacion();
            base.rotar(new Vector3(0, orientacion, 0), MT, 0);*/
            //this.getMesh().Transform = MT;

            //string sonido = env.MediaDir + "MySounds\\MachineGun.wav";
            this.setAnguloFinal(this.orientacion);
            base.setSonido(sonido);
            base.setSonidoMotor(sonido);
            base.setSonidoArma(sonido);
            base.setSonidoColision(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoItem(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoSalto(env.MediaDir + "MySounds\\Crash4.wav");
            base.setEsArma(true);
        }
        public override void Update()
        {
            if (base.getMesh().Enabled ) base.Update();
        }
        public void setTiempoDeVida(int t)
        {
            this.tiempoDeVida=t;
        }
        public int getTiempoDeVida()
        {
            return tiempoDeVida;
        }
        public override bool moverAIzquierda()
        {
            return false;
        }
        public override bool moverADerecha()
        {
            return false;
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
