using WcetAnalysis.Io;
using Xunit;

namespace WcetAnalysis.Tests;

public class DataLoaderTests
{
    [Fact]
    public void LoadFragment_ParsesCsvRowsIntoOperations()
    {
        string path = Path.GetTempFileName();
        File.WriteAllLines(path, new[]
        {
            "Название;Тип;Обращения_к_памяти;Ветвления",
            "Чтение;чтение;1;0",
            "Ветвление;вычисление;0;1",
        });

        var fragment = DataLoader.LoadFragment(path);

        Assert.Equal(2, fragment.Count);
        Assert.Equal("Чтение", fragment[0].Name);
        Assert.Equal(1, fragment[0].MemoryAccesses);
        Assert.Equal(1, fragment[1].Branches);

        File.Delete(path);
    }

    [Fact]
    public void LoadProcessorModel_ParsesJsonFields()
    {
        string path = Path.GetTempFileName();
        File.WriteAllText(path, """
        {
          "baseCost": 1,
          "cacheMissPenalty": 15,
          "branchMispredictionPenalty": 8,
          "deadline": 100
        }
        """);

        var model = DataLoader.LoadProcessorModel(path);

        Assert.Equal(1, model.BaseCost);
        Assert.Equal(15, model.CacheMissPenalty);
        Assert.Equal(8, model.BranchMispredictionPenalty);
        Assert.Equal(100, model.Deadline);

        File.Delete(path);
    }
}