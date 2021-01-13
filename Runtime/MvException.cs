using System;

namespace Multiverse
{
    public class MvException : Exception
    {
        public MvException() { }
        public MvException(string message) : base(message) { }
    }
}