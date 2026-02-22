namespace Dumpify;

/// <summary>
/// Specifies the border style for tables rendered by Dumpify.
/// </summary>
public enum TableBorderStyle
{
    /// <summary>
    /// Rounded corners using Unicode box-drawing characters (╭─╮│╰─╯).
    /// Best appearance but requires font/terminal support.
    /// </summary>
    Rounded,

    /// <summary>
    /// Square corners using Unicode box-drawing characters (┌─┐│└─┘).
    /// More widely supported than Rounded.
    /// </summary>
    Square,

    /// <summary>
    /// ASCII-only characters (+-+|+-+).
    /// Maximum compatibility with all terminals.
    /// </summary>
    Ascii,

    /// <summary>
    /// No border at all.
    /// </summary>
    None,

    /// <summary>
    /// Heavy/bold Unicode box-drawing characters (┏━┓┃┗━┛).
    /// </summary>
    Heavy,

    /// <summary>
    /// Double-line Unicode box-drawing characters (╔═╗║╚═╝).
    /// </summary>
    Double,

    /// <summary>
    /// Minimal border style with horizontal lines only (─).
    /// </summary>
    Minimal,

    /// <summary>
    /// Markdown-compatible table format (|---|).
    /// </summary>
    Markdown,
}
