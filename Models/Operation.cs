using System;
using System.Collections.Generic;
using System.Text;

namespace WcetAnalysis.Models
{
    /// Одна операция фрагмента: имя, описательный тип, число обращений к памяти и число ветвлений.
    public record Operation(string Name, string Type, int MemoryAccesses, int Branches);
}
