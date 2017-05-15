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
    class ControladorDeVehiculos
    {
        private VehiculoPrincipal autoPrincipal;
        private TwistedMetal env;
        private readonly List<Vehiculo> listaDeVehiculos = new List<Vehiculo>();
        private TgcSceneLoader loader;

        public ControladorDeVehiculos(TwistedMetal env)
        {
            this.env = env;
            loader = new TgcSceneLoader();
        }

        public void crearAutoPrincipal()
        {
            autoPrincipal = new VehiculoPrincipal(this.env);
        }
        public VehiculoPrincipal getAutoPrincipal()
        {
            return autoPrincipal;
        }
        public void crearEnemigo1()
        {
            TgcMesh auto;
            var sceneAuto = loader.loadSceneFromFile(this.env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml");
            auto = sceneAuto.Meshes[0];
            //     auto.AutoTransformEnable = true;
            //   auto.move(25, 5, 50);
            auto.Scale = new Vector3(1, 1, 1);

            Enemigo enemigo = new Enemigo(this.env, new Vector3(25, 5, 50), sceneAuto.Meshes[0]);
            enemigo.getMesh().Scale = new Vector3(1, 1, 1);
            enemigo.getMesh().rotateY(FastMath.PI_HALF);
            this.listaDeVehiculos.Add(enemigo);
        }
        public void update()
        {
            foreach (var vehiculo in this.listaDeVehiculos)
            {
                if (!vehiculo.esAutoPrincipal())
                      vehiculo.Update();
            }
            this.autoPrincipal.Update();
        }
        public void render()
        {
            foreach (var vehiculo in this.listaDeVehiculos)
            {
                if (!vehiculo.esAutoPrincipal())
                    vehiculo.Render();
            }
            this.autoPrincipal.Render();
        }
        public void dispose()
        {
            foreach (var vehiculo in this.listaDeVehiculos)
            {
                vehiculo.dispose();
            }
        }
    }
}
