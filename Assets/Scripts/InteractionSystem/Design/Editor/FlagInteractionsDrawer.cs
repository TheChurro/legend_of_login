using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using SOTU.Editor;
namespace Interactions.Editor {
    [CustomPropertyDrawer(typeof(FlagInteractions))]
    public class FlagInteractionsDrawer : PropertyDrawer
    {
        private SerializedProperty flag_prop;
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            flag_prop = property.FindPropertyRelative("flags");
            // Create property container element.
            var container = new VisualElement();

            var list_elem = new ArrayInspectorElement(flag_prop, MakeItem);
            // Add fields to the container.
            container.Add(list_elem);

            return container;
        }

        private VisualElement MakeItem(string propertyPath, int index) {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = new StyleFloat(1.0f);
            var remove_button = new Button(() => {
                flag_prop.DeleteArrayElementAtIndex(index);
                flag_prop.serializedObject.ApplyModifiedProperties();
            });
            remove_button.text = "-";
            remove_button.style.color = new StyleColor(UnityEngine.Color.white);
            remove_button.style.backgroundColor = new StyleColor(UnityEngine.Color.red);
            remove_button.style.marginBottom = new StyleLength(0.0);
            remove_button.style.marginLeft = new StyleLength(0.0);
            remove_button.style.marginTop = new StyleLength(0.0);
            remove_button.style.marginRight = new StyleLength(0.0);
            remove_button.style.borderBottomLeftRadius = new StyleLength(0.0);
            remove_button.style.borderTopLeftRadius = new StyleLength(0.0);
            remove_button.style.borderTopRightRadius = new StyleLength(0.0);
            remove_button.style.borderBottomRightRadius = new StyleLength(0.0);
            container.Add(remove_button);
            var pf = new PropertyField(null, "");
            pf.bindingPath = propertyPath;
            pf.style.flexGrow = new StyleFloat(1.0f);
            container.Add(pf);
            return container;
        }
    }
}
