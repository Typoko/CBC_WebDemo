﻿using System;
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


        public IActionResult Index(string id)
        {
            ViewBag.KarttaPath = "/TestFolder/testiKartta3.jpg";
            ViewBag.SivustoPath = "https://localhost:44340/home/index/";
            Kartta kartta = new Kartta();

            

            if (string.IsNullOrEmpty(id))
            {
                kartta.KarttaKuva = Image.Load<Rgba32>("wwwroot/TestFolder/testiKartta3.jpg");
            }
            else
            {
                ViewBag.KarttaPath = "https://i.imgur.com/" + id;
                // lataus testailua! TEMP // https://i.imgur.com/AbsNs83.jpg
                WebClient wc = new WebClient();
                Stream st = wc.OpenRead("https://i.imgur.com/" + id);
                Image<Rgba32> im = Image.Load<Rgba32>(st);
                kartta.KarttaKuva = im;
            }

            kartta.RuutuTable = new Ruutu[(kartta.KarttaKuva.Width)/20, kartta.KarttaKuva.Height/20];


            //En ymmärrä miksi, mutta new Ruutu[50,50]; ei täytä taulukkoa uusilla instansseilla.
            //Funktio täyttää taulun uusilla sen sijaan.
            SetRuutuInstances(kartta.RuutuTable);

            SetRuutuHuoneStatus(kartta.RuutuTable, kartta.KarttaKuva);

            SetRuutuSeinaStatus(kartta.RuutuTable);

            SetRuutuKulmaStatus(kartta.RuutuTable);

            SetKuvaMinMaxRuudut(kartta);


            return View(kartta);
        }

        public bool OnkoSeinaPikseli(Rgba32 pikseli)
        {
            //tummat ja harmaat sävyt lasketaan seiniksi
            if (pikseli.R < 80 && pikseli.G < 80 && pikseli.B < 80)
            {
                return true;
            }
            //?Lisätään siniselle vielä erikseen kuulakärkikynää varten?

            //Ei sopivaa väriä
            return false;
        }

        public bool OnkoSeinaValissa(int x, int y, Image<Rgba32> kuva)
        {
            
            for(int h = 0;h<7;h++)
            {
                if(OnkoSeinaPikseli(kuva[x*20+10,y*20+13+h]) || OnkoSeinaPikseli(kuva[x*20+10,(y+1)*20+h]))
                {
                    return true;
                }
            }
            
            return false;
        }

        public void SetRuutuHuoneStatus(Ruutu[,] ruudut, Image<Rgba32> kuva)
        {
            bool RuutuTyyppi = false;

            for (int i = 0; i < ruudut.GetLength(0)-1; i++)
            {
                RuutuTyyppi = false;
                for (int j = 0; j < ruudut.GetLength(1)-1; j++)
                {
                    ruudut[i, j].OnkoAvoin = RuutuTyyppi;
                    if (OnkoSeinaValissa(i, j, kuva))
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
            for (int i = 0; i < kKartta.RuutuTable.GetLength(0); i++)
            {
                for (int j = 0; j < kKartta.RuutuTable.GetLength(1); j++)
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
