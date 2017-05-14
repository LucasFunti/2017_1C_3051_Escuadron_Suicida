using Microsoft.DirectX;
using System;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class ObjetoConMovimiento
    {
        //Movimiento Horizontal
        private float VelocidadX = 0;
        private float const_aceleracionX = 15f;
        private float friccion = 0.2f;
        private float velocidad_maxima = 0;
        private float velocidad_minima = 0;
        public float orientacion = 270 * (float)Math.PI / 180;
        private float velocidadRotacion = 1f;
        //Movimiento Vertical
        private float VelocidadY = 0; //Para cuando Salta.
        private float const_aceleracionY = 0.5f;
        private float alturaMax = 0f;
        private float alturaActual = 5f;
        private float alturaInicial = 5f;
        private float gravedad = 0.3f; //Para cuando cae.
        private bool subiendo = false;
        TwistedMetal env;
        public Matrix matrixRotacion;
        public float anguloFinal = 0;
        //Mesh
        private TgcMesh mesh;
        private TgcBoundingOrientedBox boxDeColision;
        private float largoDelMesh;
        private float boxDeColisionY;
        private bool collisionFound;
        private bool chocoAdelante = false;
        private Vector3 NuevaPosicion;
        private Vector3 PosicionAnterior { get; set; }
        private Vector3 RotacionAnterior { get; set; }
      //  private TgcArrow collisionNormalArrow;
       // private TgcBox collisionPoint;
        public TgcArrow directionArrow;
     
        public ObjetoConMovimiento(TwistedMetal env)
        {
            this.env = env;
        }
       
        public void setMesh(TgcMesh Mesh)
        {
            this.mesh = Mesh;
            initBoxColisionador();
            this.updateTGCArrow(this.calcularRayoDePosicion());
        }
        public TgcMesh getMesh()
        {
            return this.mesh;
        }

        private void initBoxColisionador()
        {
            this.getMesh().AutoTransformEnable = false;
            this.getMesh().AutoUpdateBoundingBox = false;
            boxDeColision = TgcBoundingOrientedBox.computeFromAABB(this.getMesh().BoundingBox);
            var yMin = this.getMesh().BoundingBox.PMin.Y;
            var yMax = this.getMesh().BoundingBox.PMax.Y;
            boxDeColision.Extents = new Vector3(boxDeColision.Extents.X, boxDeColision.Extents.Y, boxDeColision.Extents.Z * -1);

            largoDelMesh = boxDeColision.Extents.Z;
            boxDeColisionY = (yMax + yMin) / 2 + yMin;
        }
        protected TgcBoundingOrientedBox getBoxDeColision()
        {
            return this.boxDeColision;
        }
        protected void setRotacionAnterior(Vector3 v)
        {
            this.RotacionAnterior = v;
        }
        protected void setPosicionAnterior(Vector3 v)
        {
            this.PosicionAnterior = v;
        }
        public float getAlturaActual()
        {
            return this.alturaActual;
        }
        public void setAlturaActual(float alt)
        {
            this.alturaActual= alt;
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

            if(direccion<0)
                this.setVelocidadX(this.VelocidadX + (direccion * (aceleracion*2)));
            else
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
          
            movingY = ProcesarMovimientoEnY();
            if (!movingY)
            {
                movingX = ProcesarMovimientoEnX();
                rotate = ProcesarRotacion();
            }
             //Rota solo si hay movimiento en X y no hay movimiento en Y
            if (rotate!=0 && movingX && !movingY)
                     this.doblar(rotate);

            //Si hubo rotacion y no movimiento mover las ruedas unicamente
            if (rotate != 0 && !movingX)
            {
            }

            //Si hubo desplazamiento
            if (movingX || movingY)
                   this.mover();
             
            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                //  mesh.playAnimation("Parado", true);
            }
        }
        protected void doblar(float sentido)
        {
            sentido = sentido * this.getVelocidadRotacion();
            orientacion += sentido * 1f * this.env.ElapsedTime;

            anguloFinal = anguloFinal - sentido * 1f * this.env.ElapsedTime;
            matrixRotacion = Matrix.RotationY(anguloFinal);
            this.rotar(new Vector3(0, -sentido * 1f * this.env.ElapsedTime, 0), matrixRotacion, -sentido * 1f * this.env.ElapsedTime);
            
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
        public virtual void ProcesarMovimientoDeCamara(float offsetHeight, float offsetForward)
        {
           
        }
        public bool ProcesarMovimientoEnX()
        {
            bool movingX=false;

            //Adelante
            if (this.moverAdelante())
            {
                // moveForward = -velocidadCaminar;
                this.accelerarX(1, this.getConstanteDeAsceleracionX());
                movingX = true;
            }

            //Atras
            if (this.moverAtras())
            {
                // moveForward = velocidadCaminar;
                this.accelerarX(-1, this.getConstanteDeAsceleracionX());
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
                //   return this.getVelocidadRotacion();
                return -1;

            //Izquierda
            if (this.moverAIzquierda())
                //  return -this.getVelocidadRotacion();
                return 1;
         
            return 0;
        }

        public virtual void rotar(Vector3 v,Matrix m,float rotaCamara)
        {
            this.getMesh().Transform = m;
            this.boxDeColision.rotate(v);
           
        }
        //mueve el objeto.
        public virtual void mover()
        {
            NuevaPosicion = this.calcularProximaPosicion();
            this.boxDeColision.Center = calcularCentroDelBox();

            Vector3 scale3 = new Vector3(1f, 1f, 1f);
            var m = Matrix.Scaling(scale3) * this.matrixRotacion * Matrix.Translation(NuevaPosicion);

            this.getMesh().Transform = m;
            this.getMesh().Position = NuevaPosicion;

            ProcesarColisiones();

        }
        //Calcula la próxima posicion del objeto en base a los datos de velocidad.
        protected Vector3 calcularProximaPosicion()
        {
            return new Vector3(this.getMesh().Position.X + this.getVelocidadX() * (float)System.Math.Cos(this.orientacion),
                                                  this.getMesh().Position.Y + this.getVelocidadY(),
                                                  this.getMesh().Position.Z + this.getVelocidadX() * (float)System.Math.Sin(this.orientacion));

        }
        //Calcula el centro de rotación del box de colisiones
        protected Vector3 calcularCentroDelBox()
        {
            return new Vector3(this.getMesh().Position.X + this.getVelocidadX() * (float)System.Math.Cos(this.orientacion),
              boxDeColisionY + this.getVelocidadY(), this.getMesh().Position.Z + this.getVelocidadX() * (float)System.Math.Sin(this.orientacion));
        }
        private void ProcesarColisiones()
        {
            collisionFound = false;
            chocoAdelante = false;
            var ray = calcularRayoDePosicion();
       
            foreach (var mesh in this.env.GetManejadorDeColision().MeshesColicionables)
            {
                var escenaAABB = mesh.BoundingBox;

                /*Si choca sale del bucle*/
                if (TgcCollisionUtils.testObbAABB(this.boxDeColision, escenaAABB))
                {
                    collisionFound = true;
                    float t;
                    Vector3 p;
                    chocoAdelante = (intersectRayAABB(ray, escenaAABB, out t, out p) || t > 1.0f);

                    break;
                }
            }
            /*Si choca se pone el box de choque en DarkRed*/
            if (collisionFound)
            {
                if (this.getMesh().Position.Y == 5 || this.getMesh().Position.Y >= 25)
                {
                    this.boxDeColision.setRenderColor(Color.DarkRed);
                    VolverAPosicionAnterior();
                }
            }
            else
            {
                this.boxDeColision.setRenderColor(Color.Yellow);
            }

            if (this.getMesh().Position.Y == 5)
            {
                ManejarColisionCamara();
            }
      }
        private TgcRay.RayStruct calcularRayoDePosicion()
        {
            var ray = new TgcRay.RayStruct();
            var x1 = -this.largoDelMesh * FastMath.Sin(anguloFinal);
            var z1 = -this.largoDelMesh * FastMath.Cos(anguloFinal);

            var x2 = x1 * 1.2;
            var z2 = z1 * 1.2;

            ray.origin = new Vector3(
                this.getMesh().Position.X + x1,
                this.getMesh().Position.Y,
                this.getMesh().Position.Z + z1);

            ray.direction = new Vector3(NuevaPosicion.X + (float)x2,
                NuevaPosicion.Y,
                NuevaPosicion.Z + (float)z2
                );
            return ray;
        }

        private void updateTGCArrow(TgcRay.RayStruct rayo)
        {
            directionArrow = new TgcArrow();
            directionArrow.Thickness = 5;
            directionArrow.BodyColor = Color.Red;
            directionArrow.HeadColor = Color.Green;
            directionArrow.HeadSize = new Vector2(10, 10);

            directionArrow.PStart = rayo.origin;
            directionArrow.PEnd = rayo.direction;
            directionArrow.updateValues();
        }


        public void VolverAPosicionAnterior()
        {
            this.getMesh().Position = PosicionAnterior;
            this.getMesh().Rotation = RotacionAnterior;
            this.setVelocidadX(-1 * this.getVelocidadX() * 0.5f);
        }
        public bool intersectRayAABB(TgcRay.RayStruct ray, TgcBoundingAxisAlignBox aabb, out float tmin,
          out Vector3 q)
        //, out float tmin, out Vector3 q)
        {
            var aabbMin = TgcCollisionUtils.toArray(aabb.PMin);
            var aabbMax = TgcCollisionUtils.toArray(aabb.PMax);
            var p = TgcCollisionUtils.toArray(ray.origin);
            var d = TgcCollisionUtils.toArray(ray.direction);

            tmin = 0.0f; // set to -FLT_MAX to get first hit on line
            var tmax = float.MaxValue; // set to max distance ray can travel (for segment)
            q = Vector3.Empty;

            // For all three slabs
            for (var i = 0; i < 3; i++)
            {
                if (FastMath.Abs(d[i]) < float.Epsilon)
                {
                    // Ray is parallel to slab. No hit if origin not within slab
                    if (p[i] < aabbMin[i] || p[i] > aabbMax[i]) return false;
                }
                else
                {
                    // Compute intersection t value of ray with near and far plane of slab
                    var ood = 1.0f / d[i];
                    var t1 = (aabbMin[i] - p[i]) * ood;
                    var t2 = (aabbMax[i] - p[i]) * ood;
                    // Make t1 be intersection with near plane, t2 with far plane
                    if (t1 > t2) TgcCollisionUtils.swap(ref t1, ref t2);
                    // Compute the intersection of slab intersection intervals
                    tmin = TgcCollisionUtils.max(tmin, t1);
                    tmax = TgcCollisionUtils.min(tmax, t2);
                    // Exit with no collision as soon as slab intersection becomes empty
                    if (tmin > tmax) return false;
                }
            }
            // Ray intersects all 3 slabs. Return point (q) and intersection t value (tmin)
            q = ray.origin + ray.direction * tmin;
            return true;
        }
        public virtual void ManejarColisionCamara()
        {

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
        public virtual bool moverArriba()
        {
            return false;
        }
        public virtual bool moverAbajo()
        {
            return false;
        }
        public virtual bool cambiarCamara()
        {
            return false;
        }
    }
}
