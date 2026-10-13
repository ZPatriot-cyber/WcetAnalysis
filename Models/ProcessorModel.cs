using System;
using System.Collections.Generic;
using System.Text;

namespace WcetAnalysis.Models
{
    /// Модель процессора: базовая стоимость операции, штрафы промаха кэша и ошибки предсказания перехода, и дедлайн фрагмента (в тактах).
    public record ProcessorModel(
        double BaseCost,
        double CacheMissPenalty,
        double BranchMispredictionPenalty,
        double Deadline);
}
