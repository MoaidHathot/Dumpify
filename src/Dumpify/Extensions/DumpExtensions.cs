using Dumpify.Config;
using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Dumpify.Extensions;
using Dumpify.Outputs;
using Dumpify.Renderers;
using System.Diagnostics.CodeAnalysis;

namespace Dumpify;

public static class DumpExtensions
{
    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpDebug<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, ColorConfig? colors = null, MembersConfig? members = null, TypeNamingConfig? typeNames = null, TableConfig? tableConfig = null)
        => obj.Dump(label: label, maxDepth: maxDepth, renderer: renderer, useDescriptors: useDescriptors, typeNames: typeNames, colors: colors, output: Config.Outputs.Debug, members: members, tableConfig: tableConfig);

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpTrace<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, ColorConfig? colors = null, MembersConfig? members = null, TypeNamingConfig? typeNames = null, TableConfig? tableConfig = null)
        => obj.Dump(label: label, maxDepth: maxDepth, renderer: renderer, useDescriptors: useDescriptors, typeNames: typeNames, colors: colors, output: Config.Outputs.Trace, members: members, tableConfig: tableConfig);

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpConsole<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, ColorConfig? colors = null, MembersConfig? members = null, TypeNamingConfig? typeNames = null, TableConfig? tableConfig = null)
        => obj.Dump(label: label, maxDepth: maxDepth, renderer: renderer, useDescriptors: useDescriptors, typeNames: typeNames, colors: colors, output: Config.Outputs.Console, members: members, tableConfig: tableConfig);

    public static string DumpText<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, ColorConfig? colors = null, MembersConfig? members = null, TypeNamingConfig? typeNames = null, TableConfig? tableConfig = null)
    {
        using var writer = new StringWriter();
        colors ??= ColorConfig.NoColors;
        _ = obj.Dump(label: label, maxDepth: maxDepth, renderer: renderer, useDescriptors: useDescriptors, typeNames: typeNames, colors: colors, output: new DumpOutput(writer), members: members, tableConfig: tableConfig);

        return writer.ToString().Trim();
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? Dump<T>(this T? obj, string? label = null, int? maxDepth = null, IRenderer? renderer = null, bool? useDescriptors = null, ColorConfig? colors = null, IDumpOutput? output = null, MembersConfig? members = null, TypeNamingConfig? typeNames = null, TableConfig? tableConfig = null)
    {
        var defaultConfig = DumpConfig.Default;

        output ??= defaultConfig.Output;
        renderer ??= defaultConfig.Renderer;

        var membersConfig = members ?? defaultConfig.MembersConfig;
        var typeNamingConfig = typeNames ?? defaultConfig.TypeNamingConfig;

        var rendererConfig = new RendererConfig
        {
            Label = label,
            MaxDepth = maxDepth.MustBeGreaterThan(0) ?? defaultConfig.MaxDepth,
            ColorConfig = colors ?? defaultConfig.ColorConfig,
            TableConfig = tableConfig ?? defaultConfig.TableConfig,
            TypeNamingConfig = typeNamingConfig,
            MemberProvider = new MemberProvider(membersConfig.IncludeProperties, membersConfig.IncludeFields, membersConfig.IncludePublicMembers, membersConfig.IncludeNonPublicMembers),
            TypeNameProvider = new TypeNameProvider(typeNamingConfig.UseAliases, typeNamingConfig.UseFullName, typeNamingConfig.SimplifyAnonymousObjectNames)
        };

        rendererConfig = output.AdjustConfig(rendererConfig);

        var createDescriptor = useDescriptors ?? defaultConfig.UseDescriptors;

        if(obj is null || createDescriptor is false)
        {
            if(TryRenderSafely(obj, renderer, null, rendererConfig, output, out var rendered))
            {
                OutputSafely(obj, rendered, output);
            }

            return obj;
        }

        if(!TryGenerate<T>(obj.GetType(), null, rendererConfig.MemberProvider, out var descriptor))
        {
            return obj;
        }

        if (TryRenderSafely(obj, renderer, descriptor, rendererConfig, output, out var renderedObject))
        {

            OutputSafely(obj, renderedObject, output);
        }

        return obj;
    }

    private static void OutputSafely(object? obj, IRenderedObject rendered, IDumpOutput output)
    {
        try
        {
            rendered.Output(output);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Failed to output rendered object {obj?.GetType().FullName} - {obj}]. {ex.Message}");
#if DEBUG
            Console.WriteLine(ex);
#endif
        }
    }

    private static bool TryRenderSafely<T>(T? obj, IRenderer renderer, IDescriptor? descriptor, RendererConfig config, IDumpOutput output, [NotNullWhen(true)] out IRenderedObject? renderedObject)
    {
        renderedObject = default;

        try
        {
            renderedObject = renderer.Render(obj, descriptor, config);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Failed to Render {typeof(T)} - {obj}]. {ex.Message}");
#if DEBUG
            Console.WriteLine(ex);
#endif

            return false;
        }
    }

    private static bool TryGenerate<T>(Type type, IValueProvider? valueProvider, IMemberProvider memberProvider, out IDescriptor? descriptor)
    {
        descriptor = null;

        try
        {
            descriptor = DumpConfig.Default.Generator.Generate(type, valueProvider: valueProvider, memberProvider: memberProvider);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Failed to Generate descriptor for {typeof(T)} and Property: {valueProvider}]. {ex.Message}");
#if DEBUG
            Console.WriteLine(ex);
#endif
        }

        return false;
    }
}
