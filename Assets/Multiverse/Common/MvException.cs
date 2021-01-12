using System;

namespace Multiverse.Common
{
    public class MvException : Exception
    {
        public MvException() { }
        public MvException(string message) : base(message) { }
    }
}