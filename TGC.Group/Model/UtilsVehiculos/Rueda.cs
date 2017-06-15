using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.UtilsVehiculos
{
    class Rueda : Vehiculo
    {

        public Rueda(TwistedMetal env, TgcMesh mesh, bool ruedaDelantera) : base(mesh, env)
        {

            this.setMesh(mesh);
            this.setVelocidadMaxima(70);
            this.setVelocidadMinima(-5);
            this.setConstanteDeAsceleracionX(0.7f);
            this.setEsRueda(true);
            this.setEsRuedaDelantera(ruedaDelantera);
            //      base.setPEndDirectionArrow(new Vector3(this.getMesh().Position.X, this.getMesh().Position.Y, -500));
            //   this.setAlturaInicial(this.getMesh().Position.Y);
            //camaraManager();
            this.doblar(0.001f);//Inicializa las matrices de rotación, no tocar!!

        }

        public override bool moverAdelante()
        {
            return this.env.Input.keyDown(Key.W);
        }
        public override bool moverAtras()
        {
            return this.env.Input.keyDown(Key.S);
        }
        public override bool moverADerecha()
        {
            return this.env.Input.keyDown(Key.D);
        }
        public override bool moverAIzquierda()
        {
            return this.env.Input.keyDown(Key.A);
        }
        public override bool moverArriba()
        {
            return this.env.Input.keyDown(Key.J);
        }
        public override bool moverAbajo()
        {
            return this.env.Input.keyDown(Key.K);
        }
        public override bool cambiarCamara()
        {
            return false;
        }
        public override bool cambiarMusica()
        {
            return false;
        }
        public override void Update()
        {
            /*contadorDeCiclos++;
            if (contadorDeCiclos> TiempoRetardo)
            {
                contadorDeCiclos = 0;
                adelante = true;
            }*/
            
            base.Update();
            //contadorDeCiclos++;
           // adelante = false;
        }
    }
}
