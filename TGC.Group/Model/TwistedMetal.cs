using Microsoft.DirectX;
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
using TGC.Core.Sound;
using TGC.Group.Model.UtilsEfectos;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;

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
        public List<FixedLight> LucesLst;
        private PuntoDeLuz luz;
        public Ciudad Ciudad;
        public Musica sonidos;
        public Niebla niebla;
        //private Sonido sonido;
        private PrintMessageText messages;
        private ControladorDeVehiculos controladorDeVehiculos;
        private ManejadorDeColisiones manejadorDeColiciones;
        private static TwistedMetal myInstance;
        private VehiculoPrincipal autoPrincipal;
        private Microsoft.DirectX.Direct3D.Effect efectoShaderEnvironmentMap;

        public static TwistedMetal getInstance()
        {
            return myInstance;
        }

        public TwistedMetal(string mediaDir, string shadersDir)
            : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            myInstance = this;
            sonidos = new Musica(mediaDir);

            //sonidos.startGame();
            iniciarIluminacion();
        }
       private void iniciarIluminacion()
       {
            luz = new PuntoDeLuz(this, new Vector3(100f, 100f, 100f));
          /*  this.LucesLst = new List<FixedLight>();
           Vector3 abajo = new Vector3(0, -1, 0);
           this.LucesLst.Add(new FixedLight(new Vector3(0, 100, 0), abajo, 3000f, this));
           */
      }
        public void cambiarMusica()
        {
            sonidos.nextSound();
        }

        private TgcD3dInput Input { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            sonidos.startGame();
            var d3dDevice = D3DDevice.Instance.Device;
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
            //manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getItems());
            manejadorDeColiciones.addListOfBoundingBoxItemMeshColisionable(Ciudad.getItems());

           
            controladorDeVehiculos.crearAutoPrincipal();
            controladorDeVehiculos.crearEnemigo1();
            autoPrincipal = controladorDeVehiculos.getAutoPrincipal();
            //niebla = new Niebla(this);
            // niebla.CargarCamara(controladorDeVehiculos.getAutoPrincipal().getCamara());
            //  niebla
            //     controladorDeVehiculos.addToColisionador(manejadorDeColiciones);//por ahora los autos no colisionan
            //cargarShaders();
        }
        private void cargarShaders()
        {
        
            efectoShaderEnvironmentMap = TgcShaders.loadEffect(this.ShadersDir + "EnvMap.fx");
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

          
            
            
            //sonido.Update();
            controladorDeVehiculos.update();
            Ciudad.Update();
          
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el rende de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
             PreRender();//Comentar Para iluminacion
            //BorrarPantalla(); //Para iluminacion
            //preRenderPointLight(); //Para iluminacion
            //IniciarScene();//Para iluminacion
            //  AplicarShaderEnvironment();


            //    messages.MostrarPuntoColisionVehiculoPrincipal(manejadorDeColiciones.Manager().LastCollisionPoint);
            renderNormal();
            //TerminarScene();//Para iluminacion
            //  ImprimirPantalla();//Para iluminacion
            PostRender();//Comentar Para iluminacion
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene

        }
        private void BorrarPantalla()
        {
            ClearTextures();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        }
        private void preRenderPointLight()
        {
            //una escena para cada luz
           
            luz.render(this.Ciudad.getAllMeshes(),
                           this.autoPrincipal.getCamara().Position,
                           this.autoPrincipal.getMesh(),
                           this.autoPrincipal.matrixRotacion,
                           new Vector3(-12,
                           this.autoPrincipal.getBoxDeColision().Extents.Y + 1,
                           -this.autoPrincipal.getBoxDeColision().Extents.Z + 2),
                           this.autoPrincipal.getNuevaPosicion(), this.autoPrincipal.anguloFinal, true,
                           new Vector3(350 * -FastMath.Sin(this.autoPrincipal.anguloFinal), 0, 350 * -FastMath.Cos(this.autoPrincipal.anguloFinal)));

        /*    pointLuz2.render(MapScene.Meshes,
                           AutoJugador.CamaraAuto.Position,
                           AutoJugador.TodosLosMeshes,
                           AutoJugador.matrixRotacion,
                           new Vector3(12,
                           AutoJugador.obb.Extents.Y + 1,
                           -AutoJugador.obb.Extents.Z + 2),
                           AutoJugador.newPosicion, AutoJugador.anguloFinal, lucesPrendidas,
                           new Vector3(350 * -FastMath.Sin(AutoJugador.anguloFinal), 0, 350 * -FastMath.Cos(AutoJugador.anguloFinal)));*/
          //  TerminarScene();

            /*IniciarScene();
            
            TerminarScene();*/
        }
        /// <summary>
        /// inicia un scene
        /// </summary>
        private void IniciarScene()
        {
            D3DDevice.Instance.Device.BeginScene();
        }
        /// <summary>
        /// termina un scene
        /// </summary>
        private void TerminarScene()
        {
            D3DDevice.Instance.Device.EndScene(); //termina la escena
        }
        private void ImprimirPantalla()
        {
            D3DDevice.Instance.Device.Present();
        }
        private void renderNormal()
        {
            messages.MostrarComandosPorPantalla();
            messages.MostrarVelocidadPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadX());
            messages.MostrarPosicioMeshPorPantalla(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarVelocidadYPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadY());
            messages.MostrarPosicionCamaraPorPantalla(controladorDeVehiculos.getAutoPrincipal().getCamara().Position);
            messages.MostrarDireccionVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarAnguloVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().anguloFinal);
            messages.MostrarAnguloVehiculoEnemigo(controladorDeVehiculos.getEnemigo().anguloFinal);
            messages.MostrarDireccionEnemigo(controladorDeVehiculos.getEnemigo().getMesh().Position);
            messages.MostrarVelocidadEnemigoPorPantalla(controladorDeVehiculos.getEnemigo().getVelocidadX());
            messages.MostrarTiempo();
            //  niebla.Update(controladorDeVehiculos.getAutoPrincipal().getCamara());
            //  messages.test("BoudningBox", this.autoPrincipal.getMesh().BoundingBox.computeCorners());
           
            Ciudad.Render();
            controladorDeVehiculos.render();
           // PostRender();
        }
        public void renderScene(float elapsedTime, bool cubemap)
        {
            messages.MostrarComandosPorPantalla();
            messages.MostrarVelocidadPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadX());
            messages.MostrarPosicioMeshPorPantalla(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarVelocidadYPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadY());
            messages.MostrarPosicionCamaraPorPantalla(controladorDeVehiculos.getAutoPrincipal().getCamara().Position);
            messages.MostrarDireccionVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
            messages.MostrarAnguloVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().anguloFinal);
            messages.MostrarAnguloVehiculoEnemigo(controladorDeVehiculos.getEnemigo().anguloFinal);
            messages.MostrarDireccionEnemigo(controladorDeVehiculos.getEnemigo().getMesh().Position);
            messages.MostrarVelocidadEnemigoPorPantalla(controladorDeVehiculos.getEnemigo().getVelocidadX());
            messages.MostrarTiempo();
            //  niebla.Update(controladorDeVehiculos.getAutoPrincipal().getCamara());
            //  messages.test("BoudningBox", this.autoPrincipal.getMesh().BoundingBox.computeCorners());

            Ciudad.Render();



            //  controladorDeVehiculos.render();
            this.controladorDeVehiculos.getEnemigo().Render();
            if (!cubemap)
            {
                // dibujo el mesh
                this.autoPrincipal.getMesh().Technique = "RenderScene";
                this.autoPrincipal.getMesh().render();
            }
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

        private void AplicarShaderEnvironment()
        {
            this.autoPrincipal.getMesh().Effect = efectoShaderEnvironmentMap;

            var aspectRatio = D3DDevice.Instance.AspectRatio;
            efectoShaderEnvironmentMap.SetValue("fvLightPosition", new Vector4(0, 100, 0, 0));
            efectoShaderEnvironmentMap.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(this.autoPrincipal.getCamara().Position));
            efectoShaderEnvironmentMap.SetValue("kx", (float)0.1);
            efectoShaderEnvironmentMap.SetValue("kc", (float)0.25);
            efectoShaderEnvironmentMap.SetValue("usar_fresnel", false);

            D3DDevice.Instance.Device.EndScene();
            var g_pCubeMap = new CubeTexture(D3DDevice.Instance.Device, 256, 1, Usage.RenderTarget,
                Format.A16B16G16R16F, Pool.Default);
            var pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            // ojo: es fundamental que el fov sea de 90 grados.
            // asi que re-genero la matriz de proyeccion
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(90.0f),
                    1f, 1f, 10000f);

            // Genero las caras del enviroment map
            for (var nFace = CubeMapFace.PositiveX; nFace <= CubeMapFace.NegativeZ; ++nFace)
            {
                var pFace = g_pCubeMap.GetCubeMapSurface(nFace, 0);
                D3DDevice.Instance.Device.SetRenderTarget(0, pFace);
                Vector3 Dir, VUP;
                Color color;
                switch (nFace)
                {
                    default:
                    case CubeMapFace.PositiveX:
                        // Left
                        Dir = new Vector3(1, 0, 0);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Black;
                        break;

                    case CubeMapFace.NegativeX:
                        // Right
                        Dir = new Vector3(-1, 0, 0);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Red;
                        break;

                    case CubeMapFace.PositiveY:
                        // Up
                        Dir = new Vector3(0, 1, 0);
                        VUP = new Vector3(0, 0, -1);
                        color = Color.Gray;
                        break;

                    case CubeMapFace.NegativeY:
                        // Down
                        Dir = new Vector3(0, -1, 0);
                        VUP = new Vector3(0, 0, 1);
                        color = Color.Yellow;
                        break;

                    case CubeMapFace.PositiveZ:
                        // Front
                        Dir = new Vector3(0, 0, 1);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Green;
                        break;

                    case CubeMapFace.NegativeZ:
                        // Back
                        Dir = new Vector3(0, 0, -1);
                        VUP = new Vector3(0, 1, 0);
                        color = Color.Blue;
                        break;
                }

                D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color, 1.0f, 0);
                D3DDevice.Instance.Device.BeginScene();


                //Renderizar
                //   renderScene(ElapsedTime, true);
                renderScene(1, true);

                D3DDevice.Instance.Device.EndScene();
                //string fname = string.Format("face{0:D}.bmp", nFace);
                //SurfaceLoader.Save(fname, ImageFileFormat.Bmp, pFace);
            }
            // restuaro el render target
            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
            //TextureLoader.Save("test.bmp", ImageFileFormat.Bmp, g_pCubeMap);

            // Restauro el estado de las transformaciones
            D3DDevice.Instance.Device.Transform.View = this.autoPrincipal.getCamara().GetViewMatrix();
            D3DDevice.Instance.Device.Transform.Projection = Matrix.PerspectiveFovLH(Geometry.DegreeToRadian(45.0f), aspectRatio, 1f, 10000f);

            // dibujo pp dicho
            D3DDevice.Instance.Device.BeginScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            efectoShaderEnvironmentMap.SetValue("g_txCubeMap", g_pCubeMap);
            //   renderScene(ElapsedTime, false);
            renderScene(1, false);
            g_pCubeMap.Dispose();

            D3DDevice.Instance.Device.EndScene();
            D3DDevice.Instance.Device.Present();

        }

    }
}
