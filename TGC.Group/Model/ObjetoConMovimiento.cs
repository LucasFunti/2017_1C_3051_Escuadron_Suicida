using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class ObjetoConMovimiento
    {
        //Movimiento Horizontal
        private float VelocidadX = 0;
        private float const_aceleracionX = 0.4f;
        private float friccion = 0.2f;
        private float velocidad_maxima = 0;
        private float velocidad_minima = 0;
        //Movimiento Vertical
        private float VelocidadY = 0; //Para cuando Salta.
        private float const_aceleracionY = 0.5f;
        private float alturaMax = 0f;
        private float alturaActual = 5f;
        private float alturaInicial = 5f;
        private float gravedad = 1f; //Para cuando Salta.
        private bool subiendo = false;
       

        private float velocidadRotacion = 2f;
        public float ElapsedTime { get; set; }
        private Vector3 posicion;
        public float getAlturaActual()
        {
            return this.alturaActual;
        }
        public void setAlturaActual(float alt)
        {
            this.alturaActual= alt;
        }
        public void setPosicion(Vector3 posicion)
        {
            this.posicion = posicion;
        }
        public void setVelocidadX(float NuevaVelocidadX)
        {
            this.VelocidadX = NuevaVelocidadX;
        }
        public float getVelocidadX()
        {
            return this.VelocidadX;
        }
        public void setVelocidadY(float NuevaVelocidadY)
        {
            this.VelocidadY = NuevaVelocidadY;
        }
        public float getVelocidadY()
        {
            return this.VelocidadY;
        }
        public float getVelocidadMaxima()
        {
            return this.velocidad_maxima;
        }
        public void setVelocidadMaxima(float VelocidadMax)
        {
            this.velocidad_maxima= VelocidadMax;
        }
        public float getVelocidadMinima()
        {
            return this.velocidad_minima;
        }
        public void setVelocidadMinima(float VelocidadMin)
        {
            this.velocidad_minima = VelocidadMin;
        }
        public float getConstanteDeAsceleracionX()
        {
            return this.const_aceleracionX;
        }
        public void setConstanteDeAsceleracionX(float c_aceleracion)
        {
            this.const_aceleracionX = c_aceleracion;
        }
        public float getConstanteDeAsceleracionY()
        {
            return this.const_aceleracionY;
        }
        public void setConstanteDeAsceleracionY(float c_aceleracion)
        {
            this.const_aceleracionY = c_aceleracion;
        }
        public float getFriccion()
        {
            return this.friccion;
        }
        public void setFriccion(float c_friccion)
        {
            this.friccion = c_friccion;
        }
        public float getAluraMaxima()
        {
            return this.alturaMax;
        }
        public void setAluraMaxima(float alturaMaxima)
        {
            this.alturaMax = alturaMaxima;
        }
        public float getGravedad()
        {
            return this.gravedad;
        }
        public float accelerarX(int direccion,float aceleracion)
        {
            this.setVelocidadX(this.VelocidadX + (direccion * aceleracion));
  
            if (this.getVelocidadX() >= this.getVelocidadMaxima())
                this.setVelocidadX(this.getVelocidadMaxima());

            if (this.getVelocidadX() <= this.getVelocidadMinima())
                this.setVelocidadX(this.getVelocidadMinima());

            return this.getVelocidadX();
        }
        public float accelerarY(int direccion)
        {

            if (direccion > 0)
            {
                this.setVelocidadY(this.VelocidadY + (direccion * this.getConstanteDeAsceleracionY()));
                if ((alturaActual + 1) > this.getAluraMaxima())
                {
                    subiendo = false;
                    this.setVelocidadY(0);
                }
                else
                    alturaActual = alturaActual + 1;
            }
            else
            {
                this.setVelocidadY(this.VelocidadY + (direccion * this.getGravedad()));
                alturaActual = alturaActual - 1;
            }
            return this.getVelocidadY();
        }
       
        public float getVelocidadRotacion()
        {
            return this.velocidadRotacion;
        }
        public void setVelocidadRotacion(float VRotacion)
        {
            this.velocidadRotacion= VRotacion;
        }
        public void calculosDePosicion()
        {

            //Calcular proxima posicion de personaje segun Input
            float rotate = 0;
            var movingX = false;
            var movingY = false;
            var rotating = false;
            ElapsedTime = 1;

            movingY = ProcesarMovimientoEnY();
            if (!movingY)
            {
                movingX = ProcesarMovimientoEnX();
                rotate = ProcesarRotacion();
            }
             //Rota solo si hay movimiento en X y no hay movimiento en Y
            if (rotate!=0 && movingX && !movingY)
            {

                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                //   var rotAngle = Geometry.DegreeToRadian(rotate * ElapsedTime);
                var rotAngle = (float)(System.Math.PI * (rotate * ElapsedTime) / 180.0f);
                this.rotar(rotAngle);

                //Si hubo rotacion y no movimiento mover las ruedas unicamente
                if (rotating && !movingX)
                {
                }
            }
           

            //Si hubo desplazamiento
            if (movingX || movingY)
            {
                this.mover();
                //Activar animacion de caminando
                //  personaje.playAnimation("Caminando", true);
                //this.env.DrawText.drawText("Caminando", 50, 20, System.Drawing.Color.Red);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
             //   var lastPos = this.getMesh().Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado

                //NO SE RECOMIENDA UTILIZAR! moveOrientedY mueve el personaje segun la direccion actual, realiza operaciones de seno y coseno.
               // this.getMesh().moveOrientedY(this.getVelocidadX() * ElapsedTime);

                //Detectar colisiones
                /*  var collide = false;
                  //Guardamos los objetos colicionados para luego resolver la respuesta. (para este ejemplo simple es solo 1 caja)
                  TgcBox collider = null;
                  foreach (var obstaculo in obstaculos)
                  {
                      if (TgcCollisionUtils.testAABBAABB(personaje.BoundingBox, obstaculo.BoundingBox))
                      {
                          collide = true;
                          collider = obstaculo;
                          break;
                      }
                  }
                  */
                //Si hubo colision, restaurar la posicion anterior, CUIDADO!!!!!
                //Hay que tener cuidado con este tipo de respuesta a colision, puede darse el caso que el objeto este parcialmente dentro en este y en el frame anterior.
                //para solucionar el problema que tiene hacer este tipo de respuesta a colisiones y que los elementos no queden pegados hay varios algoritmos y hacks.
                //almacenar la posicion anterior no es lo mejor para responder a una colision.
                //Una primera aproximacion para evitar que haya inconsistencia es realizar sliding
                /*  if (collide)
                  {
                      //si no esta activo el sliding es la solucion anterior de este ejemplo.
                      if (!(bool)Modifiers["activateSliding"])
                      {
                          personaje.Position = lastPos; //Por como esta el framework actualmente esto actualiza el BoundingBox.
                          text = "";
                      }
                      else
                      {
                          //La idea del slinding es simplificar el problema, sabemos que estamos moviendo bounding box alineadas a los ejes.
                          //Significa que si estoy colisionando con alguna de las caras de un AABB los planos siempre son los ejes coordenados.
                          //Entones creamos un rayo de movimiento, esto dado por la posicion anterior y la posicion actual.
                          var movementRay = lastPos - personaje.Position;
                          //Luego debemos clasificar sobre que plano estamos chocando y la direccion de movimiento
                          //Para todos los casos podemos deducir que la normal del plano cancela el movimiento en dicho plano.
                          //Esto quiere decir que podemos cancelar el movimiento en el plano y movernos en el otros.
                          var t = "";
                          var rs = Vector3.Empty;
                          if (((personaje.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                              (personaje.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0)) &&
                              ((personaje.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                              (personaje.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0)))
                          {
                              //Este primero es un caso particularse dan las dos condiciones simultaneamente entonces para saber de que lado moverse hay que hacer algunos calculos mas.
                              //por el momento solo se esta verificando que la posicion actual este dentro de un bounding para moverlo en ese plano.
                              t += "Coso conjunto!\n" +
                                  "PMin X: " + personaje.BoundingBox.PMin.X + " - " + collider.BoundingBox.PMin.X + "\n" +
                                  "PMax X: " + personaje.BoundingBox.PMax.X + " - " + collider.BoundingBox.PMax.X + "\n" +
                                  "PMin Z: " + personaje.BoundingBox.PMin.Z + " - " + collider.BoundingBox.PMin.Z + "\n" +
                                  "PMax Z: " + personaje.BoundingBox.PMax.Z + " - " + collider.BoundingBox.PMax.Z + "\n" +
                                  "Last X: " + (lastPos.X - rs.X) + " - Z: " + (lastPos.Z - rs.Z) + "\n" +
                                  "Actual X: " + (personaje.Position.X) + " - Z: " + (personaje.Position.Z) + "\n" +
                                  "move X: " + (movementRay.X) + " - Z: " + (movementRay.Z);
                              if (personaje.Position.X > collider.BoundingBox.PMin.X &&
                                  personaje.Position.X < collider.BoundingBox.PMax.X)
                              {
                                  //El personaje esta contenido en el bounding X
                                  t += "\n Sliding Z Dentro de X";
                                  rs = new Vector3(movementRay.X, movementRay.Y, 0);
                              }
                              if (personaje.Position.Z > collider.BoundingBox.PMin.Z &&
                                  personaje.Position.Z < collider.BoundingBox.PMax.Z)
                              {
                                  //El personaje esta contenido en el bounding Z
                                  t += "\n Sliding X Dentro de Z";
                                  rs = new Vector3(0, movementRay.Y, movementRay.Z);
                              }

                              //Seria ideal sacar el punto mas proximo al bounding que colisiona y chequear con eso, en ves que con la posicion.

                          }
                          else
                          {
                              if ((personaje.BoundingBox.PMax.X > collider.BoundingBox.PMax.X && movementRay.X > 0) ||
                                  (personaje.BoundingBox.PMin.X < collider.BoundingBox.PMin.X && movementRay.X < 0))
                              {
                                  t += "Sliding X";
                                  rs = new Vector3(0, movementRay.Y, movementRay.Z);
                              }
                              if ((personaje.BoundingBox.PMax.Z > collider.BoundingBox.PMax.Z && movementRay.Z > 0) ||
                                  (personaje.BoundingBox.PMin.Z < collider.BoundingBox.PMin.Z && movementRay.Z < 0))
                              {
                                  t += "Sliding Z";
                                  rs = new Vector3(movementRay.X, movementRay.Y, 0);
                              }
                          }
                          text = t;
                          personaje.Position = lastPos - rs;
                          //Este ejemplo solo se mueve en X y Z con lo cual realizar el test en el plano Y no tiene sentido.

                      }
                  }
                  */


            }

            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                //  mesh.playAnimation("Parado", true);
            }
        }
        private bool ProcesarMovimientoEnY()
        {
            bool movingY = false;

            if (this.moverArriba() && this.alturaActual == this.alturaInicial)
                subiendo = true;

            if (this.moverAbajo())
            {
                //       this.accelerarY(-1);
                //     movingY = true;
            }
            //Mover en Y (Altura)
            if (subiendo)
            {
                this.accelerarY(1);
                movingY = true;
            }

            //Mover en Y (Altura)
            if (!movingY && this.alturaActual > this.alturaInicial && !subiendo)
            {
                this.accelerarY(-1);
                movingY = true;
            }

            if (!movingY && ((this.alturaInicial > this.alturaActual) || this.getVelocidadY() != 0))
            {
                if (this.alturaInicial > this.alturaActual)
                    this.setVelocidadY(this.alturaInicial - this.alturaActual);
                else
                    this.setVelocidadY(0);

                movingY = true;
            }

            return movingY;
        }
        public bool ProcesarMovimientoEnX()
        {
            bool movingX=false;

            //Adelante
            if (this.moverAdelante())
            {
                // moveForward = -velocidadCaminar;
                this.accelerarX(-1, this.getConstanteDeAsceleracionX());
                movingX = true;
            }

            //Atras
            if (this.moverAtras())
            {
                // moveForward = velocidadCaminar;
                this.accelerarX(1, this.getConstanteDeAsceleracionX());
                movingX = true;
            }
            //Si -1 < velocidad > 1 entonces está frenado.
            if (this.getVelocidadX() > -1 && this.getVelocidadX() < 1 && !movingX)
                       this.setVelocidadX(0);
            

            if (this.getVelocidadX() > 0 && !movingX)
            {
                this.accelerarX(-1, this.getFriccion());
                movingX = true;
            }
            if (this.getVelocidadX() < 0 && !movingX)
            {
                this.accelerarX(1, this.getFriccion());
                movingX = true;
            }
            return movingX;
        }
        public float ProcesarRotacion()
        {
            //Derecha
            if (this.moverADerecha())
            return this.getVelocidadRotacion();
          
            //Izquierda
            if (this.moverAIzquierda())
              return -this.getVelocidadRotacion();
         
            return 0;
        }
        public virtual bool moverAdelante()
        {
            return false;
        }
        public virtual bool moverAtras()
        {
            return false;
        }
        public virtual bool moverADerecha()
        {
            return false;
        }
        public virtual bool moverAIzquierda()
        {
            return false;
        }
        public virtual void rotar(float rotacion)
        {

        }
        public virtual void mover()
        {

        }
        public virtual bool moverArriba()
        {
            return false;
        }
        public virtual bool moverAbajo()
        {
            return false;
        }
    }
}
