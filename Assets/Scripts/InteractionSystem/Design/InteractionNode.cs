using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

using Data;
namespace Interactions {
    [System.Serializable]
    public class Interaction {

    }

    [System.Serializable]
    public struct FlagInteractions {
        public List<Flag> flags;
        public bool FlagsMeetRequirements(HashSet<string> ref_flags) {
            if (ref_flags == null) {
                return flags.TrueForAll((flag) => flag.Instruction != FlagInstruction.Require);
            }
            foreach (var flag_req in this.flags) {
                if (flag_req.Instruction == FlagInstruction.Require && !ref_flags.Contains(flag_req.Value.ToLower())) {
                    return false;
                } else if (flag_req.Instruction == FlagInstruction.Disables && ref_flags.Contains(flag_req.Value.ToLower())) {
                    return false;
                }
            }
            return true;
        }
    }

    [Node("Interaction", module = "Interactions", help = "Base node for interactions. Can be used to silently set flags.")]
    public class InteractionNode : AbstractNode
    {
        [Input("prev", multiple = true)] public Interaction prev;
        [Output("next", multiple = false)] public Interaction next;
        [Editable] public FlagInteractions flagInteractions;
        public virtual bool HasInput() { return true; }
        public virtual bool HasOutput() { return true; }

        public override void OnAddedToGraph() {
            var prev_port = this.GetPort("prev");
            var next_port = this.GetPort("next");
            if (this.HasInput()) {
                if (prev_port == null) {
                    this.AddPort(new Port{
                        name = "prev",
                        type = typeof(Interaction),
                        acceptsMultipleConnections = true,
                        fieldName = "prev",
                        isInput = true
                    });
                }
            } else {
                if (prev_port != null) {
                    this.RemovePort(prev_port);
                }
            }
            if (this.HasOutput()) {
                if (next_port == null) {
                    this.AddPort(new Port{
                        name = "next",
                        type = typeof(Interaction),
                        acceptsMultipleConnections = false,
                        fieldName = "next",
                        isInput = false
                    });
                }
            } else {
                if (next_port != null) {
                    this.RemovePort(next_port);
                }
            }
            base.OnAddedToGraph();
        }

        public virtual InteractionNode GetNextNode() {
			var port = GetPort("next");
			if (port == null) return null;
			if (!port.IsConnected) return null;
			return port.ConnectedPorts[0].node as InteractionNode;
		}

        public bool FlagsMeetRequirements(HashSet<string> Flags) {
            return flagInteractions.FlagsMeetRequirements(Flags);
        }

    }

}