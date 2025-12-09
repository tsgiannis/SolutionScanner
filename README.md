# C# Solution Scanner & Analyzer

🚀 **Overview**  
C# Solution Scanner is a powerful Windows Forms application designed to help developers analyze and document their C# solutions. It automatically scans through entire solution structures, extracts detailed information about classes, methods, properties, and more, presenting everything in an organized, exportable format.  


[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)](https://github.com/tsgiannis/SolutionScanner)  
[![.NET](https://img.shields.io/badge/.NET-4.7.2-purple)](https://github.com/tsgiannis/SolutionScanner)  
[![License](https://img.shields.io/badge/License-MIT-green)](https://github.com/tsgiannis/SolutionScanner/blob/main/LICENSE)  

✨ **Features**  
🔍 **Intelligent Solution Analysis**  
- Recursive Scanning: Automatically discovers all .cs files within solution folders  
- Smart Project Detection: Identifies .csproj files and organizes code by project  
- Solution File Recognition: Detects .sln files for proper solution hierarchy  

📊 **Comprehensive Code Extraction**  
- Class Information: Extracts all classes with access modifiers  
- Member Details: Captures properties, fields, methods, and constructors  
- Type Discovery: Identifies enums, structs, and interfaces  
- Parameter Parsing: Extracts method/constructor parameters with types  

🌳 **Multiple View Modes**  
- Tree View: Hierarchical display showing solution → projects → files → classes → members  
- Flat List: One-line-per-item format for quick scanning  
- JSON Export: Complete structured JSON data for programmatic use  

🎨 **Enhanced User Experience**  
- Syntax Highlighting: Color-coded C# and JSON syntax for better readability  
- Modern UI: Clean, responsive interface with Material Design elements  
- Interactive Tree: Click nodes to view file contents or class details  
- Progress Indicators: Visual feedback during scanning operations  

📤 **Export Capabilities**  
- Multiple Formats: Export as text (tree or flat) or JSON  
- Clipboard Support: Copy any view to clipboard with one click  
- File Export: Save scans to disk for documentation or analysis  
- One-Click Email: Contact information with direct email composition  

🖥️ **Screenshot**  

```
┌─────────────────────────────────────────────────────────────────┐
│ C# Solution Scanner                                             │
├─────────────────────────────────────────────────────────────────┤
│ [C:\Projects\MySolution ] [Browse] [Scan]                       │
├─────────────────────────────────────────────────────────────────┤
│ Status: Ready                                                   │
├─────────────────────────────────────────────────────────────────┤
│ Tree View:                                                      │
│   ● Solution: MySolution                                        │
│     ├── Projects                                                │
│     │   ├── Project: MyProject.Core                             │
│     │   │   ├── src\Models                                      │
│     │   │   │   ├── User.cs                                     │
│     │   │   │   │   ├── public class User                       │
│     │   │   │   │   │   ├── public property Id                  │
│     │   │   │   │   │   ├── public property Name                │
│     │   │   │   │   │   └── public void Validate()              │
├─────────────────────────────────────────────────────────────────┤
│ Output:                                                         │
│ public class User {                                             │
│     public int Id { get; set; }                                 │
│     public string Name { get; set; }                            │
│     public void Validate() { }                                  │
│ }                                                               │
├─────────────────────────────────────────────────────────────────┤
│ [Text (Tree)] [Clear] [Copy] [Export] [About]                   │
└─────────────────────────────────────────────────────────────────┘
```

🛠️ **Installation**  
**Option 1: Download Executable**  
1. Download the latest release from [Releases](https://github.com/tsgiannis/SolutionScanner/releases)  
2. Extract the ZIP file  
3. Run CSharpFileScanner.exe  

**Option 2: Build from Source**  
```bash
# Clone the repository
git clone https://github.com/tsgiannis/SolutionScanner.git

# Open in Visual Studio
cd csharp-solution-scanner
CSharpFileScanner.sln

# Build and run (F5)
```

**Requirements**  
- .NET 8.0 Runtime (if using executable)  
- Visual Studio 2022 (if building from source)  
- Windows 10/11 (Windows Forms application)  

🚀 **Quick Start Guide**  
**Step 1: Select Solution Folder**  
Click the Browse button and navigate to your solution folder containing .sln and .csproj files.  

**Step 2: Scan Solution**  
Click the Scan button to analyze all C# files in the solution.  

**Step 3: Explore Results**  
- Navigate the tree view to explore solution structure  
- Click on files to view their contents with syntax highlighting  
- Click on classes to see detailed member information  

**Step 4: Export Data**  
Choose your preferred format and click Export:  
- Text (Tree): Hierarchical text representation  
- Text (Flat): Simple one-line-per-item format  
- JSON: Structured data for APIs or further processing  

📖 **Usage Examples**  
**Example 1: Scan a Solution**  

```
Input: C:\Projects\ECommerce\
Output:
Solution: ECommerce → C:\Projects\ECommerce
├── Projects
│   ├── Project: ECommerce.Core → ECommerce.Core.csproj
│   │   ├── Models
│   │   │   ├── Product.cs
│   │   │   │   ├── public class Product
│   │   │   │   │   ├── public property Id : int
│   │   │   │   │   ├── public property Name : string
│   │   │   │   │   └── public decimal CalculatePrice()
```

**Example 2: Export to JSON**  
```json
{
  "Name": "MySolution",
  "Path": "C:\\Projects\\MySolution",
  "SolutionFile": "MySolution.sln",
  "Projects": [
    {
      "Name": "MyProject.Core",
      "ProjectFile": "MyProject.Core.csproj",
      "Files": [
        {
          "FileName": "User.cs",
          "FilePath": "C:\\Projects\\MySolution\\src\\Models\\User.cs",
          "Classes": [
            {
              "Name": "User",
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
```

🎯 **Use Cases**  
**For Developers**  
- Codebase Documentation: Generate quick overviews of solution structure  
- Legacy Code Analysis: Understand unfamiliar codebases quickly  
- Architecture Review: Analyze project dependencies and organization  
- Migration Planning: Assess code structure before refactoring  

**For Teams**  
- Onboarding: Help new team members understand codebase structure  
- Knowledge Sharing: Create reference documentation for team members  
- Code Standards: Check for consistent naming and access patterns  
- Audit Trails: Document solution states at different points in time  

**For Project Managers**  
- Progress Tracking: Monitor codebase growth and complexity  
- Resource Estimation: Understand code volume for planning  
- Technical Reporting: Generate reports for stakeholders  

🔧 **Technical Details**  
**Supported C# Constructs**  
- Classes (public, private, internal, protected)  
- Properties (auto-properties with get/set)  
- Methods (with return types and parameters)  
- Constructors (with parameters)  
- Fields/Variables  
- Enums, Structs, and Interfaces  

**File Support**  
- .cs files (C# source code)  
- .csproj files (project files)  
- .sln files (solution files)  

**Performance**  
- Fast Scanning: Processes hundreds of files in seconds  
- Memory Efficient: Stream-based file reading  
- Async Operations: Non-blocking UI during scans  
- Incremental Updates: Real-time progress reporting  

📁 **Project Structure**  

```
CSharpFileScanner/
├── SolutionScanner.cs              # Main application form
├── SolutionScanner.Designer.cs     # UI designer file
├── AboutForm.cs          # About dialog
├── Program.cs           # Application entry point
├── CSharpFileScanner.csproj  # Project file
└── README.md            # This file
```

🤝 **Contributing**  
We welcome contributions! Here's how you can help:  
1. Fork the repository  
2. Create a feature branch (git checkout -b feature/AmazingFeature)  
3. Commit your changes (git commit -m 'Add some AmazingFeature')  
4. Push to the branch (git push origin feature/AmazingFeature)  
5. Open a Pull Request  

**Areas for Improvement**  
- Enhanced C# parsing (generics, async methods, attributes)  
- Additional export formats (XML, CSV, Markdown)  
- Performance optimizations for very large solutions  
- Plugin system for custom analyzers  
- Cross-platform support  

📄 **License**  
This project is licensed under the MIT License - see the [LICENSE](https://github.com/tsgiannis/SolutionScanner/blob/main/LICENSE) file for details.  

👨‍💻 **Author**  
John Tsioumpris  
- Email: tsgiannis@gmail.com  
- Website: Solutions4it.guru  
- GitHub: [@tsgiannis](https://github.com/tsgiannis)  

For all your programming needs: .NET, Python, SQL, Ms Access/VBA, ML/DL & Others  

🙏 **Acknowledgments**  
- Built with .NET Windows Forms  
- Uses MaterialSkin for modern UI components  
- Newtonsoft.Json for JSON serialization  
- Inspired by the need for better codebase visualization tools  

📞 **Support**  
- Report Issues: [GitHub Issues](https://github.com/tsgiannis/SolutionScanner/issues)  
- Feature Requests: Open an issue with enhancement label  
- Questions: Email tsgiannis@gmail.com  

⭐ **Show Your Support**  
If you find this tool useful, please give it a star on GitHub!  
[![GitHub stars](https://img.shields.io/github/stars/tsgiannis/SolutionScanner?style=social)](https://github.com/tsgiannis/SolutionScanner)  

Download now and transform how you visualize and analyze your C# solutions!  
[![Download Latest Release](https://img.shields.io/badge/Download-Latest-Release-blue)](https://github.com/tsgiannis/SolutionScanner/releases)  
[![View Source Code](https://img.shields.io/badge/View-Source-Code-green)](https://github.com/tsgiannis/SolutionScanner)
