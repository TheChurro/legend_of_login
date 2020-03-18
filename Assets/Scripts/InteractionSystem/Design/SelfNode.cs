
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;
using Data;

namespace Interactions
{
	[Node("Self", module = "Variables", help = "Returns a reference to this interactable")]
	public class SelfNode : AbstractNode {
        [Output("self")] public InteractionsGraph self;
        public override object GetOutputValue(string port_name) {
            if (port_name.Equals("self")) {
                if (this.graph is InteractionsGraph graph) {
                    return graph;
                } else {
                    throw new System.Exception("Cannot use self on non-Interactions Graphs");
                }
            }
            throw new System.Exception($"Output value {port_name} does not exist on a Self node.");
        }
	}
}