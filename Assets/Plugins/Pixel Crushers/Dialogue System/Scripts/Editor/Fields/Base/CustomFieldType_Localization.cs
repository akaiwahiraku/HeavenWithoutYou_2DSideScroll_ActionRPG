using UnityEditor;
using UnityEngine;


namespace PixelCrushers.DialogueSystem
{

    [CustomFieldTypeService.Name("Localization")]
    public class CustomFieldType_Localization : CustomFieldType
    {
        public override FieldType storeFieldAsType
        {
            get
            {
                return FieldType.Localization;
            }
        }

        public override string Draw(string currentValue, DialogueDatabase dataBase)
        {
            return EditorGUILayout.TextField(currentValue);
        }

        public override string Draw(Rect rect, string currentValue, DialogueDatabase dataBase)
        {
            return EditorGUI.TextField(rect, currentValue);
        }
    }
}