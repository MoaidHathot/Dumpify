using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Config;

public interface IColorConfig<TColor>
{
    TColor? TypeNameColor { get; set; }
    TColor? ColumnNameColor { get; set; }
    TColor? PropertyValueColor { get; set; }
    TColor? PropertyNameColor { get; set; }
    TColor? NullValueColor { get; set; }
    TColor? IgnoredValueColor { get; set; }
    TColor? MetadataInfoColor { get; set; }
    TColor? MetadataErrorColor { get; set; }
}
