using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Utils;

namespace TGC.Group.Model.UtilsEfectos
{
    class PuntoDeLuz
    {
        private TwistedMetal env;
        public Vector3 lighthPos;
        private TgcBox lightMesh;
        private Effect currentShader;
        public float lightIntensity;

        public PuntoDeLuz(TwistedMetal env, Vector3 Posicion)
        {
            this.env = env;
            lighthPos = Posicion;
            float t = 10f;

            lightMesh = TgcBox.fromSize(lighthPos, new Vector3(t, t, t), Color.White);
            lightMesh.AutoTransformEnable = false;
            lightMesh.updateValues();
            //currentShader = TgcShaders.Instance.TgcMeshPointLightShader;
            currentShader = TgcShaders.loadEffect(this.env.ShadersDir + "TgcMeshSpotLightShader.fx");
        }

        public void render(List<TgcMesh> mapMeshes, Vector3 camPos, TgcMesh meshesAuto, Matrix mr, Vector3 Posicion, Vector3 pivote, float anguloFinal, bool lucesOn, Vector3 v3)
        {
            if (lucesOn)
            {
                lightIntensity = 35f;
                lightMesh.Color = Color.White;
            }

            else
            {
                lightIntensity = 20f;
                lightMesh.Color = Color.Gray;
            }



            lightMesh.Transform = Matrix.Scaling(new Vector3(0.45f, 0.3f, -0.1f)) * Matrix.Translation(Posicion) * mr * Matrix.Translation(pivote);

            lighthPos = Posicion + pivote + v3;

            lightMesh.updateValues();


            foreach (var mesh in mapMeshes)
            {
                setEffects(camPos, mesh);
            }
            setEffects(camPos, meshesAuto);
           /*
            foreach (var mesh in meshesAuto)
            {
                setEffects(camPos, mesh);
            }*/
        }

        private void setEffects(Vector3 camPos, TgcMesh mesh)
        {
            mesh.Effect = currentShader;
            mesh.Technique = TgcShaders.Instance.getTgcMeshTechnique(mesh.RenderType);

            //2seteo valores
            mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("materialSpecularExp", 9f);

            //3 seteo variables

            mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lighthPos));
            mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camPos));
            mesh.Effect.SetValue("lightIntensity", lightIntensity);
            mesh.Effect.SetValue("lightAttenuation", 0.2f);
        }
    }
}