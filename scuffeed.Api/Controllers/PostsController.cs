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
    public class PostsController : ControllerBase
    {
        private readonly PostContext _context;

        public PostsController(PostContext context)
        {
            _context = context;
        }

        private Flair GetFlair(string flairName)
        {
            var genresQuery = _context.Flairs.ToList().Select(g => g);

            if (string.IsNullOrEmpty(flairName))
            {
                return null!;
            }

            genresQuery = genresQuery.Where(
                     g => g.FlairName == flairName);

            if (!genresQuery.Any())
            {
                genresQuery = new List<Flair> {(new Flair
                {
                    Id = 0,
                    FlairName = flairName,
                    Posts = null
                })};
            }

            return genresQuery.First();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(string? filter)
        {
            if (filter != null)
            {
                return Ok(_context.Posts
                .Where(post => post.Flair!.FlairName == filter)
                // .Where(post => (DateTime.Now - post.createdAt).Hours < 24)
                .Include(post => post.Flair));
            }
            return await _context.Posts
            // .Where(post => (DateTime.Now - post.createdAt).Hours < 24)
            .Include(post => post.Flair)
            .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, PostRequest postRequest)
        {
            var postResult = await GetPost(id);
            Post post = postResult.Value!;
            post.Title = postRequest.Title;
            post.Content = postRequest.Content;
            post.Flair = GetFlair(postRequest.Flair);

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
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

        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(PostRequest postRequest)
        {
            Flair flair = GetFlair(postRequest.Flair!);

            Post post = new Post
            {
                Id = postRequest.Id,
                Content = postRequest.Content,
                Title = postRequest.Title,
                Flair = flair
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
