#region U S A G E S

using System.Threading.Tasks;
using ExtensionsTest.Modules.Abstractions;

#endregion

namespace ExtensionsTest.Modules.Services
{
    internal class IntermediateInvokeBase : IServiceInvoke
    {
        public Task DoTask1() => Task.CompletedTask;

        public Task DoTask2() => Task.CompletedTask;
    }
}
