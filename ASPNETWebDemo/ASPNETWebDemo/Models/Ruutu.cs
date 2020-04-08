using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETWebDemo.Models
{
    public class Ruutu
    {
        
        // määritellään onko ruutu osa huonetta vai ei
        public bool OnkoAvoin = false;
        // määritellään mille reunoille ruutuun tulee piirtää seinäosuus
        public bool[] Seinat = new bool[4];
        // määritellään mihin kulmaan täytyy tehdä lisäystä huoneen muodon vuoksi
        public bool[] Kulmat = new bool[4];

        public Ruutu()
        {


        }

    }
}
