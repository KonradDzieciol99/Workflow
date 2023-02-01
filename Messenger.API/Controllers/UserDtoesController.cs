using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Socjal.API.Dto;
using Socjal.API.Persistence;

namespace Socjal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDtoesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserDtoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserDtoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserDto()
        {
            return await _context.UserDto.ToListAsync();
        }

        // GET: api/UserDtoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserDto(string id)
        {
            var userDto = await _context.UserDto.FindAsync(id);

            if (userDto == null)
            {
                return NotFound();
            }

            return userDto;
        }

        // PUT: api/UserDtoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDto(string id, UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDtoExists(id))
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

        // POST: api/UserDtoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUserDto(UserDto userDto)
        {
            _context.UserDto.Add(userDto);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDtoExists(userDto.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserDto", new { id = userDto.Id }, userDto);
        }

        // DELETE: api/UserDtoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDto(string id)
        {
            var userDto = await _context.UserDto.FindAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }

            _context.UserDto.Remove(userDto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDtoExists(string id)
        {
            return _context.UserDto.Any(e => e.Id == id);
        }
    }
}
