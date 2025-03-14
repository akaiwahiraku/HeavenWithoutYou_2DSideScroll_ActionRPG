// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(LuaOnDialogueEvent), true)]
    public class LuaOnDialogueEventEditor : ReferenceDatabaseDialogueEventEditor
    {
        protected override bool isDeprecated { get { return true; } }
    }

}
