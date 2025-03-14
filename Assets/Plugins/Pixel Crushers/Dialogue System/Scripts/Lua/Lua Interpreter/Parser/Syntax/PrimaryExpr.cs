using System.Collections.Generic;

namespace Language.Lua
{
    public partial class PrimaryExpr : Term
    {
        public BaseExpr Base;

        public List<Access> Accesses = new List<Access>();

    }
}
