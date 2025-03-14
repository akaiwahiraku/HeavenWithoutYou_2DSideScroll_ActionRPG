namespace Language.Lua
{
    public abstract partial class Expr
    {
        public abstract LuaValue Evaluate(LuaTable enviroment);

        public abstract Term Simplify();
    }
}
