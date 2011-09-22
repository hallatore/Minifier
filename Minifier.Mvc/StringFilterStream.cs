using System;
using System.IO;
using System.Text;

namespace Lervik.Minifier.Mvc
{
    // orginally from http://www.iantweedrie.com/post/2011/03/28/MVC-Razor-HTML-Minification.aspx
    internal class StringFilterStream : Stream
    {
        private Stream _sink;
        private Func<string, string> _filter;
        private long _position;

        public StringFilterStream(Stream sink, Func<string, string> filter)
        {
            _sink = sink;
            _filter = filter;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override void Flush() { _sink.Flush(); }
        public override long Length { get { return 0; } }

        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _sink.Read(buffer, offset, count);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _sink.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _sink.SetLength(value);
        }
        public override void Close()
        {
            _sink.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var data = new byte[count];
            Buffer.BlockCopy(buffer, offset, data, 0, count);
            string s = Encoding.Default.GetString(buffer);

            s = _filter(s);

            var outdata = Encoding.Default.GetBytes(s);
            _sink.Write(outdata, 0, outdata.GetLength(0));
        }
    }
}
