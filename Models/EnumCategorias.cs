using System.ComponentModel;

namespace AnunciadorV1.Models
{
    public enum EnumCategorias
    {
        [Description("Selecionar")]
        Selecionar = 0,

        //[Description("Construção")]
        //Construcao = 1,

        [Description("Indústria e Comércio")]
        Financeiro = 2,

        [Description("Profissionais e Consultoria Especializada")]
        Consultoria = 3,

        [Description("Outros")]
        Outros = 4,

        [Description("Construção, Manutenção e Tecnologia")]
        Informatica = 5,

        [Description("Serviços Pessoais e Bem-estar")]
        SaudeEBeleza = 6
    }
}
