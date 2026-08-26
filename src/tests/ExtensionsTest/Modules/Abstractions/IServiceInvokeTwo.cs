#region U S A G E S

using System.Threading.Tasks;

#endregion

namespace ExtensionsTest.Modules.Abstractions
{
    public interface IServiceInvokeTwo
    {
        Task DoTask1();
        Task DoTask2();
    }
}
