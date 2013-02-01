using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageResizer;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        private class FI
        {
            public FI(string inFile, string outFile)
            {
                InFile = inFile;
                OutFile = outFile;
            }
            public string InFile { get; private set; }
            public string OutFile { get; private set; }
            public override string ToString()
            {
                return OutFile != null ? Path.GetFileName(OutFile) : "NULL";
            }
        }

        public Form1()
        {
            InitializeComponent();
            textBox1.Text =  @"P:\Share\ALL";
            textBox2.Text = @"P:\Share\ALL2";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var inFolder = textBox1.Text;
            var outFolder = textBox2.Text;
            NewFolder(outFolder);
            var li = new List<FI>();
            int i = 0;
            foreach (var file in Directory.EnumerateFiles(inFolder, "*.*", SearchOption.AllDirectories)) {
                var path = file.Substring(inFolder.Length);
                var outFile = outFolder + path;
                var fileNamePos = outFile.LastIndexOf("\\", StringComparison.Ordinal);
                var dirName = outFile.Substring(0, fileNamePos);
                NewFolder(dirName);
                    BuildImage(file, outFile);
                    li.Add(new FI(file, outFile));
                i++;
            }
            listBox1.Items.AddRange(li.ToArray());
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

        public static string BuildImage(string inFile, string outFile, int quality = 85)
        {

            var format = new PhotoFormat { Width = 320, Height = 240, Format = "jpg", Quality = quality }.ToString();
            try {
                var result = ImageBuilder.Current.Build(inFile, outFile, new ResizeSettings(format), false, false);
                return "";
            } catch (Exception ex) {
                return String.Format("{0} : {1}", inFile, ex.Message);
            }
        }


        public class PhotoFormat
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int MaxWidth { get; set; }
            public int MaxHeight { get; set; }
            public int Quality { get; set; }
            public string Format { get; set; }
            public string Crop { get; set; }

            public override string ToString()
            {
                var list = new List<string>();
                if (Width > 0) list.Add("width=" + Width);
                if (Height > 0) list.Add("height=" + Height);
                if (MaxWidth > 0) list.Add("maxwidth=" + MaxWidth);
                if (MaxHeight > 0) list.Add("maxheight=" + MaxHeight);
                if (Quality > 0) list.Add("quality=" + Quality);
                if (!String.IsNullOrWhiteSpace("Format")) list.Add("format=" + Format);
                if (!String.IsNullOrWhiteSpace("Crop")) list.Add("crop=" + Crop);
                return String.Join("&", list);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            var fi = listBox1.SelectedItem as FI;
            if (fi != null) {
                pictureBox1.ImageLocation = fi.InFile;
                pictureBox2.ImageLocation = fi.OutFile;
                label1.Text = String.Format("{0} {1}KB / {2}KB", fi.InFile, (new FileInfo(fi.InFile)).Length / 1024, (new FileInfo(fi.OutFile)).Length / 1024);
            }
        }

    }
}
