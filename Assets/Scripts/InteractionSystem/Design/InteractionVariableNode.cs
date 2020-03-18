using UnityEngine;
using BlueGraph;
using System.Collections.Generic;

namespace Interactions {

    public abstract class VarNode : AbstractNode
    {
        [Editable] public string var_name;
        [Input(name="target")] public InteractionsGraph target;

        public abstract InteractionsVariableType VarType { get; }

        public override object GetOutputValue(string port_name)
        {
            InteractionsGraph graph = GetInputValue<InteractionsGraph>("target", target);
            if (graph == null) {
              Port p = GetPort("target");
              if (p != null && p.IsConnected) return null;
              if (this.graph is InteractionsGraph) {
                graph = this.graph as InteractionsGraph;
              } else {
                throw new System.Exception("Cannot use variables in non-interaction graphs");
              }
            }
            if (graph.GetVariable(var_name, out var var)) {
                if (var.var_type == VarType) {
                    return var.value;
                } else {
                    throw new System.Exception($"Variable {var.name} exists but is of type {var.var_type} not {VarType}");
                }
            }
            throw new System.Exception($"Variable {var_name} does not exist");
        }
    }

    [Node("Get String", module = "Variables", help = "Get a string variable.")]
    public class GetStringVarNode : VarNode
    {
        [Output] public string output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.String; } }
    }

    [Node("Get Int", module = "Variables", help = "Get an integer variable.")]
    public class GetIntVarNode : VarNode
    {
        [Output] public int output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Integer; } }
    }

    [Node("Get Boolean", module = "Variables", help = "Get a boolean variable.")]
    public class GetBoolVarNode : VarNode
    {
        [Output] public bool output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Boolean; } }
    }

    [Node("Get Float", module = "Variables", help = "Get a float variable.")]
    public class GetFloatVarNode : VarNode
    {
        [Output] public float output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Float; } }
    }

    [Node("Get Vector 2", module = "Variables", help = "Get a 2D vector variable.")]
    public class GetVec2VarNode : VarNode
    {
        [Output] public Vector2 output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Vector2; } }
    }

    [Node("Get Vector 3", module = "Variables", help = "Get a 3D vector variable.")]
    public class GetVec3VarNode : VarNode
    {
        [Output] public Vector3 output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Vector3; } }
    }

    [Node("Get Interactable", module = "Variables", help = "Get an interactable GameObject variable.")]
    public class GetInteractableNode : VarNode
    {
        [Output] public InteractionsGraph output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.InteractionsGraph; } }
    }

    [Node("Get Animator", module = "Variables", help = "Get an animator component variable.")]
    public class GetAnimatorNode : VarNode
    {
        [Output] public InteractionsGraph output;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Animator; } }
    }

    public abstract class VarSetNode : InteractionNode
    {
        [Editable] public string var_name;
        [Input(name="target")] public InteractionsGraph target;

        public abstract InteractionsVariableType VarType { get; }
        public abstract object DefaultValue { get; }
        public object Input {
            get {
                return GetInputValue("input", DefaultValue);
            }
        }
        public InteractionsGraph Target {
          get {
            InteractionsGraph graph = GetInputValue<InteractionsGraph>("target", target);
            if (target == null) {
              Port p = GetPort("target");
              if (p != null && p.IsConnected) return null;
              if (this.graph is InteractionsGraph) {
                return this.graph as InteractionsGraph;
              }
            }
            return target;
          }
        }
    }

    [Node("Set String", module = "Variables", help = "Set the value of a string variable.")]
    public class SetStringVarNode : VarSetNode
    {
        [Input(name = "input")] public string input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.String; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Int", module = "Variables", help = "Set the value of an integer variable.")]
    public class SetIntVarNode : VarSetNode
    {
        [Input(name = "input")] public int input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Integer; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Boolean", module = "Variables", help = "Set the value of a boolean variable.")]
    public class SetBoolVarNode : VarSetNode
    {
        [Input(name = "input")] public bool input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Boolean; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Float", module = "Variables", help = "Set the value of a float variable.")]
    public class SetFloatVarNode : VarSetNode
    {
        [Input(name = "input")] public float input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Float; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Vector 2", module = "Variables", help = "Set the value of a 2D vector variable.")]
    public class SetVec2VarNode : VarSetNode
    {
        [Input(name = "input")] public Vector2 input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Vector2; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Vector 3", module = "Variables", help = "Set the value of a 3D vector variable.")]
    public class SetVec3VarNode : VarSetNode
    {
        [Input(name = "input")] public Vector3 input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Vector3; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Interactable", module = "Variables", help = "Set the value of an interactable GameObject variable.")]
    public class SetInteractableNode : VarSetNode
    {
        [Input(name = "input")] public InteractionsGraph input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.InteractionsGraph; } }
        public override object DefaultValue { get { return input; } }
    }

    [Node("Set Animator", module = "Variables", help = "Set the value of an animator component variable.")]
    public class SetAnimatorNode : VarSetNode
    {
        [Input(name = "input")] public Animator input;

        public override InteractionsVariableType VarType { get { return InteractionsVariableType.Animator; } }
        public override object DefaultValue { get { return input; } }
    }
}
