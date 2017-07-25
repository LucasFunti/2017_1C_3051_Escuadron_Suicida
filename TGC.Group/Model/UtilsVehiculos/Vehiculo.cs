using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;
using TGC.Core.Textures;
using TGC.Group.Model.UtilsEfectos;
using TGC.Group.Model.UtilsVehiculos;
using TGC.Group.Model.UtilsVehiculos._2DObjects;
using TGC.Core.Geometry;

namespace TGC.Group.Model
{
    class Vehiculo : ObjetoConMovimiento
    {
        private LifeLevel lifeLevel;
        private WeaponCount weapons;
        private Humo humoCañoEscape;
        private Humo humoChoque;
        private Boolean EfectoNitro = false;
        public float tInicioHumo = 1f;
        public static float tDuracionHumo = 1f;
        public float tFinHumo = 2f;
        private Sonido sonidoPorDaño;
        private Sonido sonidoPorMuerte;
        public bool estaMuerto = false;

        public Vehiculo(TgcMesh Mesh, TwistedMetal env) : base(env)
        {
            base.setMesh(Mesh);
            //  base.getMesh().(pos);
            base.setVelocidadY(0);
            base.setAluraMaxima(20);
            //      direcionadores();
            sonidoPorDaño = new Sonido(env.MediaDir, env.ShadersDir, env.DirectSound);
            sonidoPorMuerte = new Sonido(env.MediaDir, env.ShadersDir, env.DirectSound);
            updateTGCArrow();
            iniciarNivelDeVida();
            iniciarWeaponCount();
            iniciarHumo();
        }
        public Vehiculo(TwistedMetal env) : base(env)
        {
            this.setVelocidadY(0);
            base.setAluraMaxima(100);
            //    direcionadores();
            iniciarNivelDeVida();
            iniciarWeaponCount();
            iniciarHumo();
        }

        private void playSonidoPorDaño()
        {
            Vector3 vecDisparo = new Vector3(this.getMesh().Position.X,
                                                 this.getMesh().Position.Y,
                                                 this.getMesh().Position.Z);
            if (sonidoPorDaño!= null) sonidoPorDaño.playSound(env.MediaDir + "MySounds\\Crash1.wav", vecDisparo);
        }

        private void playSonidoPorMuerte()
        {
            Vector3 vecDisparo = new Vector3(this.getMesh().Position.X,
                                                 this.getMesh().Position.Y,
                                                 this.getMesh().Position.Z);
            if (sonidoPorMuerte != null) sonidoPorMuerte.playSound(env.MediaDir + "MySounds\\Scream1.wav", vecDisparo);
        }

        public LifeLevel getLifeLevel()
        {
            return this.lifeLevel;
        }

        private void iniciarNivelDeVida()
        {
            this.lifeLevel = new LifeLevel(this.esAutoPrincipal());
        }
        private void iniciarWeaponCount()
        {
            this.weapons = new WeaponCount(this.esAutoPrincipal());
        }
        private void iniciarHumo()
        {
            humoCañoEscape = new Humo(this.env);
            humoChoque = new Humo(this.env,true);
        }
        private void TirarHumoChoque()
        {
            tInicioHumo = this.env.ElapsedTime;
            tFinHumo = tInicioHumo + tDuracionHumo;
        }
        public  virtual Boolean esAutoPrincipal()
        {
            return false;
        }

      
        //Rota el objeto y si tiene camara sobre el.
        public override void rotar(Vector3 v, Matrix m,float anguloCamara)
        {
            base.rotar(v, m, anguloCamara);
            this.rotarCamara(anguloCamara);
        }
    
        public override void mover()
        {
            base.mover(); 
         }
          
        public virtual void rotarCamara(float rotacion)
        {

        }
        public virtual void creaDisparo(Vector3 posicion)
        {
            string sonido = env.MediaDir + "MySounds\\MachineGun.wav";
            var loader = new TgcSceneLoader();
            var scene = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Objetos\\Bala\\Bala-TgcScene.xml");
            TgcMesh mesh = scene.Meshes[0];
            Vector3 scale = new Vector3(0.3f, 0.3f, 0.3f);
            mesh.Scale = scale;
            mesh.AutoUpdateBoundingBox = true;
            mesh.createBoundingBox();
            mesh.Position = posicion;
            
            
            Arma arma = new Arma(mesh, this.env, sonido, 40, this.orientacion, this.anguloFinal, scale, this.getMesh(), true);

            //arma.mover();
            ControladorDeVehiculos.getInstance().agregarArma(arma);
            base.agregarArma(arma);
        }
        public void creaMisilV()
        {
            TgcSceneLoader loader=new TgcSceneLoader();
            string sonido = env.MediaDir + "MySounds\\Launch4.wav";
            var scene = loader.loadSceneFromFile(env.MediaDir + "MeshCreator\\Meshes\\Objetos\\Misil-T\\misil-T-TgcScene.xml");
            TgcMesh mesh = scene.Meshes[0];
            mesh.AutoUpdateBoundingBox = true;
            mesh.createBoundingBox();
            mesh.Position = this.getMesh().Position;
            Vector3 scale = new Vector3(1f, 1f, 1f);

            Arma arma = new Arma(mesh, this.env, sonido, 20, this.orientacion, this.anguloFinal, scale, this.getMesh(), false);
            
            ControladorDeVehiculos.getInstance().agregarArma(arma);
            base.agregarArma(arma);
        }

       
        protected override void aplicarEfecto()
        {
            tInicioHumo = FastMath.Abs(this.env.ElapsedTime);
            tFinHumo = tInicioHumo + tDuracionHumo;
            this.humoChoque.Update(this.getNuevaPosicion(), this.anguloFinal);
        }
        public void dañoPorChoqueEnemigo()
        {
            this.lifeLevel.recibirDaño(1);
        }
        protected override void dañoPorChoque()
        {
            this.lifeLevel.recibirDañoPorChoque();
        }
        protected override void dañoPorArma()
        {
            this.lifeLevel.recibirDaño(10);
            
            if (this.lifeLevel.nivelDeVida() == 0 && this.estaMuerto == false) {
                TwistedMetal.getInstance().playSonidoPorMuerte();
                this.estaMuerto = true;
            } else
            {
                if (this.lifeLevel.nivelDeVida() > 0) playSonidoPorDaño();
            }
        }
        protected override void dañoPorDisparo()
        {
            this.lifeLevel.recibirDaño(2);
            if (this.lifeLevel.nivelDeVida() == 0 && this.estaMuerto == false)
            {
                TwistedMetal.getInstance().playSonidoPorMuerte();
                this.estaMuerto = true;
            }
            else
            {
                if (this.lifeLevel.nivelDeVida() > 0) playSonidoPorDaño();
            }

        }
        protected override void sumarVida()
        {
            this.lifeLevel.recibirVida(25);
        }
        protected override void sumarArmas()
        {
            this.weapons.sumarBalas(30);
            this.weapons.sumarMisiles(5);
        }
        public virtual void Update()
        {
           // base.CalcularMeshesCercanos();
            base.setPosicionAnterior(base.getMesh().Position);
            base.setRotacionAnterior(base.getMesh().Rotation);
            base.setAlturaActual(base.getMesh().Position.Y);
            base.calculosDePosicion();
            base.updateTGCArrow();

            if (disparar() && this.weapons.getnBalas()>0)
            {
                this.weapons.sumarBalas(-1);
                base.startDisparo();
                creaDisparo(this.getMesh().Position);
            }
            if (disparaEspecial() && this.weapons.getnMisiles()> 0) {
                this.weapons.sumarMisiles(-1);
                base.startArma();
                //   creaMisilV(this.getMesh().Position);
                creaMisilV();

            }
            this.humoCañoEscape.Update(this.getNuevaPosicion(), this.anguloFinal);
        }
        public virtual void Render()
        {
            
            base.getMesh().render();
            this.lifeLevel.render();
            this.weapons.render();

            if(this.getMesh().Enabled)
            humoCañoEscape.Render(EfectoNitro);

            if (tInicioHumo < tFinHumo)
                humoChoque.Render(false); //si es momento de choque, muestro humito

            tInicioHumo = tInicioHumo + FastMath.Abs(this.env.ElapsedTime);

        }
        public void dispose()
        {
            //base.getMesh().dispose();

           // directionArrow.dispose();

        }

    }
}
