using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotionV2.API.DTOs;
using NotionV2.API.Models;

namespace NotionV2.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> Get()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);

            var notes = await _context.Notes.Where(x => x.UserId == user.Id).ToListAsync();
            return notes;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> Get([FromRoute] int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            if (note.UserId != user.Id)
            {
                return Forbid();
            }

            return note;
        }

        [HttpPost]
        public async Task<ActionResult<Note>> Post([FromBody] NoteDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            var note = new Note
            {
                Header = dto.Header ?? "Untitled",
                Body = dto.Body ?? "",
                UserId = user.Id
            };
            
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new {id = note.Id}, note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] NoteDTO dto)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            if (note.UserId != user.Id)
            {
                return Forbid();
            }

            note.Header = dto.Header ?? "Untitled";
            note.Body = dto.Body ?? "";

            _context.Notes.Update(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == User.Identity.Name);
            if (note.UserId != user.Id)
            {
                return Forbid();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}