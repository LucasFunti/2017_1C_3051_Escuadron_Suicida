using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;


namespace TGC.Group.Model
{
    class VehiculoPrincipal : Vehiculo
    {
        
        private Vector3 pos;
        private TgcSceneLoader loader;
        private CamaraTerceraPersona camaraInterna;
        private Core.Example.TgcExample env;

        public VehiculoPrincipal(Core.Example.TgcExample env) : base(env)
        {
            loader = new TgcSceneLoader();
            this.env = env;
            var sceneHummer = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            TgcMesh mesh = sceneHummer.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.move(0, 5, 0);
            this.setMesh(mesh);
            this.setVelocidadMaxima(30);
            this.setVelocidadMinima(-30);
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
            //Hacer que la camara siga al personaje en su nueva posicion
            camaraInterna.Target = this.getMesh().Position;
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
        public override void rotarCamara(float rotAngle)
        {
                camaraInterna.rotateY(rotAngle);
        }
    }
}
