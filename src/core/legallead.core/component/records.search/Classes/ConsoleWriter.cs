using System.Text;

namespace legallead.records.search.Classes
{
    public class ConsoleWriterEventArgs : EventArgs
    {
        public string Value { get; private set; }

        public ConsoleWriterEventArgs(string value)
        {
            Value = value;
        }
    }

    public class ConsoleWriter : TextWriter
    {
        public override Encoding Encoding
        { get { return Encoding.UTF8; } }

        public override void Write(string? value)
        {
            if (value == null) return;
            WriteEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
            base.Write(value);
        }

        public override void WriteLine(string? value)
        {
            if (value == null) return;
            WriteLineEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
            base.WriteLine(value);
        }

        public event EventHandler<ConsoleWriterEventArgs>? WriteEvent;

        public event EventHandler<ConsoleWriterEventArgs>? WriteLineEvent;
    }
}