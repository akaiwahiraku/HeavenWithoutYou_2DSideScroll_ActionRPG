// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(SendMessageOnDialogueEvent), true)]
    public class SendMessageOnDialogueEventEditor : ReferenceDatabaseDialogueEventEditor
    {
        protected override bool isDeprecated { get { return true; } }
    }

}
