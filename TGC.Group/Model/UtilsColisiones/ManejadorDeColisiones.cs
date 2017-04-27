using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class ManejadorDeColisiones
    {
        private readonly List<Colisionador> objetosColisionables = new List<Colisionador>();
        private SphereTriangleCollisionManager collisionManager;
        private bool Collision = false;
      
        public ManejadorDeColisiones()
        {
            collisionManager = new SphereTriangleCollisionManager();
            //     collisionManager.GravityEnabled = true;
        }

        public void addBoundingBoxMeshColisionable(TgcMesh mesh)
        {
            objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(mesh.BoundingBox));
        }
        public void addListOfBoundingBoxMeshesColisionables(List<TgcMesh> lista)
        {
            /*    foreach (var mesh in lista)
                {
                    objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(mesh.BoundingBox));
                 }*/
            foreach (var mesh in lista)
            {
                objetosColisionables.Add(BoundingBoxCollider.fromBoundingBox(mesh.BoundingBox));
            }
        }
        public List<Colisionador> getObjetosColisionables()
        {
            return this.objetosColisionables;
        }
        public SphereTriangleCollisionManager Manager()
        {
            return this.collisionManager;
        }
        public Vector3 moverConColision(TgcMesh personaje, Vector3 movementVector)
        {
            TgcBoundingSphere characterSphere = new TgcBoundingSphere(personaje.BoundingBox.calculateBoxCenter(),
               personaje.BoundingBox.calculateBoxRadius());
            var realMovement = collisionManager.moveCharacter(characterSphere, movementVector, objetosColisionables);
            
            Collision = collisionManager.Collision;
            characterSphere.dispose();
            if (collisionManager.Collision)
            {
                float x= -1*(movementVector.X * 3);
                float z =-1*(movementVector.Z * 3);
                if (movementVector.X == collisionManager.LastCollisionPoint.X) x = x * 2;
                if (movementVector.Z == collisionManager.LastCollisionPoint.Z) z = z * 2;

                    //     if (movementVector.X == collisionManager.LastCollisionPoint.X)
                    //       x = (-200);
                    //  if (movementVector.Z == collisionManager.LastCollisionPoint.Z)
                    //     z = -200;


                    return new Vector3(x, movementVector.Y, z);
            }
            //return realMovement;
                 return movementVector;
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
              }*/
        }
    }
}
