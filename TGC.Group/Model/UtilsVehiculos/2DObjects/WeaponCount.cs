using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Text;

namespace TGC.Group.Model.UtilsVehiculos._2DObjects
{
    public class WeaponCount
    {
        private TgcText2D text2d = new TgcText2D(), text2d2 = new TgcText2D();
        private int nBalas = 200;
        private int nMisiles = 15;
        private Boolean auto_principal;

        public WeaponCount(Boolean autoPrincipal)
        {
            this.auto_principal = autoPrincipal;
            if (!autoPrincipal)
            {
                this.nBalas = 3000;
                this.nMisiles = 1500;
            }
        }
        public void render()
        {
            //Muestra la vida solo del auto principal
            if (this.auto_principal)
            {
                //text2d = new TgcText2D();
                text2d.Text = "Balas: " + nBalas.ToString() + "";
                text2d.Color = Color.WhiteSmoke;
                text2d.Align = TgcText2D.TextAlign.LEFT;
                text2d.Position = new Point((D3DDevice.Instance.Width / 3) - 100, 0);
                text2d.Size = new Size(300, 100);
                text2d.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
                text2d.Color = Color.Yellow;

                if (this.nBalas<10)
                text2d.Color = Color.OrangeRed;
                if (this.nBalas < 5)
                text2d.Color = Color.Red;
                
                text2d.render();

                //text2d2 = new TgcText2D();
                text2d2.Text = "Misiles: " + nMisiles.ToString() + "";
                text2d2.Color = Color.WhiteSmoke;
                text2d2.Align = TgcText2D.TextAlign.LEFT;
                text2d2.Position = new Point((D3DDevice.Instance.Width / 6) - 100, 0);
                text2d2.Size = new Size(300, 100);
                text2d2.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
                text2d2.Color = Color.Yellow;
                if (this.nMisiles < 10)
                    text2d2.Color = Color.OrangeRed;
                if (this.nMisiles < 5)
                    text2d2.Color = Color.Red;

                text2d2.render();
            }
        }
        public int getnBalas()
        {
            return this.nBalas;
        }
       
        public void sumarBalas(int n)
        {
            this.nBalas += n;
       }
        public int getnMisiles()
        {
            return this.nMisiles;
        }

        public void sumarMisiles(int n)
        {
            this.nMisiles += n;
        }
    }
}
