namespace AnunciadorV1.Models
{
    public class Preco
    {
        public string Id { get; set; }
        public decimal? UnitAmount { get; set; } // Valor em centavos (ex: 1000 = R$10,00)
        public string Currency { get; set; }
        public string Description { get; set; }

    }
}
