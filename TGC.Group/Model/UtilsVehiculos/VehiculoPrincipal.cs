using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Geometry;
using System.Drawing;

namespace TGC.Group.Model
{
    class VehiculoPrincipal : Vehiculo
    {
        
        private TgcSceneLoader loader;
        private CamaraTerceraPersona camaraInterna;
        private TwistedMetal env;
 

        public VehiculoPrincipal(TwistedMetal env) : base(env)
        {
            loader = new TgcSceneLoader();
            this.env = env;
            var sceneHummer = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            TgcMesh mesh = sceneHummer.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.move(0, 5, 0);
            this.setMesh(mesh);
            this.setVelocidadMaxima(10);
            this.setVelocidadMinima(-10);
            this.setConstanteDeAsceleracionX(0.1f);
            //   this.setAlturaInicial(this.getMesh().Position.Y);
             camaraManager();

        }
      
        private void camaraManager()
        {
            camaraInterna = new CamaraTerceraPersona(this.getMesh().Position, 100, 300);
            this.env.Camara = camaraInterna;
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
                } else
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
            TgcBoundingSphere characterSphere = new TgcBoundingSphere(this.getMesh().BoundingBox.calculateBoxCenter(),
            this.getMesh().BoundingBox.calculateBoxRadius());
            characterSphere.render();
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
        }
    }
}
