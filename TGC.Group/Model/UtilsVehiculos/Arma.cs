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
        private int TiempoRetardo = 1;

        private Vector3 target;
        private Boolean doblaD = false;
        private Boolean doblaI = false;
        private int tiempoDeVida = 0;

        public Arma(TgcMesh Mesh, TwistedMetal env, string sonido, float aceleracion, float orientacion, Vector3 target) : base(Mesh, env)
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
            this.target = target;
            ApuntarAlTarget();
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
        private void ApuntarAlTarget()
        {
            //1- bajo el contador que se fija cuando fue la ultima vez que apunte.

             float X1 = this.getMesh().Position.X;
            float Z1 = this.getMesh().Position.Z;

            float X2 = this.target.X;
            float Z2 = this.target.Z;

            float ang = FastMath.Atan2((Z2 - Z1), (X2 - X1));


            if (base.anguloFinal != (FastMath.PI * 3 / 2) - ang)
            {
                setAnguloFinal((FastMath.PI * 3 / 2) - ang);

                if (ang >= 0 && ang <= 3)
                    doblaD = true;
                if (ang < 0 && ang >= -3)
                    doblaI = true;


            }
         
        }
        public override bool moverAIzquierda()
        {

            return doblaI;
        }
        public override bool moverADerecha()
        {

            return doblaD;
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
