﻿namespace Language.Lua
{
    public partial class BoolLiteral : Term
    {
        public override LuaValue Evaluate(LuaTable enviroment)
        {
            return LuaBoolean.From(bool.Parse(this.Text));
        }
    }
}
