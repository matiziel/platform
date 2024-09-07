using System.Globalization;
using CodeModel;
using ConsoleMetricCalculator;

var path = "/home/mateusz/Documents/Projects/C#/my-interpreter/MyInterpreter/MyInterpreter";

var factory = new CodeModelFactory();

var project = factory.CreateProject(path);

if (project.Classes.Count == 0) {
    return;
}

var headlineNames = project.Classes.First().Metrics.Keys
    .Select(x => x.ToString())
    .Prepend<string>("Code Snippet ID")
    .ToList();

var lines = new List<string> { string.Join(",", headlineNames) };

foreach (var classDeclaration in project.Classes) {
    var metrics = classDeclaration.Metrics.Values
        .Select(x => x.ToString(CultureInfo.InvariantCulture))
        .Prepend<string>(classDeclaration.FullName)
        .ToList();
    
    lines.Add(string.Join(",", metrics));
}

CsvFileHelper.SaveToFile("/home/mateusz/Documents/School/MGR/results/test_file.csv", lines);




