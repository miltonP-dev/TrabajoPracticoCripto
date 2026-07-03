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

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPorUsuario(int usuarioId)
        {
            var transacciones = await _db.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            return Ok(transacciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var transaccion = await _db.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();
            return Ok(transaccion);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Transaccion nueva)
        {
            if (nueva.Cantidad <= 0)
                return BadRequest(new { mensaje = "La cantidad debe ser mayor a 0." });

            var simbolo = nueva.SimboloCripto.ToLower();

            if (nueva.PrecioArs <= 0)
            {
                var precio = await ObtenerPrecioArs(simbolo);
                if (precio == null)
                    return BadRequest(new { mensaje = $"No se pudo obtener el precio de {simbolo.ToUpper()}. Ingresalo manualmente." });

                nueva.PrecioArs = precio.Value;
            }

            if (nueva.Tipo == "Venta")
            {
                var disponible = await CantidadDisponible(nueva.UsuarioId, nueva.SimboloCripto.ToUpper());
                if (nueva.Cantidad > disponible)
                    return BadRequest(new { mensaje = $"No tenés suficiente {nueva.SimboloCripto.ToUpper()}. Disponible: {disponible}" });
            }

            nueva.SimboloCripto = nueva.SimboloCripto.ToUpper();
            nueva.Total = nueva.Cantidad * nueva.PrecioArs;

            _db.Transacciones.Add(nueva);
            await _db.SaveChangesAsync();

            return Ok(nueva);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Editar(int id, Transaccion datos)
        {
            var transaccion = await _db.Transacciones.FindAsync(id);
            if (transaccion == null) return NotFound();

            if (!string.IsNullOrEmpty(datos.SimboloCripto))
                transaccion.SimboloCripto = datos.SimboloCripto.ToUpper();

            if (!string.IsNullOrEmpty(datos.Tipo))
                transaccion.Tipo = datos.Tipo;

            if (datos.Cantidad > 0)
                transaccion.Cantidad = datos.Cantidad;

            if (datos.PrecioArs > 0)
            {
                transaccion.PrecioArs = datos.PrecioArs;
                transaccion.Total = transaccion.Cantidad * transaccion.PrecioArs;
            }

            if (datos.Fecha != default)
                transaccion.Fecha = datos.Fecha;

            await _db.SaveChangesAsync();
            return Ok(transaccion);
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

        private async Task<decimal> CantidadDisponible(int usuarioId, string simbolo)
        {
            var txs = await _db.Transacciones
                .Where(t => t.UsuarioId == usuarioId && t.SimboloCripto == simbolo)
                .ToListAsync();

            var comprado = txs.Where(t => t.Tipo == "Compra").Sum(t => t.Cantidad);
            var vendido = txs.Where(t => t.Tipo == "Venta").Sum(t => t.Cantidad);
            return comprado - vendido;
        }

        private async Task<decimal?> ObtenerPrecioArs(string simbolo)
        {
            try
            {
                var url = $"https://criptoya.com/api/satoshitango/{simbolo}/ars";
                var respuesta = await _http.GetFromJsonAsync<Dictionary<string, object>>(url);

                if (respuesta == null) return null;

                if (respuesta.TryGetValue("ask", out var ask) &&
                    ask is System.Text.Json.JsonElement elem &&
                    elem.TryGetDecimal(out var precio))
                {
                    return precio;
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

