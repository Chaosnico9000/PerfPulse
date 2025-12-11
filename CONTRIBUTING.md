# Contributing to PerfPulse

Thank you for considering contributing to PerfPulse! This document provides guidelines and instructions for contributing.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for everyone.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check existing issues to avoid duplicates. When creating a bug report, include:

- **Clear title and description**
- **Steps to reproduce** the issue
- **Expected behavior** vs actual behavior
- **.NET version** and operating system
- **Sample code** if applicable
- **Stack traces** or error messages

Example:
```
**Bug**: Export to JSON fails with large datasets

**Environment**: 
- PerfPulse version: 1.0.0
- .NET version: 10.0
- OS: Windows 11

**Steps to Reproduce**:
1. Run benchmark with 100,000 iterations
2. Export results to JSON
3. Error occurs: "Out of memory exception"

**Expected**: JSON file should be created
**Actual**: Application crashes with OutOfMemoryException
```

### Suggesting Enhancements

Enhancement suggestions are welcome! Please include:

- **Clear use case**: Why is this enhancement needed?
- **Proposed solution**: How should it work?
- **Alternatives considered**: What other approaches did you consider?
- **Examples**: Mock-ups or code examples if applicable

### Pull Requests

1. **Fork** the repository
2. **Create a branch** from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes** following the coding guidelines below
4. **Test your changes** thoroughly
5. **Commit** with clear messages:
   ```bash
   git commit -m "Add support for instance methods"
   ```
6. **Push** to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```
7. **Open a Pull Request** with a clear description

## Development Setup

### Prerequisites

- .NET 10 SDK or later
- Git
- Code editor (Visual Studio, VS Code, Rider, etc.)

### Building

```bash
git clone https://github.com/yourusername/PerfPulse.git
cd PerfPulse
dotnet restore
dotnet build
```

### Running

```bash
dotnet run -- benchmark path/to/assembly.dll Type.Name MethodName
```

### Testing

```bash
dotnet test
```

## Coding Guidelines

### C# Style

- Follow standard C# naming conventions
- Use `var` when type is obvious from right-hand side
- Use expression-bodied members where appropriate
- Keep methods focused and concise
- Use modern C# features (pattern matching, switch expressions, etc.)

### Code Organization

```csharp
// Good
private static void ProcessResults(BenchmarkResult result)
{
    DisplayResults(result);
    DrawHistogram(result.Timings);
}

// Avoid
private static void ProcessResults(BenchmarkResult result)
{
    // 200 lines of code here
}
```

### Error Handling

- Use descriptive error messages
- Return appropriate exit codes
- Handle edge cases gracefully

```csharp
// Good
if (string.IsNullOrWhiteSpace(assemblyPath))
{
    WriteError("Assembly path is required.");
    return 1;
}

// Avoid
if (assemblyPath == null)
{
    Console.WriteLine("Error");
    return 1;
}
```

### Comments

- Write self-documenting code when possible
- Add comments for complex logic
- Keep comments up-to-date with code changes
- Avoid stating the obvious

```csharp
// Good
// Calculate bucket index ensuring it stays within bounds
var bucketIndex = Math.Min((int)((timing - min) / range * buckets), buckets - 1);

// Avoid
// Add 1 to i
i = i + 1;
```

### Console Output

- Use existing formatting methods (`WriteError`, `WriteSuccess`, `WriteInfo`)
- Maintain consistent color scheme
- Ensure output is readable in different terminal backgrounds

## Project Structure

```
PerfPulse/
??? Program.cs              # Main entry point and benchmark logic
??? PerfPulse.csproj        # Project file
??? README.md               # Documentation
??? LICENSE                 # MIT License
??? CONTRIBUTING.md         # This file
??? .gitignore              # Git ignore rules
```

## Feature Branches

- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `docs/*` - Documentation updates
- `refactor/*` - Code refactoring
- `test/*` - Test additions/modifications

## Commit Messages

Follow conventional commit format:

```
<type>: <description>

[optional body]

[optional footer]
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or modifying tests
- `chore`: Maintenance tasks

Examples:
```
feat: Add support for instance methods

Allow benchmarking of instance methods by creating a default
instance of the target type.

Closes #42
```

```
fix: Correct median calculation for even-sized datasets

Previously used integer division which could skip the actual median
value. Now properly averages the two middle values.
```

## Testing Guidelines

### Manual Testing Checklist

Before submitting a PR, test:

- ? Command-line mode with valid inputs
- ? Interactive mode
- ? Error handling (invalid assembly, missing method, etc.)
- ? CSV export
- ? JSON export
- ? Progress bar display
- ? Help command
- ? Large iteration counts (10,000+)
- ? Methods with varying execution times

### Test Assembly

Use a simple test library:

```csharp
namespace TestLib;

public class TestMethods
{
    public static void Fast() { }
    
    public static void Slow() 
    { 
        Thread.Sleep(1); 
    }
    
    public static void Allocating()
    {
        var list = new List<int>();
        for (int i = 0; i < 1000; i++)
            list.Add(i);
    }
}
```

## Release Process

1. Update version in `PerfPulse.csproj`
2. Update `CHANGELOG.md`
3. Create release tag
4. Build release binaries
5. Create GitHub release
6. Update documentation

## Questions?

- Open an issue for questions
- Start a discussion in GitHub Discussions
- Reach out to maintainers

## Recognition

Contributors will be recognized in:
- README.md
- Release notes
- Project documentation

Thank you for contributing to PerfPulse! ??
