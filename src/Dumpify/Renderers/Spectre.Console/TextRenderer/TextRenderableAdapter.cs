using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers.Spectre.Console.TextRenderer;

internal class TextRenderableAdapter : IRenderable
{
    public string Text { get; }
    public Style? Style { get; }

    public TextRenderableAdapter(string text, Style? style = null)
    {
        Text = text;
        Style = style;
    }

    public Markup ToMarkup()
        => new Markup(Markup.Escape(Text), style: Style);

    public override string ToString()
        => Text;

    public Measurement Measure(RenderOptions options, int maxWidth)
        => throw new NotImplementedException();

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
        => throw new NotImplementedException();
}
