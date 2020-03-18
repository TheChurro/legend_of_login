using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using BlueGraph.Editor;
using BlueGraph;

using SOTU.Editor;

namespace Interactions.Editor
{
    [CustomEditor(typeof(InteractionsGraph))]
    public class InteractionsEditor : GraphEditor
    {
        protected override VisualElement BuildSideMenu(Graph graph, SerializedObject graph_serialized) {

            var variables_prop = graph_serialized.FindProperty("variables");
            var inspector = new ArrayInspectorElement(
                variables_prop,
                (string prop_path, int index) => MakeItem(variables_prop, prop_path, index)
            );
            inspector.Bind(graph_serialized);
            inspector.styleSheets.Add(Resources.Load<StyleSheet>("Styles/NodeView"));
            inspector.AddToClassList("graphView");
            inspector.AddToClassList("nodeView");
            return inspector;
        }

        private VisualElement MakeItem(SerializedProperty prop, string prop_path, int index) {
            Debug.Log("MAKING ITEM!");
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexGrow = new StyleFloat(1.0f);
            var remove_button = new Button(() => {
                prop.DeleteArrayElementAtIndex(index);
                prop.serializedObject.ApplyModifiedProperties();
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
            var pf = new PropertyField(prop.GetArrayElementAtIndex(index), "");
            pf.style.flexGrow = new StyleFloat(1.0f);
            container.Add(pf);
            return container;
        }
    }
}

