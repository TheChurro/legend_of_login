using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions.Dialogue
{
    [System.Serializable]
    public class Answer : Interactions.Interaction {
        public string list_text;
        public bool show_preview;
        public bool unique_preview;
        public string preview;
    }
    [Node("Choice", module = "Interactions", help = "Will show dialogue on screen and a choice when you reach this node.")]
    public class DialogueChoiceNode : DialogueNode
    {
        [Output(list=true, list_type=typeof(Answer))] public List<Answer> answers = new List<Answer>();

        public InteractionNode GetNextNodeForChoice(int answer) {
            var port = GetPort($"answers[{answer}]");
			if (port == null) return null;
			if (!port.IsConnected) return null;
			return port.ConnectedPorts[0].node as InteractionNode;
        }

        public override bool HasOutput() { return false; }
    }
}
