using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using contentstack.model.generator.Model;
using Contentstack.Core;
using Contentstack.Core.Configuration;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace contentstack.model.generator
{
    [Command(Name = "contentstack.model.generator", FullName = "Contentstack Model Generator", Description = "Creates c# classes from a Contentstack content types.")]

    public class ModelGenerator
    {
        [Option(CommandOptionType.SingleValue, Description = "The Contentstack API key for the Content Delivery API")]
        [Required(ErrorMessage = "You must specify the Contentstack API key for the Content Delivery API")]
        public string ApiKey { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "d", LongName = "DeliveryToken", Description = "The Delivery token for the Content Delivery API")]
        [Required(ErrorMessage = "You must specify the Contentstack API key for the Content Delivery API")]
        public string AccessToken { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The Environment fetch the content type from")]
        [Required(ErrorMessage = "You must specify the Contentstack API key for the Content Delivery API")]
        public string Environment { get; set; }

        [Option(CommandOptionType.SingleOrNoValue, Description = "The Contentstack Host for the Content Delivery API")]
        public string Host { get; set; } = "api.contentstack.io";

        [Option(CommandOptionType.SingleValue, Description = "The namespace the classes should be created in")]
        public string Namespace { get; set; } = "ContentstackModels";

        [Option(CommandOptionType.NoValue, Description = "Automatically overwrite files that already exist")]
        public bool Force { get; }

        [Option(CommandOptionType.SingleValue, Description = "Path to the file or directory to create files in")]
        public string Path { get; }

        [VersionOption("0.1")]
        public bool Version { get; }

        private string _templateStart = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentstack.Core.Models;
";

        private List<Contenttype> _contentTypes;

        public async Task<int> OnExecute(CommandLineApplication app, IConsole console)
        {

            var options = new ContentstackOptions
            {
                ApiKey = ApiKey,
                AccessToken = AccessToken,
                Environment = Environment,
                Host = Host
            };
            var client = new ContentstackClient(options);

            Console.WriteLine($"Fetching content types from {ApiKey}");
            Console.ResetColor();
            try
            {
                var list = await client.GetContentTypes();
                console.WriteLine(list);
                var json = JsonConvert.SerializeObject(list);
                _contentTypes = JsonConvert.DeserializeObject<List<Contenttype>>(json);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("There was an error communicating with the Contentstack API: " + e.Message);
                Console.Error.WriteLine("Please verify that your api key, delivery token and environment are correct");
                Console.ResetColor();
                return Program.ERROR;
            }

            var path = "";
            Console.WriteLine($"Found {_contentTypes.Count} content types.");
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

            var dir = new DirectoryInfo(path);
            if (dir.Exists == false)
            {
                Console.WriteLine($"Path {path} does not exist and will be created.");
                dir.Create();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Generating files from content type");
            Console.ResetColor();

            foreach (var contentType in _contentTypes)
            {
                string fileName = GetFileName(contentType.Title);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"Adding File {fileName} to {dir.Name}.");

                var file = new FileInfo($"{dir.FullName}{System.IO.Path.DirectorySeparatorChar}{fileName}.cs");
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
                    }
                }

                using (var sw = file.CreateText())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(_templateStart);
                    sb.AppendLine($"namespace {Namespace}");
                    //start namespace
                    sb.AppendLine("{");

                    sb.AppendLine($"    public class {FormatClassName(contentType.Title)}");
                    //start class
                    sb.AppendLine("    {");

                    foreach (var field in contentType.schema)
                    {
                        if (field.DataType == "global_field") {
                            console.WriteLine(contentType.schema);
                        }
                        sb.AppendLine($"        public {GetDatatypeForField(field)} {FirstLetterToUpperCase(field.Uid)} {{ get; set; }}");
                    }

                    //end class
                    sb.AppendLine("    }");
                    //end namespace
                    sb.AppendLine("}");

                    sw.WriteLine(sb.ToString());
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Files successfully created!");
            Console.ResetColor();

            return Program.OK;
        }

        private Dictionary<T1, T2> Dictionary<T1, T2>()
        {
            throw new NotImplementedException();
        }

        private string GetDatatype(Field field)
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
                case "reference":
                    return GetDatatypeForContentType(field);
                case "global_field":
                    return GetDatatypeForContentType(field);
                default:
                    break;
            }
            Console.WriteLine($"Field Type {field.DataType}");
            return "object";
        }

        private string GetDatatypeForField(Field field)
        {
            string dataType = GetDatatype(field);
            return field.Fieldmetadata != null && field.Fieldmetadata.RefMultiple
                ? $"List<{ dataType }>"
                : (field.IsMultiple ? $"List<{ dataType }>" : dataType);
        }

        private string GetDatatypeForContentType(Field field)
        {
            if (field.Fieldmetadata.RefMultipleContentType == false && !field.ReferenceTo.GetType().IsArray)
            {
                string referenceTo = (string)(field.ReferenceTo);
                Contenttype contentType = _contentTypes.FirstOrDefault(c => c.Uid == referenceTo);
                if (contentType != null)
                {
                    return GetFileName(contentType.Title);
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

        private string GetFileName(string fileName)
        {
            return string.Join("-", FirstLetterToUpperCase(fileName).Replace(" ", "").Split(System.IO.Path.GetInvalidFileNameChars()));
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

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}
