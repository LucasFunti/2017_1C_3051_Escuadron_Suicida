using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Example;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Item 
    {

        //Mesh
        private TgcMesh mesh;
        private TgcBoundingOrientedBox boxDeColision;
        private float largoDelMesh;
        private float boxDeColisionY;
        private Sonido sonidoColision;
        private string fileNameSound;
        private Boolean mostrarItem = true;
        private int contParaMostrar = 0;
        private int TiempoRetardo = 380;

       

        public void setFileNameSound(string fileName)
        {
            this.fileNameSound = fileName;
        }
        public void ocultarItem()
        {
            //oculto el item
            mostrarItem = false;

            Vector3 vecSonido = new Vector3(this.getMesh().Position.X - 100,
                                                this.getMesh().Position.Y,
                                                this.getMesh().Position.Z - 100);
            this.sonidoColision.playSound(fileNameSound, vecSonido);
            contParaMostrar = 0;
        }

        public void setMesh(TgcMesh Mesh)
        {
            this.mesh = Mesh;
            initBoxColisionador();
            //  this.updateTGCArrow(this.calcularRayoDePosicion());
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
            //            boxDeColision.Extents = new Vector3(boxDeColision.Extents.X, boxDeColision.Extents.Y, boxDeColision.Extents.Z * -1);
            boxDeColision.Extents = new Vector3(boxDeColision.Extents.X, boxDeColision.Extents.Y, boxDeColision.Extents.Z);

            largoDelMesh = boxDeColision.Extents.Z;
            boxDeColisionY = (yMax + yMin) / 2 + yMin;
        }
        
    }
}
