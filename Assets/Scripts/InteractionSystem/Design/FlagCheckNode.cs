using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;
using Data;

namespace Interactions
{
	[Node("Flag Check", module = "Operations/Boolean", help = "Returns true when the flag conditions are met.")]
	public class FlagCheckNode : AbstractNode {
        [Output("conditions met")] public bool conditions_met;
        [Editable] public FlagInteractions flag_requirements;
        public override object GetOutputValue(string port_name) {
            if (port_name.Equals("conditions met")) {
                if (this.graph is InteractionsGraph graph) {
                    return flag_requirements.FlagsMeetRequirements(graph.active_context.flags);
                } else {
                    throw new System.Exception("Cannot use Flag Check node on non-interaction graphs");
                }
            }
            throw new System.Exception($"Output value {port_name} does not exist on a Flag Check node.");
        }
	}
}