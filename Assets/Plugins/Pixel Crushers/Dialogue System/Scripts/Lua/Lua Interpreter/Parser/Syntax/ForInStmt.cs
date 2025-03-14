using System.Collections.Generic;

namespace Language.Lua
{
    public partial class ForInStmt : Statement
    {
        public List<string> NameList = new List<string>();

        public List<Expr> ExprList = new List<Expr>();

        public Chunk Body;

    }
}
