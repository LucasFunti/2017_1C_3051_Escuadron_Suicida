using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Particle;

namespace TGC.Group.Model.UtilsEfectos
{
    class Humo
    {
        private ParticleEmitter emitterEscape;
        private ParticleEmitter emitterNitro;
        private int selectedParticleCount;
         //private string texturaFuego;
        
        private TwistedMetal env;
        private Vector3 offsetEscape;

        public Humo(TwistedMetal env)
        {
            this.env = env;
            offsetEscape = new Vector3(10, 5, 32);
            Init();

        }

        public Humo(TwistedMetal env, bool deChoque)
        {
            this.env = env;
            if (!deChoque) return; //solo para cuando quiero que sea de choque
            Init();

        }

        private void setEmmiter(ParticleEmitter emitter)
        {
            emitter.MinSizeParticle = 0.7f;
            emitter.MaxSizeParticle = 2f;
            emitter.ParticleTimeToLive = 1f;
            emitter.CreationFrecuency = 0.04f;
            emitter.Speed = new Vector3(1, 5, 100);
        }

        public void Init()
        {

         
            //Crear emisor de particulas
          //  texturaFuego = "fuego.png";

            selectedParticleCount = 10;

            //emmiter1 humo
            emitterEscape = new ParticleEmitter(this.env.MediaDir+ "Particulas\\pisada.png", selectedParticleCount);
            emitterEscape.Position = new Vector3(0, 15, 0);

            Vector3 speed = new Vector3(5, 5, 5);
            setEmmiter(emitterEscape);

            //emmiter fuego
            /* selectedParticleCount = 10;
             emitterNitro = new ParticleEmitter(this.env.MediaDir+ "Particulas\\pisada.png, selectedParticleCount);
             emitterNitro.Position = new Vector3(0, 15, 0);

             setEmmiter(emitterNitro);*/

        }

        public void Update(Vector3 pos, float rotation)
        {
            //los dos escapes van en el mismo lugar
            emitterEscape.Position = calcularMatrizRotacion(pos, rotation);
          //  emitter2.Position = emitter1.Position; //para nitro

        }

        private Vector3 calcularMatrizRotacion(Vector3 pos, float rotation)
        {
            var matrix = Matrix.Translation(offsetEscape) * Matrix.RotationY(rotation)
                         * Matrix.Translation(pos);
            return new Vector3(matrix.M41, matrix.M42, matrix.M43); ;
        }

        public void Render(bool conNitro)
        {
            //IMPORTANTE PARA PERMITIR ESTE EFECTO.
            D3DDevice.Instance.ParticlesEnabled = true;
            D3DDevice.Instance.EnableParticles();
            if (conNitro)
            {
                emitterNitro.render(this.env.ElapsedTime);
            }
            else
            {
                emitterEscape.render(this.env.ElapsedTime);
            }


        }
    }
}
