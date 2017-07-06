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
using TGC.Group.Model.UtilsVehiculos._2DObjects;

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
        private PuntoDeLuz luz, luz2;
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
        private static bool inicializado = false;
        public  Cronometro cronometro;
        private const int MENU = 0;
        private const int PLAYING = 1;
        private const int SELECTION = 2;
        private TgcRotationalCamera camaraRotante;
        private const float ROTATION_SPEED = -0.1f;

        private TgcBox boxComenzar;
        private TgcBox boxPersonaje;
        private TgcBox boxSalir;
        private Matrix boxComenzarOrig;
        private Matrix boxPersonajeOrig;
        private Matrix boxSalirOrig;
        private int contLimpiarTruco;
        //private const int ABAJO 
        private TgcMesh personaje1;
        private TgcMesh personaje2;
        private TgcMesh personaje3;
        private TgcMesh personaje4;
        private bool EndGame = false;

        private int opcionSeleccionada = 1;

        private int gameMode = MENU;

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
            this.MediaDir = mediaDir;
            this.ShadersDir = shadersDir;
            /*sonidos = new Musica(mediaDir);
            cronometro = new Cronometro(300000, this);
            sonidos.startGame();
            iniciarIluminacion();*/
            
        }

        private void startGame()
        {
            sonidos = new Musica(this.MediaDir);
            sonidos.startGame();

            controladorDeVehiculos.crearAutoPrincipal();
            controladorDeVehiculos.crearEnemigo1();
            autoPrincipal = controladorDeVehiculos.getAutoPrincipal();

            //Agrega objetos colisionables
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getEdificios());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getSemaforos());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getArboles());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getPostesDeLuz());

            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getMeshParedes());
            //manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getItems());
            manejadorDeColiciones.addListOfBoundingBoxItemMeshColisionable(Ciudad.getItems());
            manejadorDeColiciones.addBoundingBoxMeshColisionable(controladorDeVehiculos.getEnemigo().getMesh());
        }

       private void iniciarIluminacion()
       {
            luz = new PuntoDeLuz(this, new Vector3(100f, 100f, 100f));
            luz2 =new PuntoDeLuz(this, new Vector3(100f, 100f, 100f));
        }
        public void cambiarMusica()
        {
            sonidos.nextSound();
        }

        private new TgcD3dInput Input { get; set; }

        /// <summary>
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo
        ///     procesamiento que podemos pre calcular para nuestro juego.
        ///     Borrar el codigo ejemplo no utilizado.
        /// </summary>
        public override void Init()
        {
            //startGame();
            
            var d3dDevice = D3DDevice.Instance.Device;
            messages = new PrintMessageText(this);

            //Carga la estructura de la ciudad
            manejadorDeColiciones = new ManejadorDeColisiones();
            controladorDeVehiculos = new ControladorDeVehiculos(this);
            Ciudad = new Ciudad(this);
            
            cronometro = new Cronometro(300000, this);
            //sonidos.startGame();
            iniciarIluminacion();

            TgcD3dInput input = new TgcD3dInput();
            Input = input;

            camaraRotante = new TgcRotationalCamera(
                    new Vector3(3300, 250, 3100), 2000, 0.15f, 10f, base.Input);

            this.Camara = camaraRotante;
            //camaraRotante.SetCamera(new Vector3(500, 700, 500),
            //                         new Vector3(3000, 50, 3000));

            //Creamos una caja 3D con textura
            var center = new Vector3(3000, 200, 4500);
            var texture = TgcTexture.createTexture(MediaDir + "Menu\\COMENZAR.png");
            boxComenzar = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxComenzar.AutoTransformEnable = true;
            boxComenzarOrig = boxComenzar.Transform;

            // Creamos una caja 3D con textura
            center = new Vector3(3000, 140, 4500);
            texture = TgcTexture.createTexture(MediaDir + "Menu\\PERSONAJE.png");
            boxPersonaje = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxPersonaje.AutoTransformEnable = true;
            boxPersonajeOrig = boxPersonaje.Transform;

            // Creamos una caja 3D con textura
            center = new Vector3(3000, 80, 4500);
            texture = TgcTexture.createTexture(MediaDir + "Menu\\SALIR.png");
            boxSalir = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxSalir.AutoTransformEnable = true;
            boxSalirOrig = boxSalir.Transform;


            boxComenzar.Position = new Vector3(3000, 300, 4500);
            boxComenzar.rotateY(FastMath.PI);
            boxPersonaje.Position = new Vector3(3000, 200, 4500);
            boxPersonaje.rotateY(FastMath.PI);
            boxSalir.Position = new Vector3(3000, 100, 4500);
            boxSalir.rotateY(FastMath.PI);

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            personaje1 = scene.Meshes[0];
            personaje1.AutoTransformEnable = true;
            personaje1.Position = new Vector3(3400, 200, 5700);

            scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml");
            personaje2 = scene.Meshes[0];
            personaje2.AutoTransformEnable = true;
            personaje2.Position = new Vector3(3200, 200, 5700);

            scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml");
            personaje3 = scene.Meshes[0];
            personaje3.AutoTransformEnable = true;
            personaje3.Position = new Vector3(3000, 200, 5700);

            TwistedMetal.inicializado = true;

            /*controladorDeVehiculos.crearAutoPrincipal();
            controladorDeVehiculos.crearEnemigo1();
            autoPrincipal = controladorDeVehiculos.getAutoPrincipal();

            */

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


        private void seleccionPorDefault()
        {
            
            Personaje personaje = new Personaje();
            personaje.FileSonido = MediaDir + "MySounds\\MachineGun.wav";
            personaje.FileSonidoColision = MediaDir + "MySounds\\Crash4.wav";
            personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml";
            personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine2.wav";
            personaje.FileSonidoArma = MediaDir + "MySounds\\Special5.wav";
            personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
            personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
            personaje.VelocidadMax = 70;
            personaje.VelocidadMin = -5;
            personaje.ConstanteAceleracion = 0.6f;
        }



        private void seleccionaPersonaje(int indice)
        {

            Personaje personaje = new Personaje();
            personaje.FileSonido = MediaDir + "MySounds\\MachineGun.wav";
            personaje.FileSonidoColision = MediaDir + "MySounds\\Crash4.wav";
            switch (indice)
            {
                case 1:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine2.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\Special5.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 70;
                    personaje.VelocidadMin = -5;
                    personaje.ConstanteAceleracion = 0.6f;
                    break;
                case 2:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine4.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\Launch3.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 90;
                    personaje.VelocidadMin = -15;
                    personaje.ConstanteAceleracion = 0.8f;
                    break;
                default:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine1.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\Special1.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 80;
                    personaje.VelocidadMin = -10;
                    personaje.ConstanteAceleracion = 0.7f;
                    break;
            }
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

            if (TwistedMetal.inicializado)
            {
               

                //sonido.Update();
               

                if (gameMode == PLAYING) {
                    controladorDeVehiculos.update();
                }

                if (gameMode == SELECTION) {

                    if (base.Input.keyPressed(Key.Space)) {
                        seleccionaPersonaje(opcionSeleccionada);
                        gameMode = MENU;
                        opcionSeleccionada = 1;
                    }
                    
                } else
                {
                    if (gameMode == MENU)
                    {
                        if (base.Input.keyPressed(Key.Space))
                        {
                            switch (opcionSeleccionada)
                            {
                                case 1:
                                    gameMode = PLAYING;
                                    if (Personaje.getInstance() == null) seleccionPorDefault();
                                    startGame();
                                    boxComenzar.Enabled = false;
                                    boxPersonaje.Enabled = false;
                                    boxSalir.Enabled = false;
                                    personaje1.Enabled = false;
                                    personaje2.Enabled = false;
                                    personaje3.Enabled = false;
                                    break;
                                case 2:
                                    gameMode = SELECTION;
                                    opcionSeleccionada = 1;
                                    break;
                                case 3:
                                    EndGame = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                

                if (gameMode == SELECTION || gameMode == MENU)
                {
                    if (base.Input.keyPressed(Key.Tab))
                    {
                        opcionSeleccionada += 1;
                        if (opcionSeleccionada > 3) opcionSeleccionada = 1;
                    }

                    if (gameMode == MENU) {
                        switch (opcionSeleccionada)
                        {
                            case 1:
                                boxComenzar.rotateX(ROTATION_SPEED);
                                boxPersonaje.Transform = boxPersonajeOrig;
                                boxSalir.Transform = boxSalirOrig;
                                break;
                            case 2:
                                boxPersonaje.rotateX(ROTATION_SPEED);
                                boxComenzar.Transform = boxComenzarOrig;
                                boxSalir.Transform = boxSalirOrig;
                                break;
                            case 3:
                                boxSalir.rotateX(ROTATION_SPEED);
                                boxComenzar.Transform = boxComenzarOrig;
                                boxPersonaje.Transform = boxPersonajeOrig;
                                break;
                            default:
                                break;
                        }

                    } else //Estoy en modo SELECTION
                    {
                        switch (opcionSeleccionada)
                        {
                            case 1:
                                personaje1.rotateY(ROTATION_SPEED);
                                break;
                            case 2:
                                personaje2.rotateY(ROTATION_SPEED);
                                break;
                            case 3:
                                personaje3.rotateY(ROTATION_SPEED);
                                break;
                            case 4:
                                personaje4.rotateY(ROTATION_SPEED);
                                break;
                            default:
                                break;
                        }
                    }

                }

                Ciudad.Update();
            } else {
                
            }
            
          
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
            renderNormal();
            PostRender();//Comentar Para iluminacion
            if (EndGame ) TGC.Group.Form.GameForm.ActiveForm.Close();
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene

        }
        /* //Render Con Iluminación
        public override void Render()
        {
            //Inicio el rende de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            // PreRender();//Comentar Para iluminacion
            BorrarPantalla(); //Para iluminacion
            preRenderPuntoDeLuz(); //Para iluminacion
            IniciarScene();//Para iluminacion
            //  AplicarShaderEnvironment();


            //    messages.MostrarPuntoColisionVehiculoPrincipal(manejadorDeColiciones.Manager().LastCollisionPoint);
            renderNormal();
            TerminarScene();//Para iluminacion
            ImprimirPantalla();//Para iluminacion
            //PostRender();//Comentar Para iluminacion
            //Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene

        }*/
        private void BorrarPantalla()
        {
            ClearTextures();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
        }
        private void preRenderPuntoDeLuz()
        {
            //una escena para cada luz
            //una escena para cada luz
            IniciarScene();
            luz.render(this.Ciudad.getAllMeshes(),
                           this.autoPrincipal.getCamara().Position,
                           this.autoPrincipal.getMesh(),
                           this.autoPrincipal.matrixRotacion,
                           new Vector3(-12,this.autoPrincipal.getBoxDeColision().Extents.Y + 1,
                           -this.autoPrincipal.getBoxDeColision().Extents.Z + 2),
                           this.autoPrincipal.getNuevaPosicion(), this.autoPrincipal.anguloFinal, true,
                           new Vector3(350 * -FastMath.Sin(this.autoPrincipal.anguloFinal), 0, 350 * -FastMath.Cos(this.autoPrincipal.anguloFinal)));
            //     TerminarScene();
            luz2.render(this.Ciudad.getAllMeshes(),
                               this.autoPrincipal.getCamara().Position,
                               this.autoPrincipal.getMesh(),
                               this.autoPrincipal.matrixRotacion,
                               new Vector3(-12, this.autoPrincipal.getBoxDeColision().Extents.Y + 1,-this.autoPrincipal.getBoxDeColision().Extents.Z + 2),
                              this.autoPrincipal.getNuevaPosicion(), this.autoPrincipal.anguloFinal, true,
                               new Vector3(350 * -FastMath.Sin(this.autoPrincipal.anguloFinal), 0, 350 * -FastMath.Cos(this.autoPrincipal.anguloFinal)));
              TerminarScene();

          //  IniciarScene();
            
          //  TerminarScene();
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
            if (TwistedMetal.inicializado)
            {

                if (gameMode == PLAYING)
                {
                    messages.MostrarComandosPorPantalla();
                    messages.MostrarTiempo();
                    messages.MostrarVelocidadPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadX());
                    messages.MostrarPosicioMeshPorPantalla(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
                    messages.MostrarVelocidadYPorPantalla(controladorDeVehiculos.getAutoPrincipal().getVelocidadY());
                    messages.MostrarPosicionCamaraPorPantalla(controladorDeVehiculos.getAutoPrincipal().getCamara().Position);
                    messages.MostrarDireccionVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().getMesh().Position);
                    messages.MostrarAnguloVehiculoPrincipal(controladorDeVehiculos.getAutoPrincipal().anguloFinal);
                    messages.MostrarAnguloVehiculoEnemigo(controladorDeVehiculos.getEnemigo().anguloFinal);
                    messages.MostrarDireccionEnemigo(controladorDeVehiculos.getEnemigo().getMesh().Position);
                    messages.MostrarVelocidadEnemigoPorPantalla(controladorDeVehiculos.getEnemigo().getVelocidadX());
                    controladorDeVehiculos.render();
                    this.cronometro.render(ElapsedTime);
                    //  niebla.Update(controladorDeVehiculos.getAutoPrincipal().getCamara());
                    //  messages.test("BoudningBox", this.autoPrincipal.getMesh().BoundingBox.computeCorners());
                }

                Ciudad.Render();
                personaje1.render();
                personaje2.render();
                personaje3.render();
                boxComenzar.render();
                boxPersonaje.render();
                boxSalir.render();

                messages.MostrarMensaje("gameMode = " + gameMode, 50, 600);
                messages.MostrarMensaje("opcionSeleccionada = " + opcionSeleccionada, 50, 620);

                if (gameMode == MENU)
                {

                    //camaraRotante.;
                    camaraRotante.CameraDistance = 2000;
                    camaraRotante.UpdateCamera(ElapsedTime);
                    this.Camara = camaraRotante;
                    messages.MostrarComandosDeSeleccion();
                    messages.MostrarPosicioMeshPorPantalla(camaraRotante.Position);

                   
                }
                if (gameMode == SELECTION)
                {
                    messages.MostrarComandosDeSeleccion();
                    //camaraRotante.CameraCenter = new Vector3(3000, 50, 3000);
                    //camaraRotante.SetCamera(new Vector3(250, 450, 3000), new Vector3(3000, 150, 3000));
                    camaraRotante.CameraDistance = 3100;
                    camaraRotante.UpdateCamera(ElapsedTime);
                    this.Camara = camaraRotante;
                    

                }
                messages.MostrarMensaje("boxComenzar.Position = " + boxComenzar.Position, 50, 640);

            }
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
            //Ciudad.dispose();
            controladorDeVehiculos.dispose();
            boxComenzar.dispose();
            boxPersonaje.dispose();
            boxSalir.dispose();
            personaje1.dispose();
            personaje2.dispose();
            personaje3.dispose();
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
