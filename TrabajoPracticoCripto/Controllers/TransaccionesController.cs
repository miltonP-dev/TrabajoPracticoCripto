using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabajoPracticoCripto.Data;
using TrabajoPracticoCripto.Models;

namespace TrabajoPracticoCripto.Controllers
{
    [ApiController]
    [Route("api/transacciones")]
    public class TransaccionesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;

        public TransaccionesController(AppDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _http = httpClientFactory.CreateClient();
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var transacciones = await _db.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            return Ok(transacciones);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Transaccion nueva)
        {
            var simbolo = nueva.SimboloCripto.ToUpper();

            if (nueva.PrecioUsd <= 0)
            {
                var precio = await ObtenerPrecioCriptoYa(simbolo);
                if (precio == null)
                    return BadRequest(new { mensaje = $"No se pudo obtener el precio de {simbolo}." });

                nueva.PrecioUsd = precio.Value;
            }

            nueva.SimboloCripto = simbolo;
            nueva.Total = nueva.Cantidad * nueva.PrecioUsd;
            nueva.Fecha = DateTime.Now;

            _db.Transacciones.Add(nueva);
            await _db.SaveChangesAsync();

            return Ok(nueva);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var transaccion = await _db.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();

            _db.Transacciones.Remove(transaccion);
            await _db.SaveChangesAsync();
            return Ok();
        }

        private async Task<decimal?> ObtenerPrecioCriptoYa(string simbolo)
        {
            try
            {
                var url = $"https://criptoya.com/api/{simbolo.ToLower()}/usd";
                var respuesta = await _http.GetFromJsonAsync<Dictionary<string, Dictionary<string, decimal>>>(url);

                if (respuesta == null || respuesta.Count == 0) return null;
        
                foreach (var exchange in respuesta.Values)
                {
                    if (exchange.ContainsKey("ask"))
                        return exchange["ask"];
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}

