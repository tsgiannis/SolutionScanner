using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CSharpFileScanner
{
    public partial class SolutionScanner : MaterialForm
    {
        private readonly MaterialSkinManager skinManager;
        private SolutionData currentSolution;

        public SolutionScanner()
        {
            InitializeComponent();

            // Initialize MaterialSkin
            skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new ColorScheme(
                Primary.LightBlue600, Primary.Blue700,
                Primary.Blue500, Accent.Blue400,
                TextShade.WHITE);

            // Set form to center screen
            this.StartPosition = FormStartPosition.CenterScreen;

            // Initialize output format combo
            cboOutputFormat.SelectedIndex = 0;

            // Initialize syntax highlighting colors
            InitializeSyntaxHighlighting();
        }

        private void InitializeSyntaxHighlighting()
        {
            // Set up syntax highlighting colors
            txtOutput.SelectionColor = Color.DarkBlue;
            txtOutput.SelectionBackColor = txtOutput.BackColor;
        }

        private class SolutionData
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string SolutionFile { get; set; }
            public List<ProjectData> Projects { get; set; } = new List<ProjectData>();
            public List<StandaloneFileData> StandaloneFiles { get; set; } = new List<StandaloneFileData>();
        }

        private class ProjectData
        {
            public string Name { get; set; }
            public string ProjectFile { get; set; }
            public string FolderPath { get; set; }
            public List<CsFileData> Files { get; set; } = new List<CsFileData>();
        }

        private class CsFileData
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string RelativePath { get; set; }
            public List<ClassData> Classes { get; set; } = new List<ClassData>();
            public List<EnumData> Enums { get; set; } = new List<EnumData>();
            public List<StructData> Structs { get; set; } = new List<StructData>();
            public List<InterfaceData> Interfaces { get; set; } = new List<InterfaceData>();
        }

        private class ClassData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public bool IsStatic { get; set; }
            public bool IsAbstract { get; set; }
            public bool IsSealed { get; set; }
            public List<PropertyData> Properties { get; set; } = new List<PropertyData>();
            public List<FieldData> Fields { get; set; } = new List<FieldData>();
            public List<MethodData> Methods { get; set; } = new List<MethodData>();
            public List<ConstructorData> Constructors { get; set; } = new List<ConstructorData>();
        }

        private class PropertyData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public string Type { get; set; }
            public bool HasGetter { get; set; }
            public bool HasSetter { get; set; }
        }

        private class FieldData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public string Type { get; set; }
        }

        private class MethodData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public string ReturnType { get; set; }
            public List<ParameterData> Parameters { get; set; } = new List<ParameterData>();
        }

        private class ConstructorData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public List<ParameterData> Parameters { get; set; } = new List<ParameterData>();
        }

        private class ParameterData
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private class EnumData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
            public List<string> Values { get; set; } = new List<string>();
        }

        private class StructData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
        }

        private class InterfaceData
        {
            public string Name { get; set; }
            public string AccessModifier { get; set; }
        }

        private class StandaloneFileData
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public List<ClassData> Classes { get; set; } = new List<ClassData>();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select solution folder to scan";

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFolderPath.Text))
            {
                MaterialMessageBox.Show("Please select a folder first.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(txtFolderPath.Text))
            {
                MaterialMessageBox.Show("Selected folder does not exist.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Show progress bar
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                btnScan.Enabled = false;
                btnBrowse.Enabled = false;

                // Clear previous data
                treeView1.Nodes.Clear();
                txtOutput.Clear();
                currentSolution = null;

                // Find solution file
                string solutionName = "Unknown Solution";
                string solutionFile = FindSolutionFile(txtFolderPath.Text);

                if (!string.IsNullOrEmpty(solutionFile))
                {
                    solutionName = Path.GetFileNameWithoutExtension(solutionFile);
                }
                else
                {
                    solutionName = new DirectoryInfo(txtFolderPath.Text).Name;
                }

                // Initialize solution data
                currentSolution = new SolutionData
                {
                    Name = solutionName,
                    Path = txtFolderPath.Text,
                    SolutionFile = solutionFile
                };

                // Create root node with icon
                TreeNode rootNode = new TreeNode($"🧩 Solution: {solutionName}")
                {
                    Tag = currentSolution,
                    ImageKey = "solution",
                    SelectedImageKey = "solution"
                };

                if (!string.IsNullOrEmpty(solutionFile))
                {
                    rootNode.Nodes.Add($"📄 Solution File: {Path.GetFileName(solutionFile)}");
                }

                treeView1.Nodes.Add(rootNode);

                // Scan for projects and C# files
                await System.Threading.Tasks.Task.Run(() =>
                {
                    ScanSolutionFolder(txtFolderPath.Text, rootNode);
                });

                // Expand all nodes
                treeView1.ExpandAll();

                // Update output based on selected format
                UpdateOutputFormat();

                lblStatus.Text = $"✅ Solution '{solutionName}' scanned successfully";
                MessageBox.Show($"Scanned {CountTotalItems()} items", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show($"Error scanning solution: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "❌ Error scanning solution";
            }
            finally
            {
                progressBar.Visible = false;
                btnScan.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }
        private void btnAbout_Click(object sender, EventArgs e)
        {
            using (var aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);  // 'this' makes it modal to the main form
            }
        }
        private int CountTotalItems()
        {
            if (currentSolution == null) return 0;

            int count = 0;
            count += currentSolution.Projects.Sum(p => p.Files.Count);
            count += currentSolution.StandaloneFiles.Count;

            foreach (var project in currentSolution.Projects)
            {
                foreach (var file in project.Files)
                {
                    count += file.Classes.Count;
                    count += file.Enums.Count;
                    count += file.Structs.Count;
                    count += file.Interfaces.Count;

                    foreach (var cls in file.Classes)
                    {
                        count += cls.Properties.Count;
                        count += cls.Fields.Count;
                        count += cls.Methods.Count;
                        count += cls.Constructors.Count;
                    }
                }
            }

            return count;
        }

        private string FindSolutionFile(string folderPath)
        {
            var slnFiles = Directory.GetFiles(folderPath, "*.sln", SearchOption.AllDirectories);

            // Try to find solution file in root first
            var rootSln = slnFiles.FirstOrDefault(f =>
                Path.GetDirectoryName(f).Equals(folderPath, StringComparison.OrdinalIgnoreCase));

            if (rootSln != null)
                return rootSln;

            return slnFiles.FirstOrDefault();
        }

        private void ScanSolutionFolder(string folderPath, TreeNode parentNode)
        {
            try
            {
                // Find all .csproj files
                var csprojFiles = Directory.GetFiles(folderPath, "*.csproj", SearchOption.AllDirectories);

                TreeNode projectsNode = new TreeNode("📦 Projects")
                {
                    ImageKey = "folder",
                    SelectedImageKey = "folder"
                };

                this.Invoke((MethodInvoker)delegate
                {
                    parentNode.Nodes.Add(projectsNode);
                });

                foreach (var csprojFile in csprojFiles)
                {
                    string projectName = Path.GetFileNameWithoutExtension(csprojFile);
                    string projectFolder = Path.GetDirectoryName(csprojFile);

                    // Create project data
                    var projectData = new ProjectData
                    {
                        Name = projectName,
                        ProjectFile = Path.GetFileName(csprojFile),
                        FolderPath = projectFolder
                    };

                    currentSolution.Projects.Add(projectData);

                    // Create project node
                    TreeNode projectNode = new TreeNode($"📁 {projectName}")
                    {
                        Tag = projectData,
                        ImageKey = "project",
                        SelectedImageKey = "project"
                    };

                    this.Invoke((MethodInvoker)delegate
                    {
                        projectsNode.Nodes.Add(projectNode);
                    });

                    // Scan project folder
                    ScanProjectFolder(projectFolder, projectNode, projectData);
                }

                // Scan for standalone C# files
                ScanStandaloneCsFiles(folderPath, parentNode);
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    parentNode.Nodes.Add($"❌ Error: {ex.Message}");
                });
            }
        }

        private void ScanProjectFolder(string projectFolder, TreeNode projectNode, ProjectData projectData)
        {
            try
            {
                var csFiles = Directory.GetFiles(projectFolder, "*.cs", SearchOption.AllDirectories);
                var filesByDir = csFiles.GroupBy(f => Path.GetDirectoryName(f))
                    .OrderBy(g => g.Key);

                foreach (var dirGroup in filesByDir)
                {
                    string relativePath = GetRelativePath(dirGroup.Key, projectFolder);
                    TreeNode dirNode = new TreeNode($"📂 {relativePath}")
                    {
                        ImageKey = "folder",
                        SelectedImageKey = "folder"
                    };

                    this.Invoke((MethodInvoker)delegate
                    {
                        projectNode.Nodes.Add(dirNode);
                    });

                    foreach (var csFile in dirGroup.OrderBy(f => f))
                    {
                        var fileData = ExtractFileData(csFile, projectFolder);
                        projectData.Files.Add(fileData);

                        TreeNode fileNode = new TreeNode($"📄 {Path.GetFileName(csFile)}")
                        {
                            Tag = fileData,
                            ImageKey = "file",
                            SelectedImageKey = "file"
                        };

                        this.Invoke((MethodInvoker)delegate
                        {
                            dirNode.Nodes.Add(fileNode);
                        });

                        // Add class nodes
                        foreach (var classData in fileData.Classes)
                        {
                            TreeNode classNode = new TreeNode($"👨‍💻 {classData.AccessModifier} class {classData.Name}")
                            {
                                Tag = classData,
                                ImageKey = "class",
                                SelectedImageKey = "class"
                            };

                            this.Invoke((MethodInvoker)delegate
                            {
                                fileNode.Nodes.Add(classNode);
                            });

                            // Add properties, fields, methods, constructors
                            AddClassMembersToTree(classData, classNode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    projectNode.Nodes.Add($"❌ Error: {ex.Message}");
                });
            }
        }

        private CsFileData ExtractFileData(string filePath, string basePath)
        {
            var fileData = new CsFileData
            {
                FileName = Path.GetFileName(filePath),
                FilePath = filePath,
                RelativePath = GetRelativePath(Path.GetDirectoryName(filePath), basePath)
            };

            try
            {
                var content = File.ReadAllText(filePath);

                // Extract classes
                var classMatches = Regex.Matches(content,
                    @"(public|private|internal|protected)?\s*(static\s+)?(abstract\s+)?(sealed\s+)?class\s+(\w+)",
                    RegexOptions.Multiline);

                foreach (Match match in classMatches)
                {
                    if (match.Success)
                    {
                        var classData = new ClassData
                        {
                            Name = match.Groups[5].Value,
                            AccessModifier = string.IsNullOrEmpty(match.Groups[1].Value) ? "internal" : match.Groups[1].Value,
                            IsStatic = !string.IsNullOrEmpty(match.Groups[2].Value),
                            IsAbstract = !string.IsNullOrEmpty(match.Groups[3].Value),
                            IsSealed = !string.IsNullOrEmpty(match.Groups[4].Value)
                        };

                        ExtractClassMembers(filePath, classData);
                        fileData.Classes.Add(classData);
                    }
                }

                // Extract enums, structs, interfaces similarly
                ExtractOtherTypes(content, fileData);
            }
            catch (Exception ex)
            {
                // Log error but continue
                Console.WriteLine($"Error extracting data from {filePath}: {ex.Message}");
            }

            return fileData;
        }

        private void ExtractClassMembers(string filePath, ClassData classData)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                bool insideClass = false;
                int braceCount = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    if (line.Contains($"class {classData.Name}") && !insideClass)
                    {
                        insideClass = true;
                        continue;
                    }

                    if (insideClass)
                    {
                        braceCount += line.Count(c => c == '{');
                        braceCount -= line.Count(c => c == '}');

                        // Extract properties
                        if (Regex.IsMatch(line, @"^\s*(public|private|internal|protected)\s+(\w+)\s+(\w+)\s*\{\s*get[^}]*set[^}]*\}\s*$"))
                        {
                            var match = Regex.Match(line, @"^\s*(public|private|internal|protected)\s+(\w+)\s+(\w+)");
                            if (match.Success)
                            {
                                classData.Properties.Add(new PropertyData
                                {
                                    Name = match.Groups[3].Value,
                                    AccessModifier = match.Groups[1].Value,
                                    Type = match.Groups[2].Value,
                                    HasGetter = line.Contains("get"),
                                    HasSetter = line.Contains("set")
                                });
                            }
                        }
                        // Extract constructors
                        else if (line.Contains($" {classData.Name}("))
                        {
                            var match = Regex.Match(line,
                                @"^\s*(public|private|internal|protected)?\s*" +
                                Regex.Escape(classData.Name) + @"\(([^)]*)\)");
                            if (match.Success)
                            {
                                var constructor = new ConstructorData
                                {
                                    Name = classData.Name,
                                    AccessModifier = string.IsNullOrEmpty(match.Groups[1].Value) ? "public" : match.Groups[1].Value
                                };

                                // Extract parameters
                                var paramsStr = match.Groups[2].Value;
                                if (!string.IsNullOrEmpty(paramsStr))
                                {
                                    var paramsList = paramsStr.Split(',')
                                        .Select(p => p.Trim())
                                        .Where(p => !string.IsNullOrEmpty(p));

                                    foreach (var param in paramsList)
                                    {
                                        var parts = param.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                                        if (parts.Length >= 2)
                                        {
                                            constructor.Parameters.Add(new ParameterData
                                            {
                                                Type = parts[0],
                                                Name = parts[1]
                                            });
                                        }
                                    }
                                }

                                classData.Constructors.Add(constructor);
                            }
                        }
                        // Extract methods
                        else if (Regex.IsMatch(line, @"^\s*(public|private|internal|protected)\s+(\w+\s+)?(\w+)\s*\(([^)]*)\)"))
                        {
                            var match = Regex.Match(line,
                                @"^\s*(public|private|internal|protected)\s+(\w+\s+)?(\w+)\s*\(([^)]*)\)");
                            if (match.Success)
                            {
                                var method = new MethodData
                                {
                                    Name = match.Groups[3].Value,
                                    AccessModifier = match.Groups[1].Value,
                                    ReturnType = string.IsNullOrEmpty(match.Groups[2]?.Value?.Trim()) ?
                                        "void" : match.Groups[2].Value.Trim()
                                };

                                // Extract parameters
                                var paramsStr = match.Groups[4].Value;
                                if (!string.IsNullOrEmpty(paramsStr))
                                {
                                    var paramsList = paramsStr.Split(',')
                                        .Select(p => p.Trim())
                                        .Where(p => !string.IsNullOrEmpty(p));

                                    foreach (var param in paramsList)
                                    {
                                        var parts = param.Split(' ').Where(p => !string.IsNullOrEmpty(p)).ToArray();
                                        if (parts.Length >= 2)
                                        {
                                            method.Parameters.Add(new ParameterData
                                            {
                                                Type = parts[0],
                                                Name = parts[1]
                                            });
                                        }
                                    }
                                }

                                classData.Methods.Add(method);
                            }
                        }

                        if (braceCount <= 0 && insideClass)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting class members: {ex.Message}");
            }
        }

        private void ExtractOtherTypes(string content, CsFileData fileData)
        {
            // Extract enums
            var enumMatches = Regex.Matches(content,
                @"(public|private|internal|protected)?\s*enum\s+(\w+)",
                RegexOptions.Multiline);

            foreach (Match match in enumMatches)
            {
                if (match.Success)
                {
                    string accessModifier = match.Groups[1].Value;
                    if (string.IsNullOrEmpty(accessModifier))
                        accessModifier = "internal";

                    fileData.Enums.Add(new EnumData
                    {
                        Name = match.Groups[2].Value,
                        AccessModifier = accessModifier
                    });
                }
            }

            // Extract structs
            var structMatches = Regex.Matches(content,
                @"(public|private|internal|protected)?\s*struct\s+(\w+)",
                RegexOptions.Multiline);

            foreach (Match match in structMatches)
            {
                if (match.Success)
                {
                    string accessModifier = match.Groups[1].Value;
                    if (string.IsNullOrEmpty(accessModifier))
                        accessModifier = "internal";

                    fileData.Structs.Add(new StructData
                    {
                        Name = match.Groups[2].Value,
                        AccessModifier = accessModifier
                    });
                }
            }

            // Extract interfaces
            var interfaceMatches = Regex.Matches(content,
                @"(public|private|internal|protected)?\s*interface\s+(\w+)",
                RegexOptions.Multiline);

            foreach (Match match in interfaceMatches)
            {
                if (match.Success)
                {
                    string accessModifier = match.Groups[1].Value;
                    if (string.IsNullOrEmpty(accessModifier))
                        accessModifier = "internal";

                    fileData.Interfaces.Add(new InterfaceData
                    {
                        Name = match.Groups[2].Value,
                        AccessModifier = accessModifier
                    });
                }
            }
        }

        private void AddClassMembersToTree(ClassData classData, TreeNode classNode)
        {
            // Add properties
            foreach (var prop in classData.Properties)
            {
                TreeNode propNode = new TreeNode($"🔧 {prop.AccessModifier} property {prop.Name} : {prop.Type}")
                {
                    Tag = prop,
                    ImageKey = "property",
                    SelectedImageKey = "property"
                };

                this.Invoke((MethodInvoker)delegate
                {
                    classNode.Nodes.Add(propNode);
                });
            }

            // Add constructors
            foreach (var ctor in classData.Constructors)
            {
                string paramsStr = string.Join(", ", ctor.Parameters.Select(p => $"{p.Type} {p.Name}"));
                TreeNode ctorNode = new TreeNode($"🏗️ {ctor.AccessModifier} {ctor.Name}({paramsStr})")
                {
                    Tag = ctor,
                    ImageKey = "method",
                    SelectedImageKey = "method"
                };

                this.Invoke((MethodInvoker)delegate
                {
                    classNode.Nodes.Add(ctorNode);
                });
            }

            // Add methods
            foreach (var method in classData.Methods)
            {
                string paramsStr = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                TreeNode methodNode = new TreeNode($"⚙️ {method.AccessModifier} {method.ReturnType} {method.Name}({paramsStr})")
                {
                    Tag = method,
                    ImageKey = "method",
                    SelectedImageKey = "method"
                };

                this.Invoke((MethodInvoker)delegate
                {
                    classNode.Nodes.Add(methodNode);
                });
            }
        }

        private void ScanStandaloneCsFiles(string folderPath, TreeNode parentNode)
        {
            try
            {
                // Find all .cs files in the solution folder
                var allCsFiles = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

                // Get all project folders (folders containing .csproj files)
                var projectFolders = Directory.GetFiles(folderPath, "*.csproj", SearchOption.AllDirectories)
                    .Select(f => Path.GetDirectoryName(f))
                    .ToHashSet();

                // Filter out files that are inside project folders
                var standaloneFiles = new List<string>();
                foreach (var csFile in allCsFiles)
                {
                    string fileDir = Path.GetDirectoryName(csFile);
                    bool isInProject = false;

                    // Check if file is in any project folder
                    foreach (var projectFolder in projectFolders)
                    {
                        if (fileDir.StartsWith(projectFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(fileDir, projectFolder, StringComparison.OrdinalIgnoreCase))
                        {
                            isInProject = true;
                            break;
                        }
                    }

                    if (!isInProject)
                    {
                        standaloneFiles.Add(csFile);
                    }
                }

                if (standaloneFiles.Count > 0)
                {
                    TreeNode standaloneNode = new TreeNode("📄 Standalone C# Files")
                    {
                        ImageKey = "folder",
                        SelectedImageKey = "folder"
                    };

                    this.Invoke((MethodInvoker)delegate
                    {
                        parentNode.Nodes.Add(standaloneNode);
                    });

                    // Group by directory
                    var filesByDir = standaloneFiles.GroupBy(f => Path.GetDirectoryName(f))
                        .OrderBy(g => g.Key);

                    foreach (var dirGroup in filesByDir)
                    {
                        string relativePath = GetRelativePath(dirGroup.Key, folderPath);
                        TreeNode dirNode = new TreeNode($"📂 {relativePath}")
                        {
                            ImageKey = "folder",
                            SelectedImageKey = "folder"
                        };

                        this.Invoke((MethodInvoker)delegate
                        {
                            standaloneNode.Nodes.Add(dirNode);
                        });

                        foreach (var csFile in dirGroup.OrderBy(f => f))
                        {
                            // Extract file data
                            var fileData = ExtractFileData(csFile, folderPath);

                            // Add to solution data
                            var standaloneFileData = new StandaloneFileData
                            {
                                FileName = Path.GetFileName(csFile),
                                FilePath = csFile,
                                Classes = fileData.Classes
                            };

                            currentSolution.StandaloneFiles.Add(standaloneFileData);

                            TreeNode fileNode = new TreeNode($"📄 {Path.GetFileName(csFile)}")
                            {
                                Tag = fileData,
                                ImageKey = "file",
                                SelectedImageKey = "file"
                            };

                            this.Invoke((MethodInvoker)delegate
                            {
                                dirNode.Nodes.Add(fileNode);
                            });

                            // Add class nodes
                            foreach (var classData in fileData.Classes)
                            {
                                TreeNode classNode = new TreeNode($"👨‍💻 {classData.AccessModifier} class {classData.Name}")
                                {
                                    Tag = classData,
                                    ImageKey = "class",
                                    SelectedImageKey = "class"
                                };

                                this.Invoke((MethodInvoker)delegate
                                {
                                    fileNode.Nodes.Add(classNode);
                                });

                                // Add class members
                                AddClassMembersToTree(classData, classNode);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    parentNode.Nodes.Add($"❌ Error scanning standalone files: {ex.Message}");
                });
            }
        }

        private string GetRelativePath(string fullPath, string basePath)
        {
            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(basePath))
                return string.Empty;

            Uri fullUri = new Uri(fullPath);
            Uri baseUri = new Uri(basePath + Path.DirectorySeparatorChar);

            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString()
                .Replace('/', Path.DirectorySeparatorChar));
        }

        private void UpdateOutputFormat()
        {
            if (currentSolution == null) return;

            string format = cboOutputFormat.SelectedItem?.ToString();

            switch (format)
            {
                case "Text (Tree)":
                    DisplayAsTextTree();
                    break;
                case "Text (Flat)":
                    DisplayAsFlatText();
                    break;
                case "JSON":
                    DisplayAsJson();
                    break;
                default:
                    DisplayAsTextTree();
                    break;
            }

            ApplySyntaxHighlighting();
        }

        private void DisplayAsTextTree()
        {
            StringBuilder sb = new StringBuilder();
            AppendSolutionTree(currentSolution, sb, 0);
            txtOutput.Text = sb.ToString();
        }

        private void AppendSolutionTree(SolutionData solution, StringBuilder sb, int indent)
        {
            string indentStr = new string(' ', indent * 2);
            sb.AppendLine($"{indentStr}Solution: {solution.Name}");
            sb.AppendLine($"{indentStr}Path: {solution.Path}");

            if (!string.IsNullOrEmpty(solution.SolutionFile))
            {
                sb.AppendLine($"{indentStr}Solution File: {solution.SolutionFile}");
            }

            sb.AppendLine();

            foreach (var project in solution.Projects)
            {
                sb.AppendLine($"{indentStr}├── Project: {project.Name} -> {project.ProjectFile}");

                foreach (var file in project.Files)
                {
                    string fileIndent = new string(' ', (indent + 1) * 2);
                    sb.AppendLine($"{fileIndent}├── File: {file.FileName}");

                    foreach (var cls in file.Classes)
                    {
                        string classIndent = new string(' ', (indent + 2) * 2);
                        sb.AppendLine($"{classIndent}├── {cls.AccessModifier} class {cls.Name}");

                        foreach (var prop in cls.Properties)
                        {
                            string memberIndent = new string(' ', (indent + 3) * 2);
                            sb.AppendLine($"{memberIndent}├── {prop.AccessModifier} property {prop.Name} : {prop.Type}");
                        }

                        foreach (var ctor in cls.Constructors)
                        {
                            string memberIndent = new string(' ', (indent + 3) * 2);
                            string paramsStr = string.Join(", ", ctor.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            sb.AppendLine($"{memberIndent}├── {ctor.AccessModifier} {ctor.Name}({paramsStr})");
                        }

                        foreach (var method in cls.Methods)
                        {
                            string memberIndent = new string(' ', (indent + 3) * 2);
                            string paramsStr = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            sb.AppendLine($"{memberIndent}├── {method.AccessModifier} {method.ReturnType} {method.Name}({paramsStr})");
                        }
                    }
                }
                sb.AppendLine();
            }
        }

        private void DisplayAsFlatText()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"// Solution: {currentSolution.Name}");
            sb.AppendLine($"// Path: {currentSolution.Path}");
            sb.AppendLine();

            foreach (var project in currentSolution.Projects)
            {
                sb.AppendLine($"// Project: {project.Name}");

                foreach (var file in project.Files)
                {
                    sb.AppendLine($"// File: {file.FileName}");

                    foreach (var cls in file.Classes)
                    {
                        sb.AppendLine($"{cls.AccessModifier} class {cls.Name}");

                        foreach (var prop in cls.Properties)
                        {
                            sb.AppendLine($"    {prop.AccessModifier} property {prop.Name}");
                        }

                        foreach (var ctor in cls.Constructors)
                        {
                            string paramsStr = string.Join(", ", ctor.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            sb.AppendLine($"    {ctor.AccessModifier} {ctor.Name}({paramsStr})");
                        }

                        foreach (var method in cls.Methods)
                        {
                            string paramsStr = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                            sb.AppendLine($"    {method.AccessModifier} {method.ReturnType} {method.Name}({paramsStr})");
                        }

                        sb.AppendLine();
                    }
                }
            }

            txtOutput.Text = sb.ToString();
        }

        private void DisplayAsJson()
        {
            try
            {
                string json = JsonConvert.SerializeObject(currentSolution,
                    Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                txtOutput.Text = json;
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"Error generating JSON: {ex.Message}";
            }
        }

        private void ApplySyntaxHighlighting()
        {
            if (cboOutputFormat.SelectedItem?.ToString() == "JSON")
            {
                ApplyJsonSyntaxHighlighting();
            }
            else
            {
                ApplyCSharpSyntaxHighlighting();
            }
        }

        private void ApplyJsonSyntaxHighlighting()
        {
            // Save current position
            int start = txtOutput.SelectionStart;

            // Apply JSON syntax highlighting
            string[] keywords = { "null", "true", "false" };

            foreach (string keyword in keywords)
            {
                int index = 0;
                while (index < txtOutput.Text.Length)
                {
                    index = txtOutput.Text.IndexOf(keyword, index, StringComparison.Ordinal);
                    if (index == -1) break;

                    txtOutput.Select(index, keyword.Length);
                    txtOutput.SelectionColor = Color.Blue;
                    txtOutput.SelectionBackColor = txtOutput.BackColor;

                    index += keyword.Length;
                }
            }

            // Restore position
            txtOutput.SelectionStart = start;
            txtOutput.SelectionLength = 0;
            txtOutput.SelectionColor = txtOutput.ForeColor;
        }

        private void ApplyCSharpSyntaxHighlighting()
        {
            int start = txtOutput.SelectionStart;

            // C# keywords
            string[] csharpKeywords = {
                "public", "private", "protected", "internal", "class", "struct",
                "enum", "interface", "void", "int", "string", "bool", "double",
                "float", "decimal", "object", "dynamic", "var", "using", "namespace",
                "static", "abstract", "sealed", "virtual", "override", "new", "this",
                "base", "get", "set", "return", "if", "else", "for", "foreach", "while",
                "do", "switch", "case", "default", "break", "continue", "try", "catch",
                "finally", "throw", "async", "await", "property", "variable", "method"
            };

            foreach (string keyword in csharpKeywords)
            {
                int index = 0;
                while (index < txtOutput.Text.Length)
                {
                    index = txtOutput.Text.IndexOf(keyword, index, StringComparison.Ordinal);
                    if (index == -1) break;

                    // Check if it's a whole word
                    if ((index == 0 || !char.IsLetterOrDigit(txtOutput.Text[index - 1])) &&
                        (index + keyword.Length == txtOutput.Text.Length ||
                         !char.IsLetterOrDigit(txtOutput.Text[index + keyword.Length])))
                    {
                        txtOutput.Select(index, keyword.Length);
                        txtOutput.SelectionColor = Color.Blue;
                        txtOutput.SelectionBackColor = txtOutput.BackColor;
                    }

                    index += keyword.Length;
                }
            }

            // Comments
            Regex commentRegex = new Regex(@"//.*$", RegexOptions.Multiline);
            foreach (Match match in commentRegex.Matches(txtOutput.Text))
            {
                txtOutput.Select(match.Index, match.Length);
                txtOutput.SelectionColor = Color.Green;
                txtOutput.SelectionBackColor = txtOutput.BackColor;
            }

            // Strings
            Regex stringRegex = new Regex(@"""(?:[^""\\]|\\.)*""");
            foreach (Match match in stringRegex.Matches(txtOutput.Text))
            {
                txtOutput.Select(match.Index, match.Length);
                txtOutput.SelectionColor = Color.DarkRed;
                txtOutput.SelectionBackColor = txtOutput.BackColor;
            }

            txtOutput.SelectionStart = start;
            txtOutput.SelectionLength = 0;
            txtOutput.SelectionColor = txtOutput.ForeColor;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (currentSolution == null)
            {
                // Use regular MessageBox instead of MaterialSnackBar if not available
                MessageBox.Show("No data to export", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                string format = cboOutputFormat.SelectedItem?.ToString();
                string extension = ".txt";
                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (format == "JSON")
                {
                    extension = ".json";
                    filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                }

                saveDialog.Filter = filter;
                saveDialog.FileName = $"{currentSolution.Name}_Structure{extension}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, txtOutput.Text);
                    MessageBox.Show($"Exported to {Path.GetFileName(saveDialog.FileName)}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void cboOutputFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentSolution != null)
            {
                UpdateOutputFormat();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                txtOutput.Clear();

                if (e.Node.Tag is CsFileData fileData)
                {
                    try
                    {
                        txtOutput.Text = File.ReadAllText(fileData.FilePath);
                        ApplyCSharpSyntaxHighlighting();
                    }
                    catch (Exception ex)
                    {
                        txtOutput.Text = $"Error reading file: {ex.Message}";
                    }
                }
                else if (e.Node.Tag is ClassData classData)
                {
                    DisplayClassDetails(classData);
                }
            }
        }

        private void DisplayClassDetails(ClassData classData)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{classData.AccessModifier} class {classData.Name}");
            sb.AppendLine("{");

            foreach (var prop in classData.Properties)
            {
                sb.AppendLine($"    {prop.AccessModifier} {prop.Type} {prop.Name} {{ get; set; }}");
            }

            foreach (var ctor in classData.Constructors)
            {
                string paramsStr = string.Join(", ", ctor.Parameters.Select(p => $"{p.Type} {p.Name}"));
                sb.AppendLine($"    {ctor.AccessModifier} {ctor.Name}({paramsStr})");
                sb.AppendLine("    {");
                sb.AppendLine("        // Constructor implementation");
                sb.AppendLine("    }");
            }

            foreach (var method in classData.Methods)
            {
                string paramsStr = string.Join(", ", method.Parameters.Select(p => $"{p.Type} {p.Name}"));
                sb.AppendLine($"    {method.AccessModifier} {method.ReturnType} {method.Name}({paramsStr})");
                sb.AppendLine("    {");
                sb.AppendLine("        // Method implementation");
                sb.AppendLine("    }");
            }

            sb.AppendLine("}");

            txtOutput.Text = sb.ToString();
            ApplyCSharpSyntaxHighlighting();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOutput.Text))
            {
                Clipboard.SetText(txtOutput.Text);
                MessageBox.Show("Copied to clipboard", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            txtOutput.Clear();
            currentSolution = null;
            lblStatus.Text = "Ready";
        }
    }
}