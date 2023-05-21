using System.Runtime.Serialization;

namespace Dumpify.Renderers;

public record class RenderContext(in RendererConfig Config, ObjectIDGenerator ObjectTracker, int CurrentDepth, object? RootObject);
public record class RenderContext<TState>(in RendererConfig Config, ObjectIDGenerator ObjectTracker, int CurrentDepth, object? RootObject, TState State) : RenderContext(Config, ObjectTracker, CurrentDepth, RootObject);