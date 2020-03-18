using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Data;
namespace SOTU.Editor {
    [CustomPropertyDrawer(typeof(Flag))]
    public class FlagDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Resources/FlagDrawer.uxml").CloneTree();
            container.Q<EnumField>("instruction").bindingPath = property.FindPropertyRelative("Instruction").propertyPath;
            container.Q<TextField>("domain").bindingPath = property.FindPropertyRelative("Domain").propertyPath;
            container.Q<TextField>("value").bindingPath = property.FindPropertyRelative("Value").propertyPath;
            return container;
        }
    }
}
