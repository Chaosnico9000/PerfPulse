# PerfPulse ⚡

A lightweight micro-benchmarking tool for .NET methods with detailed performance metrics and beautiful console output.

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- 📊 **Comprehensive Statistics**: Mean, Median, Min, Max, Standard Deviation, 95th/99th Percentiles
- 💾 **Memory Analysis**: Track memory usage and allocation deltas
- 🗑️ **GC Monitoring**: Track Garbage Collection counts across all generations
- 📈 **Distribution Histogram**: Visual representation of timing distribution
- 💾 **Export Options**: Export results to CSV or JSON formats
- 🎨 **Rich Console UI**: Color-coded output with progress bars and formatted metrics
- 🔧 **Two Modes**: Command-line and interactive modes

## Requirements

- .NET 10 or later
- Target methods must be **static** and **parameterless**

## Installation

### From Source

Clone the repository and build:

```bash
git clone https://github.com/yourusername/PerfPulse.git
cd PerfPulse
dotnet build -c Release
```

The executable will be available in `bin/Release/net10.0/`

### Global Tool (Optional)

You can install PerfPulse as a global .NET tool:

```bash
dotnet pack -c Release
dotnet tool install --global --add-source ./bin/Release PerfPulse
```

## Usage

### Command-Line Mode

```bash
PerfPulse benchmark <assemblyPath> <typeName> <methodName> [options]
```

#### Options

| Option | Description | Default |
|--------|-------------|---------|
| `-i`, `--iterations <number>` | Number of benchmark iterations | 1000 |
| `-w`, `--warmup <number>` | Number of warmup iterations | 50 |
| `-e`, `--export <format>` | Export format (`csv` or `json`) | - |
| `-o`, `--output <path>` | Output file path for export | `benchmark_results.{format}` |

#### Examples

Basic benchmark:
```bash
PerfPulse benchmark MyLib.dll MyLib.Calculator Add
```

With custom iterations and warmup:
```bash
PerfPulse benchmark MyLib.dll MyLib.MathUtils HeavyCalc -i 10000 -w 100
```

With JSON export:
```bash
PerfPulse benchmark MyLib.dll MyLib.Calculator Add -e json -o results.json
```

With CSV export:
```bash
PerfPulse benchmark MyLib.dll MyLib.DataProcessor Transform -e csv -o timings.csv
```

### Interactive Mode

Launch the interactive guided setup:

```bash
PerfPulse interactive
```

You'll be prompted for:
- Assembly path
- Type name (fully qualified)
- Method name
- Number of iterations
- Warmup iterations
- Export preferences

### Help

```bash
PerfPulse help
```

## Output

### Console Output

PerfPulse provides rich console output including:

```
╔═══════════════════════════════════════════════════════════╗
║              PerfPulse - Micro Benchmark Tool             ║
║           Simple performance testing for .NET methods     ║
╚═══════════════════════════════════════════════════════════╝

Assembly            : MyLib.dll
Type                : MyLib.Calculator
Method              : Add
Iterations          : 1000
Warmup              : 50

✓ Assembly loaded successfully
✓ Method 'Add' found and validated

⚡ Warming up (50 iterations)... Done!

🔥 Running benchmark (1000 iterations)...
[████████████████████████████████████████] 100%

╔═══════════════════════════════════════════════════════════╗
║                    BENCHMARK RESULTS                      ║
╚═══════════════════════════════════════════════════════════╝

⏱️  Timing Statistics:
────────────────────────────────────────────────────────────
  Mean           : 1.23 μs
  Median         : 1.20 μs
  Min            : 1.05 μs
  Max            : 2.45 μs
  Std Dev        : 0.15 μs
  95th %ile      : 1.50 μs
  99th %ile      : 1.85 μs

💾 Memory Statistics:
────────────────────────────────────────────────────────────
  Memory Before  : 2.45 MB
  Memory After   : 2.47 MB
  Memory Delta   : 20.00 KB

🗑️  Garbage Collection:
────────────────────────────────────────────────────────────
  Gen 0          : 0
  Gen 1          : 0
  Gen 2          : 0

📊 Distribution Histogram:
────────────────────────────────────────────────────────────
  1.05 μs - 1.12 μs │████████████ 145
  1.12 μs - 1.19 μs │████████████████████ 234
  ...
```

### Export Formats

#### JSON Export

Structured output including:
- Benchmark configuration
- Statistical summary
- Memory metrics
- GC metrics
- Raw timing data (all iterations)
- Timestamp

Example structure:
```json
{
  "Benchmark": {
    "AssemblyPath": "MyLib.dll",
    "TypeName": "MyLib.Calculator",
    "MethodName": "Add",
    "Iterations": 1000,
    "WarmupIterations": 50,
    "Timestamp": "2025-01-15T10:30:00.0000000Z"
  },
  "Statistics": {
    "MeanNs": 1234.56,
    "MedianNs": 1200.0,
    "MinNs": 1050.0,
    "MaxNs": 2450.0,
    "StdDevNs": 150.0,
    "P95Ns": 1500.0,
    "P99Ns": 1850.0
  },
  "Memory": {
    "BeforeBytes": 2568192,
    "AfterBytes": 2588672,
    "DeltaBytes": 20480
  },
  "GarbageCollection": {
    "Gen0": 0,
    "Gen1": 0,
    "Gen2": 0
  },
  "RawTimingsNs": [1250.45, 1198.32, ...]
}
```

#### CSV Export

Raw timing data for each iteration:
```csv
Assembly,Type,Method,Iteration,TimeNs
MyLib.dll,MyLib.Calculator,Add,1,1250.45
MyLib.dll,MyLib.Calculator,Add,2,1198.32
MyLib.dll,MyLib.Calculator,Add,3,1205.67
...
```

## Benchmark Workflow

1. **Load Assembly**: Dynamically loads the specified assembly
2. **Validate Method**: Ensures method exists, is static, and parameterless
3. **Warmup Phase**: Executes method N times to warm up JIT compiler
4. **GC Cleanup**: Forces full garbage collection before measurement
5. **Benchmark Phase**: Measures each iteration individually with progress indicator
6. **Analysis**: Calculates statistics and displays results
7. **Export** (optional): Saves results to specified format

## Example Test Assembly

Create a simple test library to benchmark:

```csharp
// TestLib.cs
namespace TestLib;

public class MathOperations
{
    public static void QuickOperation()
    {
        var sum = 0;
        for (int i = 0; i < 100; i++)
        {
            sum += i;
        }
    }

    public static void HeavyOperation()
    {
        var sum = 0;
        for (int i = 0; i < 1_000_000; i++)
        {
            sum += i * i;
        }
    }
}
```

Compile and benchmark:
```bash
dotnet build TestLib.csproj
PerfPulse benchmark TestLib.dll TestLib.MathOperations QuickOperation -i 5000
```

## Limitations

- Only supports **static methods**
- Methods must be **parameterless**
- No support for methods requiring arguments or instance context
- Assembly must be loadable at runtime
- Reflection overhead is included in measurements (typically ~50-100ns)

## Use Cases

✅ Quick performance checks without heavy framework setup  
✅ Comparing algorithm implementations  
✅ Regression testing for performance-critical methods  
✅ Educational purposes for understanding performance characteristics  
✅ CI/CD integration via JSON export  
✅ Identifying performance bottlenecks  

## Comparison to BenchmarkDotNet

PerfPulse is intentionally simpler than BenchmarkDotNet:

| Feature | PerfPulse | BenchmarkDotNet |
|---------|-----------|-----------------|
| Setup Complexity | Minimal | Requires attributes/classes |
| Method Support | Static, parameterless only | Full flexibility |
| Output | Console + CSV/JSON | Multiple formats |
| Analysis Depth | Basic statistics | Advanced (outlier detection, etc.) |
| Runtime Overhead | Reflection-based | Optimized code generation |
| Use Case | Quick checks | Comprehensive benchmarking |
| Learning Curve | Minutes | Hours |

**When to use PerfPulse**: Quick performance checks, learning, CI integration  
**When to use BenchmarkDotNet**: Production benchmarking, detailed analysis, parameter variations

## Roadmap

- [ ] Support for methods with parameters
- [ ] Support for instance methods
- [ ] XML export format
- [ ] Comparison mode (compare two methods)
- [ ] Multiple method benchmarking in one run
- [ ] HTML report generation
- [ ] Docker support

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

Please ensure:
- Code follows existing style conventions
- All tests pass
- New features include appropriate tests
- Documentation is updated

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)
- Built with .NET 10
- Console UI inspired by various CLI tools

## Support

- 🐛 [Report Issues](https://github.com/yourusername/PerfPulse/issues)
- 💡 [Request Features](https://github.com/yourusername/PerfPulse/issues)
- 📖 [Documentation](https://github.com/yourusername/PerfPulse/wiki)

## Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- Email: your.email@example.com

---

Made with ❤️ for the .NET community
