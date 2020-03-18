using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions
{
	[Node("Entry", module = "Interactions", help = "Entry point for interactions.")]
	public class InteractionEntryNode : InteractionNode {
		[Editable] public string Action;
		public override bool HasInput() { return false; }
	}
}