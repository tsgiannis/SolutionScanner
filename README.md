C# Solution Scanner & Analyzer
🚀 Overview  
C# Solution Scanner is a powerful Windows Forms application designed to help developers analyze and document their C# solutions. It automatically scans through entire solution structures, extracts detailed information about classes, methods, properties, and more, presenting everything in an organized, exportable format.

✨ Features
🔍 Intelligent Solution Analysis
Recursive Scanning: Automatically discovers all .cs files within solution folders

Smart Project Detection: Identifies .csproj files and organizes code by project

Solution File Recognition: Detects .sln files for proper solution hierarchy

📊 Comprehensive Code Extraction
Class Information: Extracts all classes with access modifiers

Member Details: Captures properties, fields, methods, and constructors

Type Discovery: Identifies enums, structs, and interfaces

Parameter Parsing: Extracts method/constructor parameters with types

🌳 Multiple View Modes
Tree View: Hierarchical display showing solution → projects → files → classes → members

Flat List: One-line-per-item format for quick scanning

JSON Export: Complete structured JSON data for programmatic use

🎨 Enhanced User Experience
Syntax Highlighting: Color-coded C# and JSON syntax for better readability

Modern UI: Clean, responsive interface with Material Design elements

Interactive Tree: Click nodes to view file contents or class details

Progress Indicators: Visual feedback during scanning operations

📤 Export Capabilities
Multiple Formats: Export as text (tree or flat) or JSON

Clipboard Support: Copy any view to clipboard with one click

File Export: Save scans to disk for documentation or analysis

One-Click Email: Contact information with direct email composition

🛠️ How It Works
1. Solution Selection
Browse to your solution folder

Tool automatically detects .sln files and project structure

Uses folder name as solution name if no solution file found

2. Deep Analysis
text
Solution: MySolution → C:\Projects\MySolution
├── Projects
│   ├── Project: MyProject.Core → MyProject.Core.csproj
│   │   ├── src
│   │   │   ├── Models
│   │   │   │   ├── User.cs
│   │   │   │   │   ├── public class User
│   │   │   │   │   │   ├── public property Id : int
│   │   │   │   │   │   ├── public property Name : string
│   │   │   │   │   │   ├── public User(string name)
│   │   │   │   │   │   └── public void Validate()
│   │   │   │   │   └── public enum UserStatus
│   │   │   │   └── Product.cs
3. Export Options
Text (Tree): Hierarchical representation with indentation

Text (Flat): Simple one-line format for quick reference

JSON: Complete structured data for APIs or further processing

💻 Technical Features
Smart Parsing
Regular expression-based C# code analysis

Brace counting for accurate scope detection

Access modifier extraction (public, private, protected, internal)

Parameter type and name parsing

Data Model
csharp
class SolutionData {
    string Name;
    string Path;
    List<ProjectData> Projects;
    List<StandaloneFileData> StandaloneFiles;
}

class ProjectData {
    string Name;
    string ProjectFile;
    List<CsFileData> Files;
}

class ClassData {
    string Name;
    string AccessModifier;
    List<PropertyData> Properties;
    List<MethodData> Methods;
    // ... and more
}
Performance Optimizations
Asynchronous scanning for large solutions

Background processing with UI thread updates

Efficient file I/O operations

Memory-conscious tree building

🎯 Use Cases
For Developers
Documentation: Generate quick overviews of solution structure

Code Review: Analyze unfamiliar codebases quickly

Architecture Analysis: Understand project dependencies and organization

Migration Planning: Assess code structure before refactoring

For Teams
Onboarding: Help new team members understand codebase structure

Knowledge Sharing: Create reference documentation

Quality Assurance: Check for consistent naming and access patterns

Audit Trails: Document solution states at different points in time

For Project Managers
Progress Tracking: Monitor codebase growth and complexity

Resource Estimation: Understand code volume and structure

Reporting: Generate technical reports for stakeholders

🔧 Installation & Usage
Requirements
.NET Framework 4.7.2+ or .NET 8.0

Windows OS

Visual Studio (for building from source)

Quick Start
Download and run the executable

Click "Browse" and select your solution folder

Click "Scan" to analyze the codebase

Explore the hierarchical tree view

Export in your preferred format

Building from Source
bash
git clone https://github.com/yourusername/csharp-solution-scanner.git
cd csharp-solution-scanner
# Open in Visual Studio and build
📈 Sample Output
Tree View Example
text
Solution: ECommerce → C:\Projects\ECommerce
├── Solution File: ECommerce.sln
├── Projects
│   ├── Project: ECommerce.Core → ECommerce.Core.csproj
│   │   ├── Models
│   │   │   ├── Product.cs
│   │   │   │   ├── public class Product
│   │   │   │   │   ├── public property Id : int
│   │   │   │   │   ├── public property Name : string
│   │   │   │   │   ├── public decimal CalculatePrice()
│   │   │   │   │   └── private bool ValidateStock()
Flat List Example
text
public class Product
    public property Id
    public property Name
    public decimal CalculatePrice()
    private bool ValidateStock()
JSON Export
json
{
  "Name": "ECommerce",
  "Path": "C:\\Projects\\ECommerce",
  "Projects": [
    {
      "Name": "ECommerce.Core",
      "ProjectFile": "ECommerce.Core.csproj",
      "Files": [
        {
          "FileName": "Product.cs",
          "Classes": [
            {
              "Name": "Product",
              "AccessModifier": "public",
              "Properties": [
                {
                  "Name": "Id",
                  "AccessModifier": "public",
                  "Type": "int"
                }
              ]
            }
          ]
        }
      ]
    }
  ]
}
🤝 Contributing
We welcome contributions! Here's how you can help:

Fork the repository

Create a feature branch

Make your improvements

Submit a pull request

Areas for Contribution
Enhanced C# parsing (partial classes, generics, etc.)

Additional export formats (XML, CSV, Markdown)

Performance optimizations for large solutions

Plugin system for custom analyzers

Cross-platform support

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

🙏 Acknowledgments
Built with .NET WinForms

Uses MaterialSkin for modern UI components

Newtonsoft.Json for JSON serialization

Inspired by the need for better codebase visualization tools

📞 Support
Issues: GitHub Issues

Email: tsgiannis@gmail.com

Website: Solutions4It.guru

Developed by John Tsioumpris
For all your programming needs: .NET, Python, SQL, Ms Access/VBA, ML/DL & Others

🚀 Get Started Today!
Whether you're analyzing a legacy codebase, documenting a new project, or simply exploring unfamiliar solutions, C# Solution Scanner provides the tools you need to understand and document C# code efficiently.

Download now and transform how you visualize and analyze your C# solutions!

https://img.shields.io/badge/Download-Latest-blue
https://img.shields.io/github/stars/tsgiannis/csharp-solution-scanner
https://img.shields.io/github/issues/tsgiannis/csharp-solution-scanner
