using UnityEngine;
using BlueGraph;
using System.Collections.Generic;

namespace Interactions {
    [Node("And", module = "Operations/Boolean", help = "And of all values going into the inputs port.")]
    public class AndNode : AbstractNode{
        [Input("inputs", multiple = true)] public bool inputs;
        [Output("output", multiple = true)] public bool and;
        public override object GetOutputValue(string port_name) {
            if (!port_name.Equals("output")) throw new System.Exception($"Port {port_name} does not exist on boolean And");
            Port p = GetPort("inputs");
            if (p == null) {
                throw new System.Exception($"Port inputs does not exist on and node but should");
            }
            foreach (Port connection in p.ConnectedPorts) {
                object val = connection.node.GetOutputValue(connection.name);
                if (val is bool b) {
                    if (!b) return false;
                } else {
                    throw new System.Exception($"Port {connection.name} on node {connection.node.id} is not a boolean");
                }
            }
            return true;
        }
    }

    [Node("Or", module = "Operations/Boolean", help = "Or of all values going into the inputs port.")]
    public class OrNode : AbstractNode{
        [Input("inputs", multiple = true)] public bool inputs;
        [Output("output", multiple = true)] public bool or;
        public override object GetOutputValue(string port_name) {
            if (!port_name.Equals("output")) throw new System.Exception($"Port {port_name} does not exist on boolean Or");
            Port p = GetPort("inputs");
            if (p == null) {
                throw new System.Exception($"Port inputs does not exist on and node but should");
            }
            foreach (Port connection in p.ConnectedPorts) {
                object val = connection.node.GetOutputValue(connection.name);
                if (val is bool b) {
                    if (b) return true;
                } else {
                    throw new System.Exception($"Port {connection.name} on node {connection.node.id} is not a boolean");
                }
            }
            return false;
        }
    }

    [Node("Xor", module = "Operations/Boolean", help = "Exclusive or of all values going into the inputs port.")]
    public class XorNode : AbstractNode{
        [Input("inputs", multiple = true)] public bool inputs;
        [Output("output", multiple = true)] public bool or;
        public override object GetOutputValue(string port_name) {
            if (!port_name.Equals("output")) throw new System.Exception($"Port {port_name} does not exist on boolean Xor");
            Port p = GetPort("inputs");
            if (p == null) {
                throw new System.Exception($"Port inputs does not exist on and node but should");
            }
            bool output = false;
            foreach (Port connection in p.ConnectedPorts) {
                object val = connection.node.GetOutputValue(connection.name);
                if (val is bool b) {
                    if (b) {
                        if (output) return false;
                        output = true;
                    }
                } else {
                    throw new System.Exception($"Port {connection.name} on node {connection.node.id} is not a boolean");
                }
            }
            return output;
        }
    }

    [Node("Not", module = "Operations/Boolean", help = "Not of value going into input port.")]
    public class NotNode : AbstractNode{
        [Input("input", multiple = false)] public bool input;
        [Output("output", multiple = true)] public bool or;
        public override object GetOutputValue(string port_name) {
            if (!port_name.Equals("output")) throw new System.Exception($"Port {port_name} does not exist on boolean Not");
            return !GetInputValue<bool>("input");
        }
    }
}