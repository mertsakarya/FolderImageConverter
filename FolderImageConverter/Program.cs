using System;
using System.Collections.Generic;
using System.IO;
using ImageResizer;
using NDesk.Options;

namespace FolderImageConverter
{
    public class FolderImageConverterOptions
    {
        public string InputPath { get; set; }
        public string Mask { get; set; }
        public string OutputPath { get; set; }
        public string Options { get; set; }
        public int Verbose { get; set; }
        public string Extension { get; set; }
    }

    public class Program
    {
        private readonly OptionSet _optionSet;
        private readonly FolderImageConverterOptions _options;

        private static void Main(string[] args) { var program = new Program(); program.Parse(args); }
        private void Parse(string[] args)
        {
            // Do these arguments the traditional way to maintain compatibility
            if (args.Length < 2)
                PrintUsageAndExit(-1);
            try {
                _optionSet.Parse(args);
                _options.Mask = _options.Mask ?? "*.*";
                _options.OutputPath = _options.OutputPath ?? Environment.CurrentDirectory;
                _options.Extension = (_options.Extension ?? "jpg").ToLowerInvariant();
                if(!(_options.Extension =="jpg" ||_options.Extension =="gif" ||_options.Extension =="png"))
                    throw new Exception("Extension must be jpg, gif or png");                
                if (_options.InputPath.Equals(_options.OutputPath)) 
                    throw new Exception("Input path cannot be same as output path.");
            } catch (Exception e) {
                PrintUsageAndExit(e);
            }
            Execute();
        }

        private void Execute()
        {
            var inFolder = _options.InputPath;
            var outFolder = _options.OutputPath ?? Environment.CurrentDirectory;
            var mask = _options.Mask ?? "*.*";
            try {
                NewFolder(outFolder);
                var list = new List<string>();
                GetFiles(list, inFolder, mask);
                foreach (var file in list) {
                    var path = file.Substring(inFolder.Length);
                    var outFile = outFolder + path;
                    var pos = outFile.LastIndexOf('.');
                    outFile = outFile.Substring(0, pos+1) + _options.Extension;
                    var fileNamePos = outFile.LastIndexOf("\\", StringComparison.Ordinal);
                    var dirName = outFile.Substring(0, fileNamePos);
                    NewFolder(dirName);
                    var result = BuildImage(file, outFile );
                    if(result != null)
                        Console.WriteLine(result);
                    if(_options.Verbose > 0)
                        Console.WriteLine(outFile);
                }
            } catch (Exception ex) {
                PrintUsageAndExit(ex);
            }
        }

        private void GetFiles(List<string> list, string folder, string mask)
        {
            try {
                foreach (var f in Directory.GetFiles(folder, mask)) {
                    list.Add(f);
                }
                foreach (var d in Directory.GetDirectories(folder)) {
                    GetFiles(list, d, mask);
                }
            }
            catch {}
        }

        private Program()
        {
            _options = new FolderImageConverterOptions();
            _optionSet = new OptionSet {
                { "i|input=", "Path for the folder to modify images.\r\n", value => _options.InputPath = value}, 
                { "o|output:", "Output folder.\r\n" + "Default: ["+Environment.CurrentDirectory+"]\r\n", value => _options.OutputPath = (String.IsNullOrEmpty(value)) ? Environment.CurrentDirectory : value}, 
                { "f|format:", "File format of output image.\r\n" + "Default: jpg, Possble values jpg|png|gif\r\n", value => _options.Extension =  value}, 
                { "m|mask:", "File mask for input folder.\r\nDefault: *.*", value => _options.Mask = (String.IsNullOrEmpty(value) ? "*.*": value)}, 
                { "s|settings=", "Query options are described in http://imageresizing.net/docs/reference \r\nDo not add format setting. Specify it with extension parameter on command line.\r\nExample: settings=\"width=320&height=240&quality=85&crop=\"",
                    value => _options.Options = value
                },
                { "v|verbose", "Verbose output.", value => ++_options.Verbose }, 
                { "h|?|help",   v => PrintUsageAndExit(0) },
            };
        }

        private void PrintUsageAndExit(Exception e)
        {
            Console.WriteLine(e.Message);
            PrintUsageAndExit(-1);
        }

        private void PrintUsageAndExit(int exitCode)
        {
            Console.WriteLine(@"
Convert Images in folder and sub folders
----------------------------------------
Copyright (C) 2008 - {0} - Mert Sakarya
----------------------------------------

Command line options:", DateTime.UtcNow.Year);

            _optionSet.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();

            Environment.Exit(exitCode);
        }

        public static void NewFolder(string s1)
        {
            var di = new DirectoryInfo(s1);
            if (di.Parent != null && !di.Exists)
                NewFolder(di.Parent.FullName);
            if (di.Exists) return;
            di.Create();
            di.Refresh();
        }

        public string BuildImage(string inFile, string outFile)
        {
            try {
                ImageBuilder.Current.Build(inFile, outFile, new ResizeSettings(_options.Options+"&format="+_options.Extension), false, false);
                return null;
            } catch (Exception ex) {
                return String.Format("{0} : {1}", inFile, ex.Message);
            }
        }
    }
}