# Security Policy

## Supported Versions

The following versions of PerfPulse are currently supported with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

We take the security of PerfPulse seriously. If you discover a security vulnerability, please follow these steps:

### How to Report

1. **Do NOT** open a public issue
2. Email security concerns to: your.email@example.com
3. Include the following information:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Suggested fix (if any)

### What to Expect

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Fix Timeline**: Depends on severity
  - **Critical**: 1-3 days
  - **High**: 1-2 weeks
  - **Medium**: 2-4 weeks
  - **Low**: Next release cycle

### Security Considerations

#### Assembly Loading

PerfPulse loads assemblies dynamically using `Assembly.LoadFrom()`. Users should:

- Only benchmark trusted assemblies
- Be aware of code execution risks
- Run in isolated environments when testing untrusted code

#### Reflection Usage

The tool uses reflection to invoke methods. Consider:

- Methods are executed in the same process
- No sandboxing is provided
- Malicious code can affect the host system

#### File System Access

PerfPulse writes export files. Users should:

- Validate output paths
- Ensure proper file permissions
- Be cautious with path traversal

### Best Practices for Users

1. **Validate Assemblies**: Only benchmark assemblies from trusted sources
2. **Use Containers**: Run in Docker or VMs when testing unknown code
3. **Limit Permissions**: Run with minimal required permissions
4. **Review Exports**: Check exported files for sensitive data
5. **Update Regularly**: Keep PerfPulse updated to the latest version

### Known Limitations

- No sandboxing of benchmarked code
- Reflection-based execution overhead
- No protection against infinite loops or resource exhaustion
- No input sanitization for assembly paths

### Disclosure Policy

- Security issues will be disclosed after a fix is available
- Credit will be given to reporters (unless anonymity is requested)
- A security advisory will be published on GitHub

## Security Updates

Security updates will be released as:
- Patch versions for minor issues
- Minor versions for moderate issues  
- Major versions only if breaking changes are required

Check [CHANGELOG.md](CHANGELOG.md) for security-related updates.

## Contact

For security-related questions:
- Email: your.email@example.com
- GitHub Security Advisory: (preferred for sensitive issues)

Thank you for helping keep PerfPulse secure!
