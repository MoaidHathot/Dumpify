using System.Collections.Immutable;

namespace Dumpify;

//Todo: RootObjectTransform is a temp workaround and should be removed once we merge the ObjectTableBuilder logic
public record class RenderContext(
    in RendererConfig Config,
    IObjectIdTracker ObjectTracker,
    int CurrentDepth,
    object? RootObject,
    object? RootObjectTransform,
    ImmutableHashSet<object> Ancestors,
    string CurrentPath,
    ImmutableDictionary<object, int> ObjectIds,
    ImmutableDictionary<object, string> ObjectPaths
);

public record class RenderContext<TState>(
    in RendererConfig Config,
    IObjectIdTracker ObjectTracker,
    int CurrentDepth,
    object? RootObject,
    object? RootObjectTransform,
    TState State,
    ImmutableHashSet<object> Ancestors,
    string CurrentPath,
    ImmutableDictionary<object, int> ObjectIds,
    ImmutableDictionary<object, string> ObjectPaths
) : RenderContext(Config, ObjectTracker, CurrentDepth, RootObject, RootObjectTransform, Ancestors, CurrentPath, ObjectIds, ObjectPaths);
