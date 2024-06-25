using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using static System.Net.Mime.MediaTypeNames;
namespace TextToSpeech.Models
{
    public class SesSentezleme
    {
        private static string abonelikAnahtari = "API_KEY";
        private static string abonelikBolgesi = "API_REGION";
        static SpeechSynthesizer sentezleyici = null;
        public string Metin { get; set; }
        public string Dil { get; set; }
        public string Konusmaci { get; set; }

        public async Task MetinOkumaAsync()
        {
            var yapilandirma = SpeechConfig.FromSubscription(abonelikAnahtari, abonelikBolgesi);
            yapilandirma.SpeechSynthesisLanguage = Dil;
            yapilandirma.SpeechSynthesisVoiceName = Konusmaci;
            using (sentezleyici = new SpeechSynthesizer(yapilandirma))
            {
                var sonuc = await sentezleyici.SpeakTextAsync(Metin);

                if (sonuc.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Metin Seslendirildi");
                }
                else if (sonuc.Reason == ResultReason.Canceled)
                {
                    var iptal = SpeechSynthesisCancellationDetails.FromResult(sonuc);
                    Console.WriteLine($"Seslendirme iptal edildi: Sebep ={iptal.Reason}");

                    if (iptal.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"Seslendirme iptal edildi: Hata Kodu - Detay ={iptal.ErrorCode}\n{iptal.ErrorDetails}");
                    }
                }
            }
        }
        public void MetinOkumayiDurdur()
        {
            sentezleyici.StopSpeakingAsync();
        }
        public static async Task<List<VoiceInfo>> SesSecenekleri()
        {
            var yapilandirma = SpeechConfig.FromSubscription(abonelikAnahtari, abonelikBolgesi);
            List<VoiceInfo> sesListesi = new List<VoiceInfo>();
            using (var sentezleyici = new SpeechSynthesizer(yapilandirma))
            {
                using (var sonuc = await sentezleyici.GetVoicesAsync())
                {
                    if (sonuc.Reason == ResultReason.VoicesListRetrieved)
                    {
                        Console.WriteLine("Ses seçenekleri getirildi.");
                        sesListesi = sonuc.Voices.ToList();
                    }
                    else if (sonuc.Reason == ResultReason.Canceled)
                    {
                        Console.WriteLine($"İptal Edildi : Detay=[{sonuc.ErrorDetails}]");
                    }
                }
            }
            return sesListesi;
        }
        public async Task SesDosyasiKaydet(string dosyaAdi)
        {
            var yapilandirma = SpeechConfig.FromSubscription(abonelikAnahtari, abonelikBolgesi);
            yapilandirma.SpeechSynthesisLanguage = Dil;
            yapilandirma.SpeechSynthesisVoiceName = Konusmaci;
            using (var dosyaCiktisi = AudioConfig.FromWavFileInput(dosyaAdi))
            using (var sentezleyici = new SpeechSynthesizer(yapilandirma, dosyaCiktisi))
            {
                using (var sonuc = await sentezleyici.SpeakTextAsync(Metin))
                {
                    if (sonuc.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Ses dosya gönderim işlemi başarılı");
                    }
                    else if (sonuc.Reason == ResultReason.Canceled)
                    {
                        var iptal = SpeechSynthesisCancellationDetails.FromResult(sonuc);
                        Console.WriteLine($"Dosya indirme işlemi iptal edildi: Sebep={iptal.Reason}");

                        if (iptal.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"Seslendirme iptal edildi: Hata Kodu - Detay ={iptal.ErrorCode}");
                        }
                    }
                }
            }
        }
    }
}
