using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace PerfPulse;

internal class Program
{
    static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length == 0)
        {
            return RunInteractiveMode();
        }

        var command = args[0].ToLowerInvariant();

        return command switch
        {
            "benchmark" => RunBenchmark(args),
            "interactive" => RunInteractiveMode(),
            "help" or "-h" or "--help" => ShowHelpReturn(),
            _ => ShowHelpReturn()
        };
    }

    private static int ShowHelpReturn()
    {
        ShowHelp();
        return 1;
    }

    private static void ShowHelp()
    {
        WriteHeader();
        Console.WriteLine("""
        Usage:
          PerfPulse benchmark <assemblyPath> <typeName> <methodName> [options]
          PerfPulse interactive
          PerfPulse help

        Options:
          -i, --iterations <number>    Number of iterations (default: 1000)
          -w, --warmup <number>        Warmup iterations (default: 50)
          -e, --export <format>        Export results (csv, json)
          -o, --output <path>          Output file path for export

        Example:
          PerfPulse benchmark MyLib.dll MyLib.MathUtils HeavyCalc -i 10000 -w 100
          PerfPulse benchmark MyLib.dll MyLib.Calculator Add -e json -o results.json
          PerfPulse interactive
        """);
    }

    private static void WriteHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""
        ╔═══════════════════════════════════════════════════════════╗
        ║              PerfPulse - Micro Benchmark Tool             ║
        ║           Simple performance testing for .NET methods     ║
        ╚═══════════════════════════════════════════════════════════╝
        """);
        Console.ResetColor();
        Console.WriteLine();
    }

    private static int RunInteractiveMode()
    {
        WriteHeader();

        Console.WriteLine("Interactive Benchmark Mode");
        Console.WriteLine(new string('─', 60));
        Console.WriteLine();

        var assemblyPath = PromptInput("Assembly Path", "");
        if (string.IsNullOrWhiteSpace(assemblyPath))
        {
            WriteError("Assembly path is required.");
            return 1;
        }

        var typeName = PromptInput("Type Name (fully qualified)", "");
        if (string.IsNullOrWhiteSpace(typeName))
        {
            WriteError("Type name is required.");
            return 1;
        }

        var methodName = PromptInput("Method Name", "");
        if (string.IsNullOrWhiteSpace(methodName))
        {
            WriteError("Method name is required.");
            return 1;
        }

        var iterations = PromptInt("Iterations", 1000);
        var warmup = PromptInt("Warmup iterations", 50);

        Console.Write("Export results? (y/n): ");
        var shouldExport = Console.ReadLine()?.ToLowerInvariant() == "y";

        string? exportFormat = null;
        string? outputPath = null;

        if (shouldExport)
        {
            Console.Write("Export format (csv/json): ");
            exportFormat = Console.ReadLine()?.ToLowerInvariant();
            outputPath = PromptInput("Output file path", $"benchmark_results.{exportFormat ?? "json"}");
        }

        Console.WriteLine();

        var config = new BenchmarkConfig
        {
            AssemblyPath = assemblyPath,
            TypeName = typeName,
            MethodName = methodName,
            Iterations = iterations,
            WarmupIterations = warmup,
            ExportFormat = exportFormat,
            OutputPath = outputPath
        };

        return ExecuteBenchmark(config);
    }

    private static string PromptInput(string prompt, string defaultValue)
    {
        Console.Write($"{prompt}");
        if (!string.IsNullOrEmpty(defaultValue))
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" [{defaultValue}]");
            Console.ResetColor();
        }
        Console.Write(": ");

        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
    }

    private static int PromptInt(string prompt, int defaultValue)
    {
        Console.Write($"{prompt}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($" [{defaultValue}]");
        Console.ResetColor();
        Console.Write(": ");

        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out var result) ? defaultValue : result;
    }

    private static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"✗ Error: {message}");
        Console.ResetColor();
    }

    private static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    private static void WriteInfo(string label, string value)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{label,-20}");
        Console.ResetColor();
        Console.WriteLine($": {value}");
    }

    private static int RunBenchmark(string[] args)
    {
        var config = ParseArguments(args);
        if (config == null)
        {
            ShowHelp();
            return 1;
        }

        return ExecuteBenchmark(config);
    }

    private static BenchmarkConfig? ParseArguments(string[] args)
    {
        if (args.Length < 4)
        {
            WriteError("Missing required arguments.");
            return null;
        }

        var config = new BenchmarkConfig
        {
            AssemblyPath = args[1],
            TypeName = args[2],
            MethodName = args[3],
            Iterations = 1000,
            WarmupIterations = 50
        };

        for (int i = 4; i < args.Length; i++)
        {
            var arg = args[i].ToLowerInvariant();

            switch (arg)
            {
                case "-i" or "--iterations":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out var iterations))
                    {
                        config.Iterations = iterations;
                        i++;
                    }
                    break;

                case "-w" or "--warmup":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out var warmup))
                    {
                        config.WarmupIterations = warmup;
                        i++;
                    }
                    break;

                case "-e" or "--export":
                    if (i + 1 < args.Length)
                    {
                        config.ExportFormat = args[i + 1].ToLowerInvariant();
                        i++;
                    }
                    break;

                case "-o" or "--output":
                    if (i + 1 < args.Length)
                    {
                        config.OutputPath = args[i + 1];
                        i++;
                    }
                    break;
            }
        }

        return config;
    }

    private static int ExecuteBenchmark(BenchmarkConfig config)
    {
        WriteHeader();

        WriteInfo("Assembly", config.AssemblyPath);
        WriteInfo("Type", config.TypeName);
        WriteInfo("Method", config.MethodName);
        WriteInfo("Iterations", config.Iterations.ToString());
        WriteInfo("Warmup", config.WarmupIterations.ToString());
        Console.WriteLine();

        Assembly asm;
        try
        {
            asm = Assembly.LoadFrom(config.AssemblyPath);
            WriteSuccess($"Assembly loaded successfully");
        }
        catch (Exception ex)
        {
            WriteError($"Failed to load assembly: {ex.Message}");
            return 1;
        }

        var type = asm.GetType(config.TypeName, throwOnError: false, ignoreCase: false);
        if (type is null)
        {
            WriteError("Type not found in assembly.");
            return 1;
        }

        var method = type.GetMethod(config.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (method is null)
        {
            WriteError("Method not found or not static.");
            return 1;
        }

        if (method.GetParameters().Length != 0)
        {
            WriteError("Method must be parameterless.");
            return 1;
        }

        WriteSuccess($"Method '{config.MethodName}' found and validated");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("⚡ Warming up");
        Console.ResetColor();
        Console.Write($" ({config.WarmupIterations} iterations)...");

        for (var i = 0; i < config.WarmupIterations; i++)
        {
            method.Invoke(null, null);
        }
        Console.WriteLine(" Done!");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);
        var gc0Before = GC.CollectionCount(0);
        var gc1Before = GC.CollectionCount(1);
        var gc2Before = GC.CollectionCount(2);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("🔥 Running benchmark");
        Console.ResetColor();
        Console.Write($" ({config.Iterations} iterations)...");
        Console.WriteLine();

        var timings = new List<double>(config.Iterations);
        var sw = new Stopwatch();

        for (var i = 0; i < config.Iterations; i++)
        {
            sw.Restart();
            method.Invoke(null, null);
            sw.Stop();
            timings.Add(sw.Elapsed.TotalNanoseconds);

            if ((i + 1) % Math.Max(1, config.Iterations / 20) == 0)
            {
                var progress = (double)(i + 1) / config.Iterations;
                DrawProgressBar(progress);
            }
        }

        Console.WriteLine();
        Console.WriteLine();

        var memoryAfter = GC.GetTotalMemory(false);
        var gc0After = GC.CollectionCount(0);
        var gc1After = GC.CollectionCount(1);
        var gc2After = GC.CollectionCount(2);

        var result = new BenchmarkResult
        {
            Timings = timings,
            MemoryBefore = memoryBefore,
            MemoryAfter = memoryAfter,
            GC0Count = gc0After - gc0Before,
            GC1Count = gc1After - gc1Before,
            GC2Count = gc2After - gc2Before,
            Iterations = config.Iterations
        };

        DisplayResults(result);
        DrawHistogram(result.Timings);

        if (!string.IsNullOrEmpty(config.ExportFormat))
        {
            ExportResults(config, result);
        }

        return 0;
    }

    private static void DrawProgressBar(double progress)
    {
        const int barWidth = 40;
        var filled = (int)(progress * barWidth);

        Console.Write("\r[");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', filled));
        Console.ResetColor();
        Console.Write(new string('░', barWidth - filled));
        Console.Write($"] {progress:P0}");
    }

    private static void DisplayResults(BenchmarkResult result)
    {
        var sorted = result.Timings.OrderBy(x => x).ToList();
        var mean = result.Timings.Average();
        var min = sorted.First();
        var max = sorted.Last();
        var median = sorted[sorted.Count / 2];
        var p95 = sorted[(int)(sorted.Count * 0.95)];
        var p99 = sorted[(int)(sorted.Count * 0.99)];
        var stdDev = Math.Sqrt(result.Timings.Average(x => Math.Pow(x - mean, 2)));

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    BENCHMARK RESULTS                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("⏱️  Timing Statistics:");
        Console.WriteLine(new string('─', 60));
        WriteMetric("Mean", FormatTime(mean), ConsoleColor.Yellow);
        WriteMetric("Median", FormatTime(median));
        WriteMetric("Min", FormatTime(min), ConsoleColor.Green);
        WriteMetric("Max", FormatTime(max), ConsoleColor.Red);
        WriteMetric("Std Dev", FormatTime(stdDev));
        WriteMetric("95th %ile", FormatTime(p95));
        WriteMetric("99th %ile", FormatTime(p99));
        Console.WriteLine();

        Console.WriteLine("💾 Memory Statistics:");
        Console.WriteLine(new string('─', 60));
        WriteMetric("Memory Before", FormatBytes(result.MemoryBefore));
        WriteMetric("Memory After", FormatBytes(result.MemoryAfter));
        var memDelta = result.MemoryAfter - result.MemoryBefore;
        WriteMetric("Memory Delta", FormatBytes(memDelta), memDelta > 0 ? ConsoleColor.Red : ConsoleColor.Green);
        Console.WriteLine();

        Console.WriteLine("🗑️  Garbage Collection:");
        Console.WriteLine(new string('─', 60));
        WriteMetric("Gen 0", result.GC0Count.ToString(), result.GC0Count > 0 ? ConsoleColor.Yellow : ConsoleColor.Green);
        WriteMetric("Gen 1", result.GC1Count.ToString(), result.GC1Count > 0 ? ConsoleColor.Yellow : ConsoleColor.Green);
        WriteMetric("Gen 2", result.GC2Count.ToString(), result.GC2Count > 0 ? ConsoleColor.Red : ConsoleColor.Green);
        Console.WriteLine();
    }

    private static void WriteMetric(string label, string value, ConsoleColor? color = null)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"{label,-15}");
        Console.ResetColor();
        Console.Write(": ");

        if (color.HasValue)
            Console.ForegroundColor = color.Value;

        Console.WriteLine(value);
        Console.ResetColor();
    }

    private static string FormatTime(double nanoseconds)
    {
        if (nanoseconds < 1000)
            return $"{nanoseconds:F2} ns";
        if (nanoseconds < 1_000_000)
            return $"{nanoseconds / 1000:F2} μs";
        if (nanoseconds < 1_000_000_000)
            return $"{nanoseconds / 1_000_000:F2} ms";
        return $"{nanoseconds / 1_000_000_000:F2} s";
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:F2} {sizes[order]}";
    }

    private static void DrawHistogram(List<double> timings)
    {
        Console.WriteLine("📊 Distribution Histogram:");
        Console.WriteLine(new string('─', 60));

        const int buckets = 20;
        const int maxBarWidth = 40;

        var sorted = timings.OrderBy(x => x).ToList();
        var min = sorted.First();
        var max = sorted.Last();
        var range = max - min;

        if (range == 0)
        {
            Console.WriteLine("  All values are identical");
            Console.WriteLine();
            return;
        }

        var histogram = new int[buckets];
        foreach (var timing in timings)
        {
            var bucketIndex = (int)((timing - min) / range * (buckets - 1));
            histogram[bucketIndex]++;
        }

        var maxCount = histogram.Max();

        for (int i = 0; i < buckets; i++)
        {
            var bucketMin = min + (range * i / buckets);
            var bucketMax = min + (range * (i + 1) / buckets);
            var count = histogram[i];
            var barWidth = maxCount > 0 ? (int)((double)count / maxCount * maxBarWidth) : 0;

            Console.Write($"  {FormatTime(bucketMin),10} - {FormatTime(bucketMax),10} │");

            if (count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(new string('█', barWidth));
                Console.ResetColor();
                Console.WriteLine($" {count}");
            }
            else
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine();
    }

    private static void ExportResults(BenchmarkConfig config, BenchmarkResult result)
    {
        var outputPath = config.OutputPath ?? $"benchmark_results.{config.ExportFormat}";

        try
        {
            if (config.ExportFormat == "csv")
            {
                ExportCsv(outputPath, config, result);
            }
            else if (config.ExportFormat == "json")
            {
                ExportJson(outputPath, config, result);
            }

            WriteSuccess($"Results exported to: {outputPath}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            WriteError($"Failed to export results: {ex.Message}");
        }
    }

    private static void ExportCsv(string path, BenchmarkConfig config, BenchmarkResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Assembly,Type,Method,Iteration,TimeNs");

        for (int i = 0; i < result.Timings.Count; i++)
        {
            sb.AppendLine($"{config.AssemblyPath},{config.TypeName},{config.MethodName},{i + 1},{result.Timings[i]:F2}");
        }

        File.WriteAllText(path, sb.ToString());
    }

    private static void ExportJson(string path, BenchmarkConfig config, BenchmarkResult result)
    {
        var sorted = result.Timings.OrderBy(x => x).ToList();
        var mean = result.Timings.Average();
        var stdDev = Math.Sqrt(result.Timings.Average(x => Math.Pow(x - mean, 2)));

        var data = new
        {
            Benchmark = new
            {
                config.AssemblyPath,
                config.TypeName,
                config.MethodName,
                config.Iterations,
                config.WarmupIterations,
                Timestamp = DateTime.UtcNow
            },
            Statistics = new
            {
                MeanNs = mean,
                MedianNs = sorted[sorted.Count / 2],
                MinNs = sorted.First(),
                MaxNs = sorted.Last(),
                StdDevNs = stdDev,
                P95Ns = sorted[(int)(sorted.Count * 0.95)],
                P99Ns = sorted[(int)(sorted.Count * 0.99)]
            },
            Memory = new
            {
                BeforeBytes = result.MemoryBefore,
                AfterBytes = result.MemoryAfter,
                DeltaBytes = result.MemoryAfter - result.MemoryBefore
            },
            GarbageCollection = new
            {
                Gen0 = result.GC0Count,
                Gen1 = result.GC1Count,
                Gen2 = result.GC2Count
            },
            RawTimingsNs = result.Timings
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(path, json);
    }
}

internal class BenchmarkConfig
{
    public required string AssemblyPath
    {
        get; set;
    }
    public required string TypeName
    {
        get; set;
    }
    public required string MethodName
    {
        get; set;
    }
    public int Iterations
    {
        get; set;
    }
    public int WarmupIterations
    {
        get; set;
    }
    public string? ExportFormat
    {
        get; set;
    }
    public string? OutputPath
    {
        get; set;
    }
}

internal class BenchmarkResult
{
    public required List<double> Timings
    {
        get; set;
    }
    public long MemoryBefore
    {
        get; set;
    }
    public long MemoryAfter
    {
        get; set;
    }
    public int GC0Count
    {
        get; set;
    }
    public int GC1Count
    {
        get; set;
    }
    public int GC2Count
    {
        get; set;
    }
    public int Iterations
    {
        get; set;
    }
}