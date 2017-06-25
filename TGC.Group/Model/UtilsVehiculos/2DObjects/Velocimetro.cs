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
     

        public Velocimetro(TwistedMetal env)
        {
            this.env = env;
            spriteVelocimetro = new CustomSprite();
            spriteVelocimetro.Bitmap = new CustomBitmap(this.env.MediaDir + "\\Velocimetro\\velocimetro.png", D3DDevice.Instance.Device);
            spriteVelocimetro.Scaling = new Vector2(0.1f, 0.1f);
            var textureSize = spriteVelocimetro.Bitmap.Size;
            spriteVelocimetro.Position = new Vector2(FastMath.Max(D3DDevice.Instance.Width - textureSize.Width, 0),
                FastMath.Max(D3DDevice.Instance.Height - textureSize.Height, 0));

            spriteAguja = new CustomSprite();
            spriteAguja.Bitmap = new CustomBitmap(this.env.MediaDir + "\\Velocimetro\\aguja.png", D3DDevice.Instance.Device);
            spriteAguja.Scaling = new Vector2(0.1f, 0.2f);
            spriteAguja.Position = new Vector2(spriteVelocimetro.Position.X + (textureSize.Width / 2.6f), spriteVelocimetro.Position.Y + (textureSize.Height / 2.6f));
            
            drawer2D = new Drawer2D();
            
        }

        public void Update(float velocidad, bool huboMarchaAtras)
        {
            if (velocidad < 0)
                spriteAguja.Rotation = (FastMath.PI / 4 - velocidad);
            else
                spriteAguja.Rotation = FastMath.PI / 4 + velocidad;
            if (velocidad < 0 && huboMarchaAtras)
                spriteAguja.Rotation = FastMath.PI / 4;
        }

        public void Render()
        {
            drawer2D.BeginDrawSprite();
            drawer2D.DrawSprite(spriteVelocimetro);
            drawer2D.DrawSprite(spriteAguja);
            drawer2D.EndDrawSprite();
      }
    }
}
