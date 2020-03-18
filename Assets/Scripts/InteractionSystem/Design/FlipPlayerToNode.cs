using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions
{
	[Node("Flip Player", module = "Helpers", help = "Flip to player to a new location.")]
	public class FlipPlayerNode : InteractionNode {
        [Editable] public PlayerController player;
        [Editable] public Transform destination;
	}
}