using System.Collections.Generic;

namespace Language.Lua
{
    public partial class ReturnStmt : Statement
    {
        public List<Expr> ExprList = new List<Expr>();

    }
}
