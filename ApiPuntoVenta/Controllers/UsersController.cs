using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPuntoVenta.Models;
using ApiPuntoVenta.Services;

namespace ApiPuntoVenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher _passhash;

        public UsersController(ApplicationDbContext context, PasswordHasher passhash)
        {
            _context = context;
            _passhash = passhash;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                // Intenta obtener la lista de usuarios 
                var users = await _context.Users.ToListAsync();
                return users;
            }
            catch (Exception)
            {
                // Si hay un error al obtener los usuarios, devuelve un error interno del servidor
                // Aquí podrías registrar el error
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                // Intenta encontrar el usuario por su ID
                var user = await _context.Users.FindAsync(id);

                // Si el usuario no se encuentra, devuelve un error de no encontrado
                if (user == null)
                {
                    return NotFound();
                }

                return user;
            }
            catch (Exception)
            {
                // Si hay un error al obtener el usuario, devuelve un error interno del servidor
                // Aquí podrías registrar el error
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            // Verifica si el objeto de usuario es nulo
            if (user == null)
            {
                return BadRequest("User is null");
            }

            // Verifica si el ID del usuario coincide con el ID de la ruta
            if (id != user.UserID)
            {
                return BadRequest();
            }

            // Verifica si el usuario existe antes de intentar actualizarlo
            if (!UserExists(id))
            {
                return NotFound();
            }

            // Marca el usuario como modificado
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si hay un error al guardar los cambios, devuelve un error interno del servidor
                // Aquí podrías registrar el error
                return StatusCode(500, "Internal server error");
            }
            // Si todo va bien, devuelve una respuesta de éxito
            return NoContent();
        }


        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            // Verifica si el objeto de usuario es nulo
            if (user == null)
            {
                return BadRequest("User is null");
            }

            // Hashea la contraseña antes de guardar el usuario
            user.Password = _passhash.HashPassword(user.Password);

            // Agrega el usuario al contexto de la base de datos
            _context.Users.Add(user);

            try
            {
                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Si hay un error al guardar los cambios, verifica si el usuario ya existe
                if (UserExists(user.UserID))
                {
                    // Si el usuario ya existe, devuelve un error de conflicto
                    return Conflict();
                }
                else
                {
                    // Si el usuario no existe, devuelve un error interno del servidor
                    // Aquí podrías registrar el error
                    return StatusCode(500, "Internal server error");
                }
            }
            // Si todo va bien, devuelve una respuesta de éxito con la ubicación del nuevo recurso
            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Intenta encontrar el usuario por su ID
                var user = await _context.Users.FindAsync(id);

                // Si el usuario no se encuentra, devuelve un error de no encontrado
                if (user == null)
                {
                    return NotFound();
                }

                // Elimina el usuario del contexto de la base de datos
                _context.Users.Remove(user);

                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Si todo va bien, devuelve una respuesta de éxito
                return NoContent();
            }
            catch (Exception)
            {
                // Si hay un error al eliminar el usuario, devuelve un error interno del servidor
                // Aquí podrías registrar el error
                return StatusCode(500, "Internal server error");
            }
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
