using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueGraph;

namespace Interactions
{
	[Node("Set Gameobject Enabled", module = "Gameobject", help = "Change gameobject enabled state.")]
	public class SetGameobjectEnabled : InteractionNode {
        [Input("gameobject")] public GameObject gameobject;
        [Input("value")] public bool value;

        public bool Value { get {
                return GetInputValue<bool>("value", value);
            }
        }

        public GameObject Gameobject {
            get {
                Debug.Log($"DEFAULT IS {(gameobject == null ? "null" : gameobject.name)}");
                return GetInputValue<GameObject>("gameobject", gameobject);
            }
        }
	}

    [Node("Destroy Gameobject", module = "Gameobject", help = "Destroy a gameobject")]
	public class DestroyGameobjectNode : InteractionNode {
        [Input("gameobject")] public GameObject gameobject;
        public GameObject Gameobject {
            get {
                return GetInputValue<GameObject>("gameobject", gameobject);
            }
        }
	}

    [Node("Get Gameobject", module = "Gameobject", help = "Get the gameobject of a monobehaviour")]
	public class GetGameobjectNode : AbstractNode {
        [Input("behaviour")] public MonoBehaviour behaviour;
        public MonoBehaviour Behaviour {
            get {
                return GetInputValue<MonoBehaviour>("behaviour", behaviour);
            }
        }
        [Output("gameobect")] public GameObject Gameobject;
        public override object GetOutputValue(string port_name) {
            if (port_name.Equals("gameobect")) {
                var behaviour = Behaviour;
                Debug.Log($"Behaviour: {behaviour}");
                if (behaviour != null) {
                    return behaviour.gameObject;
                } else {
                    throw new System.Exception("Cannot get gameobject from null MonoBehaviour");
                }
            }
            throw new System.Exception($"Output value {port_name} does not exist on a Self node.");
        }
	}
}