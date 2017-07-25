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
using TGC.Core.Text;
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
using TGC.Core.Text;

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
        private Sonido sonido;
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
        private CamaraTerceraPersona camaraMenu;
        private const float ROTATION_SPEED = -0.1f;

        private TgcBox boxComenzar;
        private TgcBox boxPersonaje;
        private TgcBox boxSalir;
        private Matrix mBoxComenzar;
        private Matrix mBoxPersonaje;
        private Matrix mBoxSalir;
        private int contLimpiarTruco;
        //private const int ABAJO 
        private TgcMesh personaje1;
        private TgcMesh personaje2;
        private TgcMesh personaje3;
        private Matrix mPersonaje1;
        private Matrix mPersonaje2;
        private Matrix mPersonaje3;
        private TgcMesh personaje4;
        private Matrix mPersonaje4;
        private int deltaSonido = 280;
        private int contSonido = 0;
        private bool EndGame = false;
        private int contadorFinal = 800;
        public bool juegoTerminado = false;
        private int contadorEnemigoFinal = 200;
        private bool enemigoFinalCreado = false;
        float rotacion = 0;
        private Vector3 puntoMenu;
        private Vector3 puntoPersonaje;
        private Vector3 puntoPersonajeOculto;
        private TgcBox boxGameOver;
        private TgcBox boxResult;
        private bool trucoHabilitadp = false;
        private bool teclaAnteriorArriba = false;
        private bool teclaAnteriorAbajo = false;
        private int contadorTruco = 0;
        private Matrix mBoxGameOver;
        private Matrix mBoxResult;

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
            
        }

        private void startGame()
        {
            sonidos = new Musica(this.MediaDir);
            sonidos.startGame();

            controladorDeVehiculos.crearAutoPrincipal();
            controladorDeVehiculos.crearEnemigo1();
            autoPrincipal = controladorDeVehiculos.getAutoPrincipal();

            Vector3 vecDisparo = new Vector3(controladorDeVehiculos.getAutoPrincipal().getMesh().Position.X,
                                                         controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Y,
                                                        controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Z);
            sonido.playSound(MediaDir + "MySounds\\risa.wav", vecDisparo);
            sonido.startSound();

            //Agrega objetos colisionables
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getEdificios());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getSemaforos());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getArboles());
            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getPostesDeLuz());

            manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getMeshParedes());
            //manejadorDeColiciones.addListOfBoundingBoxMeshesColisionables(Ciudad.getItems());
            manejadorDeColiciones.addListOfBoundingBoxItemMeshColisionable(Ciudad.getItems());
            manejadorDeColiciones.addBoundingBoxMeshColisionable(controladorDeVehiculos.getEnemigo().getMesh());

            manejadorDeColiciones.addBoundingBoxMeshColisionable(autoPrincipal.getMesh());
            //manejadorDeColiciones.addBoundingBoxMeshColisionable(controladorDeVehiculos.getEnemigo().getMesh());
        }
    
        private void terminarJuego(bool winner)
        {
            juegoTerminado = true;
            //Creamos una caja 3D con textura
            var center = new Vector3(3000, 1300, 3000);
            var centerResult = new Vector3(3000, 800, 3000);
            var texture = TgcTexture.createTexture(MediaDir + "Menu\\SuicideSquad.jpg");
            boxGameOver = TgcBox.fromSize(center, new Vector3(400, 400, 400), texture);
            boxGameOver.AutoTransformEnable = false;
            if (winner) {
                texture = TgcTexture.createTexture(MediaDir + "Menu\\GANASTE.png");
            } else {
                texture = TgcTexture.createTexture(MediaDir + "Menu\\PERDISTE.png");
            }
            boxResult = TgcBox.fromSize(centerResult, new Vector3(250, 50, 50), texture);
            boxResult.AutoTransformEnable = false;
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
            sonido = new Sonido(MediaDir, ShadersDir, DirectSound);
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
                    new Vector3(3000, 5, 3000), 2000, 0.15f, 10f, base.Input);

            camaraMenu = new CamaraTerceraPersona(new Vector3(5500, 800, 5500), 900, 3600f);

            this.Camara = camaraRotante;
            //camaraRotante.SetCamera(new Vector3(500, 700, 500),
            //                         new Vector3(3000, 50, 3000));

            //Creamos una caja 3D con textura
            var center = new Vector3(3000, 200, 4500);
            var texture = TgcTexture.createTexture(MediaDir + "Menu\\COMENZAR.png");
            boxComenzar = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxComenzar.AutoTransformEnable = false;

            // Creamos una caja 3D con textura
            center = new Vector3(3000, 140, 4500);
            texture = TgcTexture.createTexture(MediaDir + "Menu\\PERSONAJE.png");
            boxPersonaje = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxPersonaje.AutoTransformEnable = false;

            // Creamos una caja 3D con textura
            center = new Vector3(3000, 80, 4500);
            texture = TgcTexture.createTexture(MediaDir + "Menu\\SALIR.png");
            boxSalir = TgcBox.fromSize(center, new Vector3(250, 50, 50), texture);
            boxSalir.AutoTransformEnable = false;

            boxComenzar.Position = new Vector3(5500, 850, 5600);
            boxComenzar.rotateY(FastMath.PI);
            boxPersonaje.Position = new Vector3(5500, 750, 5600);
            boxPersonaje.rotateY(FastMath.PI);
            boxSalir.Position = new Vector3(5500, 650, 5600);
            boxSalir.rotateY(FastMath.PI);

            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            personaje1 = scene.Meshes[0];
            personaje1.AutoTransformEnable = false;
            personaje1.Position = new Vector3(5400, 800, 5700);

            scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml");
            personaje2 = scene.Meshes[0];
            personaje2.AutoTransformEnable = false;
            personaje2.Position = new Vector3(5200, 800, 5700);

            scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml");
            personaje3 = scene.Meshes[0];
            personaje3.AutoTransformEnable = false;
            personaje3.Position = new Vector3(5000, 800, 5700);

            scene = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Warthog\\Warthog-TgcScene.xml");
            personaje4 = scene.Meshes[0];
            personaje4.AutoTransformEnable = false;
            personaje4.Position = new Vector3(5000, 800, 5700);

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

        public void playSonidoPorMuerte()
        {
            Vector3 vecDisparo = new Vector3(controladorDeVehiculos.getAutoPrincipal().getMesh().Position.X,
                                                        controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Y,
                                                       controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Z);
            sonido.playSound(MediaDir + "MySounds\\Scream1.wav", vecDisparo);
            sonido.startSound();
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
            personaje.NroPersonaje = 1;
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
                    personaje.VelocidadMax = 60;
                    personaje.VelocidadMin = -5;
                    personaje.ConstanteAceleracion = 0.5f;
                    personaje.NroPersonaje = 1;
                    break;
                case 2:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine4.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\Launch3.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 90;
                    personaje.VelocidadMin = -15;
                    personaje.ConstanteAceleracion = 0.7f;
                    personaje.NroPersonaje = 2;
                    break;
                case 3:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine1.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\Special1.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 80;
                    personaje.VelocidadMin = -10;
                    personaje.ConstanteAceleracion = 0.6f;
                    personaje.NroPersonaje = 3;
                    break;
                case 4:
                    personaje.FileMesh = MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Warthog\\Warthog-TgcScene.xml";
                    personaje.FileSonidoMotor = MediaDir + "MySounds\\Engine5.wav";
                    personaje.FileSonidoArma = MediaDir + "MySounds\\risa.wav";
                    personaje.FileSonidoItem = MediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = MediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 80;
                    personaje.VelocidadMin = -15;
                    personaje.ConstanteAceleracion = 0.6f;
                    personaje.NroPersonaje = 4;
                    break;
                default:
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


                if (gameMode == PLAYING)
                {
                    controladorDeVehiculos.update();
                    contSonido += 1;
                    if (contSonido == deltaSonido) sonidos.nextSound();
                    if (contSonido > deltaSonido) sonidos.soundControl();
                    /*if (controladorDeVehiculos.getEnemigo().getLifeLevel().nivelDeVida() == 0 && contadorEnemigoFinal > 0)
                    {
                        contadorEnemigoFinal = contadorEnemigoFinal - 1;
                    }

                    if (contadorEnemigoFinal == 0 && !enemigoFinalCreado)
                    {
                        //Creo enemigoFinal
                        Vector3 vecDisparo = new Vector3(controladorDeVehiculos.getAutoPrincipal().getMesh().Position.X,
                                                         controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Y,
                                                        controladorDeVehiculos.getAutoPrincipal().getMesh().Position.Z);
                        sonido.playSound(MediaDir + "MySounds\\risa.wav", vecDisparo);
                        sonido.startSound();
                        controladorDeVehiculos.crearEnemigoFinal();
                        enemigoFinalCreado = true;
                        manejadorDeColiciones.addBoundingBoxMeshColisionable(controladorDeVehiculos.getEnemigo().getMesh());
                    }*/

                    //enemigoFinalCreado && 
                    if (controladorDeVehiculos.getEnemigo().getLifeLevel().nivelDeVida() == 0 )
                    {
                        terminarJuego(true);
                    }

                }

                if (gameMode == SELECTION) {

                    if (base.Input.keyPressed(Key.Space)) {
                        seleccionaPersonaje(opcionSeleccionada);
                        gameMode = MENU;
                        opcionSeleccionada = 1;
                        rotacion = 0;
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
                                    rotacion = 0;
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
                    rotacion += ROTATION_SPEED;
                    if (gameMode == MENU) {

                        puntoMenu = new Vector3(3000 - 200, 900, 3000 + 3000);
                        puntoPersonaje = new Vector3(3000, -500, 3000 + 3200);
                        puntoPersonajeOculto = new Vector3(3000, -500, 3000 + 3200);
                        switch (opcionSeleccionada)
                        {
                            case 1:
                                mBoxComenzar = Matrix.RotationX(rotacion);
                                mBoxPersonaje = Matrix.Identity;
                                mBoxSalir = Matrix.Identity;
                                break;
                            case 2:
                                mBoxComenzar = Matrix.Identity;
                                mBoxPersonaje = Matrix.RotationX(rotacion);
                                mBoxSalir = Matrix.Identity;
                                break;
                            case 3:
                                mBoxComenzar = Matrix.Identity;
                                mBoxPersonaje = Matrix.Identity;
                                mBoxSalir = Matrix.RotationX(rotacion);
                                break;
                            default:
                                break;
                        }

                    } else //Estoy en modo SELECTION
                    {
                        puntoMenu = new Vector3(3000 - 200, -500, 3000 + 3000);
                        puntoPersonaje = new Vector3(3000, 800, 3000 + 3200);
                        puntoPersonajeOculto = new Vector3(3000, -500, 3000 + 3200);
                        switch (opcionSeleccionada)
                        {
                            case 1:
                                //personaje1.rotateY(ROTATION_SPEED);
                                mPersonaje1 = Matrix.RotationY(rotacion);
                                mPersonaje2 = Matrix.Identity;
                                mPersonaje3 = Matrix.Identity;
                                mPersonaje4 = Matrix.Identity;
                                break;
                            case 2:
                                mPersonaje1 = Matrix.Identity;
                                mPersonaje2 = Matrix.RotationY(rotacion);
                                mPersonaje3 = Matrix.Identity;
                                mPersonaje4 = Matrix.Identity;
                                break;
                            case 3:
                                mPersonaje1 = Matrix.Identity;
                                mPersonaje2 = Matrix.Identity;
                                mPersonaje3 = Matrix.RotationY(rotacion);
                                mPersonaje4 = Matrix.Identity;
                                break;
                            case 4:
                                mPersonaje1 = Matrix.Identity;
                                mPersonaje2 = Matrix.Identity;
                                mPersonaje3 = Matrix.Identity;
                                mPersonaje4 = Matrix.RotationY(rotacion);
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
            Vector3 centro = new Vector3(3000, 5, 3000);
           
            Vector3 nvaPosicion = new Vector3(centro.X + 2500 * (float)System.Math.Cos(ElapsedTime),
                                               800,
                                               centro.Z + 2500 * (float)System.Math.Sin(ElapsedTime));
            Vector3 vecDireccion = new Vector3( centro.X - nvaPosicion.X,
                                                750,
                                                centro.Z - nvaPosicion.Z);
            if (juegoTerminado)
            {
                contadorFinal = contadorFinal - 1;
                rotacion += ROTATION_SPEED;

                centro = new Vector3(3000, 1500, 3000);
                var centroResult = new Vector3(3000, 3300, 3000);
                Matrix mScale = Matrix.Scaling(new Vector3(4f, 4f, 4f));
                Matrix mRot = Matrix.RotationY(FastMath.PI) *
                              Matrix.RotationYawPitchRoll((float)System.Math.Cos(rotacion / 6),
                                                          (float)System.Math.Sin(rotacion / 6),
                                                          (float)System.Math.Tan(rotacion / 6));
                Matrix mRot2 = Matrix.RotationYawPitchRoll((float)System.Math.Sin(rotacion / 6),
                                                          (float)System.Math.Cos(rotacion / 6),
                                                          1f);
                boxGameOver.Transform = mScale * mRot * Matrix.Translation(centro);
                boxResult.Transform = Matrix.Scaling(new Vector3(5f, 5f, 5f)) * mRot2 * Matrix.Translation(centroResult);
                boxGameOver.render();
                boxResult.render();
                Ciudad.Render();
                camaraMenu.rotateY(ElapsedTime / 3);
                nvaPosicion = new Vector3(4500, 8200, 4500);
                camaraRotante.SetCamera(nvaPosicion, new Vector3(3000, 2300, 3000));
                camaraMenu.Target = new Vector3(3000, 2300, 3000);
                this.Camara = camaraMenu;
                if (contadorFinal == 0)
                {
                    EndGame = true;
                }
            } else
            {
                if (TwistedMetal.inicializado)
                {

                    if (gameMode == PLAYING) {
                       
                        controladorDeVehiculos.render();
                        this.cronometro.render(ElapsedTime);
                        if (controladorDeVehiculos.getAutoPrincipal().getLifeLevel().nivelDeVida() <= 0)
                        {
                            terminarJuego(false);
                        }
                       
                    } else {
                        if (gameMode == MENU) {
                            
                            messages.MostrarComandosDeSeleccion();
                            messages.MostrarPosicioMeshPorPantalla(camaraRotante.Position);

                        }
                        if (gameMode == SELECTION) {
                            messages.MostrarComandosDeSeleccion();
                           

                        }
                        nvaPosicion = new Vector3(centro.X + 2500, 800, centro.Z + 2500);
                        //puntoMenu = new Vector3(centro.X - 200, 900, centro.Z + 3000);

                        camaraRotante.SetCamera(nvaPosicion, centro);
                        //camaraMenu.rotateY(ElapsedTime/3);
                        camaraMenu.Target = centro;
                        this.Camara = camaraMenu;

                        Vector3 ptoCamara = this.Camara.Position;
                        Vector3 vectorAlCentro = new Vector3(centro.X - ptoCamara.X, centro.Y - ptoCamara.Y, centro.Z - ptoCamara.Z);

                        vecDireccion = new Vector3(camaraMenu.Position.X * 0.8f * (float)System.Math.Cos(ElapsedTime),
                                                    850,
                                                    camaraMenu.Position.Z * 0.8f * (float)System.Math.Sin(ElapsedTime));

                        Matrix mScale = Matrix.Scaling(new Vector3(1f, 1f, 1f));
                        Matrix mRot = Matrix.RotationY(FastMath.PI);
                        boxComenzar.Transform = mScale * mRot * mBoxComenzar * Matrix.Translation(puntoMenu);
                        boxPersonaje.Transform = mScale * mRot * mBoxPersonaje * Matrix.Translation(new Vector3(puntoMenu.X, 
                                                                                                                puntoMenu.Y - 100, 
                                                                                                                puntoMenu.Z));
                        boxSalir.Transform = mScale * mRot * mBoxSalir * Matrix.Translation(new Vector3(puntoMenu.X, 
                                                                                                        puntoMenu.Y - 200, 
                                                                                                        puntoMenu.Z));

                        personaje1.Transform = mScale * mPersonaje1 * Matrix.Translation(new Vector3(puntoPersonaje.X + 200,
                                                                                                     puntoPersonaje.Y,
                                                                                                     puntoPersonaje.Z));
                        personaje2.Transform = mScale * mPersonaje2 * Matrix.Translation(new Vector3(puntoPersonaje.X,
                                                                                                     puntoPersonaje.Y,
                                                                                                     puntoPersonaje.Z));
                        personaje3.Transform = mScale * mPersonaje3 * Matrix.Translation(new Vector3(puntoPersonaje.X - 200,
                                                                                                     puntoPersonaje.Y,
                                                                                                     puntoPersonaje.Z));
                        personaje4.Transform = mScale * mPersonaje4 * Matrix.Translation(new Vector3(puntoPersonaje.X,
                                                                                                     puntoPersonaje.Y - 200,
                                                                                                     puntoPersonaje.Z));

                    }

                    Ciudad.Render();
                    personaje1.render();
                    personaje2.render();
                    personaje3.render();
                    boxComenzar.render();
                    boxPersonaje.render();
                    boxSalir.render();

                }
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
