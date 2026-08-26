#region U S I N G

using ExtensionsTest.Modules.Abstractions;

#endregion

namespace ExtensionsTest.Modules.Services
{
    public class Repo<T> : IRepo<T>
    {
        public T Get()
        {
            return default;
        }
    }
}
