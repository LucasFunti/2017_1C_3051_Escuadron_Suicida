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
           this.env.DrawText.drawText("Con A,W,D,S nos desplazamos por el escenario. J = Salto, C = Cámara, Ctrl Der = Disparo, M = Música, Espacio = Especial. ", 0, 45, System.Drawing.Color.OrangeRed);
        }

        public void MostrarComandosDeSeleccion()
        {
            //Dibuja un texto por pantalla
            //this.env.DrawText.drawText("Con clic izquierdo y el puntero movemos la camara [Actual]: " + Core.Utils.TgcParserUtils.printVector3(this.env.Camara.Position), 0, 30,System.Drawing.Color.OrangeRed);
            this.env.DrawText.drawText("Comandos disponibles: \"ESPACIO\" = Elegir, \"TAB\" = Elegir Opcion. ", 45, 600, System.Drawing.Color.Red);
        }

        public void MostrarComandosDeMenu()
        {
            //Dibuja un texto por pantalla
            //this.env.DrawText.drawText("Con clic izquierdo y el puntero movemos la camara [Actual]: " + Core.Utils.TgcParserUtils.printVector3(this.env.Camara.Position), 0, 30,System.Drawing.Color.OrangeRed);
            this.env.DrawText.drawText("Comandos disponibles: \"ESPACIO\" = Elegir, \"TAB\" = Elegir Opcion. ", 45, 600, System.Drawing.Color.Red);
        }

        public void MostrarMensaje(string msg, int x, int y)
        {
            //Dibuja un texto por pantalla
            //this.env.DrawText.drawText("Con clic izquierdo y el puntero movemos la camara [Actual]: " + Core.Utils.TgcParserUtils.printVector3(this.env.Camara.Position), 0, 30,System.Drawing.Color.OrangeRed);
            this.env.DrawText.drawText(msg, x, y, System.Drawing.Color.Red);
        }
        public void MostrarVelocidadPorPantalla(float Velocidad)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Velocidad X: " + Velocidad, 0, 60, System.Drawing.Color.Red);
 
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
        public void MostrarPosicionCamaraPorPantalla(Vector3 vector)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Pos Camara (" + vector.X + ";" + vector.Y + ";" + vector.Z + ")", 0, 150, System.Drawing.Color.Red);

        }
        public void MostrarAnguloVehiculoPrincipal(float angulo)
        {
            this.env.DrawText.drawText("Angulo AutoP: " + angulo+ "Sin"+Math.Sin(angulo) + " Cos" + Math.Cos(angulo), 0, 165, System.Drawing.Color.Red);
        }
        public void MostrarAnguloVehiculoEnemigo(float angulo)
        {
            this.env.DrawText.drawText("Angulo Enem: " + angulo + "Sin" + Math.Sin(angulo) + " Cos" + Math.Cos(angulo), 0, 180, System.Drawing.Color.Red);
        }
        public void MostrarDireccionEnemigo(Vector3 vector)
        {
            this.env.DrawText.drawText("Dirección Enemigo (" + vector.X + ";" + vector.Y + ";" + vector.Z + ")", 0, 195, System.Drawing.Color.Red);
        }
        public void MostrarVelocidadEnemigoPorPantalla(float Velocidad)
        {
            //Dibuja un texto por pantalla
            this.env.DrawText.drawText("Velocidad Enemigo X: " + Velocidad, 0, 210, System.Drawing.Color.Red);

        }
        public void MostrarOrientacionEnemigo(float orientacion)
        {
            this.env.DrawText.drawText("Orientacion Enemigo (" + orientacion + ")", 0, 225, System.Drawing.Color.Red);
        }
        public void MostrarOrientacionAuto(float orientacion)
        {
            this.env.DrawText.drawText("Orientacion auto (" + orientacion + ")", 0, 235, System.Drawing.Color.Red);
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
