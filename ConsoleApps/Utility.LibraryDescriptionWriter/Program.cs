using ProjectFileEdit;
using System;
using System.Linq;
using System.Reflection;

namespace UtilityEnum.App
{
    internal class Program
    {

        private static void Main(string[] args)
        {
            string assemblyPath = string.Empty;
            if (args == null)
            {
                Console.WriteLine("Provide assembly path");
                assemblyPath = cleanPath(Console.ReadLine());
            }
            if (args.Length == 0)
            {
                Console.WriteLine("Provide assembly path");
                assemblyPath = cleanPath(Console.ReadLine());
            }
            else if (args.Length > 1)
            {

            }
            else
            {
                assemblyPath = cleanPath(args[0]);
            }


            if (File.Exists(assemblyPath) == false)
            {
                Console.WriteLine("File not found: " + args[0]);
                Console.ReadLine();
            }

            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            //Assembly assembly = Assembly.GetAssembly(typeof(YesNo));

            string description = string.Empty;
            if (assembly.GetName().Name == "Utility.Enums")
            {
                description = DescriptionCreator.Create(
                    assembly,
                    DescriptionModifier.EnumPredicate(),
                    appendage: string.Empty);
            }
            Console.WriteLine(description);
            Console.ReadLine();

            //var lineSeparated = DescriptionCreator.CreateForMarkdown(
            //    assembly,
            //    DescriptionModifier.EnumPredicate(),
            //    appendage: " enums." + Environment.NewLine + AppendDescription);
            //Console.WriteLine(lineSeparated);

            //Console.ReadLine();

            void write(string description)
            {
                var dir = new System.IO.FileInfo(assembly.Location);
                var parent = dir.Directory.Parent.Parent.Parent.Parent;
                var name = System.IO.Path.ChangeExtension(dir.Name, "csproj");
                var file = parent.GetFiles(name, System.IO.SearchOption.AllDirectories).Single();

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("modifying...");
                try
                {
                    DescriptionModifier.ModifyDescription(description, file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("completed");
                Console.ReadLine();
            }
        }

        static string cleanPath(string rawInput)=> rawInput
    .Trim('"')                    // Remove outer quotes
    .Replace("\\\\", "\\")        // Fix double-escaped backslashes
    .Replace("\\\"", "\"");       // Fix escaped quotes
    }
}