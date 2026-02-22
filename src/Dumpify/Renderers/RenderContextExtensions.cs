namespace Dumpify;

/// <summary>
/// Extension methods for RenderContext to help with path manipulation.
/// </summary>
public static class RenderContextExtensions
{
    /// <summary>
    /// Creates a new context with the property name appended to the current path.
    /// </summary>
    /// <param name="context">The current render context</param>
    /// <param name="propertyName">The property name to append</param>
    /// <returns>A new context with the updated path</returns>
    public static RenderContext WithProperty(this RenderContext context, string propertyName)
    {
        var newPath = string.IsNullOrEmpty(context.CurrentPath)
            ? propertyName
            : $"{context.CurrentPath}.{propertyName}";
        return context with { CurrentPath = newPath };
    }

    /// <summary>
    /// Creates a new context with an index appended to the current path.
    /// </summary>
    /// <param name="context">The current render context</param>
    /// <param name="index">The array/collection index</param>
    /// <returns>A new context with the updated path</returns>
    public static RenderContext WithIndex(this RenderContext context, int index)
    {
        var newPath = $"{context.CurrentPath}[{index}]";
        return context with { CurrentPath = newPath };
    }

    /// <summary>
    /// Creates a new context with the property name appended to the current path.
    /// </summary>
    /// <typeparam name="TState">The renderer state type</typeparam>
    /// <param name="context">The current render context</param>
    /// <param name="propertyName">The property name to append</param>
    /// <returns>A new context with the updated path</returns>
    public static RenderContext<TState> WithProperty<TState>(this RenderContext<TState> context, string propertyName)
    {
        var newPath = string.IsNullOrEmpty(context.CurrentPath)
            ? propertyName
            : $"{context.CurrentPath}.{propertyName}";
        return context with { CurrentPath = newPath };
    }

    /// <summary>
    /// Creates a new context with an index appended to the current path.
    /// </summary>
    /// <typeparam name="TState">The renderer state type</typeparam>
    /// <param name="context">The current render context</param>
    /// <param name="index">The array/collection index</param>
    /// <returns>A new context with the updated path</returns>
    public static RenderContext<TState> WithIndex<TState>(this RenderContext<TState> context, int index)
    {
        var newPath = $"{context.CurrentPath}[{index}]";
        return context with { CurrentPath = newPath };
    }

    /// <summary>
    /// Creates a new context with a 2D index appended to the current path.
    /// </summary>
    /// <typeparam name="TState">The renderer state type</typeparam>
    /// <param name="context">The current render context</param>
    /// <param name="row">The row index</param>
    /// <param name="col">The column index</param>
    /// <returns>A new context with the updated path (e.g., [0,1])</returns>
    public static RenderContext<TState> WithIndex2D<TState>(this RenderContext<TState> context, int row, int col)
    {
        var newPath = $"{context.CurrentPath}[{row},{col}]";
        return context with { CurrentPath = newPath };
    }
}
