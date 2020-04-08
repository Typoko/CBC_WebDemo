using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ASPNETWebDemo.Models
{

    public class Kartta
    {
        public Image<Rgba32> KarttaKuva;// = Image.Load<Rgba32>("F:/Git_Repo/CBC_WebDemo/ASPNETWebDemo/ASPNETWebDemo/wwwroot/TestFolder/testiKartta.jpg");
        public Ruutu[,] RuutuTable;// = new Ruutu[50, 50];
        public int MinX = 0;
        public int MinY = 0;
        public int MaxX = 0;
        public int MaxY = 0;
        public int TulosteX = 0;
        public int TulosteY = 0;
    }
}
