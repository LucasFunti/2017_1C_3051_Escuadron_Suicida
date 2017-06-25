using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using TGC.Core.Geometry;
using System.Drawing;
using TGC.Group.Model.UtilsColisiones;
using TGC.Core.Utils;
using TGC.Core.Input;
using System;
using TGC.Core.Textures;
using TGC.Core.Interpolation;
using TGC.Core.Direct3D;
using TGC.Core.Shaders;

namespace TGC.Group.Model.UtilsVehiculos
{
    class VehiculoPrincipal : Vehiculo
    {
        public Velocimetro velocimetro;
        private TgcSceneLoader loader;
        private CamaraTerceraPersona camaraInterna;
        private CamaraTerceraPersona camaraInterna2;
        private TgcRotationalCamera camaraRotante;
        //private Rueda[] ruedas;
        //private List<Rueda[]> listaDeRuedas;
        public static float camaraOffsetDefaulForward = 300f;


        //Shaders
        public string TecnicaOriginal { get; private set; }
        private Microsoft.DirectX.Direct3D.Effect efectoOriginal;
        private Microsoft.DirectX.Direct3D.Effect efectoShaderChoque;

        public float ChoqueDelantero = 0;
        public float ChoqueTrasero = 0;



        public VehiculoPrincipal(TwistedMetal env) : base(env)
        {
            velocimetro = new Velocimetro(env);

            Personaje personaje = Personaje.getInstance();
            loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(personaje.FileMesh);

            this.setMesh(scene.Meshes[0]);
            this.getMesh().Position = new Vector3(100, 5, 3000);
            camaraManager();
            this.setVelocidadMaxima(personaje.VelocidadMax);
            this.setVelocidadMinima(personaje.VelocidadMin);
            this.setConstanteDeAsceleracionX(personaje.ConstanteAceleracion);
            //      base.setPEndDirectionArrow(new Vector3(this.getMesh().Position.X, this.getMesh().Position.Y, -500));
            //   this.setAlturaInicial(this.getMesh().Position.Y);

            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!

            base.setSonido(personaje.FileSonido);
            base.setSonidoMotor(personaje.FileSonidoMotor);
            base.setSonidoArma(personaje.FileSonidoArma);
            base.setSonidoColision(personaje.FileSonidoColision);
            base.setSonidoItem(personaje.FileSonidoItem);
            base.setSonidoSalto(personaje.FileSonidoSalto);

            //Seteo posicion Inicial
            //  Vector3 scale3 = new Vector3(1f, 1f, 1f);
            // var m = Matrix.Scaling(scale3) * this.matrixRotacion * Matrix.Translation(new Vector3(100, 5, 3000));
            //  var m = Matrix.Scaling(scale3) * Matrix.RotationY(1f) * Matrix.Translation(new Vector3(100, 5, 3000)) ;

            //   this.getMesh().Transform = m;
            this.cargarShaders();
            this.mover();


            //Creo las ruedas
            //listaDeRuedas = new System.Collections.Generic.List<Rueda[]>();
            /*ruedas = new Rueda[3];
            Vector3 vectorPos = new Vector3(this.getMesh().Position.X - 100, this.getMesh().Position.Y, this.getMesh().Position.Z - 100);
            var sceneRueda = loader.loadSceneFromFile(env.MediaDir + "ModelosTgc\\Robot\\Robot-TgcScene.xml");

            Vector3 scale3 = new Vector3(0.01f, 0.01f, 0.01f);
            var m = Matrix.Scaling(scale3) * this.matrixRotacion * Matrix.Translation(vectorPos);

            /*var rueda1_p1 = sceneRueda.Meshes[0];
            rueda1_p1.move(vectorPos);
            rueda1_p1.AutoTransformEnable = true;
            rueda1_p1.Position = vectorPos;
            rueda1_p1.Transform = m;
            rueda1_p1.Scale = new Vector3(1f, 1f, 1f);
            //rueda1_p1.rotateY(FastMath.PI);
            ruedas[0] = new Rueda(this.env, rueda1_p1, true);
            //var instance1 = ruedas[0].getMesh().createMeshInstance("rueda1_1");
            //instance1.rotateY(1f);
            //instance1.Scale = new Vector3(-1000, -1000, -1000);
            //ruedas[0].Instancia = instance1;

            /*var rueda1_p2 = sceneRueda.Meshes[1];
            rueda1_p2.move(vectorPos);
            rueda1_p2.AutoTransformEnable = true;
            rueda1_p2.Position = vectorPos;
            rueda1_p2.Transform = m;
            rueda1_p2.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            rueda1_p2.updateBoundingBox();
            //rueda1_p2.rotateY(FastMath.PI);
            ruedas[1] = new Rueda(this.env, rueda1_p2, true);


            var rueda1_p3 = sceneRueda.Meshes[2];
            rueda1_p3.move(vectorPos);
            rueda1_p3.AlphaBlendEnable = true;
            rueda1_p3.AutoTransformEnable = true;
            rueda1_p3.Position = vectorPos;
            rueda1_p3.Transform = m;
            rueda1_p3.Scale = new Vector3(0.1f, 0.1f, 0.1f);
            //rueda1_p3.rotateY(FastMath.PI);
            ruedas[2] = new Rueda(this.env, rueda1_p3, true);*/

            //listaDeRuedas.Add(ruedas);
            //ruedaDelantera2 = rueda.Meshes[0];
            //ruedaTrasera1 = rueda.Meshes[0];
            //ruedaTrasera2 = rueda.Meshes[0];
            cargarShaders();
        }

        private void cargarShaders()
        {
            TecnicaOriginal = this.getMesh().Technique;
            efectoOriginal = this.getMesh().Effect;
            efectoShaderChoque = TgcShaders.loadEffect(this.env.ShadersDir + "EfectoMetal_ConChoque.fx");
            this.getMesh().Technique = "RenderScene";
        }

        public override Boolean esAutoPrincipal()
        {
            return true;
        }
        private void camaraManager()
        {
            camaraInterna = new CamaraTerceraPersona(this.getMesh().Position, 100, 300f);
            camaraInterna2 = new CamaraTerceraPersona(this.getMesh().Position, 200, 400f);
            camaraRotante = new TgcRotationalCamera(
                new Vector3(this.getMesh().Position.X, 100, this.getMesh().Position.Z), 300, 0.15f, 50f, this.env.Input);
            this.env.Camara = camaraInterna;
        }
        public CamaraTerceraPersona getCamara()
        {
            return this.camaraInterna;
        }

        public override void ManejarColisionCamara()
        {
            //Actualizar valores de camara segun modifiers
            //COPIADO DE EJEMPLO COLISIONES CAMARA del tgc viewer

            //Pedirle a la camara cual va a ser su proxima posicion
            Vector3 position;
            Vector3 target;
            camaraInterna.CalculatePositionTarget(out position, out target);

            //Detectar colisiones entre el segmento de recta camara-personaje y todos los objetos del escenario
            Vector3 q;
            var minDistSq = FastMath.Pow2(camaraInterna.OffsetForward);
            foreach (var obstaculo in this.env.GetManejadorDeColision().MeshesColicionables)
            {
                //Hay colision del segmento camara-personaje y el objeto
                if (TgcCollisionUtils.intersectSegmentAABB(target, position, obstaculo.BoundingBox, out q) && obstaculo != this.getMesh())
                {
                    //Si hay colision, guardar la que tenga menor distancia
                    var distSq = Vector3.Subtract(q, target).LengthSq();
                    //Hay dos casos singulares, puede que tengamos mas de una colision hay que quedarse con el menor offset.
                    //Si no dividimos la distancia por 2 se acerca mucho al target.
                    minDistSq = FastMath.Min(distSq / 2, minDistSq);
                }
            }

            //Acercar la camara hasta la minima distancia de colision encontrada (pero ponemos un umbral maximo de cercania)
            var newOffsetForward = -FastMath.Sqrt(minDistSq);

            if (FastMath.Abs(newOffsetForward) < 10)
            {
                newOffsetForward = -10;
            }
            if (camaraOffsetDefaulForward > camaraInterna.OffsetForward)
            {
                camaraInterna.OffsetForward = (newOffsetForward - 72f * this.env.ElapsedTime) * (-1f); //enderezo lentamente

            }
            else
            {
                camaraInterna.OffsetForward = -newOffsetForward;
            }
            //Asignar la ViewMatrix haciendo un LookAt desde la posicion final anterior al centro de la camara
            camaraInterna.CalculatePositionTarget(out position, out target);
            camaraInterna.SetCamera(position, target);

        }

        public override void ProcesarMovimientoDeCamara(float offsetHeight, float offsetForward)
        {

            camaraInterna.OffsetHeight = offsetHeight;
            camaraInterna.OffsetForward = offsetForward;

        }
        public override bool moverAdelante()
        {
            return this.env.Input.keyDown(Key.W);
        }
        public override bool moverAtras()
        {
            return this.env.Input.keyDown(Key.S);
        }
        public override bool moverADerecha()
        {
            return this.env.Input.keyDown(Key.D);
        }
        public override bool moverAIzquierda()
        {
            return this.env.Input.keyDown(Key.A);
        }
        public override bool moverArriba()
        {
            return this.env.Input.keyDown(Key.J);
        }
        public override bool moverAbajo()
        {
            return this.env.Input.keyDown(Key.K);
        }
        public override bool cambiarCamara()
        {
            return this.env.Input.keyUp(Key.C);
        }
        public override bool cambiarMusica()
        {
            return this.env.Input.keyUp(Key.M);
        }
        public override bool disparar()
        {
            return this.env.Input.keyDown(Key.RightControl);
        }
        public override bool disparaEspecial()
        {
            return this.env.Input.keyUp(Key.Space);
        }
        public override void rotarCamara(float rotAngle)
        {
            camaraInterna.rotateY(rotAngle);
            //   base.updateDirectionArrowWithAngle(rotAngle);
        }

        int tipoCamara = 0;
        private void alternaCamara()
        {
            var height = camaraInterna.OffsetHeight;
            var forward = camaraInterna.OffsetForward;

            //Alterna la cámara
            tipoCamara++;
            switch (tipoCamara)
            {
                case 1:
                    height = 200;
                    forward = 500;
                    break;
                case 2:
                    this.env.Camara = camaraRotante;
                    break;
                default:
                    this.env.Camara = camaraInterna;
                    height = 100;
                    forward = 300;
                    tipoCamara = 0;
                    break;
            }
            ProcesarMovimientoDeCamara(height, forward);

        }





        int contadorAlPrincipio = 0;
        List<Arma> listaDeArmasToRemove;
        public override void Update()
        {
            listaDeArmasToRemove = new List<Arma>();
            foreach (Arma arma in base.listaDeArmas)
                if (arma.getTiempoDeVida() > 20)
                    listaDeArmasToRemove.Add(arma);

            foreach (var item in listaDeArmasToRemove)
            {
                listaDeArmas.Remove(item);
                item.dispose();
            }

            TwistedMetal tm = TwistedMetal.getInstance();

            if (cambiarCamara())
                alternaCamara();

            if (cambiarMusica())
                tm.cambiarMusica();

            //     if (disparar()) {
            //        base.startDisparo();
            //        creaDisparo(this.getMesh().Position);
            //    }

            if (moverArriba())
                base.startSalto();

            //    if (disparaEspecial()) { 
            //      base.startArma();
            //     creaMisilV(this.getMesh().Position);
            // }

            contadorAlPrincipio++;
            if (contadorAlPrincipio > 380)
                tm.sonidos.soundControl();

            base.Update();
            camaraInterna.Target = this.getMesh().Position;
            camaraRotante.SetCamera(new Vector3(camaraInterna.Position.X, 150, camaraInterna.Position.Z),
                                     new Vector3(camaraInterna.Target.X, 100, camaraInterna.Target.Z));

            /*foreach (var rueda in this.listaDeRuedas)
            {
                rueda[0].Update();
                //rueda[1].Update();
                //rueda[2].Update();
            }*/
            velocimetro.Update(this.getVelocidadX(), true);
        }
        public override void Render()
        {
            AplicarShaderChoque(); //aplica efecto;
            this.velocimetro.Render();
            base.Render();


            /*foreach (var rueda in this.listaDeRuedas)
            {
                rueda[0].Render();
                //rueda[0].Render();
                //rueda[1].Render();
                //rueda[1].Render();
                //rueda[2].Render();
               // rueda[2].Render();
            }*/

        }
        private void AplicarShaderChoque()
        {

            /*  INICIO CHOQUE */
            if (this.colisionoAlgunaVez && this.getMesh().Position.Y == 5)
                ChoqueDelantero = 1;

            if (this.colisiono() && this.getMesh().Position.Y == 5)
                ChoqueDelantero = 1;


            if (this.colisiono() && !this.colisionoPorDelante() && this.getMesh().Position.Y == 5)
                ChoqueTrasero = -1;

            efectoShaderChoque.SetValue("ChoqueAtras", ChoqueTrasero);
            efectoShaderChoque.SetValue("ChoqueAdelante", ChoqueDelantero);
            efectoShaderChoque.SetValue("fvLightPosition", new Vector4(0, 100, 0, 0));
            efectoShaderChoque.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(this.getCamara().Position));
            this.getMesh().Effect = efectoShaderChoque;





        }
        
    }
}
