
using BlueGraph;
using BlueGraph.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using SOTU.Editor;
using Interactions.Editor;
namespace Interactions.Dialogue.Editor
{
    // // [CustomNodeView(typeof(DialogueChoiceNode))]
    // class DialogueChoiceNodeView : InteractionNodeView
    // {
    //     private ArrayInspectorElement array_inspector;
    //     protected override void OnInitialize()
    //     {
    //         if (!(target is DialogueChoiceNode)) {
    //             base.OnInitialize();
    //             return;
    //         }
    //         array_inspector = new ArrayInspectorElement(m_SerializedNode.FindPropertyRelative("answers"), MakeItem, OnAddItem);
    //         this.extensionContainer.Add(array_inspector);
    //         array_inspector.BindProperty(m_SerializedNode.FindPropertyRelative("answers"));
    //         array_inspector.RegisterCallback((FocusOutEvent e) => OnPropertyChange());
    //         RefreshExpandedState();
    //     }

    //     private VisualElement MakeItem(string property_path, int index) {
    //         var container = new VisualElement();

    //         container.style.flexDirection = FlexDirection.Row;
    //         container.style.flexGrow = new StyleFloat(1.0f);
    //         var remove_button = new Button(() => {
    //             var choice_node = (target as DialogueChoiceNode);
    //             Debug.Log("REMOVING AT " + index + " [SIZE: " + choice_node.answers.Length + "]");
    //             var answers_prop = m_SerializedNode.FindPropertyRelative("answers");
    //             var guid = answers_prop.GetArrayElementAtIndex(index).FindPropertyRelative("guid").stringValue;
    //             answers_prop.DeleteArrayElementAtIndex(index);
    //             answers_prop.serializedObject.ApplyModifiedProperties();
    //             choice_node.RemovePort(choice_node.GetPort("answer " + guid));
    //         });
    //         remove_button.text = "-";
    //         remove_button.style.color = new StyleColor(UnityEngine.Color.white);
    //         remove_button.style.backgroundColor = new StyleColor(UnityEngine.Color.red);
    //         remove_button.style.marginBottom = new StyleLength(0.0);
    //         remove_button.style.marginLeft = new StyleLength(0.0);
    //         remove_button.style.marginTop = new StyleLength(0.0);
    //         remove_button.style.marginRight = new StyleLength(0.0);
    //         remove_button.style.borderBottomLeftRadius = new StyleLength(0.0);
    //         remove_button.style.borderTopLeftRadius = new StyleLength(0.0);
    //         remove_button.style.borderTopRightRadius = new StyleLength(0.0);
    //         remove_button.style.borderBottomRightRadius = new StyleLength(0.0);
    //         container.Add(remove_button);
    //         var pf = new PropertyField(null, "");
    //         pf.bindingPath = property_path;
    //         pf.style.flexGrow = new StyleFloat(1.0f);
    //         container.Add(pf);
    //         var out_port = GetOutputPort("answer " + m_SerializedNode.FindPropertyRelative(property_path).FindPropertyRelative("guid").stringValue);
    //         out_port.SetEditorField(container);
    //         // container.Add(GetOutputPort("Answer " + (target as DialogueChoiceNode).answers[index].guid.ToString()));
    //         // RefreshExpandedState();
            
    //         return container;
    //     }

    //     private void OnAddItem(string property_path, int index) {
    //         // Debug.Log("PROPERTY PATH: " + property_path);
    //         var answer_prop = m_SerializedNode.FindPropertyRelative(property_path);
    //         if (answer_prop != null) {
    //             Debug.Log("FOUND ANSWER PROP!");
    //             var new_guid = UnityEditor.GUID.Generate().ToString();
    //             answer_prop.FindPropertyRelative("guid").stringValue = new_guid;
    //             target.AddPort(new Port {
    //                 name = "answer " + new_guid,
    //                 acceptsMultipleConnections = false,
    //                 type = typeof(Interaction),
    //                 fieldName = property_path,
    //                 isInput = false
    //             });
    //         }
    //         OnPropertyChange();
    //     }
    // }
}
