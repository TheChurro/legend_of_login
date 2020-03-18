using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Interactions;
namespace Interactions.Editor {

    public class InteractionsVariableInspector : VisualElement {
        private InteractionsVariableType var_type = InteractionsVariableType.Boolean;
        private SerializedProperty interactions_variable_serialized;

        private EnumField type_selector;
        private TextField name_field;
        private IntegerField int_selector;
        private FloatField float_selector;
        private Vector2Field vec2_selector;
        private Vector3Field vec3_selector;
        private Toggle bool_selector;
        private TextField string_selector;
        private PropertyField interaction_selector;
        private PropertyField animator_selector;

        public InteractionsVariableInspector(SerializedProperty property) {
            var container = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Resources/InteractionVariable.uxml").CloneTree();
            type_selector = container.Q<EnumField>("type-selector");
            var type_prop = property.FindPropertyRelative("var_type");
            type_selector.bindingPath = type_prop.propertyPath;
            type_selector.RegisterValueChangedCallback((ChangeEvent<System.Enum> evnt) => {
                if (evnt.newValue is InteractionsVariableType var_type) {
                    this.var_type = var_type;
                    UpdateDisplay();
                }
            });
            name_field = container.Q<TextField>("name-field");
            name_field.bindingPath = property.FindPropertyRelative("name").propertyPath;
            int_selector = container.Q<IntegerField>("int-selector");
            int_selector.bindingPath = property.FindPropertyRelative("int_val").propertyPath;
            float_selector = container.Q<FloatField>("float-selector");
            float_selector.bindingPath = property.FindPropertyRelative("float_val").propertyPath;
            vec2_selector = container.Q<Vector2Field>("vec2-selector");
            vec2_selector.bindingPath = property.FindPropertyRelative("vec2_val").propertyPath;
            vec3_selector = container.Q<Vector3Field>("vec3-selector");
            vec3_selector.bindingPath = property.FindPropertyRelative("vec3_val").propertyPath;
            bool_selector = container.Q<Toggle>("bool-selector");
            bool_selector.bindingPath = property.FindPropertyRelative("bool_val").propertyPath;
            string_selector = container.Q<TextField>("string-selector");
            string_selector.bindingPath = property.FindPropertyRelative("string_val").propertyPath;
            interaction_selector = container.Q<PropertyField>("interaction-selector");
            interaction_selector.bindingPath = property.FindPropertyRelative("graph_val").propertyPath;
            animator_selector = container.Q<PropertyField>("animator-selector");
            animator_selector.bindingPath = property.FindPropertyRelative("animator_val").propertyPath;
            var_type = InteractionsVariableTypeExtensions.TypesByIndex[type_prop.enumValueIndex];
            UpdateDisplay();
            Add(container);
        }

        private DisplayStyle MatchesType(InteractionsVariableType ref_type) {
            return ref_type == var_type ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void UpdateDisplay() {
            int_selector.style.display = MatchesType(InteractionsVariableType.Integer);
            float_selector.style.display = MatchesType(InteractionsVariableType.Float);
            string_selector.style.display = MatchesType(InteractionsVariableType.String);
            bool_selector.style.display = MatchesType(InteractionsVariableType.Boolean);
            vec2_selector.style.display = MatchesType(InteractionsVariableType.Vector2);
            vec3_selector.style.display = MatchesType(InteractionsVariableType.Vector3);
            interaction_selector.style.display = MatchesType(InteractionsVariableType.InteractionsGraph);
            animator_selector.style.display = MatchesType(InteractionsVariableType.Animator);
        }
    }

    [CustomPropertyDrawer(typeof(InteractionsVariable))]
    public class InteractionsVariableDrawer : PropertyDrawer
    {
        private DisplayStyle MatchesType<T>(T a, T b) {
            return a.Equals(b) ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new InteractionsVariableInspector(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));
            var var_prop = property.FindPropertyRelative("var_type");
            EditorGUILayout.PropertyField(var_prop, new GUIContent("Type"));
            switch (InteractionsVariableTypeExtensions.TypesByIndex[var_prop.enumValueIndex]) {
                case InteractionsVariableType.Integer:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("int_val"), new GUIContent("Value"));
                    break;
                case InteractionsVariableType.Boolean:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("bool_val"), new GUIContent("Value"));
                    break;
                case InteractionsVariableType.String:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("string_val"), new GUIContent("Value"));
                    break;
                case InteractionsVariableType.Float:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("float_val"), new GUIContent("Value"));
                    break;
                case InteractionsVariableType.Vector2:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("vec2_val"), new GUIContent("Value"));
                    break;
                case InteractionsVariableType.Vector3:
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("vec3_val"), new GUIContent("Value"));
                    break;
            }
        }
    }
}