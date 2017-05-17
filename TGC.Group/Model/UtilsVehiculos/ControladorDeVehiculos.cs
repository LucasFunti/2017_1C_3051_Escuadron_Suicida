﻿using Microsoft.DirectX;
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
        private readonly List<Vehiculo> listaDeEnemigos = new List<Vehiculo>();
        private TgcSceneLoader loader;

        public ControladorDeVehiculos(TwistedMetal env)
        {
            this.env = env;
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

            //   enemigo.setPosicionInicial(new Vector3(-100, 5, 3000));
            enemigo.setPosicionInicial(new Vector3(-100, 5, -200));
            enemigo.setVelocidadMaxima(40);
            enemigo.setVelocidadMinima(-5);
            enemigo.setConstanteDeAsceleracionX(0.5f);
            this.listaDeVehiculos.Add(enemigo);
            this.listaDeEnemigos.Add(enemigo);
        }
        public void update()
        {
            foreach (var enemigo in this.listaDeEnemigos)
                enemigo.Update();
            
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
