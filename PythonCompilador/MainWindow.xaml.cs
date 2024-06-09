using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace PythonCompilador
{
    public partial class MainWindow : Window
    {
        private SyntaxAnalyzer analyzer;

        public MainWindow()
        {
            InitializeComponent();
            analyzer = new SyntaxAnalyzer();
        }

        private void OnAnalyzeClick(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text;

            if (!string.IsNullOrWhiteSpace(code))
            {
                string result = analyzer.Analyze(code);
                ResultTextBlock.Text = result;
            }
            else
            {
                MessageBox.Show("Por favor ingresa el código a analizar.");
            }
        }

        private void OnClearScreenClick(object sender, RoutedEventArgs e)
        {
            CodeTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;
        }
    }

    public class SyntaxAnalyzer
    {
        private List<(string language, Regex pattern)> languagePatterns;

        public SyntaxAnalyzer()
        {
            languagePatterns = new List<(string, Regex)>
            {
                ("Python", new Regex(@"^def\s+\w+\s*\(.*\)\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^if\s+.*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^elif\s+.*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^else\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^for\s+\w+\s+in\s+\w+\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^while\s+.*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^try\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^except\s+.*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^finally\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^class\s+\w+\s*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^import\s+\w+", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^from\s+\w+\s+import\s+\w+", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^return\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^print\s*\(.*\)", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^assert\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^break\s*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^continue\s*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^del\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^global\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^nonlocal\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^pass\s*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^raise\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^with\s+.*:", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"^yield\s+.*", RegexOptions.IgnoreCase))
            };
        }

        public string Analyze(string code)
        {
            foreach (var pattern in languagePatterns)
            {
                if (pattern.pattern.IsMatch(code))
                {
                    return $"Compilación exitosa.\nLenguaje detectado: {pattern.language}\n\nCódigo ingresado:\n{code}";
                }
            }

            return $"Error de compilación. No se detectó sintaxis de Python.\n\nCódigo ingresado:\n{code}";
        }
    }
}
