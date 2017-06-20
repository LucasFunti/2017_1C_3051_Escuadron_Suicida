using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Camara;
using TGC.Core.Fog;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Terrain;
using TGC.Core.Utils;

namespace TGC.Group.Model.UtilsEfectos
{
    public class Niebla
    {
        private Effect effect;
        private TgcFog fog;

        private TgcSkyBox skyBox;

        public bool fogShader { get; set; }
        private TwistedMetal env;

        public Niebla(TwistedMetal env)
        {
            effect = TgcShaders.loadEffect(env.ShadersDir + "TgcFogShader.fx");
            fog = new TgcFog();
            fog.Enabled = true;
            fog.StartDistance = 3000;
            fog.EndDistance = 4000f;
            fog.Density = 0.0005f;
            fog.Color = Color.Gray;
            fogShader = true;
            skyBox = env.Ciudad.getSkyBox();
            this.env = env;
          //  mapScene = gm.MapScene;
            //bosqueScene = gm.BosqueScene;
            //ahora cargo todo en el efecto de directX

        }
        public void CargarCamara(TgcCamera camara)
        {
            ConfigurarDirectX(camara.Position);
            fogShader = true;
        }
        public void CargarCamara(CamaraTerceraPersona camara)
        {
            ConfigurarDirectX(camara.Position);
            fogShader = true;
            Render();

        }

        public void Update(TgcCamera camara)
        {
            var camaraPosition = camara.Position;
            effect.SetValue("CameraPos", TgcParserUtils.vector3ToFloat4Array(camaraPosition));
        }

        private void ConfigurarDirectX(Vector3 camaraPosition)
        {
            effect.SetValue("ColorFog", fog.Color.ToArgb());
            effect.SetValue("CameraPos", TgcParserUtils.vector3ToFloat4Array(camaraPosition));
            effect.SetValue("StartFogDistance", fog.StartDistance);
            effect.SetValue("EndFogDistance", fog.EndDistance);
            effect.SetValue("Density", fog.Density);
        }

        /// <summary>
        /// seteo las tecnicas de render pero en realidad no rendereo nada aca.
        /// </summary>
        public void Render()
        {
            //primero el skybox
            foreach (var mesh in skyBox.Faces)
            {
                if (fogShader)
                {
                    mesh.Effect = effect;
                    mesh.Technique = "RenderScene";
                }
                else
                {
                    mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                    mesh.Technique = "DIFFUSE_MAP";
                }
            }

            //ahora la ciudad
            foreach (var mesh in this.env.Ciudad.getAllMeshes())
            {
                ActivarEfecto(mesh);
            }
            //ahora el bosque
            //foreach (var mesh in bosqueScene.Meshes)
            //{
            //    ActivarEfecto(mesh);
            //}
        }

        private void ActivarEfecto(TgcMesh mesh)
        {
            if (fogShader)
            {
                mesh.Effect = effect;
                mesh.Technique = "RenderScene";
            }
            else
            {
                mesh.Effect = TgcShaders.Instance.TgcMeshShader;
                mesh.Technique = "DIFFUSE_MAP";
            }
        }
    }
}
