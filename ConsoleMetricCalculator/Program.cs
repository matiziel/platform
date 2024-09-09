using System.Globalization;
using CodeModel;
using CodeModel.CaDETModel.CodeItems;
using ConsoleMetricCalculator;



var path = "/home/mateusz/Documents/School/MGR/project-to-analize/osu-2cac373365309a40474943f55c56159ed8f9433c/osu.Game";

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

var classes = project.Classes
    .Where(HasOnlyPropertyAndField)
    .Where(t => t.Modifiers.All(x => x.Value != CaDETModifierValue.Static))
    .Where(t => !t.IsInnerClass && t.InnerClasses.Count == 0)
    .ToList();

foreach (var classDeclaration in classes) {
    var metrics = classDeclaration.Metrics.Values
        .Select(x => x.ToString(CultureInfo.InvariantCulture))
        .Prepend<string>(classDeclaration.FullName)
        .ToList();
    
    lines.Add(string.Join(",", metrics) + ",1");
}

static bool HasOnlyPropertyAndField(CaDETClass classDeclaration) {
    if (classDeclaration.Members.Count == 0 &&
        (classDeclaration.Parent is null || classDeclaration.Parent.Members.Count == 0)) {
        return classDeclaration.Fields.Count > 0;
    }

    return classDeclaration.Members.All(m => m.Type.Equals(CaDETMemberType.Property)) && 
           (classDeclaration.Parent is null || classDeclaration.Parent.Members.All(m => m.Type.Equals(CaDETMemberType.Property)));
}

CsvFileHelper.SaveToFile("/home/mateusz/Documents/School/MGR/results/test_file.csv", lines);




