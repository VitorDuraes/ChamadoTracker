using System.ComponentModel.DataAnnotations;

namespace ChamadoTrackerIA.Models
{
    public class Chamado
    {
        public int Id { get; set; }
        
        [Required]
        public string Numero { get; set; } = string.Empty;
        
        [Required]
        public string Titulo { get; set; } = string.Empty;
        
        public string Assunto { get; set; } = string.Empty;
        
        public string Servico { get; set; } = string.Empty;
        
        public string Responsavel { get; set; } = string.Empty;
        
        public DateTime AbertoEm { get; set; }
        
        public DateTime? ResolvidoEm { get; set; }
    }
}

