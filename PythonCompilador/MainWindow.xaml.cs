using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Windows;

namespace PythonCompilador
{
    public class AnalizadorSintaxis
    {
        private ScriptEngine motor;
        private ScriptScope alcance;
        private StringBuilder constructorSalida;
        private List<(string, Regex)> patronesLenguaje;

        public AnalizadorSintaxis()
        {
            motor = Python.CreateEngine();
            alcance = motor.CreateScope();
            constructorSalida = new StringBuilder();

            var flujoSalida = new StreamEscritorString(constructorSalida);
            motor.Runtime.IO.SetOutput(flujoSalida, Encoding.UTF8);

            // Inicializar la lista de patrones de sintaxis
            patronesLenguaje = new List<(string, Regex)>
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

        public string EjecutarCodigoPython(string codigo)
        {
            try
            {
                constructorSalida.Clear();
                var fuente = motor.CreateScriptSourceFromString(codigo);
                fuente.Execute(alcance);
                return $"Ejecución exitosa.\n\nResultado de la ejecución:\n{constructorSalida.ToString()}";
            }
            catch (Exception ex)
            {
                return $"Error durante la ejecución del código: {ex.Message}";
            }
        }

        private class StreamEscritorString : Stream
        {
            private StringBuilder _builder;

            public StreamEscritorString(StringBuilder builder)
            {
                _builder = builder;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => 0;
            public override long Position { get => 0; set => throw new NotSupportedException(); }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count)
            {
                var text = Encoding.UTF8.GetString(buffer, offset, count);
                _builder.Append(text);
            }
        }
    }

    public partial class VentanaPrincipal : Window
    {
        private AnalizadorSintaxis analizador;

        public VentanaPrincipal()
        {
            InitializeComponent();
            analizador = new AnalizadorSintaxis();
        }

        private void AlHacerClickAnalizar(object sender, RoutedEventArgs e)
        {
            string codigo = CuadroTextoCodigo.Text;
            string resultado = analizador.EjecutarCodigoPython(codigo);
            BloqueTextoResultado.Text = resultado;
        }

        private void AlHacerClickLimpiarPantalla(object sender, RoutedEventArgs e)
        {
            CuadroTextoCodigo.Clear();
            BloqueTextoResultado.Clear();
        }
    }
}
