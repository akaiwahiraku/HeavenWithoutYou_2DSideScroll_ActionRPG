namespace Language.Lua
{
    public partial class ForStmt : Statement
    {
        public string VarName;

        public Expr Start;

        public Expr End;

        public Expr Step;

        public Chunk Body;

    }
}
