#region U S A G E S

using System.Threading.Tasks;
using ExtensionsTest.Modules.Abstractions;

#endregion

namespace ExtensionsTest.Modules.Services
{
    public class ServiceInvokeOne : IServiceInvokeOne
    {
        public async Task DoTask1()
        {
            await Task.CompletedTask;
        }

        public async Task DoTask2()
        {
            await Task.CompletedTask;
        }
    }
}
