using System.ComponentModel.DataAnnotations;

namespace ConsultaDocumentos.Web.Models
{
    public class DocumentoViewModel : BaseViewModel
    {
        public char TipoPessoa { get; set; }
        public string Numero { get; set; }
        public string Nome { get; set; }
        public DateTime DataConsulta { get; set; }
        public DateTime DataConsultaValidade { get; set; }

        // Campos PJ
        public DateTime? DataAbertura { get; set; }
        public string? NomeFantasia { get; set; }
        public string? Inscricao { get; set; }

        // Campos PF
        public DateTime? DataNascimento { get; set; }
        public string? NomeMae { get; set; }
        public string? Sexo { get; set; }

        // NOVOS CAMPOS
        [Display(Name = "Nome Social")]
        public string? NomeSocial { get; set; }

        [Display(Name = "Porte")]
        public string? Porte { get; set; }

        // Referências
        public Guid? IdNacionalidade { get; set; }
        public Guid? IdSituacao { get; set; }
    }
}
