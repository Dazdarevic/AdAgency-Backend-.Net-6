using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reklamna_agencija.Data;
using Reklamna_agencija.Models;
using System.Net;
using System.Net.Mail;

namespace Reklamna_agencija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAgencijeController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminAgencijeController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("analitika/zauzetost-po-gradovima")]
        public Dictionary<string, Dictionary<string, int>> ZauzetostPoGradovima(int godina)
        {
            var rezultati = new Dictionary<string, Dictionary<string, int>>();

            // Dohvatimo sve jedinstvene gradove iz baze
            var sviGradovi = _context.ReklamniPanoi.Select(p => p.Grad).Distinct().ToList();

            // Inicijaliziramo rezultate za sve gradove s nulama
            foreach (var grad in sviGradovi)
            {
                rezultati[grad] = Enumerable.Range(1, 12)
                    .ToDictionary(m => new DateTime(godina, m, 1).ToString("MMMM"), _ => 0);
            }

            // Dohvatimo i izračunamo podatke o zauzetosti
            var reklameGrupe = _context.Reklame
                .Where(r => r.Status == true && r.OdDatum.Year == godina)
                .Join(_context.ReklamniPanoi,
                    reklama => reklama.ReklamniPanoId,
                    pano => pano.Id,
                    (reklama, pano) => new { Grad = pano.Grad, Mesec = reklama.OdDatum.Month })
                .GroupBy(x => new { x.Grad, x.Mesec })
                .Select(g => new { g.Key.Grad, g.Key.Mesec, Count = g.Count() })
                .ToList();

            // Popunimo rezultate sa stvarnim podacima
            foreach (var grupa in reklameGrupe)
            {
                rezultati[grupa.Grad][new DateTime(godina, grupa.Mesec, 1).ToString("MMMM")] = grupa.Count;
            }

            return rezultati;
        }

        [HttpGet("analitika")]
        // Procenat zauzetosti po ceni za svaku kategoriju panoa na godišnjem nivou
        public Dictionary<string, Dictionary<string, double>> ProcenatZauzetostiPoCeniGodisnje(int godina)
        {
            // Kreiramo rečnik za čuvanje rezultata za svaki mesec
            var rezultati = new Dictionary<string, Dictionary<string, double>>();

            // Preuzimamo sve panoi i reklame odjednom
            var panoi = _context.ReklamniPanoi.ToList();
            var reklame = _context.Reklame.Where(r => r.OdDatum.Year == godina && r.Status == true).ToList();

            // Kreiramo tri grupe panoa
            var jeftiniPanoi = panoi.Where(p => p.Cijena <= 1000).ToList();
            var srednjeSkupiPanoi = panoi.Where(p => p.Cijena > 1000 && p.Cijena <= 2000).ToList();
            var skupiPanoi = panoi.Where(p => p.Cijena > 5000).ToList();

            // Prolazimo kroz sve mesece u godini (od 1 do 12)
            for (int mesec = 1; mesec <= 12; mesec++)
            {
                // Izračunavamo broj zauzetih panoa u svakoj kategoriji za dati mesec
                var brojZauzetihJeftini = jeftiniPanoi.Count(p => reklame.Any(r => r.ReklamniPanoId == p.Id && r.OdDatum.Month == mesec));
                var brojZauzetihSrednjeSkupi = srednjeSkupiPanoi.Count(p => reklame.Any(r => r.ReklamniPanoId == p.Id && r.OdDatum.Month == mesec));
                var brojZauzetihSkupi = skupiPanoi.Count(p => reklame.Any(r => r.ReklamniPanoId == p.Id && r.OdDatum.Month == mesec));

                // Ukupan broj zauzetih panoa za dati mesec
                var ukupnoZauzetih = brojZauzetihJeftini + brojZauzetihSrednjeSkupi + brojZauzetihSkupi;

                // Ako nema zauzetih panoa u mesecu, postavljamo procenat na 0
                if (ukupnoZauzetih == 0)
                {
                    rezultati.Add(
                        new DateTime(godina, mesec, 1).ToString("MMMM"),
                        new Dictionary<string, double>
                        {
                    { "Jeftini panoi", 0 },
                    { "Srednje skupi panoi", 0 },
                    { "Skupi panoi", 0 }
                        }
                    );
                }
                else
                {
                    // Izračunavamo procente
                    var procenatJeftini = (double)brojZauzetihJeftini / ukupnoZauzetih * 100;
                    var procenatSrednjeSkupi = (double)brojZauzetihSrednjeSkupi / ukupnoZauzetih * 100;
                    var procenatSkupi = (double)brojZauzetihSkupi / ukupnoZauzetih * 100;

                    // Dodajemo rezultate u rečnik
                    rezultati.Add(
                        new DateTime(godina, mesec, 1).ToString("MMMM"),
                        new Dictionary<string, double>
                        {
                    { "Jeftini panoi", procenatJeftini },
                    { "Srednje skupi panoi", procenatSrednjeSkupi },
                    { "Skupi panoi", procenatSkupi }
                        }
                    );
                }
            }

            return rezultati;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReklamniPano>>> GetReklamniPanoi()
        {
            return await _context.ReklamniPanoi.ToListAsync();
        }

        [HttpGet("panoiPoGradu/{grad}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPanoiPoGradu(string grad)
        {
            // Uzmemo sve panoe u određenom gradu
            var reklamniPanoi = await _context.ReklamniPanoi
                .Where(r => r.Grad == grad)
                .ToListAsync();

            if (reklamniPanoi == null || !reklamniPanoi.Any())
            {
                return NotFound($"Nema reklamnih panoa za grad {grad}.");
            }

            // Uzmemo sve reklame za sve panoe u ovom gradu
            var reklame = await _context.Reklame
                .Where(r => reklamniPanoi.Select(p => p.Id).Contains(r.ReklamniPanoId))
                .ToListAsync();

            // Kreiramo listu sa rezultatima koja sadrži i procenat zauzetosti i broj zauzetih dana
            var result = reklamniPanoi.Select(pano =>
            {
                // Ukupan broj dana u tekućoj godini
                int totalDaysInYear = 365;
                var currentYearStart = new DateTime(DateTime.Now.Year, 1, 1);
                var currentYearEnd = new DateTime(DateTime.Now.Year, 12, 31);

                // Filtriramo reklame koje su povezane sa ovim panoom i koje su u tekućoj godini
                var panoReklame = reklame
                    .Where(r => r.ReklamniPanoId == pano.Id &&
                                r.OdDatum <= currentYearEnd &&
                                r.DoDatum >= currentYearStart)
                    .ToList();

                // Ukupan broj dana kada je pano bio zauzet unutar tekuće godine
                int totalZauzetiDani = panoReklame
                    .Sum(r =>
                    {
                // Računamo presek između datuma reklame i tekuće godine
                        var start = r.OdDatum < currentYearStart ? currentYearStart : r.OdDatum;
                        var end = r.DoDatum > currentYearEnd ? currentYearEnd : r.DoDatum;

                        return (end - start).Days + 1; // Dodajemo 1 jer je interval inkluzivan
                    });

                // Ako nema nijedne reklame, zauzetost je 0%
                double procenatZauzetosti = totalZauzetiDani > 0
                    ? Math.Round((double)totalZauzetiDani / totalDaysInYear * 100, 2) // Zaokruživanje na 2 decimale
                    : 0;

                return new
                {
                    Pano = pano,
                    ProcenatZauzetosti = procenatZauzetosti,
                    BrojDanaZauzetosti = totalZauzetiDani // Dodajemo broj zauzetih dana
                };
            });

            return Ok(result);
        }


        [HttpGet("SlobodniReklamniPanoi/{grad}")]
        public async Task<ActionResult<IEnumerable<ReklamniPano>>> GetSlobodniReklamniPanoi(string grad)
        {
            var reklamniPanoi = await _context.ReklamniPanoi
                .Where(r => r.Grad == grad && r.StatusZauzetosti == false)
                .ToListAsync();

            return Ok(reklamniPanoi);
        }

        [HttpGet("ReklamniPanoiNotInReklame")]
        public async Task<ActionResult<IEnumerable<ReklamniPano>>> GetReklamniPanoiNotInReklame()
        {
            var reklamniPanoiNotInReklame = await _context.ReklamniPanoi
                .Where(rp => !_context.Reklame.Any(r => r.ReklamniPanoId == rp.Id))
                .ToListAsync();

            return reklamniPanoiNotInReklame;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ReklamniPano>> GetReklamniPano(int id)
        {
            var reklamniPano = await _context.ReklamniPanoi.FindAsync(id);

            if (reklamniPano == null)
            {
                return NotFound();
            }

            return reklamniPano;
        }
        [HttpGet("gradovi")]
        public async Task<ActionResult<IEnumerable<string>>> GetGradovi()
        {
            var gradovi = await _context.ReklamniPanoi
                .Select(r => r.Grad) // Uzmite samo grad
                .Distinct() // Uzmite jedinstvene gradove
                .ToListAsync();

            if (gradovi == null || !gradovi.Any())
            {
                return NotFound("Nema dostupnih gradova.");
            }

            return gradovi;
        }

        [HttpPost]
        public async Task<ActionResult<ReklamniPano>> PostReklamniPano(ReklamniPano reklamniPano)
        {
            _context.ReklamniPanoi.Add(reklamniPano);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReklamniPano), new { id = reklamniPano.Id }, reklamniPano);
        }

        [HttpPut("azuriraj/{id}")]
        public async Task<IActionResult> AzurirajReklamniPano(int id, ReklamniPano noviReklamniPano)
        {
            if (id != noviReklamniPano.Id)
            {
                return BadRequest("ID reklamnog panoa se ne podudara s ID-om u objektu.");
            }

            var postojeciReklamniPano = await _context.ReklamniPanoi.FindAsync(id);

            if (postojeciReklamniPano == null)
            {
                return NotFound("Reklamni pano s zadanim ID-om nije pronađen.");
            }

            postojeciReklamniPano.AdminAgencijeId = noviReklamniPano.AdminAgencijeId;
            postojeciReklamniPano.UrlSlike = noviReklamniPano.UrlSlike;
            postojeciReklamniPano.Adresa = noviReklamniPano.Adresa;
            postojeciReklamniPano.Dimenzija = noviReklamniPano.Dimenzija;
            postojeciReklamniPano.Osvetljenost = noviReklamniPano.Osvetljenost;
            postojeciReklamniPano.Latitude = noviReklamniPano.Latitude;
            postojeciReklamniPano.Longitude = noviReklamniPano.Longitude;

            postojeciReklamniPano.Grad = noviReklamniPano.Grad;
            postojeciReklamniPano.Zona = noviReklamniPano.Zona;
            postojeciReklamniPano.Cijena = noviReklamniPano.Cijena;
            postojeciReklamniPano.StatusZauzetosti = noviReklamniPano.StatusZauzetosti;


            // Spremite promjene u bazu podataka
            await _context.SaveChangesAsync();
            return NoContent(); // Vraća se status 204 - uspješno ažurirano
        }

        private bool ReklamniPanoExists(int id)
        {
            return _context.ReklamniPanoi.Any(e => e.Id == id);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReklamniPano(int id)
        {
            var reklamniPano = await _context.ReklamniPanoi.FindAsync(id);
            if (reklamniPano == null)
            {
                return NotFound();
            }

            _context.ReklamniPanoi.Remove(reklamniPano);
            await _context.SaveChangesAsync();

            return Ok("Uspesno izbrisano.");
        }


        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(string email, string subject, string message)
        {
            try
            {
                using (var client = new SmtpClient("smtp.office365.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("", "");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("belkisa.dazdarevic1@gmail.com"),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                }

                return Ok("Email successfully sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }
    }
}


