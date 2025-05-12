namespace AnunciadorV1.Models
{
    public class Usuario
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public bool TemAnuncioAtivo { get; set; }
        public bool AssinaturaAtiva { get; set; }
        public List<Anunciante>? Anuncios { get; set; }
    }
}
