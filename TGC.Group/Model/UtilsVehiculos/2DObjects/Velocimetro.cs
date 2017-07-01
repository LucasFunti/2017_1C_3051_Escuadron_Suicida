using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Utils;
using TGC.Group.Model.UtilsVehiculos._2DObjects;

namespace TGC.Group.Model.UtilsVehiculos
{
    public class Velocimetro
    {
        private CustomSprite spriteVelocimetro;
        private CustomSprite spriteAguja;
        private TwistedMetal env;
        private Drawer2D drawer2D;
        float velocidadMaxAuto;
        float agujaPuntoCero = FastMath.PI / 4; //0 del velocimetro
        float agujaPuntoMax = (FastMath.PI * 1.76f)- FastMath.PI / 4;// 220 en el velocimetro

        public Velocimetro(TwistedMetal env,float vMax)
        {
            this.env = env;
            //  this.velocidadMaxAuto = vMax;
            this.velocidadMaxAuto = 150;
            spriteVelocimetro = new CustomSprite();
            spriteVelocimetro.Bitmap = new CustomBitmap(this.env.MediaDir + "\\Velocimetro\\velocimetro.png", D3DDevice.Instance.Device);
            spriteVelocimetro.Scaling = new Vector2(0.8f, 0.8f);
            var textureSize = spriteVelocimetro.Bitmap.Size;

            //Velocimetro a derecha:
            //   spriteVelocimetro.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width, 0), FastMath.Max(D3DDevice.Instance.Height - textureSize.Height, 0));
            //Velocimetro a izquierda:
            spriteVelocimetro.Position = new Vector2(0,FastMath.Max(D3DDevice.Instance.Height - textureSize.Height, 0));
         
           spriteAguja = new CustomSprite();
           spriteAguja.Bitmap = new CustomBitmap(this.env.MediaDir + "\\Velocimetro\\aguja.png", D3DDevice.Instance.Device);
           spriteAguja.Scaling = new Vector2(0.2f, 0.3f);
           spriteAguja.Position = new Vector2(spriteVelocimetro.Position.X + (textureSize.Width / 2.5f), spriteVelocimetro.Position.Y + (textureSize.Height / 2.5f));

            drawer2D = new Drawer2D();
        
        }

        public void Update(float velocidad)
        {
            spriteAguja.Rotation = getAnguloAguja(velocidad);
        }
        
        public void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(spriteVelocimetro);
            drawer2D.DrawSprite(spriteAguja);
            
            drawer2D.EndDrawSprite();
      }

      private float getAnguloAguja(float velocidad)
        {
            if (velocidad <= 0)
                return agujaPuntoCero;

            return ((velocidad * agujaPuntoMax) / this.velocidadMaxAuto)+agujaPuntoCero;
        }
    }
}
