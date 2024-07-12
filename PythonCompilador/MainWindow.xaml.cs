using System;
using System.IO;
using System.Text;
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

        public AnalizadorSintaxis()
        {
            motor = Python.CreateEngine();
            alcance = motor.CreateScope();
            constructorSalida = new StringBuilder();

            var flujoSalida = new StreamEscritorString(constructorSalida);
            motor.Runtime.IO.SetOutput(flujoSalida, Encoding.UTF8);
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
