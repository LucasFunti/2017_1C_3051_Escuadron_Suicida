﻿using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;
using System.Collections.Generic;
using TGC.Examples.Camara;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;
using TGC.Group.Model.UtilsVehiculos;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class TwistedMetal : TgcExample
    {
        private Ciudad Ciudad;
        private PrintMessageText messages;
        private ControladorDeVehiculos controladorDeVehiculos;
        private ManejadorDeColisiones manejadorDeColiciones;

        public TwistedMetal(string mediaDir, string shadersDir)
            : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }



        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //Carga la estructura de la ciudad
            manejadorDeColiciones = new ManejadorDeColisiones();
            controladorDeVehiculos = new ControladorDeVehiculos(this);
            Ciudad = new Ciudad(this);
            
            messages = new PrintMessageText(this);
           
            //Agrega objetos colisionables
           manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getEdificios());
           manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getSemaforos());
           manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getArboles());
           manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getPostesDeLuz());
              
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getMeshParedes());

            controladorDeVehiculos.crearAutoPrincipal();
            controladorDeVehiculos.crearEnemigo1();
       //     controladorDeVehiculos.addToColisionador(manejadorDeColiciones);//por ahora los autos no colisionan
        }
        public ManejadorDeColisiones GetManejadorDeColision()
        {
            return this.manejadorDeColiciones;
        }
       
        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            //Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 2f);

             Ciudad.Update();
            controladorDeVehiculos.update();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el rende de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();
            messages.MostrarComandosPorPantalla();
            messages.MostrarVelocidadPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadX());
            messages.MostrarPosicioMeshPorPantalla(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarVelocidadYPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadY());
            messages.MostrarDireccionVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarTiempo();
          //  messages.test("BoudningBox", this.autoPrincipal.getMesh().BoundingBox.computeCorners());

            Ciudad.Render();
            controladorDeVehiculos.render();
        //    messages.MostrarPuntoColisionVehiculoPrincipal(manejadorDeColiciones.Manager().LastCollisionPoint);

            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Se llama cuando termina la ejecución del ejemplo.
        ///     Hacer Dispose() de todos los objetos creados.
        ///     Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            Ciudad.dispose();
            controladorDeVehiculos.dispose();
        }

        
    }
}
