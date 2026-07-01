namespace TrabajoPracticoCripto.Models.DTOs
{
    public class Usuari_DTOs
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = "";
        public string Email { get; set; } = "";
        public string Contrasenia { get; set; } = "";
        public string Transacciones { get; set; } = "";
    }
}
