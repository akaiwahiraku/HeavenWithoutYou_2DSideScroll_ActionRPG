using System.Collections.Generic;

namespace Language.Lua
{
    public partial class Args
    {
        public List<Expr> ArgList = new List<Expr>();

        public StringLiteral String;

        public TableConstructor Table;

    }
}
