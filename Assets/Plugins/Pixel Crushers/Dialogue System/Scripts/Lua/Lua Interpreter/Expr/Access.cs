namespace Language.Lua
{
    public abstract partial class Access
    {
        public abstract LuaValue Evaluate(LuaValue baseValue, LuaTable enviroment);
    }
}
