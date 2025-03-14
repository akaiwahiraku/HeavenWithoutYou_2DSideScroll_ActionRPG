using System.Collections.Generic;

namespace Language.Lua
{
    public partial class Var
    {
        public BaseExpr Base;

        public List<Access> Accesses = new List<Access>();

    }
}
