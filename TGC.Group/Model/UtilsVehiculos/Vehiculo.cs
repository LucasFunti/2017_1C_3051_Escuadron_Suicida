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
    class Vehiculo : ObjetoConMovimiento
    {

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
        }
        public Vehiculo(TwistedMetal env)
        {
            this.env = env;
            this.setVelocidadY(0);
            base.setAluraMaxima(100);
   
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
        }
        public void Render()
        {
            this.mesh.render();
            this.mesh.BoundingBox.render();
        }
        public void dispose()
        {
            this.mesh.dispose();
        }
        public override void rotar(float rotAngle)
        {
            this.getMesh().rotateY(rotAngle);
            this.rotarCamara(rotAngle);
        }
        public override void mover()
        {
            var lastPos = this.getMesh().Position;
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
