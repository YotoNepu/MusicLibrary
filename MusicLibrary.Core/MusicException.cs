using System;

namespace MusicLibrary.Core
{
    public class MusicItemException : Exception
    {
        public MusicItemException(string message) : base(message) {}
    }
}