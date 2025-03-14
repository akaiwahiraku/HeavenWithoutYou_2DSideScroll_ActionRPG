namespace Language.Lua
{
    public partial class StringLiteral : Term
    {
        public override LuaValue Evaluate(LuaTable enviroment)
        {
            return new LuaString(this.Text);
        }
    }
}
