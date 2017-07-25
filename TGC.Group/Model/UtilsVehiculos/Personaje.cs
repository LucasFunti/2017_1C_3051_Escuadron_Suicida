using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class Personaje
    {
        private string fileMesh;
        private string fileSonido;
        private string fileSonidoMotor;
        private string fileSonidoArma;
        private string fileSonidoColision;
        private string fileSonidoItem;
        private string fileSonidoSalto;
        private float velocidadMax;
        private float velocidadMin;
        private int nroPersonaje;
        private float constanteAceleracion;
        public static Personaje myInstance;
        public Personaje()
        {
            myInstance = this;
        }

        public static Personaje getInstance()
        {
            return myInstance;
        }

        public string FileMesh { get => fileMesh; set => fileMesh = value; }
        public string FileSonido { get => fileSonido; set => fileSonido = value; }
        public string FileSonidoMotor { get => fileSonidoMotor; set => fileSonidoMotor = value; }
        public string FileSonidoArma { get => fileSonidoArma; set => fileSonidoArma = value; }
        public string FileSonidoColision { get => fileSonidoColision; set => fileSonidoColision = value; }
        public string FileSonidoItem { get => fileSonidoItem; set => fileSonidoItem = value; }
        public string FileSonidoSalto { get => fileSonidoSalto; set => fileSonidoSalto = value; }
        public float VelocidadMax { get => velocidadMax; set => velocidadMax = value; }
        public float VelocidadMin { get => velocidadMin; set => velocidadMin = value; }
        public float ConstanteAceleracion { get => constanteAceleracion; set => constanteAceleracion = value; }
        public int NroPersonaje { get => nroPersonaje; set => nroPersonaje = value; }
    }
}
