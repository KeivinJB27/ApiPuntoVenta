using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPuntoVenta.Models;
using ApiPuntoVenta.Services;
using Microsoft.AspNetCore.Authorization;

namespace ApiPuntoVenta.Controllers
{
    /// <summary>
    /// Controlador para gestionar operaciones relacionadas con los usuarios del sistema.
    /// </summary>

    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher _passhash;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, PasswordHasher passhash, ILogger<UsersController> logger)
        {
            _context = context;
            _passhash = passhash;
            _logger = logger;
        }


        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                // Intenta obtener la lista de usuarios 
                var users = await _context.Users.ToListAsync();

                // Si todo va bien, devuelve una respuesta de éxito
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Registra el error
                _logger.LogError(ex, "An error occurred while getting the users.");

                // Devuelve un error interno del servidor sin detalles del error
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                // Intenta encontrar el usuario por su ID
                var user = await _context.Users.FindAsync(id);

                // Si el usuario no se encuentra, devuelve un error de no encontrado
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Registra el error
                _logger.LogError(ex, "An error occurred while getting the user.");

                // Devuelve un error interno del servidor sin detalles del error
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            // Verifica si el objeto de usuario es nulo
            if (user == null)
            {
                return BadRequest(new { message = "User is null" });
            }

            // Verifica si el ID del usuario coincide con el ID de la ruta
            if (id != user.UserID)
            {
                return BadRequest(new { message = "Mismatched user ID" });
            }

            // Verifica si el usuario existe antes de intentar actualizarlo
            if (!UserExists(id))
            {
                return NotFound(new { message = "User not found" });
            }

            // Hashea la contraseña antes de actualizar el usuario
            user.Password = _passhash.HashPassword(user.Password);

            // Marca el usuario como modificado
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Registra el error
                _logger.LogError(ex, "An error occurred while updating the user.");

                // Devuelve un error interno del servidor sin detalles del error
                return StatusCode(500, new { message = "Internal server error" });
            }
            // Si todo va bien, devuelve una respuesta de éxito
            return NoContent();
        }


        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            // Verifica si el objeto de usuario es nulo
            if (user == null)
            {
                return BadRequest(new { 
                    success = false,
                    message = "No se recibieron datos del usuario." 
                });
            }

            // Verifica si los datos de entrada son válidos
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                return BadRequest(new
                {
                    success = false,
                    errors = ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
                });
            }

            // Verifica si el usuario ya existe
            if (UserExists(user.UserID))
            {
                return Conflict(new
                {
                    success = false,
                    message = "El usuario ya existe!"
                });
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
            catch (DbUpdateException ex)
            {
                // Registra el error
                _logger.LogError(ex, "Se ha producido un error al guardar los cambios.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error interno en el servidor."
                });
            }

            // Crea un objeto de usuario para la respuesta que no incluye la contraseña
            var userResponse = new
            {
                UserID = user.UserID,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                // Incluye otros campos del usuario aquí, pero no la contraseña
            };

            return CreatedAtAction("GetUser", new { id = user.UserID }, new
            {
                success = true,
                user = userResponse
            });
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
                    return NotFound(new { message = "User not found" });
                }

                // Elimina el usuario del contexto de la base de datos
                _context.Users.Remove(user);

                // Intenta guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Si todo va bien, devuelve una respuesta de éxito
                return NoContent();
            }
            catch (Exception ex)
            {
                // Registra el error
                _logger.LogError(ex, "An error occurred while deleting the user.");

                // Devuelve un error interno del servidor sin detalles del error
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
