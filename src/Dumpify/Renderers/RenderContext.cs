using System.Runtime.Serialization;

namespace Dumpify;

//Todo: RootObjectTransform is a temp workaround and should be removed once we merge the ObjectTableBuilder logic
public record class RenderContext(in RendererConfig Config, ObjectIDGenerator ObjectTracker, int CurrentDepth, object? RootObject, object? RootObjectTransform);
public record class RenderContext<TState>(in RendererConfig Config, ObjectIDGenerator ObjectTracker, int CurrentDepth, object? RootObject, object? RootObjectTransform, TState State) : RenderContext(Config, ObjectTracker, CurrentDepth, RootObject, RootObjectTransform);