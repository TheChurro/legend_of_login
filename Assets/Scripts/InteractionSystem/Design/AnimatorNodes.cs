using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions
{
	[Node("Set Animator Boolean", module = "Operations/Animator", help = "Set a boolean value on an animation controller.")]
	public class SetAnimatorBooleanNode : InteractionNode {
        [Editable] public string parameter_name;
        [Input("value")] public bool value;
        [Editable] public Animator animator;

        public string ParameterName {
            get {
                return parameter_name;
            }
        }

        public bool Value { get {
                return GetInputValue<bool>("value", value);
            }
        }

        public Animator Animator {
            get {
                return animator;
            }
        }
	}
}