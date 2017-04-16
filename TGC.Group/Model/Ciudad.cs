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
using TGC.Core.Camara;
using System.Collections.Generic;
using TGC.Examples.Camara;
using TGC.Core.Terrain;
using TGC.Core.UserControls;
using TGC.Core.UserControls.Modifier;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class Ciudad 
    {
        private string MediaDir;
        private TgcPlane suelo;
        private TgcMesh edificio;
        private List<TgcPlane> veredas;
        private List<TgcPlane> cordones;
        private List<TgcPlane> paredes;
        private List<TgcPlane> calles;
        private List<TgcMesh> meshes;
        private TgcSceneLoader loader;
        private TgcSkyBox skyBox;

        TgcD3dInput Input;
        private TgcTexture manzanaTexture;
        private TgcTexture cordonTexture;
        private TgcTexture veredaTexture;
        private TgcTexture paredTexture;
        private TgcTexture calleTexture;
        private TgcTexture intersectionTexture;
        private Core.Example.TgcExample env;
        List<int> ListaRandom = new List<int>(7);
       
        private int CameraX, CameraY, CameraZ;
       
        public Ciudad(Core.Example.TgcExample env)
        {
            //Device de DirectX para crear primitivas.
            this.env = env;
            var d3dDevice = D3DDevice.Instance.Device;
            this.MediaDir = this.env.MediaDir;
            this.Input = this.env.Input;
           
            CameraX = -890;
            CameraY = 460;
            CameraZ = 110;
            //Carga Texturas
            manzanaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Scenes\\Ciudad\\Textures\\Floor.jpg");
            cordonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\granito.jpg");
            veredaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\piso2.jpg");
            paredTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Textures\\Ladrillo\\brick1_2.jpg");
            calleTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\f1\\f1piso2.png");
            intersectionTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Scenes\\Ciudad\\Textures\\Road Union.jpg");

            //Crea el piso de fondo

            loader = new TgcSceneLoader();
            crearPisoDeFondo();
            crearCamara(env);
            meshes = new System.Collections.Generic.List<TgcMesh>();
            veredas = new System.Collections.Generic.List<TgcPlane>();
            cordones = new System.Collections.Generic.List<TgcPlane>();
            paredes = new System.Collections.Generic.List<TgcPlane>();
            calles = new System.Collections.Generic.List<TgcPlane>();
            crearEdificios();
            crearVeredas();
            crearParedes();
            crearAuto();
            crearSemaforos();
            crearPlantas();
            

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(1000, 0, 1000);
            skyBox.Size = new Vector3(20000, 20000, 20000);


            var texturesPath = MediaDir + "Texturas\\Quake\\SkyBox3\\";

            //Configurar las texturas para cada una de las 6 caras
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "Up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "Down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "Left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "Right.jpg");

            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "Back.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "Front.jpg");
            skyBox.SkyEpsilon = 40f;
            //Inicializa todos los valores para crear el SkyBox
            skyBox.Init();
        }


        private void disposeListas()
        {
            //Renderizar instancias
            foreach (var mesh in meshes)
            {
                mesh.dispose();
            }

            //Renderizado de cordones
            foreach (var cordon in cordones)
            {
                cordon.dispose();
            }

            //Renderizado de veredas
            foreach (var v in veredas)
            {
                v.dispose();
            }

            //Renderizado de paredes
            foreach (var p in paredes)
            {
                p.dispose();
            }

            //Renderizado de paredes
            foreach (var c in calles)
            {
                c.dispose();
            }
        }

        private void crearPisoDeFondo()
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\f1\\calles.jpg");
            //var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Scenes\\Ciudad\\Textures\\Road.jpg"); 

            suelo = new TgcPlane(new Vector3(-500, 0, -500), new Vector3(6000, 0, 6000), TgcPlane.Orientations.XZplane, pisoTexture, 10f, 10f);

        }

        private void crearCamara(Core.Example.TgcExample env)
        {
            //Suelen utilizarse objetos que manejan el comportamiento de la camara.
            //Lo que en realidad necesitamos gr�ficamente es una matriz de View.
            //El framework maneja una c�mara est�tica, pero debe ser inicializada.
            //Posici�n de la camara.
            var cameraPosition = new Vector3(CameraX, CameraY, CameraZ);
            //Quiero que la camara mire hacia el origen (0,0,0).
            var lookAt = suelo.BoundingBox.calculateBoxCenter();

            //Configuro donde esta la posicion de la camara y hacia donde mira.
            //Camara.SetCamera(cameraPosition, lookAt);
            //Camara en 1ra persona
            env.Camara = new TgcFpsCamera(new Vector3(300, 600, -600), Input);

        }
        private void crearEdificios()
        {
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una c�mara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
            var scene =
                loader.loadSceneFromFile(MediaDir + "MeshCreator\\Scenes\\Ciudad\\Ciudad-TgcScene.xml");

            //Crear varias instancias del modelo original, pero sin volver a cargar el modelo entero cada vez
            var rows = 6;
            var cols = 6;
            generarGrilla(rows, cols, scene);

        }

        private void generarGrilla(int rows, int cols, TgcScene scene)
        {
            int modMesh = 0;
            for (var i = 1; i <= rows; i++)
            {
                for (var j = 1; j <= cols; j++)
                {
                    if ((((j * 2) + 1) % 2 != 0) && (((i * 2) + 1) % 2 != 0))
                        if (dibujarEdificio(getNMesh(modMesh), i, j, scene))
                        {
                            modMesh++;
                        }
                }
            }

        }
        private int getNMesh(int modMesh)
        {
            //  0,1,2,3,4,5,6,7

            System.Random generator = new System.Random();
            int n = generator.Next(0, 8);

            if (ListaRandom.Count == 7)
                ListaRandom.Clear();

            while (ListaRandom.Contains(n))
                n = generator.Next(0, 8);

            ListaRandom.Add(n);

            if (n == 1)
                return 0;

            if (n >= 0 && n <= 7)
                return n;


            return 4;
        }
        private bool dibujarEdificio(int nMesh, int i, int j, TgcScene scene)
        {

            float offset_row = 300;
            float offset_Col = 100;
            float offset_Y = 5;
            if (nMesh == 0)
            { //Edificio Blanco Espejado - chiquito
                offset_row = 220 + ((i - 1) * 900);
                offset_Col = 10 + ((j - 1) * 900);
            }
            if (nMesh == 2)
            {//Edicio Ladrillos
                offset_row = -280 + ((i - 1) * 900);
                offset_Col = 480 + ((j - 1) * 900);
            }
            if (nMesh == 3)
            { //edifcio amarillo
                offset_row = 1050 + ((i - 1) * 900);
                offset_Col = 900 + ((j - 1) * 900);
            }

            if (nMesh == 4)
            { //Edificio espejado - gris mediano
                offset_row = 300 + ((i - 1) * 900);
                offset_Col = 1000 + ((j - 1) * 900);
                offset_Y = -65;
            }
            if (nMesh == 5)
            { //Edificio alto blanco finito espejado
                offset_row = 1020 + ((i - 1) * 900);
                offset_Col = -400 + ((j - 1) * 900);
            }
            if (nMesh == 6)
            { //Edificio gris U
                offset_row = -130 + ((i - 1) * 900);
                offset_Col = -380 + ((j - 1) * 900);
            }
            if (nMesh == 7)
            { //Edificio alto blanco finito espejado - Rascacielos blanco
                offset_row = 1065 + ((i - 1) * 900);
                offset_Col = 200 + ((j - 1) * 900);
            }
            edificio = scene.Meshes[nMesh];
            var instance = edificio.createMeshInstance(edificio.Name + i + "_" + j);
            //No recomendamos utilizar AutoTransform, en juegos complejos se pierde el control. mejor utilizar Transformaciones con matrices.
            instance.AutoTransformEnable = true;
            //Desplazarlo
            // instance.move(i * offset_row, 0, j * offset_Col);
            instance.move(offset_row, offset_Y, offset_Col);
            if (nMesh == 0)
                instance.Scale = new Vector3(0.70f, 1f, 1f);

            if (nMesh == 4)
                instance.Scale = new Vector3(0.40f, 1f, 1f);

            meshes.Add(instance);

            var posicionX = instance.BoundingBox.calculateBoxCenter().X - (550 / 2);
            var posicionZ = instance.BoundingBox.calculateBoxCenter().Z - (550 / 2);
            var posicion = new Vector3(posicionX, 5, posicionZ);
            veredas.Add(new TgcPlane(posicion, new Vector3(550, 0, 550), TgcPlane.Orientations.XZplane, veredaTexture, 60, 60));
            cordones.Add(new TgcPlane(new Vector3(posicion.X, 0, posicion.Z), new Vector3(550, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(posicion.X + 550, 0, posicion.Z), new Vector3(0, 5, 550), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(posicion.X, 0, posicion.Z + 550), new Vector3(550, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(posicion.X, 0, posicion.Z), new Vector3(0, 5, 550), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));

            return true;
        }

        private void crearVeredas()
        {

            cordones.Add(new TgcPlane(new Vector3(-450, 5, -450), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450, 0, -445), new Vector3(5895, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-500, 5, -500), new Vector3(6000, 0, 50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));

            cordones.Add(new TgcPlane(new Vector3(-450, 5, -445), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(-445, 0, -445), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(-500, 5, -450), new Vector3(50, 0, 5950), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));

            cordones.Add(new TgcPlane(new Vector3(-450, 5, 5445), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450, 0, 5445), new Vector3(5900, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450, 5, 5500), new Vector3(5950, 0, -50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));

            cordones.Add(new TgcPlane(new Vector3(5445, 5, -445), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(5445, 0, -445), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(5450, 5, -450), new Vector3(50, 0, 5900), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));

        }

        private void crearParedes()
        {

            paredes.Add(new TgcPlane(new Vector3(-500, 0, -500), new Vector3(0, 100, 6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(-500, 0, -500), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(5500, 0, 5500), new Vector3(0, 100, -6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(-500, 0, 5500), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));

        }

        private TgcMesh auto;
        private TgcMesh camion;
        private TgcMesh hummer;
        private TgcMesh buggy;
        private TgcMesh patrullero;

        private void crearAuto()
        {
            var sceneHummer = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            hummer = sceneHummer.Meshes[0];
            hummer.AutoTransformEnable = true;
            hummer.move(0, 5, 0);
          //  meshes.Add(hummer);

            var sceneCamion = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\CamionCarga\\CamionCarga-TgcScene.xml");
            camion = sceneCamion.Meshes[0];
            camion.AutoTransformEnable = true;
            camion.move(((suelo.Size.X) - 600), 5, 0);
           // meshes.Add(camion);

            var sceneAuto = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Auto\\Auto-TgcScene.xml");
            auto = sceneAuto.Meshes[0];
            auto.AutoTransformEnable = true;
            auto.move(((suelo.Size.X) - 2000), 5, (suelo.Size.Z) - 800);
            auto.Scale = new Vector3(1, 1, 1);
            auto.rotateY(FastMath.PI_HALF);
           // meshes.Add(auto);

            var sceneBuggy = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Buggy\\Buggy-TgcScene.xml");
            buggy = sceneBuggy.Meshes[0];
            buggy.AutoTransformEnable = true;
            buggy.move(((suelo.Size.X) - 4000), 5, (suelo.Size.Z) - 1600);
            buggy.rotateY(FastMath.PI_HALF);
            buggy.Scale = new Vector3(2, 2, 2);
            //meshes.Add(buggy);

            var scenePatrullero = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Patrullero\\Patrullero-TgcScene.xml");
            patrullero = scenePatrullero.Meshes[0];
            patrullero.AutoTransformEnable = true;
            patrullero.move(1000, 5, (suelo.Size.Z) - 2200);
            patrullero.rotateY(FastMath.PI);
            patrullero.Scale = new Vector3(1, 1, 1);
            //meshes.Add(patrullero);

        }


        private TgcMesh semaforo;
        private void crearSemaforos()
        {

            var sceneSemaforo = loader.loadSceneFromFile(MediaDir + "ModelosTgc\\Semaforo\\Semaforo-TgcScene.xml");

            semaforo = sceneSemaforo.Meshes[0];

            for (int i = 0; i < veredas.Count; i++)
            {

                var instanciaIda = semaforo.createMeshInstance(semaforo.Name + i);
                instanciaIda.AutoTransformEnable = true;
                var posicionX = (veredas[i].Position.X) + (veredas[i].Size.X) - 20;
                var posicionY = 40;
                var posicionZ = veredas[i].Position.Z + 20;
                instanciaIda.move(posicionX, posicionY, posicionZ);
                meshes.Add(instanciaIda);

                var instanciaVuelta = semaforo.createMeshInstance(semaforo.Name + i);
                instanciaVuelta.AutoTransformEnable = true;
                var posicionX2 = (veredas[i].Position.X) + 20;
                var posicionY2 = 40;
                var posicionZ2 = veredas[i].Position.Z + (veredas[i].Size.Z) - 20;
                instanciaVuelta.move(posicionX2, posicionY2, posicionZ2);
                instanciaVuelta.rotateY(FastMath.PI);
                meshes.Add(instanciaVuelta);


            }
        }

        private TgcMesh Planta;

        private enum Plantas { Pino = 2, Palmera = 1, Nada = 4, Arbol = 3 };
        private void crearUnaPlanta(TgcScene unaScene, int i, Vector3 vectorPosicion, int n)
        {
            Planta = unaScene.Meshes[0];
            var instancia = Planta.createMeshInstance(Planta.Name + i);
            instancia.AutoTransformEnable = true;
            instancia.AlphaBlendEnable = true;
            instancia.move(vectorPosicion.X, vectorPosicion.Y, vectorPosicion.Z);
            if (n == 3)
            {
                instancia.Scale = new Vector3((float)0.5, (float)0.5, (float)0.5);
            }
            meshes.Add(instancia);

        }
        private Vector3 vectorFrontalMitadVereda(int i)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X) / 2);
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + 20;
            return new Vector3(posicionX, posicionY, posicionZ);

        }
        private Vector3 vectorTraseroMitadVereda(int i)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X) / 2);
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z)) - 20;
            return new Vector3(posicionX, posicionY, posicionZ);

        }
        private Vector3 vectorLateralDerechoMitadVereda(int i)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X)) - 20;
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z) / 2);
            return new Vector3(posicionX, posicionY, posicionZ);

        }
        private Vector3 vectorLateralIzquierdoMitadVereda(int i)
        {
            var posicionX = (veredas[i].Position.X) + 20;
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z) / 2) - 20;
            return new Vector3(posicionX, posicionY, posicionZ);

        }
        private void dibujarPlantaRandom(int j, Vector3 vectorPosicion, TgcScene scene, int i, int n)
        {
            switch (j)
            {
                case 1:
                    vectorPosicion = vectorFrontalMitadVereda(i);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 2:
                    vectorPosicion = vectorTraseroMitadVereda(i);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 3:
                    vectorPosicion = vectorLateralIzquierdoMitadVereda(i);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 4:
                    vectorPosicion = vectorLateralDerechoMitadVereda(i);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
            }

        }
        private void crearPlantas()
        {
            for (int i = 0; i < veredas.Count; i++)
            {
                var vectorPosicion = new Vector3(0, 0, 0);
                System.Random generator = new System.Random();
                for (int k = 1; k <= 4; k++)
                {
                    int n = generator.Next(1, 4);
                    switch (n)
                    {

                        case (int)Plantas.Nada:
                            break;
                        case (int)Plantas.Pino:
                            var scenePlanta = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vegetacion\\Pino\\Pino-TgcScene.xml");
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta, i, n);
                            break;
                        case (int)Plantas.Palmera:
                            var scenePlanta2 = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vegetacion\\Palmera3\\Palmera3-TgcScene.xml");
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta2, i, n);
                            break;
                        case (int)Plantas.Arbol:
                            var scenePlanta3 = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vegetacion\\ArbolBosque\\ArbolBosque-TgcScene.xml");
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta3, i, n);
                            break;
                    }


                }


            }
        }

        public void Update()
        {
            //Se actualiza la posicion del skybox.
            skyBox.Center = this.env.Camara.Position;

        }
        public void Render()
        {
            //Renderizar suelo
            suelo.render();
            //calle.render();


            //Renderizar SkyBox
            skyBox.render();

            //Renderizar instancias
            foreach (var mesh in meshes)
            {
                mesh.render();
            }

            //Renderizado de cordones
            foreach (var cordon in cordones)
            {
                cordon.render();
            }

            //Renderizado de veredas
            foreach (var v in veredas)
            {
                v.render();
            }

            //Renderizado de paredes
            foreach (var p in paredes)
            {
                p.render();
            }

            //Renderizado de calles
            foreach (var c in calles)
            {
                c.render();
            }
        }
        public void dispose()
        {

            suelo.dispose();
            //calle.dispose();
            //Al hacer dispose del original, se hace dispose automaticamente de todas las instancias
            edificio.dispose();
            auto.dispose();
            hummer.dispose();
            camion.dispose();
            buggy.dispose();
            patrullero.dispose();
            //disposeListas();

            //Liberar recursos del SkyBox
            skyBox.dispose();


        }
    }
}