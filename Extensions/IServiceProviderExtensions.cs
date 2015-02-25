using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    internal static class IServiceProviderExtensions
    {
        internal static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        internal static T GetService<S, T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(S));
        }
    }
}