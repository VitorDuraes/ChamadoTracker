using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChamadoTrackerIA.Dtos
{
    public class ChamadoCsvDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Assunto { get; set; } = string.Empty;
        public string Servico { get; set; } = string.Empty;
        public string Responsavel { get; set; } = string.Empty;
        public string AbertoEm { get; set; } = string.Empty;
        public string? ResolvidoEm { get; set; }

    }
}