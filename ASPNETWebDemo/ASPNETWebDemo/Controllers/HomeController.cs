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

namespace ASPNETWebDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            return View();
        }

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

        // private readonly IWebHostEnvironment _env;

        public IActionResult KartanMuunnos()
        {
            ViewBag.KarttaPath = "/TestFolder/testiKartta2.jpg";
            //var karttaLadattu = Image<Rgba32>.Load(_env.WebRootPath + "/testiKartta.jpg");
            Kartta kartta = new Kartta();
            kartta.KarttaKuva = Image.Load<Rgba32>("F:/Git_Repo/CBC_WebDemo/ASPNETWebDemo/ASPNETWebDemo/wwwroot/TestFolder/testiKartta2.jpg");
            
            kartta.RuutuTable = new Ruutu[(kartta.KarttaKuva.Width)/20, kartta.KarttaKuva.Height/20];

            //Image<Rgba32> karttaLadattu = Image.Load<Rgba32>("F:/Git_Repo/CBC_WebDemo/ASPNETWebDemo/ASPNETWebDemo/wwwroot/TestFolder/testiKartta.jpg");
            //Ruutu[,] RuutuStatusTable = new Ruutu[50, 50];
            //bool RuutuTyyppi = false;

            //En ymmärrä miksi, mutta new Ruutu[50,50]; ei täytä taulukkoa uusilla instansseilla.
            //Funktio täyttää taulun uusilla sen sijaan.
            SetRuutuInstances(kartta.RuutuTable);

            SetRuutuHuoneStatus(kartta.RuutuTable, kartta.KarttaKuva);

            SetRuutuSeinaStatus(kartta.RuutuTable);

            SetRuutuKulmaStatus(kartta.RuutuTable);

            SetKuvaMinMaxRuudut(kartta);

            //kartta.TulosteX = 1000;
            //kartta.TulosteY = 1000;
            
            //karttaLadattu[1, 1] = Rgba32.Red;

            //var kartta = new Image<Rgba32>(400, 400);

            //ViewBag.Karttatesti = "fail";
            //karttaLadattu[1, 1] = Rgba32.Red;

            //if (karttaLadattu[1, 1] == Rgba32.Red)
            //    ViewBag.Karttatesti = "success";

            return View(kartta);
        }

        public bool OnkoSeinaValissa(int x, int y, Image<Rgba32> kuva)
        {
            for(int h = 0;h<5;h++)
            {
                if(kuva[x*20+10,y*20+15+h] == Rgba32.Black || kuva[x*20+10,(y+1)*20+h] == Rgba32.Black)
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
            //käydään läpi keskiruudut ja reunaruudut erikseen, ettei tule out of bounds viittauksia
            //vältytään joka ruudun kohdalla out of bounds testauksen tarpeesta
            for (int i = 1; i < ruudut.GetLength(0)-1; i++)
            {
                for (int j = 1; j < ruudut.GetLength(1)-1; j++)
                {
                    if(ruudut[i,j].OnkoAvoin == false)
                    {
                        if(ruudut[i,j-1].OnkoAvoin == true)
                        {
                            ruudut[i, j].Seinat[0] = true;
                        }
                        if (ruudut[i+1, j].OnkoAvoin == true)
                        {
                            ruudut[i, j].Seinat[1] = true;
                        }
                        if (ruudut[i, j + 1].OnkoAvoin == true)
                        {
                            ruudut[i, j].Seinat[2] = true;
                        }
                        if (ruudut[i-1, j].OnkoAvoin == true)
                        {
                            ruudut[i, j].Seinat[3] = true;
                        }
                    }
                    else
                    {
                        if (ruudut[i, j - 1].OnkoAvoin == false)
                        {
                            ruudut[i, j].Seinat[0] = true;
                        }
                        if (ruudut[i + 1, j].OnkoAvoin == false)
                        {
                            ruudut[i, j].Seinat[1] = true;
                        }
                        if (ruudut[i, j + 1].OnkoAvoin == false)
                        {
                            ruudut[i, j].Seinat[2] = true;
                        }
                        if (ruudut[i - 1, j].OnkoAvoin == false)
                        {
                            ruudut[i, j].Seinat[3] = true;
                        }
                    }
                }
                //Erikseen tarkastettavat reunat
                //if (ruudut[i, 1].OnkoAvoin == true)
                //{
                //    ruudut[i, 0].Seinat[2] = true;
                //}
                //if (ruudut[ruudut.GetLength(0)-2, i].OnkoAvoin == true)
                //{
                //    ruudut[ruudut.GetLength(0)-1, i].Seinat[3] = true;
                //}
                //if (ruudut[i, ruudut.GetLength(1)-2].OnkoAvoin == true)
                //{
                //    ruudut[i, ruudut.GetLength(1)-1].Seinat[0] = true;
                //}
                //if (ruudut[1, i].OnkoAvoin == true)
                //{
                //    ruudut[0, i].Seinat[1] = true;
                //}
            }
        }

        public void SetRuutuKulmaStatus(Ruutu[,] ruudut)
        {
            bool ruutuStatus = false;
            for (int i = 1; i < ruudut.GetLength(0)-1; i++)
            {
                for (int j = 1; j < ruudut.GetLength(1)-1; j++)
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
                    if (ruudut[i, j + 1].OnkoAvoin == ruutuStatus && ruudut[i - 1, j].OnkoAvoin == ruutuStatus && ruudut[i + 1, j + 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[2] = true;
                    }
                    if (ruudut[i, j + 1].OnkoAvoin == ruutuStatus && ruudut[i + 1, j].OnkoAvoin == ruutuStatus && ruudut[i - 1, j + 1].OnkoAvoin == !ruutuStatus)
                    {
                        ruudut[i, j].Kulmat[3] = true;
                    }
                    
                }
                //Erikseen tarkastettavat reunat
                //if (ruudut[i, 1].OnkoAvoin == true)
                //{
                //    ruudut[i, 0].Seinat[2] = true;
                //}
                //if (ruudut[48, i].OnkoAvoin == true)
                //{
                //    ruudut[49, i].Seinat[3] = true;
                //}
                //if (ruudut[i, 48].OnkoAvoin == true)
                //{
                //    ruudut[i, 49].Seinat[0] = true;
                //}
                //if (ruudut[1, i].OnkoAvoin == true)
                //{
                //    ruudut[0, i].Seinat[1] = true;
                //}
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
