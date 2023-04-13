using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dumpify.Renderers;

public interface ITypeNameProvider
{
    string GetTypeName(Type type);
    (string typeName, int rank) GetJaggedArrayNameWithRank(Type arrayType);
}
