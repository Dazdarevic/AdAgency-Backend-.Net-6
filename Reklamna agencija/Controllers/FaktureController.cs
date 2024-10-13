using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reklamna_agencija.Data;
using Reklamna_agencija.Models;

namespace Reklamna_agencija.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaktureController : ControllerBase
    {
        private readonly DataContext _context;

        public FaktureController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("kreiraj-fakturu")]
        public async Task<ActionResult<Faktura>> KreirajFakturu(Faktura faktura)
        {
            try
            {
                _context.Fakture.Add(faktura);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetFaktura), new { id = faktura.Id }, faktura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška prilikom kreiranja fakture: {ex.Message}");
            }
        }
        [HttpGet("fakture-za-admina/{id}")]
        public async Task<ActionResult<IEnumerable<Faktura>>> GetFaktureZaAdmina(int id)
        {
            var fakture = await _context.Fakture
                .Where(f => f.AdminAgencijeId == id)
                .ToListAsync();

            if (fakture == null || !fakture.Any())
            {
                return NotFound();
            }

            // Eksplicitno učitavanje podataka o adminu agencije i reklami za svaku fakturu
            foreach (var faktura in fakture)
            {
                await _context.Entry(faktura)
                    .Reference(f => f.AdminAgencije)
                    .LoadAsync();

                await _context.Entry(faktura)
                    .Reference(f => f.Reklama)
                    .LoadAsync();
            }

            return fakture;
        }


        [HttpGet("fakture-za-klijenta/{id}")]
        public async Task<ActionResult<IEnumerable<Faktura>>> GetFaktureZaKlijenta(int id)
        {
            var fakture = await _context.Fakture
                .Where(f => f.Reklama.KlijentId == id)
                .ToListAsync();

            if (fakture == null || !fakture.Any())
            {
                return NotFound();
            }

            // Eksplicitno učitavanje podataka o klijentu agencije i reklami za svaku fakturu
            foreach (var faktura in fakture)
            {
                await _context.Entry(faktura)
                    .Reference(f => f.AdminAgencije)
                    .LoadAsync();

                await _context.Entry(faktura)
                    .Reference(f => f.Reklama)
                    .LoadAsync();
            }

            return fakture;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Faktura>>> GetFakture()
        {
            return await _context.Fakture.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Faktura>> GetFaktura(int id)
        {
            var faktura = await _context.Fakture.FindAsync(id);

            if (faktura == null)
            {
                return NotFound();
            }

            return faktura;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFaktura(int id, Faktura faktura)
        {
            if (id != faktura.Id)
            {
                return BadRequest();
            }

            _context.Entry(faktura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FakturaExists(id))
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
        private bool FakturaExists(int id)
        {
            return _context.Fakture.Any(e => e.Id == id);
        }


    }
}
