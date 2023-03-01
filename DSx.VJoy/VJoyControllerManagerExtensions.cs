using System.Collections.Generic;
using System.Linq;
using CoreDX.vJoy.Wrapper;

namespace DSx.VJoy
{
    public static class VJoyControllerManagerExtensions
    {
        public static IEnumerable<VJoyController> EnumerateControllers(this VJoyControllerManager manager)
        {
            if (!VJoyControllerManager.IsDriverLoaded || !manager.IsVJoyEnabled) yield break;
            foreach (var id in Enumerable.Range(0, 16).Select(x => (uint)x))
            {
                if (manager.GetVJDStatus(id).GetHashCode() != 1) continue;
                yield return new VJoyController(manager, id);
            }

            
        } 
    }
}