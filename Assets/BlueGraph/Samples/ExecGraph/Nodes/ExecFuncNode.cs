
using UnityEngine;
using BlueGraph;

namespace BlueGraphExamples.ExecGraph
{
    /// <summary>
    /// Wrapper around FuncNode to automatically add Exec IO ports
    /// </summary>
    public class ExecFuncNode : FuncNode, ICanExec
    {
        public ExecData execIn;
        protected readonly ExecData execOut;
        
        public override void Awake()
        {
            // Automatically add exec ports if they don't exist
            if (GetPort("_execIn") == null)
            {
                AddPort(new Port()
                {
                    node = this,
                    name = "_execIn",
                    acceptsMultipleConnections = true,
                    isInput = true,
                    type = typeof(ExecData),
                    fieldName = "execIn"
                });
            }
            
            if (GetPort("_execOut") == null)
            {
                AddPort(new Port()
                {
                    node = this,
                    name = "_execOut",
                    acceptsMultipleConnections = false,
                    isInput = false,
                    type = typeof(ExecData),
                    fieldName = "execOut"
                });
            }
            
            base.Awake();
        }

        public ICanExec Execute(ExecData data)
        {
            ExecuteMethod();
            return GetNextExec();
        }
        
        /// <summary>
        /// Get the next node that should be executed
        /// </summary>
        /// <returns></returns>
        public virtual ICanExec GetNextExec(string portName = "_execOut")
        {
            Port port = GetPort(portName);
            if (!port.IsConnected) {
                return null;
            }
            
            if (port.ConnectedPorts[0].node is ICanExec node)
            {
                return node;
            }

            Debug.LogWarning(
                $"<b>[{name}]</b> Output is not an instance of ICanExec. " +
                $"Cannot execute past this point."
            );

            return null;
        }
    }
}
