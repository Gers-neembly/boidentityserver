using System;

namespace Neembly.BOIDServer.WebAPI.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Record for \"{key}\" in \"{name}\"  was not found.")
        {}
    }
}
