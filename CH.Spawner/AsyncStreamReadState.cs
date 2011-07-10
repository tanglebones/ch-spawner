using System;
using System.IO;

namespace CH.Spawner
{
    internal class AsyncStreamReadState
    {
        public IWriter Writer { get; set; }
        public Stream Stream { get; set; }
        public byte[] Buffer { get; set; }
        public IAsyncResult AsyncResult { get; set; }
        public bool Complete { get; set; }
    }
}