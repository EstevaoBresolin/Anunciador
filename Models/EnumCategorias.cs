using System.ComponentModel;

namespace AnunciadorV1.Models
{
    public enum EnumCategorias
    {
        [Description("Selecionar")]
        Selecionar = 0,

        [Description("Construção")]
        Construcao = 1,

        [Description("Financeiro")]
        Financeiro = 2,

        [Description("Consultoria")]
        Consultoria = 3,

        [Description("Outros")]
        Outros = 4,

        [Description("Informatica")]
        Informatica = 5,

        [Description("Saúde e Beleza")]
        SaudeEBeleza = 6
    }
}
