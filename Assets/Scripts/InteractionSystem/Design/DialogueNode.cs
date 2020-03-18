using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions.Dialogue
{
    [Node("Dialogue", module = "Interactions", help = "Will show dialogue on screen when you reach this node.")]
    public class DialogueNode : InteractionNode {
        [Editable] public CharacterInfo Character;
        [Editable] public string Dialogue;
    }
}