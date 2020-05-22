using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASPNETWebDemo.Models;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net;
using System.Threading;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Advanced;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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

        //public string TestiString()
        //{
        //    return "Testi 123";
        //}

        //public string Kellonaika()
        //{
        //    return DateTime.Now.ToString();
        //}

        

        public IActionResult Index(string id, string imageUrl, string rKoko, string oWidth, string oHeight, string oStyle)
        {
            ViewBag.KarttaPath = "/TestFolder/tempKartta.jpg";
            ViewBag.SivustoPath = "https://localhost:44340/home/index/";
            Kartta kartta = new Kartta();
            int RuutuKoko;
            int offsetWidth;
            int offsetHeight;
            string mapStyle;

            if (string.IsNullOrEmpty(imageUrl))
            {
                //kartta.KarttaKuva = Image.Load<Rgba32>("wwwroot" + ViewBag.KarttaPath);
                ViewBag.KarttaX = 1;
                ViewBag.KarttaY = 1;
                return View(kartta);
            }
            else
            {
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead(imageUrl);
                Image<Rgba32> im = Image.Load<Rgba32>(st);
                kartta.KarttaKuva = im;

                string iExtension = Path.GetExtension(imageUrl);
                IImageFormat iForm = im.GetConfiguration().ImageFormatsManager.FindFormatByFileExtension(iExtension);

                //string base64String = im.ToBase64String<Rgba32>(iForm);
                //ViewBag.testiSource = "data:image/" + iExtension.Replace(".","") + ";base64," + "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SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSSSSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSSSSSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSSSSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSSSSSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSSSSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQAASSAAAAASSQAAAAASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSAACSQAAAACSSAAAAACSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSCSSSSSSQSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSQSSSSSSSCSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSCSSSSSSQSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSQSSSSSSSCSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSCSSSSSSQSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSQSSSSSSSCSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSQAAAAAACSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSSSSSSSSSSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSAAAAAAAAAASSSSQSSSSQAAAAAASSSSSSSSSSSSSSSSSSSSSQAAAAAAAAACSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSSSQSSSSQSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCSSSSCSSSSCSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSSSSQSSSSQSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQCQAASSSSSCSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSASQACSSSSQSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCQSSSSSSSCSSSSSCSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSQSCSSSSSSQSSSSSQSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSCQSSSSSSSCSSS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";
                ViewBag.base64StringSource = im.ToBase64String<Rgba32>(iForm); //"data:image/" + iExtension.Replace(".","") + ";base64," + base64String;

                //kartta.KarttaKuva.Save("F:/Git_Repo/CBC_WebDemo/ASPNETWebDemo/ASPNETWebDemo/wwwroot/TestFolder/tempKartta.jpg",new JpegEncoder());
                //kartta.KarttaKuva.Save("wwwroot\\TestFolder\\tempKartta.jpg", new JpegEncoder());
                //ViewBag.KarttaPath = imageUrl; //"/TestiFolder/tempKartta.jpg"; //
                ViewBag.KarttaX = kartta.KarttaKuva.Width;
                ViewBag.KarttaY = kartta.KarttaKuva.Height;
            }
            

            if (string.IsNullOrEmpty(rKoko))
            {
                RuutuKoko = 20;
            }
            else
            {
                RuutuKoko = Convert.ToInt32(rKoko);
            }

            if (string.IsNullOrEmpty(oWidth))
            {
                offsetWidth = 0;
            }
            else
            {
                offsetWidth = Convert.ToInt32(oWidth);
            }

            if (string.IsNullOrEmpty(oHeight))
            {
                offsetHeight = 0;
            }
            else
            {
                offsetHeight = Convert.ToInt32(oHeight);
            }

            if (string.IsNullOrEmpty(oStyle))
            {
                mapStyle = "styleOutline";
            }
            else
            {
                mapStyle = oStyle;
            }


            kartta.RuutuTable = new Ruutu[(kartta.KarttaKuva.Width+offsetWidth)/ RuutuKoko, (kartta.KarttaKuva.Height+offsetHeight)/ RuutuKoko];


            //Täytetään ruudukko tyhjillä instansseilla, ettei tule nulliin viittauksia
            SetRuutuInstances(kartta.RuutuTable);

            //Analysoidaan karttakuva ruuduiksi
            SetRuutuHuoneStatus(kartta.RuutuTable, kartta.KarttaKuva, RuutuKoko, offsetWidth, offsetHeight, mapStyle);

            //Selvitetään missä väleissä on seiniä
            SetRuutuSeinaStatus(kartta.RuutuTable);

            //Selvitetään mitkä kulmat on relevantteja
            SetRuutuKulmaStatus(kartta.RuutuTable);

            //Määritellään kuvan reunat, että saadaan kartasta tyhjää pois
            SetKuvaMinMaxRuudut(kartta);

            //Luodaan Json konversio "käsin" kun multidimensional olio array ei tunnu menevän läpi vakiona
            LuoJson(kartta);

            //Luodaan roll20.net API:a varten komento jolla voi luoda seinät suoraan peliin
            ViewBag.Roll20Seinat =  LuoRoll20Seinat(kartta);

            return View(kartta);
        }


        public string LuoRoll20Seinat(Kartta k)
        {
            //Malli !MapFlipper #500@200@170@0@0#500@400@170@0@0, missä # toimii seinän erottimena ja @ arvon.
            //Arvot on x, y, width, height, wall position

            StringBuilder roll20Seinat = new StringBuilder();

            Ruutu[,] ruudut = k.RuutuTable;

            //käydään ruudut läpi järjestyksessä ja aina kun löytyy seinäpala, niin seurataan seinä alusta loppuun ja lisätään listaan
            //joka ruudussa tarkastetaan aina kaikki seinäsuunnat ennen kuin seuraavaan ruutuun siirrytään
            //jo merkatut seinäpalat poistetaan käytettyinä jo seinän merkkaamiseen
            //seinät on merkattu molemmille puolille sitä, joten ainoastaan toiset (ei huoneruudut) käydään läpi

            for (int i = 0; i < (ruudut.GetLength(0) - 1); i++)
            {
                for (int j = 0; j < (ruudut.GetLength(1) - 1); j++)
                {
                    if (!ruudut[i, j].OnkoAvoin)
                    {
                        for (int h = 0; h < 4; h++)
                        {
                            if(ruudut[i,j].Seinat[h])
                            {
                                //lisätään stringbuilderin perään tavaraa
                                int x = 0;
                                int y = 0;

                                do
                                {
                                    //Poistetaan seinän tila, ettei käytetä useasti
                                    ruudut[i + x, j + y].Seinat[h] = false;
                                    
                                    //Onko kyseessä vaaka- vai pystyseinä
                                    if(h == 0 || h == 2)
                                    {
                                        x++;
                                    }
                                    else
                                    {
                                        y++;
                                    }
                                } while (ruudut[i+x, j+y].Seinat[h]);

                                //#500@200@170@0@0
                                //x,y,width,height,wall position
                                roll20Seinat.Append($"#{i}@{j}@{x}@{y}@{h}");
                            }
                        }
                    }
                }
            }
            
            return roll20Seinat.ToString();
        }

        //[HttpPost]
        //public IActionResult Index()
        //{
        //    //Request.Form["testText"].ToString()
        //    Kartta kartta = new Kartta();
        //    ViewBag.KarttaPath = "/TestFolder/tempKartta.jpg";
        //    ViewBag.SivustoPath = "https://localhost:44340/home/index/";

        //    ViewBag.KarttaX = 1;
        //    ViewBag.KarttaY = 1;
        //    return View(kartta);
        //}


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

            if (kuva.Height < y + ruuKoko + ruuToleranssi)
            {
                return false;
            }

            for (int h = 0;h<ruuToleranssi; h++)
            {
                if(OnkoSeinaPikseli(kuva[x + ruuPuolvali, y + (ruuKoko-ruuToleranssi)+h]) || OnkoSeinaPikseli(kuva[x + ruuPuolvali, y + ruuKoko + h]))
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool OnkoRuutuTumma(int x, int y, Image<Rgba32> kuva, int ruuKoko)
        {
            int ruuPuolvali = ruuKoko / 2;
            int ruuToleranssi = Convert.ToInt32((float)ruuKoko * 0.33f);

            for (int h = 0; h < ruuToleranssi; h++)
            {
                if (OnkoSeinaPikseli(kuva[x + ruuPuolvali, y + (ruuKoko - ruuToleranssi) + h]) || OnkoSeinaPikseli(kuva[x + ruuPuolvali, y + ruuKoko + h]))
                {
                    return true;
                }
            }

            return false;
        }

        //osHeight ongelma jos antaa liian paljon plussaa tai miinusta!
        //Pitäisi tarkistaa, että ei ole paljon yli 50% per ruudun koko. Voidaan asettaa -ruuKoKo kunnes alle tai yli sen puolen
        public void SetRuutuHuoneStatus(Ruutu[,] ruudut, Image<Rgba32> kuva, int ruuKoko, int osWidth, int osHeight, string mapStyle)
        {
            bool RuutuTyyppi;

            osHeight %= ruuKoko;
            if (osHeight > ruuKoko / 2) { osHeight -= ruuKoko; }
            if (osHeight < -ruuKoko / 2) { osHeight += ruuKoko; }

            for (int i = 0; i < (ruudut.GetLength(0)-1)*ruuKoko; i+=ruuKoko)
            {
                RuutuTyyppi = false;
                for (int j = 0; j < (ruudut.GetLength(1)-1)*ruuKoko; j+=ruuKoko)
                {
                    if (mapStyle == "styleOutline")
                    {
                        ruudut[i / ruuKoko, j / ruuKoko].OnkoAvoin = RuutuTyyppi;
                        if (OnkoSeinaValissa(i + osWidth, j + osHeight, kuva, ruuKoko))
                        {
                            RuutuTyyppi = !RuutuTyyppi;
                        }
                    }
                    else if (mapStyle == "styleDonjon")
                    {
                        ruudut[i / ruuKoko, j / ruuKoko].OnkoAvoin = !OnkoSeinaPikseli(kuva[i + osWidth + 1, j + osHeight + 1]);
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

            //Tehdään pieni neliö jos ei löydy yhtään huoneruutua
            if(kKartta.MinX>kKartta.MaxX)
            { 
                kKartta.MinX = 1;
                kKartta.MinY = 1;
                kKartta.MaxX = 3;
                kKartta.MaxY = 3;
            }

            kKartta.TulosteX = (kKartta.MaxX - kKartta.MinX + 1) * 20;
            kKartta.TulosteY = (kKartta.MaxY - kKartta.MinY + 1) * 20;

        }
    }
}
