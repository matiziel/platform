namespace ConsoleMetricCalculator;

public class CsvFileHelper {
    public static void SaveToFile(string path, IEnumerable<string> lines) {
        if (!lines.Any()) {
            return;
        }

        using var writer = new StreamWriter(path);

        foreach (var line in lines) {
            writer.WriteLine(line);
        }
    }
}