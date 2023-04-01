using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

public record class RenderContext(in RendererConfig Config, ObjectIDGenerator ObjectTracker, int CurrentDepth);
