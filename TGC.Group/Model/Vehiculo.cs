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
     
        Core.Example.TgcExample env;
        
        private TgcMesh mesh;       

        public Vehiculo(TgcMesh Mesh, Vector3 pos)
        {
            this.mesh = Mesh;
            this.setPosicion(pos);
            this.setVelocidadY(0);
        }
        public Vehiculo(Core.Example.TgcExample env)
        {
            this.env = env;
            this.setVelocidadY(0);
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
            base.calculosDePosicion();
        }
        public void Render()
        {
            this.mesh.render();
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
            this.getMesh().moveOrientedY(this.getVelocidadX() * this.ElapsedTime);
        }
        public virtual void rotarCamara(float rotacion)
        {

        }
       
    }
}
