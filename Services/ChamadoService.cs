using Microsoft.EntityFrameworkCore;
using ChamadoTrackerIA.Data;
using ChamadoTrackerIA.Models;
using System.Text;

namespace ChamadoTrackerIA.Services
{
    public class ChamadoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ChamadoParserService _parserService;

        public ChamadoService(ApplicationDbContext context, ChamadoParserService parserService)
        {
            _context = context;
            _parserService = parserService;
        }

        public async Task<List<Chamado>> ProcessarTextoAsync(string textoColado)
        {
            var chamados = _parserService.ParseChamados(textoColado);

            if (chamados.Any())
            {
                _context.Chamados.AddRange(chamados);
                await _context.SaveChangesAsync();
            }

            return chamados;
        }

        public async Task<List<Chamado>> ObterTodosChamadosAsync()
        {
            return await _context.Chamados
                .OrderByDescending(c => c.AbertoEm)
                .ToListAsync();
        }

        public async Task<Chamado?> ObterChamadoPorIdAsync(int id)
        {
            return await _context.Chamados.FindAsync(id);
        }

        public async Task<List<Chamado>> ObterChamadosFiltradosAsync(DateTime? dataInicio = null,
            DateTime? dataFim = null, string? responsavel = null, string? servico = null)
        {
            var query = _context.Chamados.AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(c => c.AbertoEm >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(c => c.AbertoEm <= dataFim.Value);

            if (!string.IsNullOrEmpty(responsavel))
                query = query.Where(c => c.Responsavel.Contains(responsavel));

            if (!string.IsNullOrEmpty(servico))
                query = query.Where(c => c.Servico.Contains(servico));

            return await query.OrderByDescending(c => c.AbertoEm).ToListAsync();
        }

        public async Task<bool> AtualizarChamadoAsync(Chamado chamado)
        {
            try
            {
                _context.Chamados.Update(chamado);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExcluirChamadoAsync(int id)
        {
            try
            {
                var chamado = await _context.Chamados.FindAsync(id);
                if (chamado != null)
                {
                    _context.Chamados.Remove(chamado);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> ExportarCsvAsync(int mes, int ano)
        {
            var dataInicio = new DateTime(ano, mes, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1);

            var chamados = await _context.Chamados
                .Where(c => c.AbertoEm >= dataInicio && c.AbertoEm <= dataFim)
                .OrderBy(c => c.AbertoEm)
                .ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Numero;Titulo;Assunto;Servico;Responsavel;AbertoEm;ResolvidoEm");

            foreach (var chamado in chamados)
            {
                csv.AppendLine($"\"{chamado.Numero}\";\"{chamado.Titulo}\";\"{chamado.Assunto}\";\"{chamado.Servico}\";\"{chamado.Responsavel}\";\"{chamado.AbertoEm:dd/MM/yyyy HH:mm}\";\"{chamado.ResolvidoEm?.ToString("dd/MM/yyyy HH:mm") ?? ""}\"");
            }

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        }

        public async Task<byte[]> ExportarCsvMesAtualAsync()
        {
            var agora = DateTime.Now;
            return await ExportarCsvAsync(agora.Month, agora.Year);
        }
    }
}

