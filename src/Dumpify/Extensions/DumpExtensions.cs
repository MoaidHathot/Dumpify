using Dumpify.Descriptors;
using Dumpify.Descriptors.ValueProviders;
using Dumpify.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Dumpify;

public static class DumpExtensions
{
    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpDebug<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))] string? autoLabel = null
    )
    {
        outputConfig ??= new OutputConfig { WidthOverride = 250 };
        return obj.Dump(
            label: label,
            maxDepth: maxDepth,
            renderer: renderer,
            useDescriptors: useDescriptors,
            typeNames: typeNames,
            colors: colors,
            output: Dumpify.Outputs.Debug,
            members: members,
            tableConfig: tableConfig,
            outputConfig: outputConfig,
            typeRenderingConfig: typeRenderingConfig,
            autoLabel: autoLabel
        );
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpTrace<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))] string? autoLabel = null
    )
    {
        outputConfig ??= new OutputConfig { WidthOverride = 250 };
        return obj.Dump(
            label: label,
            maxDepth: maxDepth,
            renderer: renderer,
            useDescriptors: useDescriptors,
            typeNames: typeNames,
            colors: colors,
            output: Dumpify.Outputs.Trace,
            members: members,
            tableConfig: tableConfig,
            outputConfig: outputConfig,
            typeRenderingConfig: typeRenderingConfig,
            autoLabel: autoLabel
        );
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DumpConsole<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))] string? autoLabel = null
    ) =>
        obj.Dump(
            label: label,
            maxDepth: maxDepth,
            renderer: renderer,
            useDescriptors: useDescriptors,
            typeNames: typeNames,
            colors: colors,
            output: Dumpify.Outputs.Console,
            members: members,
            tableConfig: tableConfig,
            outputConfig: outputConfig,
            typeRenderingConfig: typeRenderingConfig,
            autoLabel: autoLabel
        );

    public static string DumpText<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))] string? autoLabel = null
    )
    {
        using var writer = new StringWriter();

        colors ??= ColorConfig.NoColors;
        outputConfig ??= new OutputConfig { WidthOverride = 1000, HeightOverride = 1000 };

        _ = obj.Dump(
            label: label,
            maxDepth: maxDepth,
            renderer: renderer,
            useDescriptors: useDescriptors,
            typeNames: typeNames,
            colors: colors,
            output: new DumpOutput(writer),
            members: members,
            tableConfig: tableConfig,
            outputConfig: outputConfig,
            typeRenderingConfig: typeRenderingConfig,
            autoLabel: autoLabel
        );

        return writer.ToString().Trim();
    }

    [return: NotNullIfNotNull(nameof(obj))]
    public static T? Dump<T>(
        this T? obj,
        string? label = null,
        int? maxDepth = null,
        IRenderer? renderer = null,
        bool? useDescriptors = null,
        ColorConfig? colors = null,
        IDumpOutput? output = null,
        MembersConfig? members = null,
        TypeNamingConfig? typeNames = null,
        TableConfig? tableConfig = null,
        OutputConfig? outputConfig = null,
        TypeRenderingConfig? typeRenderingConfig = null,
        [CallerArgumentExpression(nameof(obj))] string? autoLabel = null
    )
    {
        var defaultConfig = DumpConfig.Default;

        output ??= defaultConfig.Output;
        renderer ??= defaultConfig.Renderer;

        var membersConfig = members ?? defaultConfig.MembersConfig;
        var typeNamingConfig = typeNames ?? defaultConfig.TypeNamingConfig;

        var rendererConfig = new RendererConfig
        {
            Label = label ?? (defaultConfig.UseAutoLabels ? autoLabel : null),
            MaxDepth = maxDepth.MustBeGreaterThan(0) ?? defaultConfig.MaxDepth,
            ColorConfig = colors ?? defaultConfig.ColorConfig,
            TableConfig = tableConfig ?? defaultConfig.TableConfig,
            TypeNamingConfig = typeNamingConfig,
            TypeRenderingConfig = typeRenderingConfig ?? defaultConfig.TypeRenderingConfig,
            MemberProvider = new MemberProvider(
                membersConfig.IncludeProperties,
                membersConfig.IncludeFields,
                membersConfig.IncludePublicMembers,
                membersConfig.IncludeNonPublicMembers,
                membersConfig.IncludeVirtualMembers,
                membersConfig.MemberFilter
            ),
            TypeNameProvider = new TypeNameProvider(
                typeNamingConfig.UseAliases,
                typeNamingConfig.UseFullName,
                typeNamingConfig.SimplifyAnonymousObjectNames,
                typeNamingConfig.SeparateTypesWithSpace
            ),
        };

        outputConfig ??= defaultConfig.OutputConfig;

        rendererConfig = output.AdjustConfig(rendererConfig);

        var createDescriptor = useDescriptors ?? defaultConfig.UseDescriptors;

        if (obj is null || createDescriptor is false)
        {
            if (TryRenderSafely(obj, renderer, null, rendererConfig, output, out var rendered))
            {
                OutputSafely(obj, rendered, output, outputConfig);
            }

            return obj;
        }

        if (!TryGenerate<T>(obj.GetType(), null, rendererConfig.MemberProvider, out var descriptor))
        {
            return obj;
        }

        if (
            TryRenderSafely(
                obj,
                renderer,
                descriptor,
                rendererConfig,
                output,
                out var renderedObject
            )
        )
        {
            OutputSafely(obj, renderedObject, output, outputConfig);
        }

        return obj;
    }

    private static void OutputSafely(
        object? obj,
        IRenderedObject rendered,
        IDumpOutput output,
        OutputConfig config
    )
    {
        try
        {
            rendered.Output(output, config);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[Failed to output rendered object {obj?.GetType().FullName} - {obj}]. {ex.Message}"
            );
#if DEBUG
            Console.WriteLine(ex);
#endif
        }
    }

    private static bool TryRenderSafely<T>(
        T? obj,
        IRenderer renderer,
        IDescriptor? descriptor,
        RendererConfig config,
        IDumpOutput output,
        [NotNullWhen(true)] out IRenderedObject? renderedObject
    )
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

    private static bool TryGenerate<T>(
        Type type,
        IValueProvider? valueProvider,
        IMemberProvider memberProvider,
        out IDescriptor? descriptor
    )
    {
        descriptor = null;

        try
        {
            descriptor = DumpConfig.Default.Generator.Generate(
                type,
                valueProvider: valueProvider,
                memberProvider: memberProvider
            );
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[Failed to Generate descriptor for {typeof(T)} and Property: {valueProvider}]. {ex.Message}"
            );
#if DEBUG
            Console.WriteLine(ex);
#endif
        }

        return false;
    }
}