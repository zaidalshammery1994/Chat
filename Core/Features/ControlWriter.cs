using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace Core.Features
{

    public class ControlWriter : TextWriter
    {
        private TextBox _textBox { set; get; }

        public ControlWriter(TextBox textBox)
        {
            this._textBox = textBox;
            Console.SetOut(this);
        }

        public override void Write(string value)
        {
            _textBox.Dispatcher.BeginInvoke(new ThreadStart(() => _textBox.AppendText(value + " ")));
        }

        public override void WriteLine(string value)
        {
            _textBox.Dispatcher.BeginInvoke(new ThreadStart(() => _textBox.AppendText(value + "\r\n")));
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}
