using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TextToSpeech.Models;

namespace TextToSpeech.Controllers
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
            SesSentezleme sesSentezleme = new SesSentezleme();
            string[] tarayiciDilleri = Request.Headers["Accept-Language"].ToString().Split(',');
            sesSentezleme.Dil = tarayiciDilleri[0];
            return View(sesSentezleme);
        }

        [HttpPost]
        public IActionResult Index(SesSentezleme sesSentezleme)
        {
            _ = sesSentezleme.MetinOkumaAsync();
            return View(sesSentezleme);
        }
        [HttpPost]
        public IActionResult Durdur(SesSentezleme sesSentezleme)
        {
            sesSentezleme.MetinOkumayiDurdur();
            return Ok();
        }
        [HttpPost]
        public IActionResult SesKaydet(SesSentezleme sesSentezleme, string dosyaTuru)
        {
            if( !dosyaTuru.Equals("mp3") && !dosyaTuru.Equals("wav"))
            {
                dosyaTuru = "mp3";
            }
            if(sesSentezleme.Metin == null)
            {
                sesSentezleme.Metin = "";
            }
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var dosyaAdi = myuuidAsString + '.' +dosyaTuru;
            sesSentezleme.SesDosyasiKaydet(dosyaAdi).Wait();
            var yol = Path.Combine(Directory.GetCurrentDirectory(), dosyaAdi);
            var bellek = new MemoryStream();
            using (var stream = new FileStream(yol, FileMode.Open))
            {
                stream.CopyTo(bellek);
            }
            bellek.Position = 0;
            System.IO.File.Delete(yol);
            return File(bellek, "audio/"+dosyaTuru, dosyaAdi);
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
    }
}
