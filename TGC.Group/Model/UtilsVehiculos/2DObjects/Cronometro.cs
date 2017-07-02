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
    public class Cronometro
    {
        private TgcText2D text2d;
        TwistedMetal env;
        private float tiempoMax;
        private float time = 0;

        public Cronometro(float tiempo, TwistedMetal env)
        {
            this.env = env;
            tiempoMax = tiempo;
        }

        public void render(float elapseElapsedTime)
        {
            text2d = new TgcText2D();
            text2d.Text = this.GenerarMinutosYSegundos(elapseElapsedTime);
            text2d.Color = Color.WhiteSmoke;
            text2d.Align = TgcText2D.TextAlign.LEFT;
            text2d.Position = new Point(D3DDevice.Instance.Width - 150, 0);
            text2d.Size = new Size(300, 100);
            text2d.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
            text2d.Color = Color.Yellow;
            text2d.render();
        }
        private String GenerarMinutosYSegundos(float elapseElapsedTime)
        {
            if (elapseElapsedTime < 200)
                time += elapseElapsedTime;

            var seg = Math.Truncate(time % 60);
            var min = Math.Truncate(time / 60);

            checkGanador(tiempoMax, min);

            var segundos = "";
            var minutos = "";

            if (min < 10)
                minutos = "0";

            if (seg < 10)
                segundos = "0";

            segundos = segundos + seg.ToString();
            minutos = minutos + min.ToString();
           
            return minutos + ":" + segundos + " ";
        }
        private void checkGanador(float tiempoMax, double minutoDoble)
        {
            float min = Convert.ToSingle(minutoDoble);
            if (tiempoMax < min)
            {
               // this.env.TerminarJuego(true);
            }
        }
    }
}
