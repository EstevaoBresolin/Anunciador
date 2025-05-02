namespace AnunciadorV1.Models
{
    public class Produto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public List<Preco> Precos { get; set; } = new();
    }
}
