using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using ChamadoTrackerIA.Dtos;
using ChamadoTrackerIA.Models;
using ChamadoTrackerIA.Data;


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

        public async Task<bool> ExcluirTodosChamadosAsync()
        {
            try
            {
                _context.Chamados.RemoveRange(_context.Chamados);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<byte[]> ExportarCsvAsync(int mes, int ano)
        {
            var localDataInicio = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Local);
            var localDataFim = localDataInicio.AddMonths(1).AddTicks(-1);

            var dataInicio = TimeZoneInfo.ConvertTimeToUtc(localDataInicio);
            var dataFim = TimeZoneInfo.ConvertTimeToUtc(localDataFim);


            var chamados = await _context.Chamados
                //.Where(c => c.AbertoEm >= dataInicio && c.AbertoEm <= dataFim)
                .OrderBy(c => c.AbertoEm)
                .ToListAsync();
                foreach (var c in chamados)
                {
                    Console.WriteLine($"AbertoEm (UTC): {c.AbertoEm:o}");
                }


                var chamadosDto = chamados.Select(c => new ChamadoCsvDto
                {
                    Id = c.Id,
                    Numero = c.Numero,
                    Titulo = c.Titulo,
                    Assunto = c.Assunto,
                    Servico = c.Servico,
                    Responsavel = c.Responsavel,
                    AbertoEm = c.AbertoEm.ToString("dd/MM/yyyy HH:mm"),
                    ResolvidoEm = c.ResolvidoEm?.ToString("dd/MM/yyyy HH:mm")

                }).ToList();

                using var memoryStream = new MemoryStream();
                using var writer = new StreamWriter(memoryStream, new UTF8Encoding(true));
                using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                });
                csv.WriteHeader<ChamadoCsvDto>();
                await csv.NextRecordAsync();

                foreach (var chamado in chamadosDto)
                {
                    csv.WriteRecord(chamado);
                    await csv.NextRecordAsync();
                }
                await writer.FlushAsync();
                return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportarCsvMesAtualAsync()
        {
            var agora = DateTime.UtcNow;
            return await ExportarCsvAsync(agora.Month, agora.Year);
        }

    }
}

