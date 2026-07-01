using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrabajoPracticoCripto.Data;

namespace TrabajoPracticoCripto.Controllers
{
    [ApiController]
    [Route("api/portafolio")]
    public class PortafolioController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;

        public PortafolioController(AppDbContext db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _http = httpClientFactory.CreateClient();
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obtener(int usuarioId)
        {
            var transacciones = await _db.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            var grupos = transacciones.GroupBy(t => t.SimboloCripto);
            var tenencias = new List<object>();

            decimal costoTotal = 0;
            decimal valorTotal = 0;

            foreach (var grupo in grupos)
            {
                var comprado = grupo.Where(t => t.Tipo == "Compra").Sum(t => t.Cantidad);
                var vendido = grupo.Where(t => t.Tipo == "Venta").Sum(t => t.Cantidad);
                var cantidadActual = comprado - vendido;

                if (cantidadActual <= 0) continue;

                var costoCompras = grupo.Where(t => t.Tipo == "Compra").Sum(t => t.Total);
                var precioPromedio = comprado > 0 ? costoCompras / comprado : 0;
                var costoBase = precioPromedio * cantidadActual;

                var precioActual = await ObtenerPrecioCriptoYa(grupo.Key) ?? 0;
                var valorActual = cantidadActual * precioActual;
                var gananciaPerdida = valorActual - costoBase;

                costoTotal += costoBase;
                valorTotal += valorActual;

                tenencias.Add(new
                {
                    simbolo = grupo.Key,
                    cantidad = cantidadActual,
                    precioPromedio = Math.Round(precioPromedio, 2),
                    costoBase = Math.Round(costoBase, 2),
                    precioActual = Math.Round(precioActual, 2),
                    valorActual = Math.Round(valorActual, 2),
                    gananciaPerdida = Math.Round(gananciaPerdida, 2)
                });
            }

            return Ok(new
            {
                tenencias,
                costoTotal = Math.Round(costoTotal, 2),
                valorTotal = Math.Round(valorTotal, 2),
                gananciaPerdidaTotal = Math.Round(valorTotal - costoTotal, 2)
            });
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
