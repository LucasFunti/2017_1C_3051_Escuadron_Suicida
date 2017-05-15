using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Geometry;
using System.Drawing;
using TGC.Group.Model.UtilsColisiones;
using TGC.Core.Utils;
using System;

namespace TGC.Group.Model
{
    class VehiculoPrincipal : Vehiculo
    {
        
        private TgcSceneLoader loader;
        private CamaraTerceraPersona camaraInterna;
        public static float camaraOffsetDefaulForward = 300f;

        public VehiculoPrincipal(TwistedMetal env) : base(env)
        {
            loader = new TgcSceneLoader();
            var sceneHummer = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            TgcMesh mesh = sceneHummer.Meshes[0];
          //  mesh.AutoTransformEnable = true;
            mesh.move(0, 5, 0);
            this.setMesh(mesh);
            this.setVelocidadMaxima(70);
            this.setVelocidadMinima(-5);
            this.setConstanteDeAsceleracionX(0.1f);
      //      base.setPEndDirectionArrow(new Vector3(this.getMesh().Position.X, this.getMesh().Position.Y, -500));
            //   this.setAlturaInicial(this.getMesh().Position.Y);
             camaraManager();

        }
        public override Boolean esAutoPrincipal()
        {
            return true;
        }
        private void camaraManager()
        {
            camaraInterna = new CamaraTerceraPersona(this.getMesh().Position, 100, 300);
            this.env.Camara = camaraInterna;
        }

       
        public override void ManejarColisionCamara()
        {
            //Actualizar valores de camara segun modifiers
            //COPIADO DE EJEMPLO COLISIONES CAMARA del tgc viewer

            //Pedirle a la camara cual va a ser su proxima posicion
            Vector3 position;
            Vector3 target;
            camaraInterna.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            Vector3 q;
            var minDistSq = FastMath.Pow2(camaraInterna.OffsetForward);
            foreach (var obstaculo in this.env.GetManejadorDeColision().MeshesColicionables)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, obstaculo.BoundingBox, out q))
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    var distSq = Vector3.Subtract(q, target).LengthSq();
                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }

            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)
            var newOffsetForward = -FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = -10;
            }
            if (camaraOffsetDefaulForward > camaraInterna.OffsetForward)
            {
                camaraInterna.OffsetForward = (newOffsetForward - 72f * this.env.ElapsedTime) * (-1f); //enderezo lentamente

            }
            else
            {
                camaraInterna.OffsetForward = -newOffsetForward;
            }
           //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            camaraInterna.CalculatePositionTarget(out position, out target);
            camaraInterna.SetCamera(position, target);

        }
        public override void ProcesarMovimientoDeCamara(float offsetHeight, float offsetForward)
        {
            
            camaraInterna.OffsetHeight = offsetHeight;
            camaraInterna.OffsetForward = offsetForward;

        }
        public override bool moverAdelante()
        {
            return this.env.Input.keyDown(Key.W);
        }
        public override bool moverAtras()
        {
            return this.env.Input.keyDown(Key.S);
        }
        public override bool moverADerecha()
        {
            return this.env.Input.keyDown(Key.D);
        }
        public override bool moverAIzquierda()
        {
            return this.env.Input.keyDown(Key.A);
        }
        public override bool moverArriba()
        {
            return this.env.Input.keyDown(Key.J);
        }
        public override bool moverAbajo()
        {
            return this.env.Input.keyDown(Key.K);
        }
        public override bool cambiarCamara()
        {
            return this.env.Input.keyDown(Key.C);
        }
        public override void rotarCamara(float rotAngle)
        {
            camaraInterna.rotateY(rotAngle);
         //   base.updateDirectionArrowWithAngle(rotAngle);
        }
        public override void Update()
        {
            base.Update();
            if (cambiarCamara())
            {
                var height = camaraInterna.OffsetHeight;
                var forward = camaraInterna.OffsetForward;
                if (height == 100)
                {
                    height = 200;
                    forward = 500;
                }
                else
                {
                    height = 100;
                    forward = 300;
                }
                ProcesarMovimientoDeCamara(height, forward);
            }
            camaraInterna.Target = this.getMesh().Position;
        }
        public override void Render()
        {
            base.Render();

        }
    }
}
