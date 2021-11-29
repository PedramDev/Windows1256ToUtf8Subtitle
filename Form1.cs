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

namespace PersianCoder
{
    public partial class Form1 : Form
    {
        private IUtf8Checker utf8Checker;
        public Form1()
        {
            InitializeComponent();
            utf8Checker = new Utf8Checker();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = false;
        }
        public void FileToFa(string path)
        {
            var isUtf8 = utf8Checker.Check(path);

            if (!isUtf8)
            {
                var sourceBytes = File.ReadAllBytes(path);

                string s = Encoding.GetEncoding(1256).GetString(sourceBytes);
                byte[] Utf8Data = Encoding.UTF8.GetBytes(s);

                File.WriteAllBytes(path, Utf8Data);
            }
        }

        private List<string> DirsInDepth(List<string> dirs, int? maxDepth)
        {
            var allDirs = new List<string>();

            if (maxDepth <= 0 || maxDepth == null)
            {
                foreach (var dir in dirs)
                {
                    var currentRoot = Directory.GetDirectories(dir).ToList();
                    var inDepth = DirsInDepth(currentRoot, --maxDepth);

                    allDirs.AddRange(inDepth);
                    allDirs.AddRange(currentRoot);
                }
            }

            return allDirs;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            int? maxDeth = null;
            if (checkBox1.Checked)
            {
                maxDeth = (int)numericUpDown1.Value;
            }

            var filepaths = new List<string>();
            foreach (var s in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                if (Directory.Exists(s))
                {
                    foreach (var item in DirsInDepth(new List<string>() { s }, maxDeth))
                    {
                        filepaths.AddRange(Directory.GetFiles(item));
                    }
                }
                else
                {
                    filepaths.Add(s);
                }
            }


            foreach (var item in filepaths)
            {
                var allowedExt = new string[] { ".srt", ".txt", ".sub" };
                var ext = Path.GetExtension(item);

                if (allowedExt.Contains(ext))
                {
                    FileToFa(item);
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragOver(object sender, DragEventArgs e)
        {

        }

        private void Form1_DragLeave(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = checkBox1.Checked;
        }
    }
}
