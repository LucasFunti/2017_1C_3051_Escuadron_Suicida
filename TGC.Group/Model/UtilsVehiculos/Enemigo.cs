using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model.UtilsVehiculos
{
    class Enemigo : Vehiculo
    {
        private int TiempoRetardo = 0; //Por ahora no hay retardo
        private int TiempoDisparo = 0;
        private int TiempoDisparoMisil = 0;
        private int TiempoRetardoDoblar = 0;
        private int contadorDeCiclos = 0;
        private bool adelante = false;
     //   private Vector3 TargetPosition; //la pos a la que voy.
        private VehiculoPrincipal autoP;
        private Boolean doblaD=false;
        private Boolean doblaI = false;
        private Vector3 targetPos;
        private Boolean bolDisparar = false;
        private Boolean bolDispararMisil = false;
        private List<Vector3> nodos = new List<Vector3>();

        public Enemigo(TwistedMetal env, TgcMesh mesh) : base(mesh, env)
        {

            base.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!

         }
        public void setAutoTarget(VehiculoPrincipal auto)
        {
            this.autoP = auto;
            targetPos = this.autoP.getMesh().Position;
        }
        List<Arma> listaDeArmasToRemove;
        public override void Update()
        {
            //Elimina las armas qeu esten flotando por más de 50 ciclos
            listaDeArmasToRemove = new List<Arma>();
            foreach (Arma arma in base.listaDeArmas)
                if (arma.getTiempoDeVida() > 20)
                     listaDeArmasToRemove.Add(arma);
          
            /*foreach (var item in listaDeArmasToRemove)
            {
                listaDeArmas.Remove(item);
                item.dispose();
            }*/

            TiempoRetardoDoblar++;
            contadorDeCiclos++;

            if (contadorDeCiclos < TiempoRetardo)
                return;

            contadorDeCiclos = 0;

            doblaD = false;
            doblaI = false;
            
            if(TiempoRetardoDoblar>10)
                ApuntarAlTarget();

            //siempre intenta avanzar;    
            adelante = true;
            
            //Cada 70 ciclos dispara
            bolDisparar = false;
            if (TiempoDisparo>150)
            {
                bolDisparar = true;
                TiempoDisparo = 0;
            }
            //Cada 150 ciclos dispara un misil 
            bolDispararMisil = false;
            if (TiempoDisparoMisil > 300)
            {
                bolDispararMisil = true;
                TiempoDisparoMisil = 0;
            }
            TiempoDisparoMisil++;
            TiempoDisparo++;

            base.Update();

            adelante = false;
        }
        public override void setPosicionInicial(Vector3 vect)
        {
        
            this.getMesh().Transform = Matrix.Scaling(new Vector3(1, 1, 1)) * Matrix.Translation(this.autoP.getMesh().Position - new Vector3(0, 0, -600));
            this.getMesh().Position = this.autoP.getMesh().Position - new Vector3(0, 0, -600);
        }
        private void ApuntarAlTarget()
        {
            //Se fija que el target haya cambiado de posicion
            if (targetPos == this.autoP.getMesh().Position)
                 return;

             targetPos = this.autoP.getMesh().Position;


             float X1 = this.getMesh().Position.X;
             float Z1 = this.getMesh().Position.Z;

             float X2 = this.autoP.getMesh().Position.X;
             float Z2 = this.autoP.getMesh().Position.Z;

             float ang = FastMath.Atan2((Z2 - Z1), (X2 - X1));


             if (base.anguloFinal != (FastMath.PI * 3 / 2) - ang)
             {
                 setAnguloFinal((FastMath.PI * 3 / 2) - ang);
                float f = this.autoP.orientacion;
                 if (ang >= 0 && ang <= 3)
                     doblaD=true;
                 if (ang < 0 && ang >= -3)
                     doblaI = true;
             }
        }
        public override bool disparar()
        {
            return bolDisparar;
        }
        public override bool disparaEspecial()
        {
            return bolDispararMisil;
        }
        private Vector3 nodoMasCercano(Vector3 posActual, Vector3 rtn_nodo, float d)
        {
            float daux=0;
            foreach (Vector3 nodo in this.nodos)
            {
                if (nodo != rtn_nodo)
                {
                    daux = Vector3.Length(nodo - posActual);
                    if (daux <= d && daux != d)
                    {
                        d = daux;
                        rtn_nodo = nodoMasCercano(posActual, nodo, d);
                    }
                }
            }
            return rtn_nodo;
        }

        //mueve el objeto.
        /*   public override void mover()
           {
               Vector3 nuevaPos = this.calcularProximaPosicion();
               base.getBoxDeColision().Center = calcularCentroDelBox();


               Vector3 scale3 = new Vector3(1f, 1f, 1f);
               var m = Matrix.Scaling(scale3) * base.matrixRotacion * Matrix.Translation(nuevaPos);

               base.getMesh().Transform = m;
               base.getMesh().Position = nuevaPos;


               if (!this.getEsRueda())
               {
                   base.ProcesarChoques();
               }


           }*/
        //Calcula la próxima posicion del objeto en base a los datos de velocidad.
        /*    protected override Vector3 calcularProximaPosicion()
          {
              return new Vector3(this.getMesh().Position.X + this.getVelocidadX() * (float)System.Math.Cos(this.orientacion),
                                                    this.getMesh().Position.Y + this.getVelocidadY(),
                                                    this.getMesh().Position.Z + this.getVelocidadX() * (float)System.Math.Sin(this.orientacion));

          }
          //Calcula el centro de rotación del box de colisiones
                   protected override Vector3 calcularCentroDelBox()
                  {
                      return new Vector3(this.getMesh().Position.X + this.getVelocidadX() * (float)Math.Cos(base.anguloFinal),
                        base.getBoxDeColision().Position.Y + this.getVelocidadY(), this.getMesh().Position.Z + this.getVelocidadX() * (float)Math.Sin(base.anguloFinal));
                  }
              protected override void doblar(float sentido)
             {
                 float X1 = this.getMesh().Position.X;
                 float Z1 = this.getMesh().Position.Z;

                 float X2 = this.autoP.getMesh().Position.X;
                 float Z2 = this.autoP.getMesh().Position.Z;

                 float ang = FastMath.Atan2((Z2 - Z1), (X2 - X1));

                     setAnguloFinal((FastMath.PI * 3 / 2) - ang);
                     base.matrixRotacion = Matrix.RotationY(base.anguloFinal);
                     base.getMesh().Transform = base.matrixRotacion;
                     base.getBoxDeColision().rotate(new Vector3(0, (FastMath.PI * 3 / 2) - ang, 0));
             }*/
        public override bool moverAIzquierda()
        {

            return doblaI;
        }
        public override bool moverADerecha()
        {

            return doblaD;
        }
        public override bool moverAdelante()
        {
            return adelante;
        }

        private void testNextPosition()
        {

        }
    }
}
