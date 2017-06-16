using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Model;

namespace TGC.Group.Form
{
    /// <summary>
    ///     GameForm es el formulario de entrada, el mismo invocara a nuestro modelo  que extiende TgcExample, e inicia el
    ///     render loop.
    /// </summary>
    public partial class GameForm : System.Windows.Forms.Form
    {

        public Musica sonidos;
        /// <summary>
        ///     Constructor de la ventana.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Ejemplo del juego a correr
        /// </summary>
        private TgcExample Modelo { get; set; }

        /// <summary>
        ///     Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        ///     Permite manejar el sonido.
        /// </summary>
        private TgcDirectSound DirectSound { get; set; }

        /// <summary>
        ///     Permite manejar los inputs de la computadora.
        /// </summary>
        private TgcD3dInput Input { get; set; }

        private void GameForm_Load(object sender, EventArgs e)
        {

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            int posicionX = this.Size.Width - 40 - 232;
            int laMitad = this.Size.Height / 2;
            int posicionY = laMitad - 177;

            BtnComenzar.Location = new System.Drawing.Point(posicionX, posicionY);
            posicionY = posicionY + 142;
            BtnPersonaje.Location = new System.Drawing.Point(posicionX, posicionY);
            posicionY = posicionY + 142;
            BtnSalir.Location = new System.Drawing.Point(posicionX, posicionY);

            
            int laMitadEnX = this.Size.Width / 2;
            int laMitadEnY = this.Size.Height / 2;
            posicionX = laMitadEnX - 425;
            posicionY = laMitadEnY - 125;

            picture1.Visible = false;
            picture1.Location = new System.Drawing.Point(posicionX, posicionY);
            posicionX = posicionX + 300;

            picture2.Visible = false;
            picture2.Location = new System.Drawing.Point(posicionX, posicionY);
            posicionX = posicionX + 300;

            picture3.Visible = false;
            picture3.Location = new System.Drawing.Point(posicionX, posicionY);

            sonidos = new Musica(currentDirectory + Game.Default.MediaDirectory);
            sonidos.menuSound();
            sonidos.startSound();

           
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        /// <summary>
        ///     Inicio todos los objetos necesarios para cargar el ejemplo y directx.
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicación
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);
            
            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            //Juego a ejecutar, si quisiéramos tener diferentes modelos aquí podemos cambiar la instancia e invocar a otra clase.
            Modelo = new TwistedMetal(currentDirectory + Game.Default.MediaDirectory,
                currentDirectory + Game.Default.ShadersDirectory);

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);

            //Cargar juego.
            ExecuteModel();

        }

        /// <summary>
        ///     Comienzo el loop del juego.
        /// </summary>
        public void InitRenderLoop()
        {
            while (ApplicationRunning)
            {
                //Renderizo si es que hay un ejemplo activo.
                if (Modelo != null)
                {
                    //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios.
                    if (ApplicationActive())
                    {
                        Modelo.Update();
                        Modelo.Render();
                    }
                    else
                    {
                        //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU.
                        Thread.Sleep(100);
                    }
                }
                // Process application messages.
                Application.DoEvents();
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Modelo.ResetDefaultConfig();
                Modelo.DirectSound = DirectSound;
                Modelo.Input = Input;
                Modelo.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Modelo != null)
            {
                Modelo.Dispose();
                Modelo = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            StopCurrentExample();

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }

        private void BtnComenzar_Click(object sender, EventArgs e)
        {
            if (Personaje.getInstance() == null) seleccionPorDefault();
            BtnComenzar.Visible = false;
            BtnPersonaje.Visible = false;
            BtnSalir.Visible = false;

            //pictureEspera.Visible = true;

            //Iniciar graficos.
            InitGraphics();

            //Titulo de la ventana principal.
            Text = Modelo.Name + " - " + Modelo.Description;

            //Focus panel3D.
            panel3D.Focus();

            //Inicio el ciclo de Render.
            InitRenderLoop();

            
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnComenzar_MouseHover(object sender, EventArgs e)
        {
            BtnComenzar.BackColor = System.Drawing.Color.LawnGreen;
            BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnComenzar_MouseLeave(object sender, EventArgs e)
        {
            BtnComenzar.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnComenzar_Enter(object sender, EventArgs e)
        {
            BtnComenzar.BackColor = System.Drawing.Color.LawnGreen;
            BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnComenzar_Leave(object sender, EventArgs e)
        {
            BtnComenzar.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnPersonaje_MouseHover(object sender, EventArgs e)
        {
            BtnPersonaje.BackColor = System.Drawing.Color.LawnGreen;
            BtnComenzar.BackColor = System.Drawing.Color.PaleGreen;
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnPersonaje_MouseLeave(object sender, EventArgs e)
        {
            BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnPersonaje_Enter(object sender, EventArgs e)
        {
            BtnPersonaje.BackColor = System.Drawing.Color.LawnGreen;
            BtnComenzar.BackColor = System.Drawing.Color.PaleGreen;
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnPersonaje_Leave(object sender, EventArgs e)
        {
            BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnSalir_MouseHover(object sender, EventArgs e)
        {
            BtnSalir.BackColor = System.Drawing.Color.LawnGreen;
            BtnComenzar.BackColor = System.Drawing.Color.PaleGreen;
            BtnPersonaje.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnSalir_MouseLeave(object sender, EventArgs e)
        {
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void BtnSalir_Enter(object sender, EventArgs e)
        {
            BtnSalir.BackColor = System.Drawing.Color.LawnGreen;
        }

        private void BtnSalir_Leave(object sender, EventArgs e)
        {
            BtnSalir.BackColor = System.Drawing.Color.PaleGreen;
        }

        private void seleccionPorDefault()
        {
            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            string mediaDir = currentDirectory + Game.Default.MediaDirectory;
            Personaje personaje = new Personaje();
            personaje.FileSonido = mediaDir + "MySounds\\MachineGun.wav";
            personaje.FileSonidoColision = mediaDir + "MySounds\\Crash4.wav";
            personaje.FileMesh = mediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml";
            personaje.FileSonidoMotor = mediaDir + "MySounds\\Engine2.wav";
            personaje.FileSonidoArma = mediaDir + "MySounds\\Special5.wav";
            personaje.FileSonidoItem = mediaDir + "MySounds\\PickUp2.wav";
            personaje.FileSonidoSalto = mediaDir + "MySounds\\portazo.wav";
            personaje.VelocidadMax = 70;
            personaje.VelocidadMin = -5;
            personaje.ConstanteAceleracion = 0.6f;
        }

        private void seleccionaPersonaje(int indice)
        {
            picture1.Visible = false;
            picture2.Visible = false;
            picture3.Visible = false;
            BtnComenzar.Visible = true;
            BtnPersonaje.Visible = true;
            BtnSalir.Visible = true;

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            string mediaDir = currentDirectory + Game.Default.MediaDirectory;

            Personaje personaje = new Personaje();
            personaje.FileSonido = mediaDir + "MySounds\\MachineGun.wav";
            personaje.FileSonidoColision = mediaDir + "MySounds\\Crash4.wav";
            switch (indice)
            {
                case 1:
                    personaje.FileMesh = mediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml";
                    personaje.FileSonidoMotor =  mediaDir + "MySounds\\Engine2.wav";
                    personaje.FileSonidoArma = mediaDir + "MySounds\\Special5.wav";
                    personaje.FileSonidoItem = mediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = mediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 70;
                    personaje.VelocidadMin = -5;
                    personaje.ConstanteAceleracion = 0.6f;
                    break;
                case 2:
                    personaje.FileMesh = mediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml";
                    personaje.FileSonidoMotor = mediaDir + "MySounds\\Engine4.wav";
                    personaje.FileSonidoArma = mediaDir + "MySounds\\Launch3.wav";
                    personaje.FileSonidoItem = mediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = mediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 90;
                    personaje.VelocidadMin = -15;
                    personaje.ConstanteAceleracion = 0.8f;
                    break;
                default:
                    personaje.FileMesh = mediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml";
                    personaje.FileSonidoMotor = mediaDir + "MySounds\\Engine1.wav";
                    personaje.FileSonidoArma = mediaDir + "MySounds\\Special1.wav";
                    personaje.FileSonidoItem = mediaDir + "MySounds\\PickUp2.wav";
                    personaje.FileSonidoSalto = mediaDir + "MySounds\\portazo.wav";
                    personaje.VelocidadMax = 80;
                    personaje.VelocidadMin = -10;
                    personaje.ConstanteAceleracion = 0.7f;
                    break;
            }
        }

        private void picture1_Click(object sender, EventArgs e)
        {
            seleccionaPersonaje(1);
        }

        private void picture2_Click(object sender, EventArgs e)
        {
            seleccionaPersonaje(2);
        }

        private void picture3_Click(object sender, EventArgs e)
        {
            seleccionaPersonaje(3);
        }

        private void BtnPersonaje_Click(object sender, EventArgs e)
        {
            BtnComenzar.Visible = false;
            BtnPersonaje.Visible = false;
            BtnSalir.Visible = false;
            picture1.Visible = true;
            picture2.Visible = true;
            picture3.Visible = true;
        }
    }
}