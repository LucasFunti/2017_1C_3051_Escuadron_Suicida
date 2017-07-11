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

        public Arma(TgcMesh Mesh, TwistedMetal env, string sonido, float aceleracion, 
                    float orientacion, float angulo, Vector3 scale) : base(Mesh, env)
        {
            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!
            this.getMesh().AutoTransformEnable = false;
            this.getMesh().AutoUpdateBoundingBox = true;
            this.getMesh().createBoundingBox();
            this.setVelocidadMaxima(150);
            this.setVelocidadMinima(0);
            this.setConstanteDeAsceleracionX(aceleracion);
            this.orientacion = orientacion;
            this.escalado = scale;
            this.getMesh().Scale = scale;
            this.setAnguloFinal(angulo);
            base.setSonido(sonido);
            base.setSonidoMotor(sonido);
            base.setSonidoArma(sonido);
            base.setSonidoColision(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoItem(env.MediaDir + "MySounds\\Crash4.wav");
            base.setSonidoSalto(env.MediaDir + "MySounds\\Crash4.wav");
            base.setEsArma(true);
            this.doblar(orientacion);
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
