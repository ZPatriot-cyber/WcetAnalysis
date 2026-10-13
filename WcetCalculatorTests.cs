using WcetAnalysis.Logic;
using WcetAnalysis.Models;
using Xunit;

namespace WcetAnalysis.Tests;

public class WcetCalculatorTests
{
    private static readonly ProcessorModel Model =
        new(BaseCost: 1, CacheMissPenalty: 15, BranchMispredictionPenalty: 8, Deadline: 100);

    [Fact]
    public void BestCase_IgnoresMemoryAndBranches_ReturnsBaseCost()
    {
        var op = new Operation("Тест", "вычисление", MemoryAccesses: 5, Branches: 3);

        double result = WcetCalculator.BestCase(op, Model);

        Assert.Equal(1, result);
    }

    [Fact]
    public void WorstCase_AddsCacheMissAndBranchPenalties()
    {
        var op = new Operation("Тест", "вычисление", MemoryAccesses: 1, Branches: 1);

        double result = WcetCalculator.WorstCase(op, Model);

        // 1 (база) + 1*15 (промах кэша) + 1*8 (ошибка предсказания) = 24
        Assert.Equal(24, result);
    }

    [Fact]
    public void WorstCase_WithoutMemoryOrBranches_EqualsBaseCost()
    {
        var op = new Operation("Тест", "вычисление", MemoryAccesses: 0, Branches: 0);

        Assert.Equal(1, WcetCalculator.WorstCase(op, Model));
    }

    [Fact]
    public void Bcet_SumsBestCaseOverFragment()
    {
        Assert.Equal(8, WcetCalculator.Bcet(MakeSampleFragment(), Model));
    }

    [Fact]
    public void Wcet_SumsWorstCaseOverFragment()
    {
        Assert.Equal(99, WcetCalculator.Wcet(MakeSampleFragment(), Model));
    }

    [Fact]
    public void NondeterminismRatio_IsWcetDividedByBcet()
    {
        double ratio = WcetCalculator.NondeterminismRatio(MakeSampleFragment(), Model);

        Assert.Equal(12.375, ratio, precision: 3);
    }

    [Fact]
    public void SourceBreakdown_SplitsMemoryAndBranchContributions()
    {
        var (memory, branches) = WcetCalculator.SourceBreakdown(MakeSampleFragment(), Model);

        Assert.Equal(75, memory);
        Assert.Equal(16, branches);
    }

    [Fact]
    public void MeetsDeadline_TrueWhenWcetWithinDeadline()
    {
        Assert.True(WcetCalculator.MeetsDeadline(MakeSampleFragment(), Model)); // WCET = 99 ≤ 100
    }

    [Fact]
    public void MeetsDeadline_FalseWhenDeadlineIsLowerThanWcet()
    {
        var strictModel = Model with { Deadline = 50 };

        Assert.False(WcetCalculator.MeetsDeadline(MakeSampleFragment(), strictModel));
    }

    private static List<Operation> MakeSampleFragment() => new()
    {
        new Operation("Чтение уставки", "чтение", 1, 0),
        new Operation("Чтение показания датчика", "чтение", 1, 0),
        new Operation("Вычисление ошибки регулирования", "вычисление", 0, 0),
        new Operation("Вычисление пропорциональной составляющей", "вычисление", 0, 0),
        new Operation("Вычисление интегральной составляющей", "вычисление", 1, 1),
        new Operation("Вычисление дифференциальной составляющей", "вычисление", 1, 0),
        new Operation("Ограничение выходного сигнала", "вычисление", 0, 1),
        new Operation("Запись управляющего воздействия", "запись", 1, 0),
    };
}