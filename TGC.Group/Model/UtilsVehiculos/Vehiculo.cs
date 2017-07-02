using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using TGC.Group.Model.UtilsEfectos;
using TGC.Group.Model.UtilsVehiculos;

namespace TGC.Group.Model
{
    class Vehiculo : ObjetoConMovimiento
    {
        private LifeLevel lifeLevel;
        private Humo humoCañoEscape;
        private Humo humoChoque;
        private Boolean EfectoNitro=false;
        public float tInicioHumo = 1f; 
        public static float tDuracionHumo = 1f;
        public float tFinHumo = 2f;


        public Vehiculo(TgcMesh Mesh, TwistedMetal env) : base(env)
        {
            base.setMesh(Mesh);
          //  base.getMesh().(pos);
            base.setVelocidadY(0);
            base.setAluraMaxima(20);
            //      direcionadores();
            updateTGCArrow();
            iniciarNivelDeVida();
            iniciarHumo();
        }
        public Vehiculo(TwistedMetal env) : base(env)  
        {
            this.setVelocidadY(0);
            base.setAluraMaxima(100);
            //    direcionadores();
            iniciarNivelDeVida();
            iniciarHumo();
        }
        private void iniciarNivelDeVida()
        {
            this.lifeLevel = new LifeLevel(this.esAutoPrincipal());
        }
        private void iniciarHumo()
        {
            humoCañoEscape = new Humo(this.env);
            humoChoque = new Humo(this.env,true);
        }
        private void TirarHumoChoque()
        {
            tInicioHumo = this.env.ElapsedTime;
            tFinHumo = tInicioHumo + tDuracionHumo;
        }
        public  virtual Boolean esAutoPrincipal()
        {
            return false;
        }

        /*     private void direcionadores()
             {
                 directionArrow = new TgcArrow();
                 directionArrow.BodyColor = Color.Red;
                 directionArrow.HeadColor = Color.Green;
                 directionArrow.Thickness = 0.4f;
                 directionArrow.HeadSize = new Vector2(5, 10);

                 //Linea para normal de colision
                 collisionNormalArrow = new TgcArrow();
                 collisionNormalArrow.BodyColor = Color.Blue;
                 collisionNormalArrow.HeadColor = Color.Yellow;
                 collisionNormalArrow.Thickness = 0.4f;
                 collisionNormalArrow.HeadSize = new Vector2(2, 5);

                 //Caja para marcar punto de colision
                 collisionPoint = TgcBox.fromSize(new Vector3(4, 4, 4), Color.Red);
                 collisionPoint.AutoTransformEnable = true;
             }*/

        /*   public void setPEndDirectionArrow(Vector3 vector)
           {
               directionArrow.PEnd = vector;
           }
           public Vector3 getPEndDirectionArrow(Vector3 vector)
           {
               return directionArrow.PEnd;
           }*/
        //Rota el objeto y si tiene camara sobre el.
        public override void rotar(Vector3 v, Matrix m,float anguloCamara)
        {
            base.rotar(v, m, anguloCamara);
            this.rotarCamara(anguloCamara);
        }
     /*   public void updateDirectionArrowWithAngle(float rotAngle)
        {
            directionArrow.PStart = this.getMesh().Position;
            float nvoPtoZ = 0.0f;
            float nvoPtoX = 0.0f;

            //Si el punto directionArrow.PEnd está sobre el plano z = -500 o el plano z = 5500
                if (directionArrow.PEnd.Z == -500 || directionArrow.PEnd.Z == 5500)
            {
                int signo = -1;
                if (directionArrow.PEnd.Z == 5500) signo = 1;

                nvoPtoZ = directionArrow.PEnd.Z;
                float distanciaEnZ = Core.Utils.FastMath.Abs(directionArrow.PEnd.Z - this.getMesh().Position.Z);
                nvoPtoX = directionArrow.PEnd.X + (signo * (Core.Utils.FastMath.Tan(rotAngle) * distanciaEnZ));
                if (nvoPtoX > 5500 || nvoPtoX < -500)
                {

                    float excedenteEnX = 0.0f;
                    if (nvoPtoX > 5500)
                    {
                        excedenteEnX = nvoPtoX - 5500;
                        nvoPtoX = 5500;
                    }
                    if (nvoPtoX < -500)
                    {
                        excedenteEnX = nvoPtoX - (-500);
                        nvoPtoX = -500;
                    }
                    //Debo calcular nvo punto Z en base al excedente y luego el punto x sera 5500 ó -500
                    nvoPtoZ = directionArrow.PEnd.Z - (Core.Utils.FastMath.Tan(Core.Utils.FastMath.PI_HALF - Core.Utils.FastMath.Abs(rotAngle)) * excedenteEnX);

                }
                

            }
            else //Entonces el punto directionArrow.PEnd está sobre el plano x = -500 o el plano x = 5500
            {
                int signo = 1;
                if (directionArrow.PEnd.X == 5500) signo = -1;

                nvoPtoX = directionArrow.PEnd.X;
                float distanciaEnX = Core.Utils.FastMath.Abs(directionArrow.PEnd.X - this.getMesh().Position.X);
                nvoPtoZ = directionArrow.PEnd.Z + (signo * (Core.Utils.FastMath.Tan(rotAngle) * distanciaEnX));
                if (nvoPtoZ > 5500 || nvoPtoZ < -500)
                {

                    float excedenteEnZ = 0.0f;
                    if (nvoPtoZ > 5500)
                    {
                        excedenteEnZ = nvoPtoZ - 5500;
                        nvoPtoZ = 5500;
                    }
                    if (nvoPtoZ < -500)
                    {
                        excedenteEnZ = nvoPtoZ - (-500);
                        nvoPtoZ = -500;
                    }
                    //Debo calcular nvo punto Z en base al excedente y luego el punto x sera 5500 ó -500
                    nvoPtoX = directionArrow.PEnd.X - (Core.Utils.FastMath.Tan(Core.Utils.FastMath.PI_HALF - Core.Utils.FastMath.Abs(rotAngle)) * excedenteEnZ);

                }
            }

            directionArrow.PEnd = new Vector3(nvoPtoX, this.getMesh().Position.Y, nvoPtoZ);
            directionArrow.updateValues();
        }
   
        public void updateDirectionArrow(Vector3 vectorMove)
        {

            Vector3 vecPos = this.getMesh().Position;
            directionArrow.PStart = vecPos;

            Vector3 vecResultante = Vector3.Multiply(vectorMove, 6000);
            //calculo la interseccion de la recta con los planos de las paredes.
            //El landa menor positivo es el que debo proyectar

            float landaZmin = 0;
            if ( (-500 - vecPos.Z) != 0) landaZmin = (-500 - vecPos.Z) / vectorMove.Z;

            float landaMinimo = landaZmin;
            float landaZmax = 0;
            if ((5500 - vecPos.Z) != 0) landaZmax = (5500 - vecPos.Z) / vectorMove.Z;

            //comparo landaZmax con landaMinimo
            if (landaMinimo <= 0) {
                landaMinimo = landaZmax;
            } else {
                if (landaZmax > 0 && landaZmax < landaMinimo) landaMinimo = landaZmax;
            }

            float landaXmin = 0;
            if ((-500 - vecPos.X) != 0) landaXmin = (-500 - vecPos.X) / vectorMove.X;

            //comparo landaXmin con landaMinimo
            if (landaMinimo <= 0) {
                landaMinimo = landaXmin;
            } else {
                if (landaXmin > 0 && landaXmin < landaMinimo) landaMinimo = landaXmin;
            }

            float landaXmax = 0;
            if ((5500 - vecPos.X) != 0) landaXmax = (5500 - vecPos.X) / vectorMove.X;

            //comparo landaXmax con landaMinimo
            if (landaMinimo <= 0){
                landaMinimo = landaXmax;
            } else {
                if (landaXmax > 0 && landaXmax < landaMinimo) landaMinimo = landaXmax;
            }



            if (landaMinimo == landaZmin)
            {
                directionArrow.PEnd = new Vector3(vecPos.X + (landaZmin * vectorMove.X), 50, -500);
            }

            if (landaMinimo == landaZmax)
            {
                directionArrow.PEnd = new Vector3(vecPos.X + (landaZmax * vectorMove.X), 50, 5500);
            }

            if (landaMinimo == landaXmin)
            {
                directionArrow.PEnd = new Vector3(-500, 50, vecPos.Z + (landaXmin * vectorMove.Z));
            }

            if (landaMinimo == landaXmax)
            {
                directionArrow.PEnd = new Vector3(5500, 50, vecPos.Z + (landaXmax * vectorMove.Z));
            }

            
            directionArrow.updateValues();
        }
        */
        public override void mover()
        {
            base.mover(); 
         }
          
        public virtual void rotarCamara(float rotacion)
        {

        }
        public void creaDisparo(Vector3 posicion)
        {
            string sonido = env.MediaDir + "MySounds\\MachineGun.wav";
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Objetos\\Vela\\Vela-TgcScene.xml");
            TgcMesh mesh = scene.Meshes[0];
            mesh.AutoTransformEnable = false;
            mesh.AutoUpdateBoundingBox = true;
            mesh.createBoundingBox();
            mesh.Position = posicion;
            Arma arma = new Arma(mesh, this.env, sonido, 40, this.orientacion, base.directionArrow.PEnd);

            arma.mover();
            ControladorDeVehiculos.getInstance().agregarArma(arma);
            base.agregarArma(arma);
        }
        public void creaMisilV()
        {
            TgcSceneLoader loader=new TgcSceneLoader();
            string sonido = env.MediaDir + "MySounds\\Launch4.wav";
            var scene = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml");
            TgcMesh mesh = scene.Meshes[0];
            mesh.AutoTransformEnable = false;
            mesh.AutoUpdateBoundingBox = true;
            mesh.createBoundingBox();
           // mesh.Rotation.Y=;
         //   mesh.rotateY(45);
            mesh.Position = this.getMesh().Position;
        //   mesh.Transform = this.getMesh().Transform;
            Arma arma = new Arma(mesh, this.env, sonido, 20, this.orientacion, base.directionArrow.PEnd);
            
            arma.mover();
           // arma.getMesh().rotateY(FastMath.p);
            ControladorDeVehiculos.getInstance().agregarArma(arma);
            base.agregarArma(arma);
            // mesh.rotateX(FastMath.PI);
            //mesh.Rotation = this.getMesh().Rotation;
            //  mesh.Scale = new Vector3(0.3f, 0.3f, 0.3f);
            //  mesh.Rotation = this.getMesh().Rotation;

            //  var m = Matrix.Scaling(0.3f, 0.3f, 0.3f) * Matrix.RotationY(this.anguloFinal*this.orientacion) * Matrix.Translation(this.getMesh().Position);
            // mesh.Transform=m;

            // mesh.move(0, 20, 0);
            //matrixRotacion = this.getMesh().Transform;
            //matrixRotacion = Matrix.RotationY(orientacion);
            //mesh.rotateY(orientacion);
            //this.rotar(new Vector3(0, valor, 0), matrixRotacion, 0);
            //Matrix matrixTransform = Matrix.Multiply(mesh.Transform, this.getMesh().Transform);
            //this.rotar(new Vector3(0, -sentido * 1f * this.env.ElapsedTime, 0), matrixRotacion, -sentido * 1f * this.env.ElapsedTime);
            //mesh.Transform = Matrix.Multiply(this.getMesh().Transform, mesh.Transform);


            // var m = Matrix.Scaling(mesh.Scale) * matrixRotacion * Matrix.Translation(this.getMesh().Position);
            //      Arma arma = new Arma(mesh, this.env, sonido, 20, orientacion, base.directionArrow.PEnd);

            //base.agregarArma(arma);
        }
        /*  private void creaMisilV(Vector3 posicion)
          {
              string sonido = env.MediaDir + "MySounds\\Launch4.wav";
              var loader = new TgcSceneLoader();
              var scene = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml");
              TgcMesh mesh = scene.Meshes[0];
              mesh.AutoTransformEnable = false;
              mesh.AutoUpdateBoundingBox = true;
              mesh.createBoundingBox();
           //   var m = Matrix.Scaling(new Vector3(0.5f, 0.5f, 0.5f)) * Matrix.RotationY(0.001f) * Matrix.Translation(posicion);
       //       mesh.Transform = m;
          //    mesh.Position = posicion;
              mesh.Position = posicion;
          //    mesh.rotateX(FastMath.PI);
           //   mesh.Scale = new Vector3(0.5f, 0.5f, 0.5f);
            //  mesh.move(0, 20, 0); ;
              Arma arma = new Arma(mesh, this.env, sonido, 20, this.orientacion, base.directionArrow.PEnd);
              ControladorDeVehiculos.getInstance().agregarArma(arma);
              base.agregarArma(arma);
          }*/
        protected override void aplicarEfecto()
        {
            tInicioHumo = FastMath.Abs(this.env.ElapsedTime);
            tFinHumo = tInicioHumo + tDuracionHumo;
            this.humoChoque.Update(this.getNuevaPosicion(), this.anguloFinal);
        }
        public virtual void Update()
        {
           // base.CalcularMeshesCercanos();
            base.setPosicionAnterior(base.getMesh().Position);
            base.setRotacionAnterior(base.getMesh().Rotation);
            base.setAlturaActual(base.getMesh().Position.Y);
            base.calculosDePosicion();
            base.updateTGCArrow();

            if (disparar())
            {
                base.startDisparo();
                creaDisparo(this.getMesh().Position);
            }
            if (disparaEspecial()) { 
                  base.startArma();
                //   creaMisilV(this.getMesh().Position);
                creaMisilV();

            }
            this.humoCañoEscape.Update(this.getNuevaPosicion(), this.anguloFinal);
 
            //Actualizar valores de la linea de movimiento
            //   directionArrow.PStart = this.getMesh().Position;
            //directionArrow.PEnd = this.getMesh().Position + Vector3.Multiply(this.getMesh().Position, 50);
            // directionArrow.updateValues();

            //Actualizar valores de normal de colision
            /*     if (this.env.GetManejadorDeColision().Manager().Collision)
                 {
                     collisionNormalArrow.PStart = this.env.GetManejadorDeColision().Manager().LastCollisionPoint;
                     collisionNormalArrow.PEnd = this.env.GetManejadorDeColision().Manager().LastCollisionPoint +
                                                 Vector3.Multiply(this.env.GetManejadorDeColision().Manager().LastCollisionNormal, 80);

                     collisionNormalArrow.updateValues();


                     collisionPoint.Position = this.env.GetManejadorDeColision().Manager().LastCollisionPoint;
                     collisionPoint.render();
                 }*/
        }
        public virtual void Render()
        {
            
            base.getMesh().render();
            this.lifeLevel.render();
            humoCañoEscape.Render(EfectoNitro);

            if (tInicioHumo < tFinHumo)
                humoChoque.Render(false); //si es momento de choque, muestro humito

            tInicioHumo = tInicioHumo + FastMath.Abs(this.env.ElapsedTime);
            //     base.getBoxDeColision().render();
            // base.getBoxDeColision().render();
            //   directionArrow.render();
            //   collisionNormalArrow.render();
            //  collisionPoint.render();

        }
        public void dispose()
        {
            base.getMesh().dispose();
            directionArrow.dispose();
        }

    }
}
