using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Geometry;
using System.Drawing;

namespace TGC.Group.Model
{
    class Vehiculo : ObjetoConMovimiento
    {
        private TgcArrow collisionNormalArrow;
        private TgcBox collisionPoint;
        private TgcArrow directionArrow;
        TwistedMetal env;
        private bool alturaL = false;
        private TgcMesh mesh;       

        public Vehiculo(TgcMesh Mesh, Vector3 pos)
        {
            this.mesh = Mesh;
            base.setPosicion(pos);
            base.setVelocidadY(0);
            base.setAluraMaxima(20);
            //  base.setAlturaInicial(Mesh.Position.Y);
            direcionadores();
        }
        public Vehiculo(TwistedMetal env)
        {
            this.env = env;
            this.setVelocidadY(0);
            base.setAluraMaxima(100);
            direcionadores();
        }
        private void direcionadores()
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
        }
        public void setMesh(TgcMesh Mesh)
        {
            this.mesh = Mesh;
        }
        public TgcMesh getMesh()
        {
            return this.mesh;
        }       
        public virtual void Update()
        {
            base.setAlturaActual(this.getMesh().Position.Y);
            base.calculosDePosicion();
            //Actualizar valores de la linea de movimiento
            directionArrow.PStart = this.getMesh().Position;
            directionArrow.PEnd = this.getMesh().Position + Vector3.Multiply(this.getMesh().Position, 50);
            directionArrow.updateValues();

            //Actualizar valores de normal de colision
            if (this.env.GetManejadorDeColision().Manager().Collision)
            {
                collisionNormalArrow.PStart = this.env.GetManejadorDeColision().Manager().LastCollisionPoint;
                collisionNormalArrow.PEnd = this.env.GetManejadorDeColision().Manager().LastCollisionPoint +
                                            Vector3.Multiply(this.env.GetManejadorDeColision().Manager().LastCollisionNormal, 80);

                collisionNormalArrow.updateValues();


                collisionPoint.Position = this.env.GetManejadorDeColision().Manager().LastCollisionPoint;
                collisionPoint.render();
            }
        }
        public virtual void Render()
        {
            this.mesh.render();
            this.mesh.BoundingBox.render();
            //Render linea
            directionArrow.render();
            collisionNormalArrow.render();
            collisionPoint.render();

        }
        public void dispose()
        {
            this.mesh.dispose();
            directionArrow.dispose();
        }
        public override void rotar(float rotAngle)
        {
            this.getMesh().rotateY(rotAngle);
            this.rotarCamara(rotAngle);
           //this.directionArrow.PEnd.
        }
        public override void mover()
        {
          //  var lastPos = this.getMesh().Position;
            Vector3 v3=  this.env.GetManejadorDeColision().moverConColision(this.getMesh(), new Vector3(Core.Utils.FastMath.Sin(this.getMesh().Rotation.Y) * this.getVelocidadX(),
           this.getVelocidadY(), Core.Utils.FastMath.Cos(this.getMesh().Rotation.Y) * this.getVelocidadX()));
            //  this.getMesh().move(new Vector3(Core.Utils.FastMath.Sin(this.getMesh().Rotation.Y) * this.getVelocidadX(),
            // this.getVelocidadY(), Core.Utils.FastMath.Cos(this.getMesh().Rotation.Y) * this.getVelocidadX()));
            this.getMesh().move(v3);


        }
        public virtual void rotarCamara(float rotacion)
        {

        }
       
    }
}
