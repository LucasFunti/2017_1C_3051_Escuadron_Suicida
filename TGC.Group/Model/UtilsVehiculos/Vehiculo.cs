using Microsoft.DirectX;
using TGC.Core.SceneLoader;


namespace TGC.Group.Model
{
    class Vehiculo : ObjetoConMovimiento
    {
        
        TwistedMetal env;
       

        public Vehiculo(TgcMesh Mesh, Vector3 pos, TwistedMetal env) : base(env)
        {
            this.env = env;
            base.setMesh(Mesh);
            base.getMesh().move(pos);
            base.setVelocidadY(0);
            base.setAluraMaxima(20);
      //      direcionadores();
        }
        public Vehiculo(TwistedMetal env) : base(env)
        {
            this.env = env;
            this.setVelocidadY(0);
            base.setAluraMaxima(100);
        //    direcionadores();
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
       
       
        public virtual void Update()
        {
            base.setPosicionAnterior(base.getMesh().Position);
            base.setRotacionAnterior(base.getMesh().Rotation);
            base.setAlturaActual(base.getMesh().Position.Y);
            base.calculosDePosicion();
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
            base.getBoxDeColision().render();
            directionArrow.render();
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
