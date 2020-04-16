using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASPNETWebDemo.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net;

namespace ASPNETWebDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string TestiString()
        {
            return "Testi 123";
        }

        public string Kellonaika()
        {
            return DateTime.Now.ToString();
        }


        public IActionResult UusiSivu()
        {
            List<Asiakas> asiakkaat = new List<Asiakas>()
            {
                new Asiakas()
                {
                    AsiakasID = 100,
                    asiakkaanNimi = "Nimi1",
                    sahkoPosti = "testi@123.fi"
                },
                new Asiakas()
                {
                    AsiakasID = 101,
                    asiakkaanNimi = "Nimi2",
                    sahkoPosti = "te2222sti@123.fi"
                }
            };

            ViewBag.NapinVari = "danger";
            ViewBag.NaytaLista = "false";
            
            return View(asiakkaat);
        }


        public IActionResult Index(string id, string imageUrl, string rKoko)
        {
            ViewBag.KarttaPath = "/TestFolder/testiKartta.jpg";
            ViewBag.SivustoPath = "https://localhost:44340/home/index/";
            Kartta kartta = new Kartta();
            int RuutuKoko;

            //ViewBag.testi = testi;

            if (string.IsNullOrEmpty(rKoko))
            {
                RuutuKoko = 20;
            }
            else
            {
                RuutuKoko = Convert.ToInt32(rKoko);
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                kartta.KarttaKuva = Image.Load<Rgba32>("wwwroot"+ ViewBag.KarttaPath);
            }
            else
            {
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead(imageUrl);
                Image <Rgba32> im = Image.Load<Rgba32>(st);
                kartta.KarttaKuva = im;
                ViewBag.KarttaPath = imageUrl;
                ViewBag.KarttaX = kartta.KarttaKuva.Width;
                ViewBag.KarttaY = kartta.KarttaKuva.Height;
            }

            kartta.RuutuTable = new Ruutu[(kartta.KarttaKuva.Width)/ RuutuKoko, kartta.KarttaKuva.Height/ RuutuKoko];


            //Täytetään ruudukko tyhjillä instansseilla, ettei tule nulliin viittauksia
            SetRuutuInstances(kartta.RuutuTable);

            //Analysoidaan karttakuva ruuduiksi
            SetRuutuHuoneStatus(kartta.RuutuTable, kartta.KarttaKuva, RuutuKoko);

            //Selvitetään missä väleissä on seiniä
            SetRuutuSeinaStatus(kartta.RuutuTable);

            //Selvitetään mitkä kulmat on relevantteja
            SetRuutuKulmaStatus(kartta.RuutuTable);

            //Määritellään kuvan reunat, että saadaan kartasta tyhjää pois
            SetKuvaMinMaxRuudut(kartta);

            //Luodaan Json konversio "käsin" kun multidimensional olio array ei tunnu menevän läpi vakiona
            LuoJson(kartta);



            return View(kartta);
        }

        //Luodaan Json tarvittavista muuttujista
        //Tehdään näin koska 2 ulotteinen taulukko jonka sisällä on olioita ei tuntunut menevän läpi
        public void LuoJson(Kartta k)
        {

            string karttaTauluJson = "{\"RuutuTable\":[";

            for (int i = k.MinX; i <= k.MaxX; i++)
            {
                for (int j = k.MinY; j <= k.MaxY; j++)
                {
                    karttaTauluJson += Newtonsoft.Json.JsonConvert.SerializeObject(k.RuutuTable[i, j]) + ",";
                }
            }
            karttaTauluJson = karttaTauluJson.Remove(karttaTauluJson.Length - 1);
            karttaTauluJson += "]";

            karttaTauluJson += ",\"MinX\":"+ 0 + ",\"MinY\":" + 0 + ",\"MaxX\":" + (k.MaxX-k.MinX+1) + ",\"MaxY\":" + (k.MaxY-k.MinY+1) + ",\"TulosteX\":" + k.TulosteX + ",\"TulosteY\":" + k.TulosteY + "}";
            
            ViewBag.karttaTauluJson = karttaTauluJson;
        }

        public bool OnkoSeinaPikseli(Rgba32 pikseli)
        {
            int toleranssi = 80;
            //tummat ja harmaat sävyt lasketaan seiniksi
            if (pikseli.R <= toleranssi && pikseli.G <= toleranssi && pikseli.B <= toleranssi)
            {
                return true;
            }
            //?Lisätään siniselle vielä erikseen kuulakärkikynää varten?

            //Ei sopivaa väriä
            return false;
        }

        public bool OnkoSeinaValissa(int x, int y, Image<Rgba32> kuva, int ruuKoko)
        {
            int ruuPuolvali = ruuKoko / 2;
            int ruuToleranssi = Convert.ToInt32((float)ruuKoko * 0.33f);

            for (int h = 0;h<ruuToleranssi; h++)
            {
                if(OnkoSeinaPikseli(kuva[x*ruuKoko + ruuPuolvali, y*ruuKoko + (ruuKoko-ruuToleranssi)+h]) || OnkoSeinaPikseli(kuva[x*ruuKoko + ruuPuolvali, (y+1)*ruuKoko + h]))
                {
                    return true;
                }
            }
            
            return false;
        }

        public void SetRuutuHuoneStatus(Ruutu[,] ruudut, Image<Rgba32> kuva, int ruuKoko)
        {
            bool RuutuTyyppi = false;

            for (int i = 0; i < ruudut.GetLength(0)-1; i++)
            {
                RuutuTyyppi = false;
                for (int j = 0; j < ruudut.GetLength(1)-1; j++)
                {
                    ruudut[i, j].OnkoAvoin = RuutuTyyppi;
                    if (OnkoSeinaValissa(i, j, kuva, ruuKoko))
                    {
                        RuutuTyyppi = !RuutuTyyppi;
                    }
                }
            }

        }

        public void SetRuutuInstances(Ruutu[,] ruudut)
        {
            for (int i = 0; i < ruudut.GetLength(0); i++)
            {
                for (int j = 0; j < ruudut.GetLength(1); j++)
                {
                    ruudut[i, j] = new Ruutu();
                }
            }

        }

        public void SetRuutuSeinaStatus(Ruutu[,] ruudut)
        {
            int x = ruudut.GetLength(0);
            int y = ruudut.GetLength(1);

            //käydään läpi keskiruudut ja reunaruudut erikseen, ettei tule out of bounds viittauksia
            //vältytään joka ruudun kohdalla out of bounds testauksen tarpeesta
            for (int i = 1; i < x-1; i++)
            {
                for (int j = 1; j < y-1; j++)
                {
                    bool OnkoRuutu = ruudut[i, j].OnkoAvoin;
                    //Testataan onko eri tyyppinen ruutu vieressä. Jos on niin välissä on seinä.
                    if (ruudut[i, j - 1].OnkoAvoin == !OnkoRuutu)
                    {
                        ruudut[i, j].Seinat[0] = true;
                    }
                    if (ruudut[i + 1, j].OnkoAvoin == !OnkoRuutu)
                    {
                        ruudut[i, j].Seinat[1] = true;
                    }
                    if (ruudut[i, j + 1].OnkoAvoin == !OnkoRuutu)
                    {
                        ruudut[i, j].Seinat[2] = true;
                    }
                    if (ruudut[i - 1, j].OnkoAvoin == !OnkoRuutu)
                    {
                        ruudut[i, j].Seinat[3] = true;
                    }
                }
            }
            
            //Reunat erikseen (ei kulmat) ettei mennä out of bounds ruudukossa
            //Pystysuunta
            for(int j = 1;j < y-1; j++)
            {
                bool OnkoRuutu = ruudut[0, j].OnkoAvoin;
                bool OnkoRuutu2 = ruudut[x-1, j].OnkoAvoin;

                if (ruudut[0, j - 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[0, j].Seinat[0] = true;
                }
                if (ruudut[1, j].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[0, j].Seinat[1] = true;
                }
                if (ruudut[0, j + 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[0, j].Seinat[2] = true;
                }

                if (ruudut[x-1, j - 1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[x - 1, j].Seinat[0] = true;
                }
                if (ruudut[x - 1, j + 1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[x - 1, j].Seinat[2] = true;
                }
                if (ruudut[x - 2, j].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[x - 1, j].Seinat[3] = true;
                }
            }
            //Vaakasuunta
            for (int i = 1; i < x - 1; i++)
            {
                bool OnkoRuutu = ruudut[i, 0].OnkoAvoin;
                bool OnkoRuutu2 = ruudut[i, y-1].OnkoAvoin;

                if (ruudut[i+1, 0].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[i, 0].Seinat[1] = true;
                }
                if (ruudut[i, 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[i, 0].Seinat[2] = true;
                }
                if (ruudut[i-1, 0].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[i, 0].Seinat[3] = true;
                }

                if (ruudut[i, y-2].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[i, y-1].Seinat[0] = true;
                }
                if (ruudut[i+1, y-1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[i, y - 1].Seinat[1] = true;
                }
                if (ruudut[i - 1, y - 1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[i, y - 1].Seinat[3] = true;
                }
            }
        }



        public void SetRuutuKulmaStatus(Ruutu[,] ruudut)
        {
            bool ruutuStatus = false;
            int x = ruudut.GetLength(0);
            int y = ruudut.GetLength(1);

            for (int i = 1; i < x-1; i++)
            {
                for (int j = 1; j < y-1; j++)
                {
                    ruutuStatus = ruudut[i, j].OnkoAvoin;

                    if (ruudut[i, j - 1].OnkoAvoin == ruutuStatus && ruudut[i - 1, j].OnkoAvoin == ruutuStatus && ruudut[i - 1, j - 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[0] = true;
                    }
                    if (ruudut[i, j - 1].OnkoAvoin == ruutuStatus && ruudut[i + 1, j].OnkoAvoin == ruutuStatus && ruudut[i + 1, j - 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[1] = true;
                    }
                    if (ruudut[i, j + 1].OnkoAvoin == ruutuStatus && ruudut[i + 1, j].OnkoAvoin == ruutuStatus && ruudut[i + 1, j + 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[2] = true;
                    }
                    if (ruudut[i, j + 1].OnkoAvoin == ruutuStatus && ruudut[i - 1, j].OnkoAvoin == ruutuStatus && ruudut[i - 1, j + 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[3] = true;
                    }
                }
            }

            //Reuna ruudut erikseen ettei mennä out of bounds ruudukossa. Kulmaruutujen kulmat erikseen.
            //Pystysuunta
            for (int j = 1; j < y - 1; j++)
            {
                bool OnkoRuutu = ruudut[0, j].OnkoAvoin;
                bool OnkoRuutu2 = ruudut[x - 1, j].OnkoAvoin;

               
                if (ruudut[0, j - 1].OnkoAvoin == OnkoRuutu && ruudut[0 + 1, j].OnkoAvoin == OnkoRuutu && ruudut[0 + 1, j - 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[0, j].Kulmat[1] = true;
                }
                if (ruudut[0, j + 1].OnkoAvoin == OnkoRuutu && ruudut[0 + 1, j].OnkoAvoin == OnkoRuutu && ruudut[0 + 1, j + 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[0, j].Kulmat[2] = true;
                }

                if (ruudut[x - 1, j - 1].OnkoAvoin == OnkoRuutu2 && ruudut[x - 2, j].OnkoAvoin == OnkoRuutu2 && ruudut[x - 2, j - 1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[x - 1, j].Kulmat[0] = true;
                }
                if (ruudut[x - 1, j + 1].OnkoAvoin == OnkoRuutu2 && ruudut[x - 2, j].OnkoAvoin == OnkoRuutu2 && ruudut[x - 2, j + 1].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[x - 1, j].Kulmat[3] = true;
                }
                                
            }
            //Vaakasuunta
            for (int i = 1; i < x - 1; i++)
            {
                bool OnkoRuutu = ruudut[i, 0].OnkoAvoin;
                bool OnkoRuutu2 = ruudut[i, y - 1].OnkoAvoin;

                if (ruudut[i+1, 0].OnkoAvoin == OnkoRuutu && ruudut[i, 1].OnkoAvoin == OnkoRuutu && ruudut[i+1, 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[i, 0].Kulmat[2] = true;
                }
                if (ruudut[i-1, 0].OnkoAvoin == OnkoRuutu && ruudut[i, 1].OnkoAvoin == OnkoRuutu && ruudut[i-1, 1].OnkoAvoin == !OnkoRuutu)
                {
                    ruudut[i, 0].Kulmat[3] = true;
                }

                if (ruudut[i-1, y - 1].OnkoAvoin == OnkoRuutu2 && ruudut[i, y - 2].OnkoAvoin == OnkoRuutu2 && ruudut[i-1, y - 2].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[i, y - 1].Kulmat[0] = true;
                }
                if (ruudut[i+1, y - 1].OnkoAvoin == OnkoRuutu2 && ruudut[i, y - 2].OnkoAvoin == OnkoRuutu2 && ruudut[i+1, y - 2].OnkoAvoin == !OnkoRuutu2)
                {
                    ruudut[i, y - 1].Kulmat[1] = true;
                }
            }

            //Vielä kaikki neljä ruudukon kulmaa
            //voidaan verrata suoraan kulman suuntaiseen, koska viereiset on aina suljettuja
            if (ruudut[1, 1].OnkoAvoin)
            {
                ruudut[0, 0].Kulmat[2] = true;
            }
            if (ruudut[x-2, 1].OnkoAvoin)
            {
                ruudut[x-1, 0].Kulmat[3] = true;
            }
            if (ruudut[x-2, y-2].OnkoAvoin)
            {
                ruudut[x-1, y-1].Kulmat[0] = true;
            }
            if (ruudut[1, y-2].OnkoAvoin)
            {
                ruudut[0, y-1].Kulmat[1] = true;
            }

        }

        public void SetKuvaMinMaxRuudut(Kartta kKartta)
        {
            kKartta.MinX = kKartta.RuutuTable.GetLength(0)-1;
            kKartta.MinY = kKartta.RuutuTable.GetLength(1)-1;
            kKartta.MaxX = 0;
            kKartta.MaxY = 0;
            for (int i = 1; i < kKartta.RuutuTable.GetLength(0)-1; i++)
            {
                for (int j = 1; j < kKartta.RuutuTable.GetLength(1)-1; j++)
                {
                    if (kKartta.RuutuTable[i, j].OnkoAvoin)
                    {
                        if (kKartta.MinX>i)
                        {
                            kKartta.MinX = i;
                        }
                        if (kKartta.MinY>j)
                        {
                            kKartta.MinY = j;
                        }
                        if (kKartta.MaxX<i)
                        {
                            kKartta.MaxX = i;
                        }
                        if (kKartta.MaxY<j)
                        {
                            kKartta.MaxY = j;
                        }
                    }
                }
            }
            kKartta.MinX--;
            kKartta.MinY--;
            kKartta.MaxX++;
            kKartta.MaxY++;

            kKartta.TulosteX = (kKartta.MaxX - kKartta.MinX + 1) * 20;
            kKartta.TulosteY = (kKartta.MaxY - kKartta.MinY + 1) * 20;

        }
    }
}
