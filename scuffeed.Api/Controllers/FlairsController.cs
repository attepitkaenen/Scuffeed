using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace scuffeed.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlairsController : ControllerBase
    {        
        private readonly PostContext _context;

        public FlairsController(PostContext context)
        {
            _context = context;
        }

        // GET: api/Flairs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flair>>> GetFlairs(string? filter)
        {
            if (_context.Flairs == null)
            {
                return NotFound();
            }
            return await _context.Flairs.ToListAsync();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlair(int id)
        {
            var flair = await _context.Flairs.FindAsync(id);
            if (flair == null)
            {
                return NotFound();
            }

            _context.Flairs.Remove(flair);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}