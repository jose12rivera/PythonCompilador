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
        private ScriptEngine motor; // Motor para ejecutar código Python
        private ScriptScope alcance; // Alcance de ejecución del código
        private StringBuilder constructorSalida; // Almacena la salida del código ejecutado
        private List<(string, Regex)> patronesLenguaje; // Lista de patrones de sintaxis de Python

        public AnalizadorSintaxis()
        {
            motor = Python.CreateEngine(); // Crea un motor de Python
            alcance = motor.CreateScope(); // Crea un nuevo alcance de ejecución
            constructorSalida = new StringBuilder(); // Inicializa el StringBuilder para la salida

            // Configura el flujo de salida para redirigir la salida del motor Python a el StringBuilder
            var flujoSalida = new StreamEscritorString(constructorSalida);
            motor.Runtime.IO.SetOutput(flujoSalida, Encoding.UTF8);

            // Inicializar la lista de patrones de sintaxis para detectar construcciones del lenguaje Python
            patronesLenguaje = new List<(string, Regex)>
            {
                ("Python", new Regex(@"^def\s+\w+\s*\(.*\)\s*:", RegexOptions.IgnoreCase)), // Definición de función
                ("Python", new Regex(@"^if\s+.*:", RegexOptions.IgnoreCase)), // Sentencia if
                ("Python", new Regex(@"^elif\s+.*:", RegexOptions.IgnoreCase)), // Sentencia elif
                ("Python", new Regex(@"^else\s*:", RegexOptions.IgnoreCase)), // Sentencia else
                ("Python", new Regex(@"^for\s+\w+\s+in\s+\w+\s*:", RegexOptions.IgnoreCase)), // Bucle for
                ("Python", new Regex(@"^while\s+.*:", RegexOptions.IgnoreCase)), // Bucle while
                ("Python", new Regex(@"^try\s*:", RegexOptions.IgnoreCase)), // Bloque try
                ("Python", new Regex(@"^except\s+.*:", RegexOptions.IgnoreCase)), // Bloque except
                ("Python", new Regex(@"^finally\s*:", RegexOptions.IgnoreCase)), // Bloque finally
                ("Python", new Regex(@"^class\s+\w+\s*:", RegexOptions.IgnoreCase)), // Definición de clase
                ("Python", new Regex(@"^import\s+\w+", RegexOptions.IgnoreCase)), // Importación de módulo
                ("Python", new Regex(@"^from\s+\w+\s+import\s+\w+", RegexOptions.IgnoreCase)), // Importación específica
                ("Python", new Regex(@"^return\s+.*", RegexOptions.IgnoreCase)), // Sentencia return
                ("Python", new Regex(@"^print\s*\(.*\)", RegexOptions.IgnoreCase)), // Función print
                ("Python", new Regex(@"^assert\s+.*", RegexOptions.IgnoreCase)), // Sentencia assert
                ("Python", new Regex(@"^break\s*", RegexOptions.IgnoreCase)), // Sentencia break
                ("Python", new Regex(@"^continue\s*", RegexOptions.IgnoreCase)), // Sentencia continue
                ("Python", new Regex(@"^del\s+.*", RegexOptions.IgnoreCase)), // Sentencia del
                ("Python", new Regex(@"^global\s+.*", RegexOptions.IgnoreCase)), // Declaración global
                ("Python", new Regex(@"^nonlocal\s+.*", RegexOptions.IgnoreCase)), // Declaración nonlocal
                ("Python", new Regex(@"^pass\s*", RegexOptions.IgnoreCase)), // Sentencia pass
                ("Python", new Regex(@"^raise\s+.*", RegexOptions.IgnoreCase)), // Sentencia raise
                ("Python", new Regex(@"^with\s+.*:", RegexOptions.IgnoreCase)), // Context manager con with
                ("Python", new Regex(@"^yield\s+.*", RegexOptions.IgnoreCase)), // Sentencia yield
                ("Python", new Regex(@"\bFalse\b", RegexOptions.IgnoreCase)), // Literal False
                ("Python", new Regex(@"\bNone\b", RegexOptions.IgnoreCase)), // Literal None
                ("Python", new Regex(@"\bTrue\b", RegexOptions.IgnoreCase)), // Literal True
                ("Python", new Regex(@"\band\b", RegexOptions.IgnoreCase)), // Operador lógico and
                ("Python", new Regex(@"\bas\b", RegexOptions.IgnoreCase)), // Palabra clave as
                ("Python", new Regex(@"\basync\b", RegexOptions.IgnoreCase)), // Palabra clave async
                ("Python", new Regex(@"\bawait\b", RegexOptions.IgnoreCase)), // Palabra clave await
                ("Python", new Regex(@"\bin\b", RegexOptions.IgnoreCase)), // Operador in
                ("Python", new Regex(@"\bis\b", RegexOptions.IgnoreCase)), // Operador is
                ("Python", new Regex(@"\blambda\b", RegexOptions.IgnoreCase)), // Palabra clave lambda
                ("Python", new Regex(@"\bnot\b", RegexOptions.IgnoreCase)), // Operador lógico not
                ("Python", new Regex(@"\bor\b", RegexOptions.IgnoreCase)) // Operador lógico or
            };
        }

        public string EjecutarCodigoPython(string codigo)
        {
            try
            {
                constructorSalida.Clear(); // Limpia la salida previa
                var fuente = motor.CreateScriptSourceFromString(codigo); // Crea una fuente de script a partir del código Python
                fuente.Execute(alcance); // Ejecuta el código en el alcance creado
                return $"Ejecución exitosa.\n\nResultado de la ejecución:\n{constructorSalida.ToString()}"; // Devuelve el resultado
            }
            catch (Exception ex)
            {
                return $"Error durante la ejecución del código: {ex.Message}"; // Manejo de errores
            }
        }

        private class StreamEscritorString : Stream
        {
            private StringBuilder _builder; // Almacena el StringBuilder para la salida

            public StreamEscritorString(StringBuilder builder)
            {
                _builder = builder; // Inicializa el constructor con el StringBuilder
            }

            public override bool CanRead => false; // No se puede leer
            public override bool CanSeek => false; // No se puede buscar
            public override bool CanWrite => true; // Se puede escribir
            public override long Length => 0; // Longitud siempre cero
            public override long Position { get => 0; set => throw new NotSupportedException(); } // No soporta posición
            public override void Flush() { } // Método flush vacío
            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException(); // No soporta lectura
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(); // No soporta búsqueda
            public override void SetLength(long value) => throw new NotSupportedException(); // No soporta establecer longitud
            public override void Write(byte[] buffer, int offset, int count)
            {
                var text = Encoding.UTF8.GetString(buffer, offset, count); // Convierte el buffer a string
                _builder.Append(text); // Agrega el texto al StringBuilder
            }
        }
    }

    public partial class VentanaPrincipal : Window
    {
        private AnalizadorSintaxis analizador; // Instancia del analizador de sintaxis

        public VentanaPrincipal()
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            analizador = new AnalizadorSintaxis(); // Crea una nueva instancia del analizador
        }

        private void AlHacerClickAnalizar(object sender, RoutedEventArgs e)
        {
            string codigo = CuadroTextoCodigo.Text; // Obtiene el código del cuadro de texto
            string resultado = analizador.EjecutarCodigoPython(codigo); // Ejecuta el código Python
            BloqueTextoResultado.Text = resultado; // Muestra el resultado en el bloque de texto
        }

        private void AlHacerClickLimpiarPantalla(object sender, RoutedEventArgs e)
        {
            CuadroTextoCodigo.Clear(); // Limpia el cuadro de texto de código
            BloqueTextoResultado.Clear(); // Limpia el bloque de texto de resultado
        }
    }
}
