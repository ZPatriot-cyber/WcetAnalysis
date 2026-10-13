using System.Globalization;
using System.Text.Json;
using WcetAnalysis.Models;

namespace WcetAnalysis.Io;

public static class DataLoader
{
    public static List<Operation> LoadFragment(string path)
    {
        var result = new List<Operation>();
        var lines = File.ReadAllLines(path)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .Skip(1); // пропускаем заголовок

        foreach (var line in lines)
        {
            var parts = line.Split(';');
            result.Add(new Operation(
                Name: parts[0].Trim(),
                Type: parts[1].Trim(),
                MemoryAccesses: int.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                Branches: int.Parse(parts[3].Trim(), CultureInfo.InvariantCulture)));
        }
        return result;
    }

    public static ProcessorModel LoadProcessorModel(string path)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        var root = doc.RootElement;
        return new ProcessorModel(
            BaseCost: root.GetProperty("baseCost").GetDouble(),
            CacheMissPenalty: root.GetProperty("cacheMissPenalty").GetDouble(),
            BranchMispredictionPenalty: root.GetProperty("branchMispredictionPenalty").GetDouble(),
            Deadline: root.GetProperty("deadline").GetDouble());
    }
}