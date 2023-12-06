//using ApiPuntoVenta.Models;
//using ApiPuntoVenta.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ApiPuntoVenta.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly PasswordHasher _passwordHasher;
//        private readonly IConfiguration _configuration;

//        public AuthController(ApplicationDbContext context, PasswordHasher passwordHasher, IConfiguration configuration)
//        {
//            _context = context;
//            _passwordHasher = passwordHasher;
//            _configuration = configuration;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(User model)
//        {
//            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);

//            if (user == null)
//            {
//                return Unauthorized();
//            }

//            var passwordVerified = _passwordHasher.VerifyPassword(user.Password, model.Password);
//            if (!passwordVerified)
//            {
//                return Unauthorized();
//            }

//            var token = GenerateJwtToken(user);

//            return Ok(new { Token = token });
//        }

//        private string GenerateJwtToken(User user)
//        {
//            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            var claims = new[] {
//                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
//                new Claim("name", user.Name),
//                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//            };

//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddMinutes(120),
//                signingCredentials: credentials
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
