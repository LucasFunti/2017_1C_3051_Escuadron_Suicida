using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Text;
using TGC.Core.Utils;
using TGC.Group.Model.UtilsVehiculos._2DObjects;
namespace TGC.Group.Model.UtilsVehiculos
{
    public class LifeLevel
    {
        private TgcText2D text2d;
        TwistedMetal env;
        private float lifeLevel = 0;
        private Boolean auto_principal;

        public LifeLevel(Boolean autoPrincipal)
        {
            this.lifeLevel = 100;
            this.auto_principal = autoPrincipal;
        }
        public void render()
        {
            //Muestra la vida solo del auto principal
            if (!this.auto_principal)
                return;

                text2d = new TgcText2D();
                text2d.Text = "Vida: " + Math.Truncate(this.lifeLevel).ToString() + "%";
                text2d.Color = Color.WhiteSmoke;
                text2d.Align = TgcText2D.TextAlign.LEFT;
                text2d.Position = new Point((D3DDevice.Instance.Width / 2) - 100, 0);
                text2d.Size = new Size(300, 100);
                text2d.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
                text2d.Color = Color.Yellow;
                text2d.render();
           
        }
        public float nivelDeVida()
        {
            return this.lifeLevel;
        }
        public Boolean stillAlive()
        {
            if (this.lifeLevel > 0.05)
                return true;
            else
                return false;
        }
        public void recibirDañoPorChoque()
        {
            this.recibirDaño(5);
        }
        public void recibirDaño(float daño)
        {
            this.lifeLevel = this.lifeLevel - daño;
            if (this.lifeLevel < 0)
                this.lifeLevel = 0f;

        }
        public void recibirVida(float vida)
        {
            this.lifeLevel = this.lifeLevel + vida;
            if (this.lifeLevel > 100)
                this.lifeLevel = 100f;

        }
    }
}
