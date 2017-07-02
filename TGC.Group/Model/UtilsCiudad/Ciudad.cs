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
using TGC.Core.BoundingVolumes;
namespace TGC.Group.Model
{
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar el modelo que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class Ciudad
    {
        private string MediaDir;
       
        private List<TgcPlane> veredas;
        private List<TgcPlane> cordones;
        private List<TgcPlane> paredes;
        private List<TgcPlane> calles;
        private List<TgcMesh> edificios;
        private List<TgcMesh> semaforos;
        private List<TgcMesh> arboles;
        private List<TgcMesh> LpostesDeLuz;
        private List<TgcMesh> meshes;
        private List<TgcMesh> items;

        private int[] itemsTiempoInvisibilidad;

        private TgcSceneLoader loader;
        private TgcSkyBox skyBox;
       
        private TwistedMetal env;
        List<int> ListaRandom = new List<int>(7);
        private TgcMesh semaforo;
        private TgcMesh posteDeLuz;
        private TgcPlane suelo;
        private TgcMesh edificio;
        private TgcTexture cordonTexture;
        private TgcTexture veredaTexture;
        private TgcTexture paredTexture;

        //float fix_pos
        float fix_posZ = 500;
        float fix_posX = 500;

        //Constantes para velocidades de movimiento
        private const float ROTATION_SPEED = 0.5f;
        private const float MOVEMENT_SPEED = 5f;
        private int TiempoRetardo = 3;
        private int contadorDeCiclos = 0;
        //Variable direccion de movimiento
        private float currentMoveDir = 1f;
        //  colicion
        /*  private TgcArrow collisionNormalArrow;
          private TgcBox collisionPoint;
          private TgcArrow directionArrow;
          private TgcBoundingSphere characterSphere;
          private SphereTriangleCollisionManager collisionManager;

          //Autos
          private List<Vehiculo> vehiculos;*/
        private static Ciudad myInstance;

        public static Ciudad getInstance()
        {
            return myInstance;
        }

        public Ciudad(TwistedMetal env)
        {
            //Device de DirectX para crear primitivas.
            this.env = env;
            myInstance = this;
            var d3dDevice = D3DDevice.Instance.Device;
            this.MediaDir = this.env.MediaDir;
         
            //Carga Texturas
            veredaTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\piso2.jpg");
            cordonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\granito.jpg");
            //Crea el piso de fondo

            loader = new TgcSceneLoader();
            
            meshes = new System.Collections.Generic.List<TgcMesh>();
            edificios = new System.Collections.Generic.List<TgcMesh>();
            semaforos = new System.Collections.Generic.List<TgcMesh>();
            veredas = new System.Collections.Generic.List<TgcPlane>();
            cordones = new System.Collections.Generic.List<TgcPlane>();
            paredes = new System.Collections.Generic.List<TgcPlane>();
            calles = new System.Collections.Generic.List<TgcPlane>();
            arboles = new System.Collections.Generic.List<TgcMesh>();
            items = new System.Collections.Generic.List<TgcMesh>();
            LpostesDeLuz = new System.Collections.Generic.List<TgcMesh>();

            crearPisoDeFondo();
            crearEdificios();
            meshes.AddRange(edificios);
            crearVeredas();
            crearParedes();
         //   crearAuto();
            crearSemaforos();
            meshes.AddRange(semaforos);
            crearPlantas(1);
            crearPlantas(70);
        //    crearRueda();
            iniciarCielo();
            crearItems();
         //   iniciarColisionador();


        }
        public List<TgcMesh> getAllMeshes()
        {
            return this.meshes;
        }


        private void crearPisoDeFondo()
        {
            var pisoTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Texturas\\f1\\calles.jpg");
           
            suelo = new TgcPlane(new Vector3(0, 0, 0), new Vector3(6000, 0, 6000), TgcPlane.Orientations.XZplane, pisoTexture, 10f, 10f);

        }
        //private void crearRueda()
        //{
        //    var texturaRueda = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "rueda.jpg");
//
 //           rueda = new TgcCylinder(new Vector3(0,10,0),100,5);
  //          rueda.UseTexture = true;
   //         rueda.Scale=new Vector3(400,400,400);
    //        rueda.move(-100,0,0);
            

      //  }

        private void crearEdificios()
        {
            //Internamente el framework construye la matriz de view con estos dos vectores.
            //Luego en nuestro juego tendremos que crear una cámara que cambie la matriz de view con variables como movimientos o animaciones de escenas.
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
            instance.AutoTransformEnable = true; //AS
                                                  //Desplazarlo
                                                  //  Vector3 scale3 = new Vector3(1f, 1f, 1f);
                                                  // var m = Matrix.Scaling(scale3) * Matrix.RotationY(0.001f) * Matrix.Translation(new Vector3(100, 5, 3000));

            //  instance.Transform = m;
            // instance.Position = new Vector3(100, 5, 3000);

            //Matrix m=new Matrix();
            //Vector3 pos = new Vector3(offset_row + fix_posX, offset_Y, offset_Col + fix_posZ);
            instance.move(offset_row+ fix_posX, offset_Y, offset_Col+ fix_posZ);
            
            //m = Matrix.Scaling(new Vector3(1f, 1f, 1f)) * Matrix.RotationY(0.001f) * Matrix.Translation(pos);
            if (nMesh == 0)
            {
                  instance.Scale = new Vector3(0.70f, 1f, 1f);  
                //m = Matrix.Scaling(new Vector3(0.70f, 1f, 1f)) * Matrix.RotationY(0.0001f) * Matrix.Translation(pos);
            }
            if (nMesh == 4)
            {
                 instance.Scale = new Vector3(0.40f, 1f, 1f); 
              //  m = Matrix.Scaling(new Vector3(0.40f, 1f, 1f)) * Matrix.RotationY(0.001f) * Matrix.Translation(pos);
            }
            //  instance.Transform = m;
            // instance.Position = pos;
           
            edificios.Add(instance);
            
            var posicionX = instance.BoundingBox.calculateBoxCenter().X - (550 / 2);
            var posicionZ = instance.BoundingBox.calculateBoxCenter().Z - (550 / 2);
            var posicion = new Vector3(posicionX, 5, posicionZ);
            veredas.Add(new TgcPlane(posicion, new Vector3(550, 0, 550), TgcPlane.Orientations.XZplane, veredaTexture, 60, 60));
            cordones.Add(new TgcPlane(new Vector3(posicion.X, 0, posicion.Z), new Vector3(550, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(posicion.X + 550, 0, posicion.Z ), new Vector3(0, 5, 550), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(posicion.X , 0, posicion.Z + 550), new Vector3(550, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(posicion.X, 0, posicion.Z ), new Vector3(0, 5, 550), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));

            return true;
        }

        private void crearVeredas()
        {
            cordones.Add(new TgcPlane(new Vector3(-450 + fix_posX, 5, -450 + fix_posZ), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450 + fix_posX, 0, -445 + fix_posZ), new Vector3(5895, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-500 + fix_posX, 5, -500 + fix_posZ), new Vector3(6000, 0, 50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));
            for(int i = 0; i < 30; i++)
            {
                var sceneRueda = loader.loadSceneFromFile(MediaDir + "PosteDeLuz\\Poste de luz-TgcScene.xml");
                posteDeLuz = sceneRueda.Meshes[0];
                posteDeLuz.AutoTransformEnable = true;
                posteDeLuz.Position = new Vector3(-455 + (i * (5900 / 30))+ fix_posX, 5, -450+ fix_posZ);
                posteDeLuz.rotateY(FastMath.PI_HALF);
                posteDeLuz.Scale = new Vector3(1, 3, 1);
                posteDeLuz.move(0, 50, 0);
                posteDeLuz.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(new Vector3(posteDeLuz.Position.X, posteDeLuz.Position.Y, posteDeLuz.Position.Z-33)));
                meshes.Add(posteDeLuz);
                LpostesDeLuz.Add(posteDeLuz);
            }
            cordones.Add(new TgcPlane(new Vector3(-450 + fix_posX, 5, -445 + fix_posZ), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(-445 + fix_posX, 0, -445 + fix_posZ), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(-500 + fix_posX, 5, -450 + fix_posZ), new Vector3(50, 0, 5950), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));
            for (int i = 0; i < 30; i++)
            {
                var sceneRueda = loader.loadSceneFromFile(MediaDir + "PosteDeLuz\\Poste de luz-TgcScene.xml");
                posteDeLuz = sceneRueda.Meshes[0];
                posteDeLuz.AutoTransformEnable = true;
                posteDeLuz.Position = new Vector3(-455 + fix_posX, 5, -450 + (i * (5900 / 30)) + fix_posZ);
                posteDeLuz.rotateY(FastMath.PI);
                posteDeLuz.Scale = new Vector3(1, 3, 1);
                posteDeLuz.move(0, 50, 0);
                posteDeLuz.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(new Vector3(posteDeLuz.Position.X-33, posteDeLuz.Position.Y, posteDeLuz.Position.Z )));
                meshes.Add(posteDeLuz);
                LpostesDeLuz.Add(posteDeLuz);
            }
            cordones.Add(new TgcPlane(new Vector3(-450+ fix_posX, 5, 5445 + fix_posZ), new Vector3(5900, 0, 5), TgcPlane.Orientations.XZplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450 + fix_posX, 0, 5445 + fix_posZ), new Vector3(5900, 5, 0), TgcPlane.Orientations.XYplane, cordonTexture, 40, 1));
            cordones.Add(new TgcPlane(new Vector3(-450 + fix_posX, 5, 5500 + fix_posZ), new Vector3(5950, 0, -50), TgcPlane.Orientations.XZplane, veredaTexture, 60, 1));
            for (int i = 0; i < 30; i++)
            {
                var sceneRueda = loader.loadSceneFromFile(MediaDir + "PosteDeLuz\\Poste de luz-TgcScene.xml");
                posteDeLuz = sceneRueda.Meshes[0];
                posteDeLuz.AutoTransformEnable = true;
                posteDeLuz.Position = new Vector3(-455 + (i * (5900 / 30)) + fix_posX, 5, 5445 + fix_posZ);
                posteDeLuz.rotateY(FastMath.PI + FastMath.PI_HALF);
                posteDeLuz.Scale = new Vector3(1, 3, 1);
                posteDeLuz.move(0, 50, 0);
                posteDeLuz.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(new Vector3(posteDeLuz.Position.X , posteDeLuz.Position.Y, posteDeLuz.Position.Z+33)));
                meshes.Add(posteDeLuz);
                LpostesDeLuz.Add(posteDeLuz);
            }
            cordones.Add(new TgcPlane(new Vector3(5445 + fix_posX, 5, -445 + fix_posZ), new Vector3(5, 0, 5890), TgcPlane.Orientations.XZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(5445 + fix_posX, 0, -445 + fix_posZ), new Vector3(0, 5, 5890), TgcPlane.Orientations.YZplane, cordonTexture, 1, 40));
            cordones.Add(new TgcPlane(new Vector3(5450 + fix_posX, 5, -450 + fix_posZ), new Vector3(50, 0, 5900), TgcPlane.Orientations.XZplane, veredaTexture, 1, 60));
            for (int i = 0; i < 30; i++)
            {
                var sceneRueda = loader.loadSceneFromFile(MediaDir + "PosteDeLuz\\Poste de luz-TgcScene.xml");
                posteDeLuz = sceneRueda.Meshes[0];
                posteDeLuz.AutoTransformEnable = true;
                posteDeLuz.Position = new Vector3(5445 + fix_posX, 5, -450 + (i * (5900 / 30)) + fix_posZ);
                posteDeLuz.rotateY(FastMath.TWO_PI);
                posteDeLuz.Scale = new Vector3(1, 3, 1);
                posteDeLuz.move(0, 50, 0);
                posteDeLuz.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(new Vector3(posteDeLuz.Position.X+33, posteDeLuz.Position.Y, posteDeLuz.Position.Z)));
                meshes.Add(posteDeLuz);
                LpostesDeLuz.Add(posteDeLuz);
            }
        }

        private void crearParedes()
        {
            paredTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "MeshCreator\\Textures\\Ladrillo\\brick1_2.jpg");

            paredes.Add(new TgcPlane(new Vector3(0, 0, 0), new Vector3(0, 100, 6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            
            paredes.Add(new TgcPlane(new Vector3(0, 0, 0), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(6000, 0, 6000), new Vector3(0, 100, -6000), TgcPlane.Orientations.YZplane, paredTexture, 60, 1));
            paredes.Add(new TgcPlane(new Vector3(0, 0, 6000), new Vector3(6000, 100, 0), TgcPlane.Orientations.XYplane, paredTexture, 60, 1));

        }

        private void creaUnItem( string fileMesh, Vector3 posicion, float rotacion)
        {
            var sceneCura = loader.loadSceneFromFile(fileMesh);
            TgcMesh meshCura = sceneCura.Meshes[0];
            meshCura.AutoTransformEnable = true;
            meshCura.AutoUpdateBoundingBox = true;
            meshCura.createBoundingBox();
            meshCura.Position = posicion;
            meshCura.rotateX(rotacion);
            meshCura.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            meshCura.move(0, 20, 0);
            //cura.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(new Vector3(posteDeLuz.Position.X + 33, posteDeLuz.Position.Y, posteDeLuz.Position.Z)));
            /*Item cura = new Item(this.env.MediaDir, this.env.ShadersDir);
            cura.setMesh(meshCura);
            cura.setFileNameSound(MediaDir + "MySounds\\PickUp2.wav");*/

            items.Add(meshCura);
        }

        private void crearItems()
        {

            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Cura\\cura-TgcScene.xml", new Vector3(2609, 15, 2450), FastMath.PI_HALF);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml", new Vector3(5237, 10, 50), FastMath.PI);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml", new Vector3(50, 10, 5240), FastMath.PI);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml", new Vector3(5295, 10, 5223), FastMath.PI);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-V\\misil-V-TgcScene.xml", new Vector3(2728, 10, 5559), FastMath.PI);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-V\\misil-V-TgcScene.xml", new Vector3(1676, 10, 60), FastMath.PI);
            creaUnItem(MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-V\\misil-V-TgcScene.xml", new Vector3(2602, 10, 719), FastMath.PI);
            itemsTiempoInvisibilidad = new int[7];
            for ( var i = 0; i<7; i++)
            {
                itemsTiempoInvisibilidad[i] = 380;
            }
        }

        private void crearSemaforos()
        {

            var sceneSemaforo = loader.loadSceneFromFile(MediaDir + "ModelosTgc\\Semaforo\\Semaforo-TgcScene.xml");

            semaforo = sceneSemaforo.Meshes[0];

            for (int i = 0; i < veredas.Count; i++)
            {

                var instanciaIda = semaforo.createMeshInstance(semaforo.Name + i);
                instanciaIda.Scale = new Vector3(2, 3, 2);
                instanciaIda.AutoTransformEnable = true;
                var posicionX = (veredas[i].Position.X) + (veredas[i].Size.X) - 20;
                var posicionY = 40;
                var posicionZ = veredas[i].Position.Z + 20;
                instanciaIda.move(posicionX, posicionY, posicionZ);
                instanciaIda.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(instanciaIda.Position));
                semaforos.Add(instanciaIda);

                var instanciaVuelta = semaforo.createMeshInstance(semaforo.Name + i);
                instanciaVuelta.AutoTransformEnable = true;
                instanciaVuelta.Scale = new Vector3(2, 3, 2);
                var posicionX2 = (veredas[i].Position.X) + 20;
                var posicionY2 = 40;
                var posicionZ2 = veredas[i].Position.Z + (veredas[i].Size.Z) - 20;
                instanciaVuelta.move(posicionX2, posicionY2, posicionZ2);
                instanciaVuelta.rotateY(FastMath.PI);
                instanciaVuelta.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(instanciaVuelta.Position));
                semaforos.Add(instanciaVuelta);


            }
        }

        private TgcMesh Planta;

        private enum Plantas { Pino = 2, Palmera = 1, Nada = 4, Arbol = 3 };
        private void crearUnaPlanta(TgcScene unaScene, int i, Vector3 vectorPosicion, int n)
        {
            Planta = unaScene.Meshes[0];
      //      Planta.Scale = new Vector3(1, 3, 1);
            var instancia = Planta.createMeshInstance(Planta.Name + i);
            instancia.AutoTransformEnable = true;
            instancia.AlphaBlendEnable = true;
            instancia.move(vectorPosicion.X, vectorPosicion.Y, vectorPosicion.Z);
            
     
            var scenePasto = loader.loadSceneFromFile(MediaDir + "Pasto\\Pasto-TgcScene.xml");
            var pasto = scenePasto.Meshes[0];
            pasto.AutoTransformEnable = true;
            pasto.AlphaBlendEnable = true;
            pasto.move(vectorPosicion.X, vectorPosicion.Y, vectorPosicion.Z);
           
            if (n == 3)
            {
                instancia.Scale = new Vector3((float)0.5, (float)0.5, (float)0.5);
                pasto.Scale = new Vector3(1,1,1);
            }
            instancia.BoundingBox.transform(Matrix.Scaling(new Vector3(0.1f, 3, 0.1f)) * Matrix.Translation(instancia.Position));
            //     instancia.BoundingBox.scaleTranslate
            arboles.Add(instancia);
            pasto.BoundingBox.transform(Matrix.Scaling(new Vector3(0.2f, 1, 0.2f)) * Matrix.Translation(pasto.Position));
            arboles.Add(pasto);
            meshes.Add(instancia);
            meshes.Add(pasto);
        }
        private Vector3 vectorFrontalMitadVereda(int i,int variacionPosicion)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X) / 2);
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + 20;
            return new Vector3(posicionX + variacionPosicion, posicionY, posicionZ );

        }
        private Vector3 vectorTraseroMitadVereda(int i,int variacionPosicion)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X) / 2);
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z)) - 20;
            return new Vector3(posicionX + variacionPosicion, posicionY, posicionZ );

        }
        private Vector3 vectorLateralDerechoMitadVereda(int i,int variacionPosicion)
        {
            var posicionX = (veredas[i].Position.X) + ((veredas[i].Size.X)) - 20;
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z) / 2);
            return new Vector3(posicionX , posicionY, posicionZ + variacionPosicion);

        }
        private Vector3 vectorLateralIzquierdoMitadVereda(int i,int variacionPosicion)
        {
            var posicionX = (veredas[i].Position.X) + 20;
            var posicionY = 10;
            var posicionZ = veredas[i].Position.Z + ((veredas[i].Size.Z) / 2) - 20;
            return new Vector3(posicionX , posicionY, posicionZ + variacionPosicion);

        }
        private void dibujarPlantaRandom(int j, Vector3 vectorPosicion, TgcScene scene, int i, int n,int variacionPosicion)
        {
            
            switch (j)
            {
                case 1:
                    vectorPosicion = vectorFrontalMitadVereda(i,variacionPosicion);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 2:
                    vectorPosicion = vectorTraseroMitadVereda(i,variacionPosicion);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 3:
                    vectorPosicion = vectorLateralIzquierdoMitadVereda(i,variacionPosicion);
                    crearUnaPlanta(scene, i, vectorPosicion, n);
                    break;
                case 4:
                    vectorPosicion = vectorLateralDerechoMitadVereda(i,variacionPosicion);
                    crearUnaPlanta(scene, i, vectorPosicion * variacionPosicion, n);
                    
                    break;
            }

        }
        private void crearPlantas(int variacionPosicion)
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
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta, i, n,variacionPosicion);
                            break;
                        case (int)Plantas.Palmera:
                            var scenePlanta2 = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vegetacion\\Palmera3\\Palmera3-TgcScene.xml");
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta2, i, n,variacionPosicion);
                            break;
                        case (int)Plantas.Arbol:
                            var scenePlanta3 = loader.loadSceneFromFile(MediaDir + "MeshCreator\\Meshes\\Vegetacion\\ArbolBosque\\ArbolBosque-TgcScene.xml");
                            dibujarPlantaRandom(k, vectorPosicion, scenePlanta3, i, n,variacionPosicion);
                            break;
                    }


                }


            }
        }

        public void Update()
        {
            //Se actualiza la posicion del skybox.
            skyBox.Center = this.env.Camara.Position;

            contadorDeCiclos++;
            if (contadorDeCiclos > TiempoRetardo)
            {
                contadorDeCiclos = 0;
                foreach (var item in items)
                {
                    //TgcMesh mesh = item.getMesh();
                    item.rotateY(ROTATION_SPEED);
                    item.move(0, MOVEMENT_SPEED * currentMoveDir, 0);
                    if (item.Position.Y > 30 || item.Position.Y < 0)
                    {
                        currentMoveDir *= -1;
                    }
                }
            }
            
        }

        public void setNotVisible(int posicion)
        {
            itemsTiempoInvisibilidad[posicion] = 0;
        }

        public bool getVisible(int posicion)
        {
            return (itemsTiempoInvisibilidad[posicion] >= 380);
        }
        private int posEncontrada = -1;
        public void Render()
        {


            foreach (TgcMesh mesh in this.getAllMeshes())
            {
                //rendereo solo lo que esta dentro del frustrum
                var c = TgcCollisionUtils.classifyFrustumAABB(this.env.Frustum, mesh.BoundingBox);
                if (c != TgcCollisionUtils.FrustumResult.OUTSIDE)
                {
                    mesh.render();
                }
            }
            
            //Renderizar suelo
            suelo.render();
            //calle.render();


            //Renderizar SkyBox
            skyBox.render();

            //Renderizar instancias
          //  foreach (var mesh in meshes)
          //        mesh.render();

            //Renderizar items
            int nroItem = 0;
            foreach (var item in items)
            {
                
                if (itemsTiempoInvisibilidad[nroItem] < 380)
                {
                    item.Enabled = false;
                    //posEncontrada = nroItem;
                    itemsTiempoInvisibilidad[nroItem] = itemsTiempoInvisibilidad[nroItem]++;
                } else
                {
                    item.Enabled = true;
                    
                }
                item.render();


                nroItem++;
            }

            //Renderizado de cordones
            foreach (var cordon in cordones)
                cordon.render();
 

            //Renderizado de veredas
            foreach (var v in veredas)
                 v.render();
 

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
         /*   foreach (var vehiculo in vehiculos)
            {
                vehiculo.getMesh().render();
            }*/

            //mostrarBounding();
          
        }
        public void dispose()
        {

            suelo.dispose();
            //calle.dispose();
            //Al hacer dispose del original, se hace dispose automaticamente de todas las instancias
            edificio.dispose();
            /*  auto.dispose();
              hummer.dispose();
              camion.dispose();
              buggy.dispose();
              patrullero.dispose();
              */
          /*  foreach (var vehiculo in vehiculos)
            {
                vehiculo.getMesh().dispose();
            }*/
            //disposeListas();

            //Liberar recursos del SkyBox
            skyBox.dispose();


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
        private void mostrarBounding()
        {
            foreach (var mesh in edificios)
                mesh.BoundingBox.render();
            foreach (var mesh in semaforos)
                mesh.BoundingBox.render();
            foreach (var mesh in arboles)
                mesh.BoundingBox.render();
            foreach (var mesh in LpostesDeLuz)
                mesh.BoundingBox.render();
            
        }
        public TgcSkyBox getSkyBox()
        {
            return this.skyBox;
        }
        private void iniciarCielo()
        {
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
        public List<TgcMesh> getEdificios()
        {
            return this.edificios;
        }
        public List<TgcMesh> getSemaforos()
        {
            return this.semaforos;
        }
        public List<TgcMesh> getArboles()
        {
            return this.arboles;
        }
        public List<TgcMesh> getPostesDeLuz()
        {
            return this.LpostesDeLuz;
        }
        public List<TgcMesh> getItems()
        {
            return this.items;
        }

        public List<TgcMesh> getMeshParedes()
        {
            List<TgcMesh> meshDeParedes = new System.Collections.Generic.List<TgcMesh>();
            foreach (var p in this.paredes)
            {
                meshDeParedes.Add(p.toMesh("Pared") );
            }
            return meshDeParedes;
        }
      
    }
}