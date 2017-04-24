using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class PrintMessageText
    {
        private Core.Example.TgcExample env;
        public PrintMessageText(Core.Example.TgcExample env)
        {
            this.env = env;
        }

        public void MostrarComandosPorPantalla()
        {
            //Dibuja un texto por pantalla
           this.env.DrawText.drawText("Con clic izquierdo y el puntero movemos la camara [Actual]: " + Core.Utils.TgcParserUtils.printVector3(this.env.Camara.Position), 0, 30,System.Drawing.Color.OrangeRed);
           this.env.DrawText.drawText("Con A,W,D,S nos desplazamos por el escenario.", 0, 45, System.Drawing.Color.OrangeRed);
        }
        public void MostrarVelocidadPorPantalla(float Velocidad)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Velocidad: " + Velocidad, 0, 60, System.Drawing.Color.Red);
 
        }
        public void MostrarAlturaPorPantalla(float altura)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Altura: " + altura, 0, 75, System.Drawing.Color.Red);

        }
        public void MostrarVelocidadYPorPantalla(float altura)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Velocidad Y: " + altura, 0, 90, System.Drawing.Color.Red);

        }
        public void MostrarDireccionVehiculoPrincipal(Vector3 vector)
        {
            this.env.DrawText.drawText("Dirección ("+vector.X+";" + vector.Y+";" + vector.Z+")", 0, 105, System.Drawing.Color.Red);
        }
        public void MostrarPuntoColisionVehiculoPrincipal(Vector3 vector)
        {
            this.env.DrawText.drawText("Colision en (" + vector.X + ";" + vector.Y + ";" + vector.Z + ")", 0, 120, System.Drawing.Color.Red);
        }
    }
}
