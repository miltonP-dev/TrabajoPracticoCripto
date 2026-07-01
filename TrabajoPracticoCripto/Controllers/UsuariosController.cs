using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabajoPracticoCripto.Data;
using TrabajoPracticoCripto.Models;

namespace TrabajoPracticoCripto.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsuariosController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar(Usuario usuario)
        {
            var existe = await _db.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario);

            if (existe)
                return BadRequest(new { mensaje = "Ese usuario ya existe." });

            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                id = usuario.Id,
                nombreUsuario = usuario.NombreUsuario,
                email = usuario.Email
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Usuario datos)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u =>
                u.NombreUsuario == datos.NombreUsuario &&
                u.Contrasenia == datos.Contrasenia);

            if (usuario == null)
                return BadRequest(new { mensaje = "Usuario o contraseña incorrectos." });

            return Ok(new
            {
                id = usuario.Id,
                nombreUsuario = usuario.NombreUsuario,
                email = usuario.Email
            });    
        }
    }
}
