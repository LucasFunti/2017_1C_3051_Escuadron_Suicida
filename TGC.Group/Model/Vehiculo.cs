using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    class Vehiculo
    {
        private float VelocidadX = 0;
        private float VelocidadY = 0; //Para cuando Salta.
        private float const_aceleracion =0.5f;
        private float velocidad_maxima =30;
        private float velocidad_minima = -30;

        Core.Example.TgcExample env;
        private Vector3 posicion;
        private TgcMesh mesh;
        public float ElapsedTime { get; set; }

        public Vehiculo(TgcMesh Mesh, Vector3 pos)
        {
            this.mesh = Mesh;
            this.posicion = pos;
            this.VelocidadY = 0;
        }
        public Vehiculo(Core.Example.TgcExample env)
        {
            this.env = env;
            this.VelocidadY = 0;
        }
        public void setMesh(TgcMesh Mesh)
        {
            this.mesh = Mesh;
        }
        public TgcMesh getMesh()
        {
            return this.mesh;
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
        public virtual void Update()
        {
            calculosDePosicion();
        }
        public void Render()
        {
            this.mesh.render();
        }
        public void dispose()
        {
            this.mesh.dispose();
        }


        public float accelerar(int direccion)
        {
            this.VelocidadX = this.VelocidadX + (direccion*this.const_aceleracion);
            if (this.VelocidadX >= this.velocidad_maxima)
                this.VelocidadX = this.velocidad_maxima;
             
            if (this.VelocidadX <= this.velocidad_minima)
                this.VelocidadX = this.velocidad_minima;
  
            return this.VelocidadX;
        }

        public void calculosDePosicion()
        {

            //obtener velocidades de Modifiers
            var velocidadRotacion = 3f;

            //Calcular proxima posicion de personaje segun Input
            float rotate = 0;
            var moving = false;
            var rotating = false;
            ElapsedTime = 1;

            //Adelante
            if (this.moverAdelante())
            {
                // moveForward = -velocidadCaminar;
                this.accelerar(-1);
                moving = true;
            }

            //Atras
            if (this.moverAtras())
            {
                // moveForward = velocidadCaminar;
                this.accelerar(1);
                moving = true;
            }

            //Derecha
            if (this.moverADerecha())
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (this.moverAIzquierda())
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            if (this.getVelocidadX()> 0 && !moving)
            {
                this.accelerar(-1);
                moving = true;
            }
            if (this.getVelocidadX() < 0 && !moving)
            {
                this.accelerar(1);
                moving = true;
            }
            //Si hubo rotacion
            if (rotating)
            {

                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                //   var rotAngle = Geometry.DegreeToRadian(rotate * ElapsedTime);
                var rotAngle = (float)(System.Math.PI * (rotate * ElapsedTime) / 180.0f);
                this.getMesh().rotateY(rotAngle);
                this.rotarCamara(rotAngle);
                
                
            }

            //Si hubo desplazamiento
            if (moving)
            {
                //Activar animacion de caminando
                //  personaje.playAnimation("Caminando", true);
                //this.env.DrawText.drawText("Caminando", 50, 20, System.Drawing.Color.Red);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                var lastPos = this.getMesh().Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado

                //NO SE RECOMIENDA UTILIZAR! moveOrientedY mueve el personaje segun la direccion actual, realiza operaciones de seno y coseno.
                this.getMesh().moveOrientedY(this.getVelocidadX() * ElapsedTime);

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
        public virtual void rotarCamara(float rotacion)
        {
           
        }
    }
}
