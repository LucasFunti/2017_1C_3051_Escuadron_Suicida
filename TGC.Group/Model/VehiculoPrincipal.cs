using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;


namespace TGC.Group.Model
{
    class VehiculoPrincipal : Vehiculo
    {
        
        private Vector3 pos;
        private TgcSceneLoader loader;
        private CamaraTerceraPersona camaraInterna;
        private Core.Example.TgcExample env;

        public VehiculoPrincipal(Core.Example.TgcExample env) : base(env)
        {
            loader = new TgcSceneLoader();
            this.env = env;
            var sceneHummer = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Vehiculos\\Hummer\\Hummer-TgcScene.xml");
            TgcMesh mesh = sceneHummer.Meshes[0];
            mesh.AutoTransformEnable = true;
            mesh.move(0, 5, 0);
            this.setMesh(mesh);

            camaraManager();

        }

        private void camaraManager()
        {
            camaraInterna = new CamaraTerceraPersona(this.getMesh().Position, 100, 300);
            this.env.Camara = camaraInterna;
        }

        public override void Update()
        {

            //Hacer que la camara siga al personaje en su nueva posicion
            this.calculosDePosicion();
            camaraInterna.Target = this.getMesh().Position;
        }
        public override void calculosDePosicion()
        {

            //obtener velocidades de Modifiers
            var velocidadCaminar = 10f;
            var velocidadRotacion = 3f;

            //Calcular proxima posicion de personaje segun Input
            var moveForward = 0f;
            float rotate = 0;
            var moving = false;
            var rotating = false;
            ElapsedTime = 1;

            //Adelante
            if (this.env.Input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (this.env.Input.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (this.env.Input.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (this.env.Input.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                //   var rotAngle = Geometry.DegreeToRadian(rotate * ElapsedTime);
                var rotAngle = (float)(System.Math.PI * (rotate * ElapsedTime) / 180.0f);
                this.getMesh().rotateY(rotAngle);
                camaraInterna.rotateY(rotAngle);
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
                this.getMesh().moveOrientedY(moveForward * ElapsedTime);

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
    }
}
