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
            ViewBag.KarttaPath = "/TestFolder/testiKartta.jpg";
            //var karttaLadattu = Image<Rgba32>.Load(_env.WebRootPath + "/testiKartta.jpg");
            Image<Rgba32> karttaLadattu = Image.Load<Rgba32>("F:/Git_Repo/CBC_WebDemo/ASPNETWebDemo/ASPNETWebDemo/wwwroot/TestFolder/testiKartta.jpg");
            bool[,] RuutuStatusTable = new bool[50, 50];
            bool RuutuTyyppi = false;

            for(int i = 0;i<49;i++)
            {
                RuutuTyyppi = false;
                for(int j = 0;j<49;j++)
                {
                    RuutuStatusTable[i, j] = RuutuTyyppi;
                    if (OnkoSeinaValissa(i,j,karttaLadattu))
                    {
                        RuutuTyyppi = !RuutuTyyppi;
                    }
                }
            }

            //karttaLadattu[1, 1] = Rgba32.Red;

            //var kartta = new Image<Rgba32>(400, 400);

            ViewBag.Karttatesti = "fail";
            karttaLadattu[1, 1] = Rgba32.Red;

            if (karttaLadattu[1, 1] == Rgba32.Red)
                ViewBag.Karttatesti = "success";

            return View(RuutuStatusTable);
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
    }
}
