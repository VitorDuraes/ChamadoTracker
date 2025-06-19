using System.Text.Json;
using System.Text.RegularExpressions;
using ChamadoTrackerIA.Models;

namespace ChamadoTrackerIA.Services
{
    public class ChamadoParserService
    {
        public List<Chamado> ParseChamados(string textoColado)
        {
            var chamados = new List<Chamado>();

            if (string.IsNullOrWhiteSpace(textoColado))
                return chamados;

            var blocos = textoColado.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var bloco in blocos)
            {
                var chamado = ParseBlocoChamado(bloco.Trim());
                if (chamado != null)
                {
                    chamados.Add(chamado);
                }
            }

            return chamados;
        }

        private Chamado? ParseBlocoChamado(string bloco)
        {
            if (string.IsNullOrWhiteSpace(bloco))
                return null;

            var linhas = bloco.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(l => l.Trim())
                             .Where(l => !string.IsNullOrEmpty(l))
                             .ToArray();

            if (linhas.Length == 0)
                return null;

            var chamado = new Chamado();
            var primeiraLinha = linhas[0];
            var segundaLinha = linhas.Length > 1 ? linhas[1] : string.Empty;

            var numeroMatch = Regex.Match(primeiraLinha, @"^(\d+)$");
            if (numeroMatch.Success)
            {
                chamado.Numero = numeroMatch.Groups[1].Value;
                if (linhas.Length > 1)
                {
                    chamado.Titulo = segundaLinha;
                }
                else
                {
                    chamado.Titulo = "Título não informado";
                }
            }
            else
            {
                numeroMatch = Regex.Match(primeiraLinha, @"^(\d+)\s*-\s*(.+)$");
                if (numeroMatch.Success)
                {
                    chamado.Numero = numeroMatch.Groups[1].Value;
                    chamado.Titulo = numeroMatch.Groups[2].Value.Trim();
                }
                else
                {
                    chamado.Titulo = primeiraLinha;
                    chamado.Numero = "N/A";
                }
            }

            int startIndex = (numeroMatch.Success && Regex.IsMatch(primeiraLinha, @"^\d+$")) ? 2 : 1;
            for (int i = startIndex; i < linhas.Length; i++)
            {
                var linha = linhas[i];

                var dataMatch = Regex.Match(linha, @"(\d{2}/\d{2}/\d{4}\s+\d{2}:\d{2})");
                if (dataMatch.Success)
                {
                    if (DateTime.TryParseExact(dataMatch.Groups[1].Value, "dd/MM/yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out DateTime data))
                    {
                        if (chamado.AbertoEm == default(DateTime))
                        {
                            chamado.AbertoEm = data;
                        }
                        else if (!chamado.ResolvidoEm.HasValue)
                        {
                            chamado.ResolvidoEm = data;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(chamado.Assunto))
                {
                    chamado.Assunto = linha;
                    chamado.Servico = linha;
                }
                else if (string.IsNullOrEmpty(chamado.Responsavel))
                {
                    if (linha.Contains(" ") && !Regex.IsMatch(linha, @"\d"))
                    {
                        chamado.Responsavel = linha;
                    }
                }
            }
            if (string.IsNullOrEmpty(chamado.Assunto))
                chamado.Assunto = chamado.Titulo;

            if (string.IsNullOrEmpty(chamado.Servico))
                chamado.Servico = chamado.Assunto;

            if (string.IsNullOrEmpty(chamado.Responsavel))
                chamado.Responsavel = "Não informado";

            if (chamado.AbertoEm == default(DateTime))
                chamado.AbertoEm = DateTime.Now;

            return chamado;
        }
    }
}

