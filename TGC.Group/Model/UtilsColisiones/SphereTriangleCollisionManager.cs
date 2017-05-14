using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TGC.Core.Collision;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Herramienta para realizar el movimiento de una Esfera con detección de colisiones,
    ///     efecto de Sliding y gravedad.
    ///     Basado en el paper de Kasper Fauerby
    ///     http://www.peroxide.dk/papers/collision/collision.pdf
    ///     Su utiliza una estrategia distinta al paper en el nivel más bajo de colisión.
    ///     No se analizan colisiones a nivel de tríangulo, sino que todo objeto se descompone
    ///     a nivel de un BoundingBox con 6 caras rectangulares.
    /// </summary>
    public class SphereTriangleCollisionManager
    {
        private const float EPSILON = 0.05f;
        private static readonly Vector3 UP_VECTOR = new Vector3(0, 1, 0);

        private readonly TgcBoundingSphere movementSphere;

        private readonly List<Colisionador> objetosCandidatos;

        private Colisionador lastCollider;

        public SphereTriangleCollisionManager()
        {
            GravityEnabled = true;
            GravityForce = new Vector3(0, -10, 0);
            SlideFactor = 1.3f;
            LastCollisionNormal = Vector3.Empty;
            movementSphere = new TgcBoundingSphere();
            objetosCandidatos = new List<Colisionador>();
            lastCollider = null;
            OnGroundMinDotValue = 0.8f;
            Collision = false;
            LastMovementVector = Vector3.Empty;
        }

        /// <summary>
        ///     Vector que representa la fuerza de gravedad.
        ///     Debe tener un valor negativo en Y para que la fuerza atraiga hacia el suelo
        /// </summary>
        public Vector3 GravityForce { get; set; }

        /// <summary>
        ///     Habilita o deshabilita la aplicación de fuerza de gravedad
        /// </summary>
        public bool GravityEnabled { get; set; }

        /// <summary>
        ///     Multiplicador de la fuerza de Sliding
        /// </summary>
        public float SlideFactor { get; set; }

        /// <summary>
        ///     Normal de la ultima superficie con la que hubo colision
        /// </summary>
        public Vector3 LastCollisionNormal { get; private set; }

        /// <summary>
        ///     Valor que indica la maxima pendiente que se puede trepar sin empezar
        ///     a sufrir los efectos de gravedad. Valor entre [0, 1] siendo 0 que puede
        ///     trepar todo y 1 que no puede trepar nada.
        ///     El valor Y de la normal de la superficie contra la que se colisiona tiene
        ///     que ser superior a este parametro para permitir trepar la pendiente.
        /// </summary>
        public float OnGroundMinDotValue { get; set; }

        /// <summary>
        ///     Ultimo punto de colision
        /// </summary>
        public Vector3 LastCollisionPoint { get; private set; }

        /// <summary>
        ///     Indica si hubo colision
        /// </summary>
        public bool Collision { get; private set; }

        /// <summary>
        ///     Ultimo vector de desplazamiento real.
        ///     Indica lo que realmente se pudo mover.
        /// </summary>
        public Vector3 LastMovementVector { get; private set; }

        /// <summary>
        ///     Mover BoundingSphere con detección de colisiones, sliding y gravedad.
        ///     Se actualiza la posición del centrodel BoundingSphere.
        /// </summary>
        /// <param name="characterSphere">BoundingSphere del cuerpo a mover</param>
        /// <param name="movementVector">Movimiento a realizar</param>
        /// <param name="colliders">Obstáculos contra los cuales se puede colisionar</param>
        /// <returns>Desplazamiento relativo final efecutado al BoundingSphere</returns>
        public Vector3 moveCharacter(TgcBoundingSphere characterSphere, Vector3 movementVector, List<Colisionador> colliders)
        {
            var originalSphereCenter = characterSphere.Center;

            //Mover
            collideWithWorld(characterSphere, movementVector, colliders, true, 1);

            //Aplicar gravedad
            if (GravityEnabled)
            {
                collideWithWorld(characterSphere, GravityForce, colliders, true, OnGroundMinDotValue);
            }

            //Calcular el desplazamiento real que hubo
            LastMovementVector = characterSphere.Center - originalSphereCenter;
            return LastMovementVector;
        }

        /// <summary>
        ///     Detección de colisiones, filtrando los obstaculos que se encuentran dentro del radio de movimiento
        /// </summary>
        private void collideWithWorld(TgcBoundingSphere characterSphere, Vector3 movementVector,
            List<Colisionador> colliders, bool sliding, float slidingMinY)
        {
            //Ver si la distancia a recorrer es para tener en cuenta
            var distanceToTravelSq = movementVector.LengthSq();
            if (distanceToTravelSq < EPSILON)
            {
                return;
            }

            //Dejar solo los obstáculos que están dentro del radio de movimiento de la esfera
            var halfMovementVec = Vector3.Multiply(movementVector, 0.5f);
            movementSphere.setValues(
                characterSphere.Center + halfMovementVec,
                halfMovementVec.Length() + characterSphere.Radius
                );
            objetosCandidatos.Clear();
            foreach (var collider in colliders)
            {
                if (collider.Enable && TgcCollisionUtils.testSphereSphere(movementSphere, collider.BoundingSphere))
                {
                    objetosCandidatos.Add(collider);
                }
            }

            //Detectar colisiones y deplazar con sliding
            doCollideWithWorld(characterSphere, movementVector, objetosCandidatos, 0, movementSphere, sliding,
                slidingMinY);
        }

        /// <summary>
        ///     Detección de colisiones recursiva
        /// </summary>
        public void doCollideWithWorld(TgcBoundingSphere characterSphere, Vector3 movementVector,
            List<Colisionador> colliders, int recursionDepth, TgcBoundingSphere movementSphere, bool sliding,
            float slidingMinY)
        {
            //Limitar recursividad
            if (recursionDepth > 5)
            {
                return;
            }

            //Posicion deseada
            var originalSphereCenter = characterSphere.Center;
            var nextSphereCenter = originalSphereCenter + movementVector;

            //Buscar el punto de colision mas cercano de todos los objetos candidatos
            Collision = false;
            Vector3 q;
            float t;
            Vector3 n;
            var minT = float.MaxValue;
            foreach (var collider in colliders)
            {
                //Colisionar Sphere en movimiento contra Collider (cada Collider resuelve la colision)
                if (collider.intersectMovingSphere(characterSphere, movementVector, movementSphere, out t, out q, out n))
                {
                    //Quedarse con el menor instante de colision
                    if (t < minT)
                    {
                        minT = t;
                        Collision = true;
                        LastCollisionPoint = q;
                        LastCollisionNormal = n;
                        lastCollider = collider;
                    }
                }
            }

            //Si nunca hubo colisión, avanzar todo lo requerido
            if (!Collision)
            {
                //Avanzar todo lo pedido
                //lastCollisionDistance = movementVector.Length();
                characterSphere.moveCenter(movementVector);
                return;
            }

            //Solo movernos si ya no estamos muy cerca
            if (minT >= EPSILON)
            {
                //Restar un poco al instante de colision, para movernos hasta casi esa distancia
                minT -= EPSILON;
                var realMovementVector = movementVector * minT;

                //Mover el BoundingSphere
                characterSphere.moveCenter(realMovementVector);

                //Quitarle al punto de colision el EPSILON restado al movimiento, para no afectar al plano de sliding
                var v = Vector3.Normalize(realMovementVector);
                LastCollisionPoint -= v * EPSILON;
            }

            if (sliding)
            {
                //Calcular plano de Sliding, como un plano tangete al punto de colision con la esfera, apuntando hacia el centro de la esfera
                var slidePlaneOrigin = LastCollisionPoint;
                var slidePlaneNormal = characterSphere.Center - LastCollisionPoint;
                slidePlaneNormal.Normalize();
                var slidePlane = Plane.FromPointNormal(slidePlaneOrigin, slidePlaneNormal);

                //Calcular vector de movimiento para sliding, proyectando el punto de destino original sobre el plano de sliding
                var distance = TgcCollisionUtils.distPointPlane(nextSphereCenter, slidePlane);
                var newDestinationPoint = nextSphereCenter - distance * slidePlaneNormal;
                var slideMovementVector = newDestinationPoint - LastCollisionPoint;

                //No hacer recursividad si es muy pequeño
                slideMovementVector.Scale(SlideFactor);
                if (slideMovementVector.Length() < EPSILON)
                {
                    return;
                }

                if (LastCollisionNormal.Y <= slidingMinY)
                {
                    //Recursividad para aplicar sliding
                    doCollideWithWorld(characterSphere, slideMovementVector, colliders, recursionDepth + 1,
                        movementSphere, sliding, slidingMinY);
                }
            }
        }

        /// <summary>
        ///     Indica si el objeto se encuentra con los pies sobre alguna superficie, sino significa
        ///     que está cayendo o saltando.
        /// </summary>
        /// <returns>True si el objeto se encuentra parado sobre una superficie</returns>
        public bool isOnTheGround()
        {
            if (LastCollisionNormal == Vector3.Empty)
                return false;

            //return true;
            //return lastCollisionNormal.Y >= onGroundMinDotValue;
            return LastCollisionNormal.Y >= 0;
        }
    }
}
