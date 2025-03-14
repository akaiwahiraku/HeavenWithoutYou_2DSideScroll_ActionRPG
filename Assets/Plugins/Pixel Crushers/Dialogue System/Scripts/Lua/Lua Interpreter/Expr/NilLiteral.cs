namespace Language.Lua
{
    public partial class NilLiteral : Term
    {
        public override LuaValue Evaluate(LuaTable enviroment)
        {
            return LuaNil.Nil;
        }
    }
}
