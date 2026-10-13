using System.Globalization;
using WcetAnalysis.Logic;
using WcetAnalysis.Models;

namespace WcetAnalysis.Reporting;

public static class ConsoleReport
{
    public static void PrintTable(List<Operation> fragment, ProcessorModel model)
    {
        string[] headers = { "Операция", "Тип", "Лучшее", "Худшее" };

        var rows = fragment.Select(op => new[]
        {
            op.Name,
            op.Type,
            Fmt(WcetCalculator.BestCase(op, model)),
            Fmt(WcetCalculator.WorstCase(op, model))
        }).ToList();

        var widths = headers
            .Select((h, i) => Math.Max(h.Length, rows.Max(r => r[i].Length)))
            .ToArray();

        string FormatRow(IEnumerable<string> cells) =>
            string.Join(" | ", cells.Select((c, i) => c.PadRight(widths[i])));

        Console.WriteLine("=== Операции фрагмента (время в тактах) ===");
        Console.WriteLine(FormatRow(headers));
        Console.WriteLine(string.Join("-+-", widths.Select(w => new string('-', w))));
        foreach (var row in rows)
            Console.WriteLine(FormatRow(row));
        Console.WriteLine();
    }

    public static void PrintConclusion(List<Operation> fragment, ProcessorModel model)
    {
        double bcet = WcetCalculator.Bcet(fragment, model);
        double wcet = WcetCalculator.Wcet(fragment, model);
        double ratio = WcetCalculator.NondeterminismRatio(fragment, model);
        var (memory, branches) = WcetCalculator.SourceBreakdown(fragment, model);
        bool meets = WcetCalculator.MeetsDeadline(fragment, model);

        Console.WriteLine("=== Заключение ===");
        Console.WriteLine($"BCET (лучшее время) = {Fmt(bcet)} такт.");
        Console.WriteLine($"WCET (худшее время) = {Fmt(wcet)} такт.");
        Console.WriteLine($"Коэффициент недетерминизма = {ratio.ToString("0.###", CultureInfo.InvariantCulture)}");
        Console.WriteLine($"Вклад памяти в WCET = {Fmt(memory)} такт.");
        Console.WriteLine($"Вклад ветвлений в WCET = {Fmt(branches)} такт.");
        Console.WriteLine($"Дедлайн = {Fmt(model.Deadline)} такт.");
        Console.WriteLine(meets
            ? "Вывод: фрагмент укладывается в дедлайн даже в худшем случае."
            : "Вывод: в худшем случае фрагмент НЕ укладывается в дедлайн.");
    }

    private static string Fmt(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);
}