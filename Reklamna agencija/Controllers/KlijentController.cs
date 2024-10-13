using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reklamna_agencija.Data;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KlijentController : ControllerBase
    {
        private readonly DataContext _context;

        public KlijentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reklama>>> GetReklame()
        {
            return await _context.Reklame.ToListAsync();
        }

        [HttpGet("neodobrene-reklame")]
        public async Task<ActionResult<IEnumerable<Reklama>>> GetNeodobreneReklame()
        {
            var neodobreneReklame = await _context.Reklame
                .Where(r => r.Status == false)
                .ToListAsync();

            if (!neodobreneReklame.Any())
            {
                return NotFound();
            }

            // Eksplicitno učitavanje podataka o klijentu i reklamnom panou za svaku reklamu
            foreach (var reklama in neodobreneReklame)
            {
                await _context.Entry(reklama)
                    .Reference(r => r.Klijent)
                    .LoadAsync();

                await _context.Entry(reklama)
                    .Reference(r => r.ReklamniPano)
                    .LoadAsync();
            }

            return neodobreneReklame;
        }
        [HttpGet("odobrene-reklame")]
        public async Task<ActionResult<IEnumerable<Reklama>>> GetOdobreneReklame()
        {
            var neodobreneReklame = await _context.Reklame
                .Where(r => r.Status == true)
                .ToListAsync();

            if (!neodobreneReklame.Any())
            {
                return NotFound();
            }

            // Eksplicitno učitavanje podataka o klijentu i reklamnom panou za svaku reklamu
            foreach (var reklama in neodobreneReklame)
            {
                await _context.Entry(reklama)
                    .Reference(r => r.Klijent)
                    .LoadAsync();

                await _context.Entry(reklama)
                    .Reference(r => r.ReklamniPano)
                    .LoadAsync();
            }

            return neodobreneReklame;
        }

        [HttpGet("slobodnipanoi")]
        public List<ReklamniPano> VratiPanoeZaDatum([FromQuery] DateTime zadatiDatum, [FromQuery] string grad)
        {
            // Uzimamo sve panoe iz baze koji su u zadatom gradu
            var sviPanoiUGradu = _context.ReklamniPanoi
                .Where(p => p.Grad == grad) // Filtriramo panoe po gradu
                .ToList();

            // Uzimamo panoe koji imaju reklame na zadati datum
            var zauzetiPanoi = _context.Reklame
                .Where(r => r.OdDatum <= zadatiDatum && r.DoDatum >= zadatiDatum) // Provera da li se reklama dešava na zadati datum
                .Select(r => r.ReklamniPanoId)
                .ToList();

            // Vraćamo panoe koji su u gradu i nisu zauzeti na zadati datum
            var slobodniPanoi = sviPanoiUGradu
                .Where(p => !zauzetiPanoi.Contains(p.Id))
                .ToList();

            return slobodniPanoi;
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Reklama>> GetReklama(int id)
        {
            var reklama = await _context.Reklame.FindAsync(id);

            if (reklama == null)
            {
                return NotFound();
            }

            return reklama;
        }

        [HttpGet("klijent-reklame/{id}")]
        public async Task<ActionResult<IEnumerable<Reklama>>> GetReklameByKlijentId(int id)
        {
            var reklame = await _context.Reklame.Where(r => r.KlijentId == id).ToListAsync();

            if (!reklame.Any())
            {
                return NotFound();
            }

            return reklame;
        }

        [HttpPost("dodaj-reklamu")]
        public async Task<ActionResult<Reklama>> PostReklama(Reklama reklama)
        {
            _context.Reklame.Add(reklama);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReklama), new { id = reklama.Id }, reklama);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReklama(int id, Reklama reklama)
        {
            if (id != reklama.Id)
            {
                return BadRequest();
            }

            _context.Entry(reklama).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReklamaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("odobri-reklamu/{id}")]
        public async Task<IActionResult> OdobriReklamuIKreirajFakturu(int id)
        {
            // Pronađite reklamu i njen povezani reklamni pano
            var reklama = await _context.Reklame
                .Include(r => r.ReklamniPano)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reklama == null)
            {
                return NotFound();
            }

            try
            {
                // Ažurirajte status reklame na odobreno
                reklama.Status = true;
                _context.Entry(reklama).State = EntityState.Modified;

                // Ažurirajte status zauzetosti reklamnog panoa na true
                if (reklama.ReklamniPano != null)
                {
                    reklama.ReklamniPano.StatusZauzetosti = true;
                    _context.Entry(reklama.ReklamniPano).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

                int brojDana = (int)(reklama.DoDatum - reklama.OdDatum).TotalDays;

                // Izračunavanje iznosa novcani na osnovu cene reklamnog panoa i broja dana
                decimal iznosNovcani = (decimal)(reklama.ReklamniPano.Cijena * brojDana);

                // Kreiranje nove fakture
                var novaFaktura = new Faktura
                {
                    AdminAgencijeId = reklama.ReklamniPano.AdminAgencijeId,
                    ReklamaId = reklama.Id,
                    Datum = DateTime.UtcNow,
                    IznosNovcani = iznosNovcani.ToString(),
                    Status = true
                };

                _context.Fakture.Add(novaFaktura);
                await _context.SaveChangesAsync();

                return Ok("Reklama je uspešno odobrena, a faktura je kreirana.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom odobravanja reklame i kreiranja fakture: {ex.Message}");
            }
        }



        [HttpDelete("obrisi-reklamu/{id}")]
        public async Task<IActionResult> DeleteReklama(int id)
        {
            var reklama = await _context.Reklame.FindAsync(id);
            if (reklama == null)
            {
                return NotFound();
            }

            _context.Reklame.Remove(reklama);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("obrisi-stare-reklame")]
        public async Task<IActionResult> ObrisiStareReklame()
        {
            try
            {
                // Pronalazi sve reklame čiji je OdDatum manji od današnjeg datuma
                var danasnjiDatum = DateTime.UtcNow;
                var stareReklame = await _context.Reklame
                    .Where(r => r.OdDatum < danasnjiDatum)
                    .ToListAsync();

                if (!stareReklame.Any())
                {
                    return NotFound("Nema starih reklama za brisanje.");
                }

                // Uklanja reklame iz konteksta
                _context.Reklame.RemoveRange(stareReklame);
                await _context.SaveChangesAsync();

                return Ok($"Uspešno obrisano {stareReklame.Count} reklama.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom brisanja starih reklama: {ex.Message}");
            }
        }


        private bool ReklamaExists(int id)
        {
            return _context.Reklame.Any(e => e.Id == id);
        }
    }
}
