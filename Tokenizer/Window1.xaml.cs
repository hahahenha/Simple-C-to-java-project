using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.IO;

namespace Tokenizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void openClick(object sender, RoutedEventArgs e)            //打开java文件至codeText中显示
        {
            System.Windows.Forms.OpenFileDialog file = new OpenFileDialog();
            file.Filter = "所有文件|*.*|java文件|*.java";
            file.FilterIndex = 2;
            file.RestoreDirectory = true;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader sr = File.OpenText(file.FileName);
                SourceFile source = new SourceFile();
                while (sr.EndOfStream != true)
                {
                    source.insert(sr.ReadToEnd());
                }
                ColorSyntaxVisitor visitor = new ColorSyntaxVisitor(codeText);
                source.Accept(visitor);
            }
        }

        private void changeClick(object sender, RoutedEventArgs e)          //将java文件映射为c#文件
        {
            TextRange textRange = new TextRange(codeText.Document.ContentStart, codeText.Document.ContentEnd);
            string text = textRange.Text;
            Mapping mapping = new Mapping();
            ChangeFile change = new ChangeFile();
            change.insert(mapping.TextChange(text));
            ColorSyntaxVisitor visitor = new ColorSyntaxVisitor(changeText);
            change.Accept(visitor);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog file = new SaveFileDialog();
            file.Filter = "所有文件|*.*|C#文件|*.cs";
            file.FilterIndex = 2;
            file.RestoreDirectory = true;
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = File.AppendText(file.FileName);
                TextRange textRange = new TextRange(changeText.Document.ContentStart, changeText.Document.ContentEnd);
                sw.Write(textRange.Text);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
