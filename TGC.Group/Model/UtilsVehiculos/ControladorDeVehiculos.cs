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
        private List<Vehiculo> listaDeVehiculos = new List<Vehiculo>();
        private readonly List<Vehiculo> listaDeEnemigos = new List<Vehiculo>();
        private readonly List<Vehiculo> listaDeArmas = new List<Vehiculo>();
        private TgcSceneLoader loader;
        private static ControladorDeVehiculos myInstance;
        Enemigo enemigo1aux;
        Enemigo enemigoFinal;
        public static ControladorDeVehiculos getInstance()
        {
            return myInstance;
        }

        public void agregarArma(Arma arma)
        {
            this.env.GetManejadorDeColision().addBoundingBoxMeshArmaDisparada(arma.getMesh());
            this.listaDeArmas.Add(arma);
            this.listaDeVehiculos.Add(arma);
        }
        
        public ControladorDeVehiculos(TwistedMetal env)
        {
            this.env = env;
            myInstance = this;
            loader = new TgcSceneLoader();
        }
        public List<Vehiculo> getListaDeAutos()
        {
            return listaDeVehiculos;
        }
        public void crearAutoPrincipal()
        {
            autoPrincipal = new VehiculoPrincipal(this.env);
           
            //     listaDeVehiculos.Add(autoPrincipal);
        }
        public VehiculoPrincipal getAutoPrincipal()
        {
            return autoPrincipal;
        }
        public void addToColisionador(ManejadorDeColisiones manejadorDeColisiones)
        {
            foreach (var vehiculo in this.listaDeVehiculos)
            {
              if (!vehiculo.esAutoPrincipal())
                  manejadorDeColisiones.addBoundingBoxMeshColisionable(vehiculo.getMesh());
            }
                 
            
        }
        public void crearEnemigo1()
        {

           var sceneAuto = loader.loadSceneFromFile(this.env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml");
          
            Enemigo enemigo = new Enemigo(this.env,  sceneAuto.Meshes[0]);
            enemigo.setAutoTarget(this.getAutoPrincipal());
            //   enemigo.setPosicionInicial(new Vector3(-100, 5, 3000));


            enemigo.setPosicionInicial(new Vector3(5000, 5, 5750));
            enemigo.setVelocidadMaxima(10);
            enemigo.setVelocidadMinima(-5);
            enemigo.setConstanteDeAsceleracionX(0.5f);
            enemigo1aux = enemigo;
            this.listaDeVehiculos.Add(enemigo);
            this.listaDeEnemigos.Add(enemigo);
        }

        public void crearEnemigoFinal()
        {

            var sceneAuto = loader.loadSceneFromFile(this.env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\GruaExcavadora\\GruaExcavadora-TgcScene.xml");

            //Enemigo enemigoF = new Enemigo(this.env, sceneAuto.Meshes[0]);
            TgcMesh enemigoF = sceneAuto.Meshes[0];
            enemigoF.AutoUpdateBoundingBox = true;
            enemigo1aux.setMesh(enemigoF);
            enemigo1aux.esEnemigoFinal = true;
            enemigo1aux.getLifeLevel().recibirVida(100);
            enemigo1aux.setAutoTarget(this.getAutoPrincipal());
            //   enemigo.setPosicionInicial(new Vector3(-100, 5, 3000));

            enemigo1aux.estaMuerto = false;
            //enemigoF.getLifeLevel();
            enemigo1aux.setPosicionInicial(new Vector3(5000, 5, 5750));
            enemigo1aux.setVelocidadMaxima(10);
            enemigo1aux.setVelocidadMinima(-5);
            enemigo1aux.setConstanteDeAsceleracionX(0.5f);
            //enemigoFinal = enemigoF;
            //this.listaDeVehiculos.Add(enemigoF);
            //this.listaDeEnemigos.Add(enemigoF);
        }

        public Enemigo getEnemigo()
        {
            return enemigo1aux;
        }
        public Enemigo getEnemigoFinal()
        {
            return enemigoFinal;
        }
        public void update()
        {
            foreach (var enemigo in this.listaDeEnemigos)
            {
                enemigo.Update();
            }

            foreach (var arma in this.listaDeArmas)
                if (arma.getMesh().Enabled ) arma.Update();

            this.autoPrincipal.Update();

            if (TgcCollisionUtils.testObbObb(this.autoPrincipal.getBoxDeColision(), this.getEnemigo().getBoxDeColision()))
            {
                this.autoPrincipal.dañoPorChoqueEnemigo();
                this.getEnemigo().VolverAPosicionAnterior();
            }

        }
        public void render()
        {
            foreach (var vehiculo in this.listaDeVehiculos)
            { 
                if (!vehiculo.esAutoPrincipal()){
                    if (!vehiculo.estaMuerto) vehiculo.Render();
                }
            }
            this.autoPrincipal.Render();
        }

        public void deshabilitarObjeto(TgcMesh arma)
        {
            var listaDeVehiculosBackup = this.listaDeVehiculos;
            this.listaDeVehiculos = new List<Vehiculo>();
            foreach (var vehiculo in listaDeVehiculosBackup)
                if (vehiculo.getMesh() != arma)
                    this.listaDeVehiculos.Add(vehiculo);
        }

        public void dispose()
        {
            foreach (var vehiculo in this.listaDeVehiculos)
                vehiculo.dispose();
        }
    }
}
