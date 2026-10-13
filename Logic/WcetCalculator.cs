using WcetAnalysis.Models;

namespace WcetAnalysis.Logic;

public static class WcetCalculator
{
    // Лучший случай: все обращения к памяти — попадания в кэш, все переходы предсказаны верно, штрафы не начисляются.
    public static double BestCase(Operation op, ProcessorModel model) => model.BaseCost;

    // Худший случай: каждое обращение к памяти — промах кэша, каждое ветвление — ошибка предсказания перехода.
    public static double WorstCase(Operation op, ProcessorModel model) =>
        model.BaseCost
        + op.MemoryAccesses * model.CacheMissPenalty
        + op.Branches * model.BranchMispredictionPenalty;

    public static double Bcet(IEnumerable<Operation> fragment, ProcessorModel model) =>
        fragment.Sum(op => BestCase(op, model));

    public static double Wcet(IEnumerable<Operation> fragment, ProcessorModel model) =>
        fragment.Sum(op => WorstCase(op, model));

    public static double NondeterminismRatio(IEnumerable<Operation> fragment, ProcessorModel model) =>
        Wcet(fragment, model) / Bcet(fragment, model);

    // Отдельный вклад источников недетерминизма в WCET.
    public static (double Memory, double Branches) SourceBreakdown(
        IEnumerable<Operation> fragment, ProcessorModel model)
    {
        var list = fragment.ToList();
        double memory = list.Sum(op => op.MemoryAccesses) * model.CacheMissPenalty;
        double branches = list.Sum(op => op.Branches) * model.BranchMispredictionPenalty;
        return (memory, branches);
    }

    public static bool MeetsDeadline(IEnumerable<Operation> fragment, ProcessorModel model) =>
        Wcet(fragment, model) <= model.Deadline;
}
