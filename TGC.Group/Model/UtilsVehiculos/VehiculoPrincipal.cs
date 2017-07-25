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
        
        private Rueda[] ruedas;
        private List<Rueda[]> listaDeRuedas;
        public static float camaraOffsetDefaulForward = 300f;

        private Matrix mTransRuedaDelDer;
        private Matrix mTransRuedaDelIzq;
        private Matrix mTransRuedaTraDer;
        private Matrix mTransRuedaTraIzq;
        private Matrix mEscRuedas;
        //Shaders
        public string TecnicaOriginal { get; private set; }
        private Microsoft.DirectX.Direct3D.Effect efectoOriginal;
        private Microsoft.DirectX.Direct3D.Effect efectoShaderChoque;

        public float ChoqueDelantero = 0;
        public float ChoqueTrasero = 0;

        private void seteaPosicionRuedas(int nro)
        {
            switch (nro)
            {
                case 1:
                    mTransRuedaDelDer = Matrix.Translation(new Vector3(-39, 0, -49));
                    mTransRuedaTraDer = Matrix.Translation(new Vector3(-39, 0, 68));
                    mTransRuedaDelIzq = Matrix.Translation(new Vector3(30, 0, -49));
                    mTransRuedaTraIzq = Matrix.Translation(new Vector3(30, 0, 68));
                    mEscRuedas = Matrix.Scaling(new Vector3(0.1f, 0.1f, 0.1f));
                    break;
                case 2:
                    mTransRuedaDelDer = Matrix.Translation(new Vector3(-28, 0, -23));
                    mTransRuedaTraDer = Matrix.Translation(new Vector3(-28, 0, 40));
                    mTransRuedaDelIzq = Matrix.Translation(new Vector3(22, 0, -23));
                    mTransRuedaTraIzq = Matrix.Translation(new Vector3(22, 0, 40));
                    mEscRuedas = Matrix.Scaling(new Vector3(0.055f, 0.055f, 0.055f));
                    break;
                case 3:
                    mTransRuedaDelDer = Matrix.Translation(new Vector3(-24, 0, -40));
                    mTransRuedaTraDer = Matrix.Translation(new Vector3(-24, 0, 57));
                    mTransRuedaDelIzq = Matrix.Translation(new Vector3(18, 0, -40));
                    mTransRuedaTraIzq = Matrix.Translation(new Vector3(18, 0, 57));
                    mEscRuedas = Matrix.Scaling(new Vector3(0.07f, 0.07f, 0.07f));
                    break;
                default:
                    break;
            }
        }

        public VehiculoPrincipal(TwistedMetal env) : base(env)
        {
          
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
            this.getMesh().AutoUpdateBoundingBox = true;

            seteaPosicionRuedas(personaje.NroPersonaje);

            Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);
            Matrix mEsc = Matrix.Scaling(scale);
            //var sceneRueda = loader.loadSceneFromFile(env.MediaDir + "Wheel\\wheel2-TgcScene.xml");
            var sceneRueda = loader.loadSceneFromFile(env.MediaDir + "Wheel\\wheel2-TgcScene.xml");

            Vector3 pos = new Vector3(-40, 0, -48);
            Matrix mRot = Matrix.RotationY(FastMath.PI);            
            Matrix mTra = Matrix.Translation(pos);
            
            TgcMesh rueda1_p1 = sceneRueda.Meshes[0];
            rueda1_p1.AutoTransformEnable = false;
            rueda1_p1.Transform = mEsc * mRot * mTra ;
            TgcMesh rueda1_p2 = sceneRueda.Meshes[1];
            rueda1_p2.AutoTransformEnable = false;
            rueda1_p2.Transform = mEsc * mRot * mTra;
            TgcMesh rueda1_p3 = sceneRueda.Meshes[2];
            rueda1_p3.AutoTransformEnable = false;
            rueda1_p3.Transform = mEsc * mRot * mTra;

            TgcMesh[] meshRuedas = new TgcMesh[] { rueda1_p1, rueda1_p2, rueda1_p3 };
            base.setRuedaDelDer(meshRuedas);

            pos = new Vector3(-40, 0, 68);
            mRot = Matrix.RotationY(FastMath.PI);
            mTra = Matrix.Translation(pos);

            TgcMesh rueda2_p1 = sceneRueda.Meshes[0];
            rueda2_p1.AutoTransformEnable = false;
            rueda2_p1.Transform = mEsc * mRot * mTra;
            TgcMesh rueda2_p2 = sceneRueda.Meshes[1];
            rueda2_p2.AutoTransformEnable = false;
            rueda2_p2.Transform = mEsc * mRot * mTra;
            TgcMesh rueda2_p3 = sceneRueda.Meshes[2];
            rueda2_p3.AutoTransformEnable = false;
            rueda2_p3.Transform = mEsc * mRot * mTra;

            meshRuedas = new TgcMesh[] { rueda2_p1, rueda2_p2, rueda2_p3 };
            base.setRuedaTraDer(meshRuedas);

            pos = new Vector3(30, 0, -48);
            mRot = Matrix.RotationY(FastMath.PI);
            mTra = Matrix.Translation(pos);

            TgcMesh rueda3_p1 = sceneRueda.Meshes[0];
            rueda3_p1.AutoTransformEnable = false;
            rueda3_p1.Transform = mEsc * mRot * mTra;
            TgcMesh rueda3_p2 = sceneRueda.Meshes[1];
            rueda3_p2.AutoTransformEnable = false;
            rueda3_p2.Transform = mEsc * mRot * mTra;
            TgcMesh rueda3_p3 = sceneRueda.Meshes[2];
            rueda3_p3.AutoTransformEnable = false;
            rueda3_p3.Transform = mEsc * mRot * mTra;

            meshRuedas = new TgcMesh[] { rueda3_p1, rueda3_p2, rueda3_p3 };
            base.setRuedaDelIzq(meshRuedas);

            pos = new Vector3(30, 0, 68);
            mRot = Matrix.RotationY(FastMath.PI);
            mTra = Matrix.Translation(pos);

            TgcMesh rueda4_p1 = sceneRueda.Meshes[0];
            rueda4_p1.AutoTransformEnable = false;
            rueda4_p1.Transform = mEsc * mRot * mTra;
            TgcMesh rueda4_p2 = sceneRueda.Meshes[1];
            rueda4_p2.AutoTransformEnable = false;
            rueda4_p2.Transform = mEsc * mRot * mTra;
            TgcMesh rueda4_p3 = sceneRueda.Meshes[2];
            rueda4_p3.AutoTransformEnable = false;
            rueda4_p3.Transform = mEsc * mRot * mTra;

            meshRuedas = new TgcMesh[] { rueda4_p1, rueda4_p2, rueda4_p3 };
            base.setRuedaTraIzq(meshRuedas);

            this.cargarShaders();
            this.mover();

            crearVelocimetro();
            cargarShaders();
        }

        private void cargarShaders()
        {
            TecnicaOriginal = this.getMesh().Technique;
            efectoOriginal = this.getMesh().Effect;
            efectoShaderChoque = TgcShaders.loadEffect(this.env.ShadersDir + "EfectoMetal_ConChoque.fx");
            this.getMesh().Technique = "RenderScene";
        }
        private void crearVelocimetro()
        {
            velocimetro = new Velocimetro(env,this.getVelocidadMaxima());

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

        public override void Update()
        {

            TwistedMetal tm = TwistedMetal.getInstance();

            if (cambiarCamara())
                alternaCamara();

            if (cambiarMusica())
                tm.cambiarMusica();

  
            if (moverArriba())
                base.startSalto();

            base.Update();
            camaraInterna.Target = this.getMesh().Position;
            camaraRotante.SetCamera(new Vector3(camaraInterna.Position.X, 150, camaraInterna.Position.Z),
                                     new Vector3(camaraInterna.Target.X, 100, camaraInterna.Target.Z));

            /*foreach (var rueda in base.getRueda())
            {
                rueda.up
            }*/
            this.velocimetro.Update(FastMath.Abs(this.getVelocidadX()));
        }

        public override void Render()
        {
            AplicarShaderChoque(); //aplica efecto;
          
            base.Render();
            this.velocimetro.Render();
            //mEscRuedas = Matrix.Scaling(new Vector3(0.1f, 0.1f, 0.1f));
            
            Matrix mRot = Matrix.RotationY(FastMath.PI_HALF);
            var T = Matrix.Translation(0, 1, 0);
            var pivoteRueda = Vector3.TransformCoordinate(new Vector3(0, 5f, 0.0f), T);

            Matrix A = Matrix.Translation(-pivoteRueda.X, -pivoteRueda.Y, -pivoteRueda.Z);
            Matrix B = Matrix.Translation(pivoteRueda.X, pivoteRueda.Y, pivoteRueda.Z);


            //mTransRuedaDelDer = Matrix.Translation(new Vector3(-39, 0, -49));
            foreach (var rueda in base.getRuedaDelDer())
            {
                T = A * base.transRuedaDel * mTransRuedaDelDer;
                if (!base.transGiro.Equals(Matrix.Identity)) T = T * base.transGiro;
                rueda.Transform = mEscRuedas * T * B * this.getMesh().Transform;
                rueda.render();
            }

            //mTransRuedaTraDer = Matrix.Translation(new Vector3(-39, 0, 68));
            foreach (var rueda in base.getRuedaTraDer())
            {
                T = A * base.transRuedaTra * mTransRuedaTraDer;
                if (!base.transGiro.Equals(Matrix.Identity)) T = T * base.transGiro;
                rueda.Transform = mEscRuedas * T * B * this.getMesh().Transform;
                rueda.render();
            }

            //mTransRuedaDelIzq = Matrix.Translation(new Vector3(30, 0, -49));
            foreach (var rueda in base.getRuedaDelIzq())
            {
                T = A * base.transRuedaDel * mTransRuedaDelIzq;
                if (!base.transGiro.Equals(Matrix.Identity)) T = T * base.transGiro;
                rueda.Transform = mEscRuedas * T * B * this.getMesh().Transform;
                rueda.render();
            }

            //mTransRuedaTraIzq = Matrix.Translation(new Vector3(30, 0, 68));
            foreach (var rueda in base.getRuedaTraIzq())
            {
                T = A * base.transRuedaTra * mTransRuedaTraIzq;
                if (!base.transGiro.Equals(Matrix.Identity)) T = T * base.transGiro;
                rueda.Transform = mEscRuedas * T * B * this.getMesh().Transform;
                rueda.render();
            }

        }
        private void AplicarShaderChoque()
        {

            /*  INICIO CHOQUE */
            if (this.colisionoAlgunaVez && this.getMesh().Position.Y == 5)
                ChoqueDelantero = 1;

            if (this.colisiono() && !this.colisionoAlgunaVez && this.getMesh().Position.Y == 5)
                ChoqueTrasero = -1;
            
            if (this.colisiono() && this.getMesh().Position.Y == 5)
               ChoqueDelantero = 1;

            float vida = base.getLifeLevel().nivelDeVida();
            int energia = 10;
            if (vida < 100) {
                if (vida < 40) {
                    if (vida < 20) {
                        energia = 2;
                    } else {
                        energia = 4;
                    }
                } else {
                    if (vida < 70) {
                        energia = 7;
                    }
                }
            }

            efectoShaderChoque.SetValue("Energia", energia);
            efectoShaderChoque.SetValue("ChoqueAtras", ChoqueTrasero);
            efectoShaderChoque.SetValue("ChoqueAdelante", ChoqueDelantero);
            efectoShaderChoque.SetValue("fvLightPosition", new Vector4(0, 100, 0, 0));
            efectoShaderChoque.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(this.getCamara().Position));
            this.getMesh().Effect = efectoShaderChoque;
            this.getMesh().Technique = "RenderScene";




        }
        
    }
}
