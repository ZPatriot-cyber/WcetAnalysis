using WcetAnalysis.Io;
using WcetAnalysis.Models;
using WcetAnalysis.Reporting;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string dataDir = Path.Combine(AppContext.BaseDirectory, "data");

List<Operation> fragment = DataLoader.LoadFragment(Path.Combine(dataDir, "fragment.csv"));
ProcessorModel model = DataLoader.LoadProcessorModel(Path.Combine(dataDir, "processor.json"));

ConsoleReport.PrintTable(fragment, model);
ConsoleReport.PrintConclusion(fragment, model);

Console.WriteLine();
Console.WriteLine("Нажмите Enter для выхода...");
Console.ReadLine();