using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions
{
	[Node("Branch", module = "Interactions", help = "Switch between two branches based off a boolean value.")]
	public class BranchNode : InteractionNode {
        [Input("condition")] public bool condition;
        [Output("when true", multiple = false)] public Interaction when_true;
        [Output("when false", multiple = false)] public Interaction when_false;
		public override bool HasOutput() { return false; }

        public bool Condition { get {
                return GetInputValue<bool>("condition", condition);
            }
        }

        public override InteractionNode GetNextNode() {
            Port port = null;
			if (Condition) {
                port = GetPort("when true");
            } else {
                port = GetPort("when false");
            }
			if (port == null) return null;
			if (!port.IsConnected) return null;
			return port.ConnectedPorts[0].node as InteractionNode;
		}
	}
}