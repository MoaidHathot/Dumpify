using Dumpify.Descriptors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

public interface IRenderer
{
    IRenderedObject Render(object? obj, IDescriptor? descriptor, RendererConfig config);
}
