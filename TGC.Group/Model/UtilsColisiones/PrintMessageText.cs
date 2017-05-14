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
           //this.env.DrawText.drawText("Con clic izquierdo y el puntero movemos la camara [Actual]: " + Core.Utils.TgcParserUtils.printVector3(this.env.Camara.Position), 0, 30,System.Drawing.Color.OrangeRed);
           this.env.DrawText.drawText("Con A,W,D,S nos desplazamos por el escenario. J = Salto, C = Cambia la cámara. ", 0, 45, System.Drawing.Color.OrangeRed);
        }
        public void MostrarVelocidadPorPantalla(float Velocidad)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Velocidad X: " + Math.Abs(Velocidad), 0, 60, System.Drawing.Color.Red);
 
        }
        public void MostrarPosicioMeshPorPantalla(Vector3 vector)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Posición(" + vector.X + ";" + vector.Y + ";" + vector.Z + ")", 0, 75, System.Drawing.Color.Red);

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
        public void MostrarTiempo()
        {
            this.env.DrawText.drawText("Time: " + env.ElapsedTime + ")", 0, 135, System.Drawing.Color.Red);
        }
        public void test(String text, Vector3[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
         //          this.env.DrawText.drawText(text+" (" + vector[i].X + ";" + vector[i].Y + ";" + vector[i].Z + ")", 0, (135+15*i), System.Drawing.Color.Red);
       //        this.env.DrawText.drawText(text + " (" + vector[i].X + ";" + vector[i].Y + ";" + vector[i].Z + ")", Math.Abs((int)(vector[i].X))*2, (int)vector[i].Z, System.Drawing.Color.Red);


            }
        }
    }
}
