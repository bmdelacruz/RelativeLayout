using System;

namespace Map.Controls.Exceptions
{
    public class ControlNotFoundException : Exception
    {
        public ControlNotFoundException(string name) : base($"Cannot find control. Name=\"{name}\"") { }
    }
}
