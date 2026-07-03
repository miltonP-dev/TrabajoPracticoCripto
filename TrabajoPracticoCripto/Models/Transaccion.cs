namespace TrabajoPracticoCripto.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string SimboloCripto { get; set; } = "";
        public string Tipo { get; set; } = ""; 
        public decimal Cantidad { get; set; }
        public decimal PrecioArs { get; set; }  
        public decimal Total { get; set; }       
        public DateTime Fecha { get; set; }
    }
}
