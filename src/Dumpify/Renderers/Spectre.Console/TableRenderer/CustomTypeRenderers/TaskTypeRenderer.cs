using Dumpify.Descriptors;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Threading.Tasks;

namespace Dumpify;

internal class TaskTypeRenderer(IRendererHandler<IRenderable, SpectreRendererState> handler) : ICustomTypeRenderer<IRenderable>
{
    private readonly IRendererHandler<IRenderable, SpectreRendererState> _handler = handler;

    public Type DescriptorType => typeof(CustomDescriptor);

    public IRenderable Render(IDescriptor descriptor, object obj, RenderContext baseContext, object? handleContext)
    {
        var context = (RenderContext<SpectreRendererState>)baseContext;
        var task = (Task)obj;
        var type = obj.GetType();
        var isGenericTask = IsGenericTaskType(type);

        var tableBuilder = new ObjectTableBuilder(context, descriptor, obj)
            .AddDefaultColumnNames();

        // Render Id
        AddPropertyRow(tableBuilder, context, nameof(Task.Id), task.Id);

        // Render Status
        AddPropertyRow(tableBuilder, context, nameof(Task.Status), task.Status);

        // Render IsCompleted
        AddPropertyRow(tableBuilder, context, nameof(Task.IsCompleted), task.IsCompleted);

        // Render IsCanceled
        AddPropertyRow(tableBuilder, context, nameof(Task.IsCanceled), task.IsCanceled);

        // Render IsFaulted
        AddPropertyRow(tableBuilder, context, nameof(Task.IsFaulted), task.IsFaulted);

        // Render CreationOptions
        AddPropertyRow(tableBuilder, context, nameof(Task.CreationOptions), task.CreationOptions);

        // Render AsyncState
        if (task.AsyncState is null)
        {
            tableBuilder.AddRow(null, null, nameof(Task.AsyncState), _handler.RenderNullValue(null, context));
        }
        else
        {
            var asyncStateDescriptor = DumpConfig.Default.Generator.Generate(task.AsyncState.GetType(), null, context.Config.MemberProvider);
            var asyncStateRendered = _handler.RenderDescriptor(task.AsyncState, asyncStateDescriptor, context);
            tableBuilder.AddRow(asyncStateDescriptor, task.AsyncState, nameof(Task.AsyncState), asyncStateRendered);
        }

        // Render Exception (safe to access, returns null if no exception)
        if (task.Exception is null)
        {
            tableBuilder.AddRow(null, null, nameof(Task.Exception), _handler.RenderNullValue(null, context));
        }
        else
        {
            var exceptionDescriptor = DumpConfig.Default.Generator.Generate(task.Exception.GetType(), null, context.Config.MemberProvider);
            var exceptionRendered = _handler.RenderDescriptor(task.Exception, exceptionDescriptor, context);
            tableBuilder.AddRow(exceptionDescriptor, task.Exception, nameof(Task.Exception), exceptionRendered);
        }

        // Render Result (only for Task<T>, not for non-generic Task)
        if (isGenericTask)
        {
            RenderResult(tableBuilder, context, task, type);
        }

        return tableBuilder.Build();
    }

    private void RenderResult(ObjectTableBuilder tableBuilder, RenderContext<SpectreRendererState> context, Task task, Type type)
    {
        // Only access Result if the task completed successfully (RanToCompletion)
        // Using Status == TaskStatus.RanToCompletion for netstandard2.0 compatibility
        if (task.Status == TaskStatus.RanToCompletion)
        {
            // Safe to access Result - task is completed successfully
            var resultProperty = type.GetProperty("Result");
            var result = resultProperty?.GetValue(task);

            if (result is null)
            {
                tableBuilder.AddRow(null, null, "Result", _handler.RenderNullValue(null, context));
            }
            else
            {
                var resultDescriptor = DumpConfig.Default.Generator.Generate(result.GetType(), null, context.Config.MemberProvider);
                var resultRendered = _handler.RenderDescriptor(result, resultDescriptor, context);
                tableBuilder.AddRow(resultDescriptor, result, "Result", resultRendered);
            }
        }
        else
        {
            // Task not completed successfully - show appropriate message
            var message = task.Status switch
            {
                TaskStatus.Faulted => "[Faulted]",
                TaskStatus.Canceled => "[Canceled]",
                _ => "[Not Completed]"
            };

            var markup = new Markup(Markup.Escape(message), new Style(foreground: context.State.Colors.MetadataInfoColor));
            tableBuilder.AddRow(null, null, "Result", markup);
        }
    }

    private void AddPropertyRow<T>(ObjectTableBuilder tableBuilder, RenderContext<SpectreRendererState> context, string propertyName, T value)
    {
        var descriptor = DumpConfig.Default.Generator.Generate(typeof(T), null, context.Config.MemberProvider);
        var rendered = _handler.RenderDescriptor(value, descriptor, context);
        tableBuilder.AddRow(descriptor, value, propertyName, rendered);
    }

    public (bool shouldHandle, object? handleContext) ShouldHandle(IDescriptor descriptor, object obj)
    {
        var isTask = typeof(Task).IsAssignableFrom(descriptor.Type);
        return (isTask, null);
    }

    /// <summary>
    /// Checks if a type is or inherits from Task&lt;T&gt; (handles internal runtime types like ContinuationResultTaskFromTask&lt;T&gt;)
    /// </summary>
    private static bool IsGenericTaskType(Type type)
    {
        var currentType = type;
        while (currentType != null)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return true;
            }
            currentType = currentType.BaseType;
        }
        return false;
    }
}
