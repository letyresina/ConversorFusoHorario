using System;
using System.Collections.Generic;
using ConversorFusoHorario;

namespace ConversorFusoHorario
{
    // Implementação de IConversorHora
    public class ConversorHora : IConversorHora
    {
        public DateTime ConverterParaFusoHorario(DateTime dataHora, string idFusoDestino)
        {
            var fusoDestino = TimeZoneInfo.FindSystemTimeZoneById(idFusoDestino);
            return TimeZoneInfo.ConvertTime(dataHora, fusoDestino);
        }

        public string ObterFusoHorarioDaData(string dataHoraStr)
        {
            if (DateTime.TryParse(dataHoraStr, out _))
                return TimeZoneInfo.Local.Id;
            throw new FormatException("Data/hora inválida.");
        }
    }

    // Implementação de IAgendaEntrada
    public class AgendaEntrada : IAgendaEntrada
    {
        public DateTime DataHora { get; set; }
        public string Titulo { get; set; }
        private readonly IConversorHora conversor;

        public AgendaEntrada(DateTime dataHora, string titulo, IConversorHora conversorHora)
        {
            DataHora = dataHora;
            Titulo = titulo;
            conversor = conversorHora;
        }

        private DateTime ConverterSeNecessario(string? idFusoDestino)
        {
            if (!string.IsNullOrEmpty(idFusoDestino))
                return conversor.ConverterParaFusoHorario(DataHora, idFusoDestino);
            return DataHora;
        }

        public void Imprimir(string? idFusoDestino = null)
        {
            var data = ConverterSeNecessario(idFusoDestino);
            Console.WriteLine($"{data:dd/MM/yyyy HH:mm} - {Titulo}");
        }

        public void ImprimirHora(string? idFusoDestino = null)
        {
            var data = ConverterSeNecessario(idFusoDestino);
            Console.WriteLine(data.ToString("HH:mm"));
        }

        public void ImprimirDia(string? idFusoDestino = null)
        {
            var data = ConverterSeNecessario(idFusoDestino);
            Console.WriteLine(data.ToString("dd/MM/yyyy"));
        }

        public void ImprimirDiaSemana(string? idFusoDestino = null)
        {
            var data = ConverterSeNecessario(idFusoDestino);
            Console.WriteLine(data.ToString("dddd"));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var conversor = new ConversorHora();
            var agenda = new List<IAgendaEntrada>();

            while (true)
            {
                Console.WriteLine("\n1 - Adicionar compromisso");
                Console.WriteLine("2 - Listar compromissos por dia");
                Console.WriteLine("3 - Listar compromissos por data/hora");
                Console.WriteLine("4 - Sair");
                Console.Write("Opção: ");
                var opcao = Console.ReadLine();

                if (opcao == "1")
                {
                    try
                    {
                        Console.Write("Data/hora (yyyy-MM-dd HH:mm): ");
                        DateTime dataHora = DateTime.Parse(Console.ReadLine());

                        Console.Write("Fuso horário de origem (ex: E. South America Standard Time): ");
                        var fusoOrigem = Console.ReadLine();
                        var tzOrigem = TimeZoneInfo.FindSystemTimeZoneById(fusoOrigem);
                        var dataHoraComFuso = TimeZoneInfo.ConvertTime(dataHora, tzOrigem);

                        Console.Write("Título: ");
                        string titulo = Console.ReadLine();

                        agenda.Add(new AgendaEntrada(dataHoraComFuso, titulo, conversor));
                        Console.WriteLine("Compromisso adicionado!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro: {ex.Message}");
                    }
                }
                else if (opcao == "2")
                {
                    Console.Write("Informe a data (yyyy-MM-dd): ");
                    var data = DateTime.Parse(Console.ReadLine());

                    Console.Write("Fuso horário destino: ");
                    var fusoDestino = Console.ReadLine();

                    foreach (var entrada in agenda)
                    {
                        var dataConvertida = conversor.ConverterParaFusoHorario(entrada.DataHora, fusoDestino);
                        if (dataConvertida.Date == data.Date)
                        {
                            entrada.Imprimir(fusoDestino);
                        }
                    }
                }
                else if (opcao == "3")
                {
                    Console.Write("Informe a data/hora exata (yyyy-MM-dd HH:mm): ");
                    var dataHora = DateTime.Parse(Console.ReadLine());

                    Console.Write("Fuso horário destino: ");
                    var fusoDestino = Console.ReadLine();

                    foreach (var entrada in agenda)
                    {
                        var dataConvertida = conversor.ConverterParaFusoHorario(entrada.DataHora, fusoDestino);
                        if (dataConvertida == dataHora)
                        {
                            entrada.Imprimir(fusoDestino);
                        }
                    }
                }
                else if (opcao == "4")
                {
                    break;
                }
            }
        }
    }
}