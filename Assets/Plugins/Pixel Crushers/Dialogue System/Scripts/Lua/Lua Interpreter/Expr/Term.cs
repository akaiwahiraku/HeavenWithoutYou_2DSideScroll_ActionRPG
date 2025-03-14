using System;

namespace Language.Lua
{
    public partial class Term : Expr
    {
        public override LuaValue Evaluate(LuaTable enviroment)
        {
            throw new NotImplementedException();
        }

        public override Term Simplify()
        {
            return this;
        }
    }
}
