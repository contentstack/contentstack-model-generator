using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using contentstack.CMA;
using contentstack.model.generator.Model;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace contentstack.model.generator
{
    [Command(Name = "contentstack.model.generator", FullName = "Contentstack Model Generator", Description = "Creates c# classes from a Contentstack content types.")]
    public class ModelGenerator
    {
        [Option(CommandOptionType.SingleValue, Description = "The Contentstack API key for the Content Delivery API")]
        [Required(ErrorMessage = "You must specify the Contentstack API key for the Content Delivery API")]
        public string ApiKey { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "d", LongName = "delivery-token", Description = "The Delivery token for the Content Delivery API")]
        [Required(ErrorMessage = "You must specify the Contentstack API key for the Content Delivery API")]
        public string DeliveryToken { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "e", LongName = "endpointlon", Description = "The Contentstack Host for the Content Delivery API")]
        public string Host { get; set; } = "api.contentstack.io";

        [Option(CommandOptionType.SingleValue, Description = "The namespace the classes should be created in")]
        public string Namespace { get; set; } = "ContentstackModels";

        [Option(CommandOptionType.NoValue, Description = "Automatically overwrite files that already exist")]
        public bool Force { get; }

        [Option(CommandOptionType.SingleValue, Description = "The Modular block Class Prefix.")]
        public string ModularBlockPrefix { get; } = "MB";

        [Option(CommandOptionType.SingleValue, Description = "The Modular block Class Prefix.")]
        public string GroupPrefix { get; } = "Group";

        [Option(CommandOptionType.SingleValue, Description = "Path to the file or directory to create files in")]
        public string Path { get; }

        [VersionOption("0.2.1")]
        public bool Version { get; }

        private string _templateStart = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentstack.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;";

        private List<Contenttype> _contentTypes;
        private Dictionary<string, List<Contenttype>> _modularBlocks = new Dictionary<string, List<Contenttype>>();

        public async Task<int> OnExecute(CommandLineApplication app, IConsole console)
        {

            var options = new ContentstackOptions
            {
                ApiKey = ApiKey,
                AccessToken = DeliveryToken,
                Host = Host
            };
            var client = new ContentstackClient(options);

            try
            {
                Console.WriteLine($"Fetching Content Types from {ApiKey}");
                var contentType = await client.GetContentTypes();
                var ContentTypeJson = JsonConvert.SerializeObject(contentType);
                _contentTypes = JsonConvert.DeserializeObject<List<Contenttype>>(ContentTypeJson);
                Console.WriteLine($"Found {_contentTypes.Count} content types.");

                Console.WriteLine($"Fetching Global Fields from {ApiKey}");
                var globalField = await client.GetGlobalFields();
                var globalFieldsJson = JsonConvert.SerializeObject(globalField);
                var globalFields = JsonConvert.DeserializeObject<List<Contenttype>>(globalFieldsJson);
                _contentTypes.AddRange(globalFields);
                Console.WriteLine($"Found {globalFields.Count} Global Fields.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("There was an error communicating with the Contentstack API: " + e.Message);
                Console.Error.WriteLine("Please verify that your api key, delivery token and environment are correct");
                return Program.ERROR;
            }
            Console.ResetColor();

            var path = "";
            if (string.IsNullOrEmpty(Path))
            {
                path = Directory.GetCurrentDirectory();
                Console.WriteLine($"No path specified, creating files in current working directory {path}");
            }
            else
            {
                Console.WriteLine($"Path specified. Files will be created at {Path}");
                path = Path;
            }
            path = $"{path}/Models";
            DirectoryInfo dir = CreateDirectory(path);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Generating files from content type");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            CreateLinkClass(Namespace, dir);
            CreateHelperClass(Namespace, dir);
            CreateStringHelperClass(Namespace, dir);
            foreach (var contentType in _contentTypes)
            {
                CreateFile(FormatClassName(contentType.Title), Namespace, contentType, dir, null, true);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Files successfully created!");
            Console.ResetColor();
            Console.WriteLine($"Opening {dir.FullName}");
            OpenFolderatPath(dir.FullName);
            return Program.OK;
        }

        private string GetDatatype(Field field, string contentTypeName)
        {
            switch (field.DataType)
            {
                case "text":
                    return "string";
                case "number":
                    return "double";
                case "isodate":
                    return "DateTime";
                case "file":
                    return "Asset";
                case "boolean":
                    return "bool";
                case "link":
                    return "ContentstackLink";
                case "reference":
                    return GetDatatypeForContentType(field);
                case "global_field":
                    return GetDatatypeForContentType(field);
                case "blocks":
                    return GetDataTypeForModularBlock(field, contentTypeName);
                case "group":
                    return GetDataTypeForGroup(field, contentTypeName);
                default:
                    Console.WriteLine(field.DataType);
                    break;
            }
            return "object";
        }

        private string GetDatatypeForField(Field field, string contentTypeName)
        {
            string dataType = GetDatatype(field, contentTypeName);
            return field.Fieldmetadata != null && field.Fieldmetadata.RefMultiple
                ? $"List<{ dataType }>"
                : (field.IsMultiple ? $"List<{ dataType }>" : dataType);
        }

        private string GetDatatypeForContentType(Field field)
        {
            Console.Write(field.ReferenceTo);
            Console.Write(field.ReferenceTo.GetType());
            if (field.Fieldmetadata.RefMultipleContentType == false && !field.ReferenceTo.GetType().IsArray)
            {
                string referenceTo = (string)(field.ReferenceTo);
                Contenttype contentType = _contentTypes.FirstOrDefault(c => c.Uid == referenceTo);
                if (contentType != null)
                {
                    return FormatClassName(contentType.Title);
                }
            }
            else if (field.ReferenceTo.GetType() == typeof(JArray))
            {
                JArray array = field.ReferenceTo as JArray;
                if (array.Count == 1)
                {
                    string referenceTo = (string)(array.First);
                    Contenttype contentType = _contentTypes.FirstOrDefault(c => c.Uid == referenceTo);
                    if (contentType != null)
                    {
                        return FormatClassName(contentType.Title);
                    }
                }
            }
            return "object";
        }

        private string GetDefaultValueForField(Field field)
        {
            if (field.Fieldmetadata.Defaultvalue != null && field.Fieldmetadata.Defaultvalue.ToString().Length > 0 && field.DataType != "link")
            {
                return $" = {field.Fieldmetadata.Defaultvalue};";
            }
            return "";
        }

        private string FormatClassName(string name)
        {
            return RemoveUnallowedCharacters(FirstLetterToUpperCase(name));
        }

        private string RemoveUnallowedCharacters(string s)
        {
            return Regex.Replace(s, @"[^A-Za-z0-9_]", "");
        }

        private string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            var value = Regex.Replace(s, @"[\d-]","").Replace("_", " ");
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value).Replace(" ", "");
        }
        private void OpenFolderatPath(string path)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private string GetDataTypeForModularBlock(Field field, string contentTypeName)
        {
            return $"{ModularBlockPrefix}{contentTypeName}{FormatClassName(field.DisplayName)}".Replace(" ","");
        }

        private string GetDataTypeForGroup(Field field, string contentTypeName)
        {
            return $"{GroupPrefix}{contentTypeName}{FormatClassName(field.DisplayName)}".Replace(" ", "");
        }

        private void CreateLinkClass(string NameSpace, DirectoryInfo directoryInfo)
        {
            // Create File for LinkClass
            string contentstackLinkClass = "ContentstackLink";
            var file = shouldCreateFile(contentstackLinkClass, directoryInfo);
            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    // Adding using at start of file
                    AddUsingDirectives(null, sb);

                    // Creating namespace 
                    AddNameSpace($"{NameSpace}.{directoryInfo.Name}", sb);

                    // Creating Class
                    AddClass(contentstackLinkClass, sb);

                    sb.AppendLine("         public string Title { get; set; }");
                    sb.AppendLine("         public string Href { get; set; }");

                    // End of namespace and class
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateGlobalFieldModular(string contentTypeName, string nameSpace, String ReferenceTo, DirectoryInfo directoryInfo, string extendsClass = null)
        {
            var globalField = _contentTypes.Find(contentType =>
            {
                return contentType.Uid == ReferenceTo;
            });

            var usingDirectiveList = new List<string>();
            var fieldsBlock = globalField.Schema.FindAll(field =>
            {
                return field.DataType == "blocks";
            });


            if (fieldsBlock.Count > 0)
            {
                usingDirectiveList.Add($"using {Namespace}.Models.{FormatClassName(globalField.Title)}{FirstLetterToUpperCase("blocks")};");
            }

            var fieldsGroup = globalField.Schema.FindAll(field =>
            {
                return field.DataType == "group";
            });


            if (fieldsGroup.Count > 0)
            {
                usingDirectiveList.Add($"using {Namespace}.Models.{FormatClassName(globalField.Title)}{FirstLetterToUpperCase("group")};");
            }

            // Create File for ContentType
            var file = shouldCreateFile(contentTypeName, directoryInfo);
            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    // Adding using at start of file
                    AddUsingDirectives(usingDirectiveList, sb);

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);

                    // Creating Class
                    AddClass(extendsClass != null ? $"{contentTypeName} : {extendsClass}" : contentTypeName, sb);

                    //Adding Params to contentType
                    AddParams(globalField.Title, globalField.Schema, sb);

                    // End of namespace and class
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateFile(string contentTypeName, string nameSpace, Contenttype contentType, DirectoryInfo directoryInfo, string extendsClass = null, bool addConst = false)
        {

            Console.WriteLine($"Extracting Modular Blocks in {contentTypeName}.");

            // Get modular Block within ContentType
            var usingDirectiveList = new List<string>();
            string modularUsingDirective = CreateModularBlocks(nameSpace, contentTypeName, contentType.Schema, directoryInfo);
            if (modularUsingDirective != null)
            {
                usingDirectiveList.Add(modularUsingDirective);
            }

            Console.WriteLine($"Extracting Groups in {contentTypeName}.");

            string grpupUsingDirective = CreateGroup(nameSpace, contentTypeName, contentType.Schema, directoryInfo);
            usingDirectiveList.Add(grpupUsingDirective);

            // Create File for ContentType
            var file = shouldCreateFile(contentTypeName, directoryInfo);
            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    // Adding using at start of file
                    AddUsingDirectives(usingDirectiveList, sb);

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);

                    // Creating Class
                    AddClass(extendsClass != null ? $"{contentTypeName} : {extendsClass}" : contentTypeName, sb);

                    if (addConst)
                    {
                        // Add Const
                        sb.AppendLine($"        public const string ContentType = \"{contentType.Uid}\";");
                        sb.AppendLine($"        public string Uid {{ get; set; }}");
                    }

                    //Adding Params to contentType
                    AddParams(contentTypeName, contentType.Schema, sb);

                    // End of namespace and class
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private FileInfo shouldCreateFile(string fileName, DirectoryInfo directoryInfo)
        {
            var file = new FileInfo($"{directoryInfo.FullName}{System.IO.Path.DirectorySeparatorChar}{fileName}.cs");
            if (file.Exists && !Force)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                var prompt = Prompt.GetYesNo($"The folder already contains a file with the name {file.Name}. Do you want to overwrite it?", true);
                Console.ResetColor();
                if (prompt == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Skipping {file.Name}");
                    Console.ResetColor();
                    return null;
                }
            }
            Console.WriteLine($"Adding File {fileName} to {directoryInfo.FullName}.");
            return file;
        }

        private DirectoryInfo CreateDirectory(string path)
        {
            var dir = new DirectoryInfo(path);
            if (dir.Exists == false)
            {
                Console.WriteLine($"Path {path} does not exist and will be created.");
                dir.Create();
            }
            return dir;
        }

        private string CreateModularBlocks(string nameSpace, string contentTypeName, List<Field> fields, DirectoryInfo directoryInfo)
        {

            var tuple = ExtractFieldType("blocks", contentTypeName, fields, nameSpace, directoryInfo);
            string modularUsingDirective = tuple.Item1;
            string modularNameSpace = tuple.Item2;
            DirectoryInfo directory = tuple.Item3;
            var modularBlockFields = tuple.Item4;

            foreach (var mb in modularBlockFields)
            {
                var blockTypes = new Dictionary<string, string>();
                var modularBlockMainClass = GetDataTypeForModularBlock(mb, contentTypeName);

                foreach (var contentT in mb.Blocks)
                {
                    
                    string className = $"{ModularBlockPrefix}{contentTypeName}{FormatClassName(contentT.Title)}";
                    blockTypes[contentT.Uid] = className;
                    if (contentT.ReferenceTo != null)
                    {
                        CreateGlobalFieldModular(className, modularNameSpace, contentT.ReferenceTo, directory, modularBlockMainClass);
                    }
                    else
                    {
                        CreateFile(className, modularNameSpace, contentT, directory, modularBlockMainClass);
                    }
                }

                if (blockTypes.Count > 0)
                {
                    var enumName = $"{modularBlockMainClass}Enum";
                    CreateModularBlockEnum(enumName, modularNameSpace, blockTypes, directory);
                    CreateModularBlockClass(modularBlockMainClass, modularNameSpace, enumName, directory);
                }
                CreateModularBlockConverter(modularNameSpace, modularBlockMainClass, blockTypes, directory);
            }
            return modularUsingDirective;
        }

        private string CreateGroup(string nameSpace, string contentTypeName, List<Field> fields, DirectoryInfo directoryInfo)
        {
            var tuple = ExtractFieldType("group", contentTypeName, fields, nameSpace, directoryInfo);
            string groupusingDirective = tuple.Item1;
            string groupNameSpace = tuple.Item2;
            DirectoryInfo directory = tuple.Item3;
            var groupFields = tuple.Item4;

            foreach (var group in groupFields)
            {
                var groupNameClass = GetDataTypeForGroup(group, contentTypeName);
                CreateGroupClass(groupNameClass, groupNameSpace, group, directory);
            }
            return groupusingDirective;
        }

        private Tuple<string, string, DirectoryInfo, List<Field>> ExtractFieldType(string type, string contentTypeName, List<Field> Schema, string nameSpace, DirectoryInfo directoryInfo)
        {
            string usingDirective = null;
            DirectoryInfo directory = null;
            string NameSpace = null;

            var fields = Schema.FindAll(field =>
            {
                return field.DataType == type;
            });


            if (fields.Count > 0)
            {
                directory = CreateDirectory($"{directoryInfo.FullName}/{contentTypeName}{FirstLetterToUpperCase(type)}");
                NameSpace = $"{nameSpace}.{directoryInfo.Name}";
                usingDirective = $"using {NameSpace}.{directory.Name};";
            }
            return new Tuple<string, string, DirectoryInfo, List<Field>>(usingDirective, NameSpace, directory, fields);
        }

        private void AddParams(string contentType, List<Field> schema, in StringBuilder sb)
        {
            foreach (var field in schema)
            {
                if (field.Uid.Contains("_"))
                {
                    sb.AppendLine($"        [JsonProperty(propertyName: \"{field.Uid}\")]");
                }
                if (field.DataType == "text")
                {
                    if (field.Fieldmetadata.isMarkdown)
                    {
                        sb.AppendLine($"        public {GetDatatypeForField(field, contentType)} {FirstLetterToUpperCase(field.Uid)} {{");

                        sb.AppendLine($"            set");
                        sb.AppendLine($"            {{");
                        sb.AppendLine($"                this.{FirstLetterToUpperCase(field.Uid)}Store = value;");
                        sb.AppendLine($"            }}");

                        sb.AppendLine($"            get");
                        sb.AppendLine($"            {{");
                        sb.AppendLine($"                return this.{FirstLetterToUpperCase(field.Uid)}Store.{(field.IsMultiple ? "ToListHtml()" : "ToHtml()" )};");
                        sb.AppendLine($"            }}");

                        sb.AppendLine($"        }}");
                        sb.AppendLine($"        private {GetDatatypeForField(field, contentType)} {FirstLetterToUpperCase(field.Uid)}Store;");
                        continue;
                    }
                }
                sb.AppendLine($"        public {GetDatatypeForField(field, contentType)} {FirstLetterToUpperCase(field.Uid)} {{ get; set; }}"); 
            }
        }

        private void AddEnd(in StringBuilder sb)
        {
            //end class/ enum
            sb.AppendLine("    }");
            //end namespace
            sb.AppendLine("}");
        }

        private void AddUsingDirectives(List<string> usingDirective, in StringBuilder sb)
        {
            sb.AppendLine(_templateStart);
            if (usingDirective != null)
            {
                foreach (var directive in usingDirective)
                {
                    sb.AppendLine(directive);
                }
            }
            sb.AppendLine($"");
        }

        private void AddNameSpace(string nameSpace, in StringBuilder sb)
        {
            sb.AppendLine($"namespace {nameSpace}");
            sb.AppendLine("{");
        }

        private void AddClass(string contentTypeName, in StringBuilder sb)
        {
            //start class
            sb.AppendLine($"    public partial class {contentTypeName}");
            sb.AppendLine("    {");
        }
        private void AddStaticClass(string contentTypeName, in StringBuilder sb)
        {
            //start class
            sb.AppendLine($"    public static class {contentTypeName}");
            sb.AppendLine("    {");
        }
        private void AddEnum(string enumName, in StringBuilder sb)
        {
            //start class
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");
        }

        private void CreateGroupClass(string groupName, string nameSpace, Field field, DirectoryInfo directoryInfo)
        {
            Console.WriteLine($"Extracting Modular Blocks in {groupName}.");

            // Get modular Block within Group
            var usingDirectiveList = new List<string>();
            string modularUsingDirective = CreateModularBlocks(nameSpace, groupName, field.Schema, directoryInfo);
            if (modularUsingDirective != null)
            {
                usingDirectiveList.Add(modularUsingDirective);
            }

            Console.WriteLine($"Extracting Groups in {groupName}.");
            // Get Group within Group
            string grpupUsingDirective = CreateGroup(nameSpace, groupName, field.Schema, directoryInfo);
            usingDirectiveList.Add(grpupUsingDirective);

            FileInfo file = shouldCreateFile(groupName, directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();

                    // Adding using at start of file
                    AddUsingDirectives(usingDirectiveList, sb);

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);

                    AddClass(groupName, sb);

                    //Adding Params to contentType
                    AddParams(groupName, field.Schema, sb);


                    // End of namespace and Enum
                    AddEnd(sb);
                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }
        private void CreateModularBlockClass(string modularBlockClassName, string nameSpace, string enumName, DirectoryInfo directoryInfo)
        {
            FileInfo file = shouldCreateFile(modularBlockClassName, directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);

                    AddClass(modularBlockClassName, sb);

                    sb.AppendLine($"        private {enumName} blockType;");
                    sb.AppendLine($"        public {enumName} BlockType {{ get => blockType; set => blockType = value; }}");

                    // End of namespace and Enum
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateModularBlockEnum(string enumName, string nameSpace, Dictionary<string, string> blockTypes, DirectoryInfo directoryInfo)
        {
            FileInfo file = shouldCreateFile(enumName, directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("using System.ComponentModel;");

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);
                    // Creating Enum
                    AddEnum(enumName, sb);

                    var count = blockTypes.Count;
                    foreach (var blocks in blockTypes)
                    {
                        count--;
                        var appendEnd = count == 0 ? "" : ",";
                        sb.AppendLine($"        [DisplayName(displayName: \"{blocks.Key}\")]");
                        sb.AppendLine($"        {FirstLetterToUpperCase(blocks.Key)}{appendEnd}");
                    }

                    // End of namespace and Enum
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateHelperClass(string nameSpace, DirectoryInfo directoryInfo)
        {
            string className = "ContentstackHelper";
            FileInfo file = shouldCreateFile(className, directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(_templateStart);
                    sb.AppendLine("using System.ComponentModel;");
                    sb.AppendLine("using System.Reflection;");

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);
                    // Creating Enum
                    AddClass(className, sb);

                    sb.AppendLine(@"       public static string GetDescription(Enum en)");
                    sb.AppendLine("        {");
                    sb.AppendLine("             Type type = en.GetType();");
                    sb.AppendLine("             MemberInfo[] memInfo = type.GetMember(en.ToString());");
                    sb.AppendLine("             if (memInfo != null && memInfo.Length > 0)");
                    sb.AppendLine("             {");
                    sb.AppendLine("                 object[] attrs = memInfo[0].GetCustomAttributes(typeof(DisplayNameAttribute),false);");
                    sb.AppendLine("                 if (attrs != null && attrs.Length > 0)");
                    sb.AppendLine("                 return ((DisplayNameAttribute)attrs[0]).DisplayName;");
                    sb.AppendLine("             }");
                    sb.AppendLine("             return en.ToString();");
                    sb.AppendLine("        }");


                    sb.AppendLine("        public static bool FieldExists(string fieldName, JObject jObject)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            return jObject[fieldName] != null;");
                    sb.AppendLine("        }");
                    // End of namespace and Enum
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateStringHelperClass(string nameSpace, DirectoryInfo directoryInfo)
        {
            string className = "ContentstackStringExtension";
            FileInfo file = shouldCreateFile(className, directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("using Markdig;");
                    sb.AppendLine(_templateStart);
                    sb.AppendLine("using System.ComponentModel;");
                    sb.AppendLine("using System.Reflection;");

                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);
                    // Creating Enum
                    AddStaticClass(className, sb);

                    sb.AppendLine();
                    sb.AppendLine(@"       public static string ToHtml(this String str)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            if (str != null)");
                    sb.AppendLine("            {");
                    sb.AppendLine("                 var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();");
                    sb.AppendLine("                 return Markdown.ToHtml(str, pipeline);");
                    sb.AppendLine("            }");
                    sb.AppendLine("            return string.Empty;");
                    sb.AppendLine("        }");

                    sb.AppendLine("        public static List<string> ToListHtml(this List<string> str)");
                    sb.AppendLine("        {");
                    sb.AppendLine("            List<string> result = new List<string>();");
                    sb.AppendLine("            foreach (var value in str)");
                    sb.AppendLine("            {");
                    sb.AppendLine("                 result.Add(value.ToHtml());");
                    sb.AppendLine("            }");
                    sb.AppendLine("            return result;");
                    sb.AppendLine("        }");

                    // End of namespace and Enum
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }

        private void CreateModularBlockConverter(string nameSpace, string className, Dictionary<string, string> blockTypes, DirectoryInfo directoryInfo)
        {
            FileInfo file = shouldCreateFile($"{className}Converter", directoryInfo);

            if (file != null)
            {
                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();

                    sb.AppendLine("using System;");
                    sb.AppendLine("using Newtonsoft.Json;");
                    sb.AppendLine("using System.Reflection;");
                    sb.AppendLine("using Newtonsoft.Json.Linq;");
                    sb.AppendLine("using System.ComponentModel;");
                    
                    // Creating namespace 
                    AddNameSpace($"{nameSpace}.{directoryInfo.Name}", sb);
                    // Creating Enum
                    AddClass($"{className}Converter : JsonConverter<{className}>", sb);

                    sb.AppendLine($"        protected {className} Create(Type objectType, JObject jObject)");
                    sb.AppendLine("        {");
                    foreach (var blocks in blockTypes)
                    {
                        string blockTypeName = FirstLetterToUpperCase(blocks.Key);
                        string blockClass = blocks.Value;
                        sb.AppendLine($"            if (ContentstackHelper.FieldExists(ContentstackHelper.GetDescription({className}Enum.{blockTypeName}), jObject))");
                        sb.AppendLine("             {");
                        sb.AppendLine($"                 {blockClass} block = new {blockClass}();");
                        sb.AppendLine($"                 block.BlockType = {className}Enum.{blockTypeName};");
                        sb.AppendLine($"                 return block;");
                        sb.AppendLine("             }");
                    }
                    sb.AppendLine($"        return new {className}();");
                    sb.AppendLine("        }");

                    sb.AppendLine($"        public override {className} ReadJson(JsonReader reader, Type objectType, {className} existingValue, bool hasExistingValue, JsonSerializer serializer)");
                    sb.AppendLine("        {");
                    sb.AppendLine("             JObject jObject = JObject.Load(reader);");
                    sb.AppendLine($"             {className} target = Create(objectType, jObject);");
                    sb.AppendLine("             serializer.Populate(jObject.GetValue(ContentstackHelper.GetDescription(target.BlockType)).CreateReader(), target);");
                    sb.AppendLine("             return target;");
                    sb.AppendLine("         }");

                    sb.AppendLine($"        public override void WriteJson(JsonWriter writer, {className} value, JsonSerializer serializer)");
                    sb.AppendLine("        {");
                    sb.AppendLine("        }");

                    // End of namespace and Enum
                    AddEnd(sb);

                    // write to file
                    sw.WriteLine(sb.ToString());
                }
            }
        }
    }
}
