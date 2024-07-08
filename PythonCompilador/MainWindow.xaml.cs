using System; // Importa el espacio de nombres System y métodos básicos de .NET.
using System.Collections.Generic; // proporciona clases genéricas como List<T>.
using System.IO; //proporciona clases para trabajar con archivos y flujos de datos.
using System.Text; // La clases para trabajar con codificaciones y manipulaciones de texto.
using System.Text.RegularExpressions; //proporciona clases para trabajar con expresiones regulares.
using System.Windows; // La clases para crear aplicaciones de Windows Presentation Foundation (WPF).
using IronPython.Hosting; // proporciona clases para hospedar y ejecutar scripts de IronPython en aplicaciones .NET.
using Microsoft.Scripting.Hosting; //proporciona clases para el motor de scripting y el entorno de ejecución.

namespace PythonCompilador
{
    public class SyntaxAnalyzer
    {
        private ScriptEngine engine; // Instancia del motor de scripting de Python
        private ScriptScope scope; // Ámbito de ejecución para el código Python
        private StringBuilder outputBuilder; // StringBuilder para capturar la salida del código Python
        private List<(string language, Regex pattern)> languagePatterns; // Lista de patrones de sintaxis

        public SyntaxAnalyzer()
        {
            // Crear una instancia del motor de scripting de Python
            engine = Python.CreateEngine();
            // Crear un ámbito de ejecución para el código Python
            scope = engine.CreateScope();
            // Inicializar el StringBuilder para capturar la salida del código Python
            outputBuilder = new StringBuilder();

            
            var outputStream = new StringWriterStream(outputBuilder);// Crea un nuevo StringWriterStream pasando el StringBuilder.
            engine.Runtime.IO.SetOutput(outputStream, Encoding.UTF8);// Redirigir la salida de Python a nuestro StringBuilder personalizado

            // Inicializar la lista de patrones de sintaxis
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
                ("Python", new Regex(@"^yield\s+.*", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bFalse\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bNone\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bTrue\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\band\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bas\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\basync\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bawait\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bin\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bis\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\blambda\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bnot\b", RegexOptions.IgnoreCase)),
                ("Python", new Regex(@"\bor\b", RegexOptions.IgnoreCase))
            };
        }

        public string Analyze(string code)
        {
            foreach (var pattern in languagePatterns)
            {
                if (pattern.pattern.IsMatch(code))
                {
                    try
                    {
                        // Compilar el código Python para detectar errores de sintaxis
                        var source = engine.CreateScriptSourceFromString(code);
                        source.Compile();
                        return $"Compilación exitosa.\nLenguaje detectado: {pattern.language}\n\nCódigo ingresado:\n{code}";
                    }
                    catch (Exception ex)
                    {
                        // Capturar y devolver cualquier error de compilación
                        return $"Error de compilación:\n{ex.Message}\n\nCódigo ingresado:\n{code}";
                    }
                }
            }

            return $"Error de compilación. No se detectó sintaxis de Python.\n\nCódigo ingresado:\n{code}";
        }

        public string ExecutePythonCode(string code)
        {
            try
            {
                // Limpiar el StringBuilder antes de ejecutar el código
                outputBuilder.Clear();

                // Ejecutar el código Python
                var source = engine.CreateScriptSourceFromString(code);
                source.Execute(scope);

                // Retornar el resultado de la ejecución
                return $"Ejecución exitosa.\n\nResultado de la ejecución:\n{outputBuilder.ToString()}";
            }
            catch (Exception ex)
            {
                // Capturar y devolver cualquier error durante la ejecución
                return $"Error durante la ejecución del código: {ex.Message}";
            }
        }

        // Clase personalizada que convierte un StringWriter a un Stream
        private class StringWriterStream : Stream
        {
            private StringBuilder _stringBuilder; // StringBuilder donde se almacenará la salida de Python

            public StringWriterStream(StringBuilder stringBuilder)
            {
                _stringBuilder = stringBuilder;
            }

            public override bool CanRead => false; // No se puede leer desde este Stream
            public override bool CanSeek => false; // No se puede buscar en este Stream
            public override bool CanWrite => true; // Se puede escribir en este Stream
            public override long Length => 0; // Longitud del Stream, no aplicable aquí

            public override long Position
            {
                get => 0;
                set => throw new NotSupportedException(); // No se admite la modificación de la posición
            }

            public override void Flush()
            {
                // No hay necesidad de hacer nada aquí
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException(); // No se admite la lectura
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException(); // No se admite la búsqueda
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException(); // No se admite la modificación de la longitud
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                // Convertir los bytes a cadena y añadirla al StringBuilder
                var text = Encoding.UTF8.GetString(buffer, offset, count);
                _stringBuilder.Append(text);
            }
        }
    }

    public partial class MainWindow : Window
    {
        private SyntaxAnalyzer analyzer; // Instancia de la clase SyntaxAnalyzer

        public MainWindow()
        {
            InitializeComponent(); // Inicializar los componentes de la interfaz de usuario
            analyzer = new SyntaxAnalyzer(); // Crear una instancia de SyntaxAnalyzer
        }

        private void OnAnalyzeClick(object sender, RoutedEventArgs e)
        {
            string code = CodeTextBox.Text; // Obtener el código ingresado por el usuario

            if (!string.IsNullOrWhiteSpace(code))
            {
                // Analizar el código Python ingresado
                string result = analyzer.Analyze(code);
                ResultTextBlock.Text = result;

                if (result.Contains("Compilación exitosa"))
                {
                    // Ejecutar el código Python si la compilación fue exitosa
                    string executionResult = analyzer.ExecutePythonCode(code);
                    ResultTextBlock.Text += $"\n\n{executionResult}";
                }
            }
            else
            {
                // Mostrar un mensaje si el cuadro de texto está vacío
                MessageBox.Show("Por favor ingresa el código a analizar.");
            }
        }

        private void OnClearScreenClick(object sender, RoutedEventArgs e)
        {
            // Limpiar el cuadro de texto y el bloque de resultados
            CodeTextBox.Text = string.Empty;
            ResultTextBlock.Text = string.Empty;
        }
    }
}
