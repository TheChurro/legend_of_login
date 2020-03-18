
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BlueGraph.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomNodeViewAttribute : Attribute
    {
        public Type nodeType;

        public CustomNodeViewAttribute(Type nodeType)
        {
            this.nodeType = nodeType; 
        }
    }

    public class PortListView : BindableElement, INotifyValueChanged<int> {
        public override VisualElement contentContainer => list_container;
        public AbstractNode parent_node;
        public VisualElement list_container;
        public Button add_button;
        public List<PortView> items;
        public int item_size;
        public Type list_type;
        public string list_name;
        private EdgeConnectorListener m_ConnectorListener;
        private SerializedProperty m_SerializedList;
        private Action m_OnPropertyChange;
        private Action m_UpdateBinding;
        public void UpdateBinding(AbstractNode new_node) {
            parent_node = new_node;
            List<PortView> to_remove = new List<PortView>();
            foreach (var view in items) {
                var new_port = new_node.GetPort(view.target.name);
                if (new_port != null) {
                    view.target = new_port;
                } else {
                    to_remove.Add(view);
                }
            }
            foreach (var view in to_remove) {
                items.Remove(view);
                view.RemoveFromHierarchy();
            }
        }
        public PortListView(AbstractNode parent_node, Type list_type, string list_name, SerializedProperty serializedNode, EdgeConnectorListener connectorListener, Action onPropertyChange, Action updateBinding) {
            m_ConnectorListener = connectorListener;
            m_SerializedList = serializedNode.FindPropertyRelative(list_name);
            m_OnPropertyChange = onPropertyChange;
            m_UpdateBinding = updateBinding;
            items = new List<PortView>();
            list_container = new VisualElement();
            this.list_name = list_name;
            this.list_type = list_type;
            this.parent_node = parent_node;
            add_button = new Button(() => {
                var size = m_SerializedList.arraySize;
                // Add an output port for this array element
                m_UpdateBinding();
                this.parent_node.AddPort(new Port{
                    name = $"{list_name}[{size}]",
                    isInput = false,
                    acceptsMultipleConnections = false,
                    type = list_type,
                    fieldName = $"{m_SerializedList.propertyPath}.Array.data[{size}]"
                });
                m_SerializedList.serializedObject.Update();
                m_UpdateBinding();
                m_SerializedList.InsertArrayElementAtIndex(size);
                m_SerializedList.serializedObject.ApplyModifiedProperties();
                m_UpdateBinding();
                m_OnPropertyChange();
            });
            add_button.text = list_name;
            add_button.style.color = new StyleColor(Color.white);
            add_button.style.backgroundColor = new StyleColor(Color.green);
            this.hierarchy.Add(add_button);
            this.hierarchy.Add(list_container);

            var property = m_SerializedList.Copy();
            var endProperty = property.GetEndProperty();
    
            //We prefill the container since we know we will need this
            property.NextVisible(true); // Expand the first child.
            do
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                    break;
                if (property.propertyType == SerializedPropertyType.ArraySize)
                {
                    item_size = property.intValue;
                    bindingPath = property.propertyPath;
                    break;
                }
            }
            while (property.NextVisible(false)); // Never expand children.
            UpdateCreatedItems();
            this.Bind(m_SerializedList.serializedObject);
        }

        private void RemoveItem(int i) {
            items[i].RemoveFromHierarchy();
            items.RemoveAt(i);
        }
        private VisualElement AddItem(string property_path, int index) {
            var port_name = $"{this.list_name}[{index}]";
            var port = parent_node.GetPort(port_name);
            var prop = m_SerializedList.GetArrayElementAtIndex(index);
            if (port == null) {
                Debug.Log("Added port!");
                port = new Port{
                    name = port_name,
                    isInput = false,
                    acceptsMultipleConnections = false,
                    type = list_type,
                    fieldName = prop.propertyPath
                };
                parent_node.AddPort(port);
                m_SerializedList.serializedObject.Update();
            }
            var view = PortView.Create(port, port.type, m_ConnectorListener);
            view.hideEditorFieldOnConnection = false;
            
            var field = new PropertyField(prop, "");
            field.Bind(m_SerializedList.serializedObject);
            field.RegisterCallback((FocusOutEvent e) => m_OnPropertyChange());

            var container = new VisualElement();
            container.AddToClassList("property-field-container");
            container.style.flexDirection = FlexDirection.Row;
            var remove_button = new Button(() => {
                m_UpdateBinding();
                m_SerializedList.DeleteArrayElementAtIndex(index);
                m_SerializedList.serializedObject.ApplyModifiedProperties();
                m_UpdateBinding();
                int new_size = m_SerializedList.arraySize;
                var canvas = GetFirstAncestorOfType<CanvasView>();
                var connections = new List<Edge>(items[index].connections);
                foreach (var connection in connections) {
                    canvas.RemoveEdge(connection, false);
                }
                for (int i = index; i < new_size; i++) {
                    connections = new List<Edge>(items[i + 1].connections);
                    foreach (Edge connection in connections) {
                        var connect_to = connection.input;
                        canvas.RemoveEdge(connection, false);
                        if (connect_to != null) {
                            canvas.AddEdge(new Edge{
                                input = connect_to,
                                output = items[i]
                            }, false);
                        }
                    }
                }
                canvas.DirtyAll();
                // Remove the last port from the list!
                parent_node.RemovePort(parent_node.GetPort($"{list_name}[{new_size}]"));
                m_SerializedList.serializedObject.Update();
                m_UpdateBinding();
                m_OnPropertyChange();
            });
            remove_button.text = "-";
            remove_button.style.color = new StyleColor(UnityEngine.Color.white);
            remove_button.style.backgroundColor = new StyleColor(UnityEngine.Color.red);
            remove_button.style.marginBottom = new StyleLength(0.0);
            remove_button.style.marginLeft = new StyleLength(0.0);
            remove_button.style.marginTop = new StyleLength(0.0);
            remove_button.style.marginRight = new StyleLength(0.0);
            remove_button.style.borderBottomLeftRadius = new StyleLength(0.0);
            remove_button.style.borderTopLeftRadius = new StyleLength(0.0);
            remove_button.style.borderTopRightRadius = new StyleLength(0.0);
            remove_button.style.borderBottomRightRadius = new StyleLength(0.0);
            container.Add(remove_button);
            container.Add(field);
            field.style.flexGrow = new StyleFloat(1.0f);

            view.SetEditorField(container);
    
            this.items.Add(view);
            Add(view);
            m_OnPropertyChange();
            return view;
        }
        private bool UpdateCreatedItems() {
            int currentSize = this.items.Count;
    
            int targetSize = item_size;
    
            if (targetSize < currentSize)
            {
                for (int i = currentSize-1; i >= targetSize; --i)
                {
                    RemoveItem(i);
                }
                return true;
            }
            else if (targetSize > currentSize)
            {
                for (int i = currentSize; i < targetSize; ++i)
                {
                    AddItem($"{m_SerializedList.propertyPath}.Array.data[{i}]", i);
                }
    
                return true; //we created new Items
            }
    
            return false;
        }

        public void SetValueWithoutNotify(int newSize)
        {
            this.item_size = newSize;
    
            if (UpdateCreatedItems())
            {
                //We rebind the array
                this.Bind(m_SerializedList.serializedObject);
            }
        }
    
        public int value
        {
            get => item_size;
            set
            {
                if (item_size == value) return;
            
                if (panel != null)
                {
                    using (ChangeEvent<int> evt = ChangeEvent<int>.GetPooled(item_size, value))
                    {
                        evt.target = this;
                    
                        // The order is important here: we want to update the value, then send the event,
                        // so the binding writes and updates the serialized object
                        item_size = value;
                        SendEvent(evt);
                    
                        //Then we remove or create + bind the needed items
                        SetValueWithoutNotify(value);
                    }
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }
    }

    public class NodeView : Node, ICanDirty
    {
        public AbstractNode target;
        
        public List<PortView> inputs = new List<PortView>();
        public List<PortView> outputs = new List<PortView>();
        public Dictionary<string, PortListView> listOutputs = new Dictionary<string, PortListView>();
        
        public CommentView comment;

        protected EdgeConnectorListener m_ConnectorListener;
        protected SerializedProperty m_SerializedNode;

        public void Initialize(AbstractNode node, SerializedProperty serializedNode, EdgeConnectorListener connectorListener)
        {
            // Check to see if we can find ourselves in the serialized object for the node. If not
            // we will replace node with the proper serialized object...
            if (serializedNode.serializedObject.targetObject is Graph target_graph) {
                bool found_self = false;
                int found_id = -1;
                for (int i = 0; i < target_graph.nodes.Count; i++) {
                    if (target_graph.nodes[i] == node) {
                        found_self = true;
                        Debug.Log("Found Self!");
                    }
                    if (target_graph.nodes[i].id.Equals(node.id)) {
                        Debug.Log($"Found ID: {i}");
                        found_id = i;
                    }
                }
                if (!found_self) {
                    Debug.Log("Did not find self.");
                    if (found_id != -1) {
                        node = target_graph.nodes[found_id];
                    }
                }
            }
            viewDataKey = node.id.ToString();
            target = node;

            m_SerializedNode = serializedNode;
            m_ConnectorListener = connectorListener;
            
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/NodeView"));
            AddToClassList("nodeView");
            
            SetPosition(new Rect(node.graphPosition, Vector2.one));
            title = node.name;
            
            // Custom OnDestroy() handler via https://forum.unity.com/threads/request-for-visualelement-ondestroy-or-onremoved-event.718814/
            RegisterCallback<DetachFromPanelEvent>((e) => OnDestroy());
            RegisterCallback<TooltipEvent>(OnTooltip);

            UpdatePorts();

            OnInitialize();

            // TODO: Don't do it this way. Move to an OnInitialize somewhere else.
            if (node is FuncNode func)
            {
                func.Awake();
            }
        }

        /// <summary>
        /// Executed once this node has been added to the canvas
        /// </summary>
        protected virtual void OnInitialize()
        {

        }
        
        /// <summary>
        /// Executed when we're about to detach this element from the graph. 
        /// </summary>
        protected virtual void OnDestroy()
        {
            
        }

        private bool ParseListPort(string port, out string list_name, out int index)
        {
            list_name = null;
            index = -1;
            var list_format_split = port.Split('[');
            if (list_format_split.Length != 2) return false;
            if (!list_format_split[1].EndsWith("]")) return false;
            var index_str = list_format_split[1].Substring(0, list_format_split[1].Length - 1);
            if (int.TryParse(index_str, out index))
            {
                list_name = list_format_split[0];
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Make sure our list of PortViews and editables sync up with our NodePorts
        /// </summary>
        protected void UpdatePorts()
        {
            var reflectionData = NodeReflection.GetNodeType(target.GetType());
            var ports_for_lists = new Dictionary<string, List<Port>>();
            if (reflectionData != null) {
                foreach (PortReflectionData port in reflectionData.ports) {
                    if (port.isListField) {
                        ports_for_lists.Add(port.fieldName, new List<Port>());
                        var port_list_view = new PortListView(
                            target,
                            port.type,
                            port.fieldName,
                            m_SerializedNode,
                            m_ConnectorListener,
                            OnPropertyChange,
                            UpdateBinding
                        );
                        outputContainer.Add(port_list_view);
                        listOutputs[port.fieldName] = port_list_view;
                    }
                }
            }
            foreach (var port in target.ports)
            {
                // If the port name has the format of a list port, cache it in our list of list ports...
                if (ParseListPort(port.name, out var list_name, out int index)) {
                    //SKIP!
                } else {
                    if (port.isInput)
                    {
                        AddInputPort(port);
                    }
                    else
                    {
                        AddOutputPort(port);
                    }
                }
            }
            
            if (reflectionData != null) 
            {
                foreach (var editable in reflectionData.editables)
                {
                    AddEditableField(m_SerializedNode.FindPropertyRelative(editable.fieldName));
                }
            }
            
            // Toggle visibility of the extension container
            RefreshExpandedState();

            // Update state classes
            EnableInClassList("hasInputs", inputs.Count > 0);
            EnableInClassList("hasOutputs", outputs.Count > 0 || listOutputs.Count > 0);
        }

        protected void AddEditableField(SerializedProperty prop)
        {
            var field = new PropertyField(prop);
            field.Bind(m_SerializedNode.serializedObject);
            field.RegisterCallback((FocusOutEvent e) => OnPropertyChange());
            
            extensionContainer.Add(field);
        }

        protected virtual void AddInputPort(Port port)
        {
            var view = PortView.Create(port, port.type, m_ConnectorListener);

            // If we want to display an inline editable field as part 
            // of the port, create a new PropertyField and bind it. 
            if (port.fieldName != null)
            {
                var prop = m_SerializedNode.FindPropertyRelative(port.fieldName);
                if (prop != null)
                {
                    var field = new PropertyField(prop, " ");
                    field.Bind(m_SerializedNode.serializedObject);
                    field.RegisterCallback((FocusOutEvent e) => OnPropertyChange());

                    var container = new VisualElement();
                    container.AddToClassList("property-field-container");
                    container.Add(field);

                    view.SetEditorField(container);
                }
            }
            
            inputs.Add(view);
            inputContainer.Add(view);
        }
        
        protected virtual void AddOutputPort(Port port)
        {
            var view = PortView.Create(port, port.type, m_ConnectorListener);
            
            outputs.Add(view);
            outputContainer.Add(view);
        }

        public PortView GetInputPort(string name)
        {
            return inputs.Find((port) => port.portName == name);
        }

        public PortView GetOutputPort(string name)
        {
            var single_out_port = outputs.Find((port) => port.portName == name);
            if (single_out_port != null) return single_out_port;
            if (ParseListPort(name, out string list_name, out int index)) {
                if (listOutputs.ContainsKey(list_name)) {
                    return listOutputs[list_name].items.Find((port) => port.portName == name);
                }
            }
            return null;
        }
        
        public PortView GetCompatibleInputPort(PortView output)
        { 
            return inputs.Find((port) => port.IsCompatibleWith(output));
        }
    
        public PortView GetCompatibleOutputPort(PortView input)
        {
            return outputs.Find((port) => port.IsCompatibleWith(input));
        }

        public virtual void UpdateBinding() {
            AbstractNode new_node = null;
            if (m_SerializedNode.serializedObject.targetObject is Interactions.InteractionsGraph g) {
                foreach (AbstractNode node in g.nodes) {
                    if (node != null) {
                        if (node != target && node.id.Equals(target.id)) {
                            new_node = node;
                        }
                    }
                }
            }
            if (new_node == null) return;
            target = new_node;
            List<PortView> to_remove = new List<PortView>();
            foreach (var view in inputs) {
                var new_port = new_node.GetPort(view.target.name);
                if (new_port != null) {
                    view.target = new_port;
                } else {
                    to_remove.Add(view);
                }
            }
            foreach (var view in to_remove) {
                inputs.Remove(view);
                view.RemoveFromHierarchy();
            }
            to_remove.Clear();
            foreach (var view in outputs) {
                var new_port = new_node.GetPort(view.target.name);
                if (new_port != null) {
                    view.target = new_port;
                } else {
                    to_remove.Add(view);
                }
            }
            foreach (var view in to_remove) {
                outputs.Remove(view);
                view.RemoveFromHierarchy();
            }
            foreach (var view in listOutputs.Values) {
                view.UpdateBinding(new_node);
            }
        }

        /// <summary>
        /// A property has been updated, either by a port or a connection 
        /// </summary>
        public virtual void OnPropertyChange()
        {
            Debug.Log($"{name} propchange");

            // TODO: Cache. This lookup will be slow.
            var canvas = GetFirstAncestorOfType<CanvasView>();
            canvas?.Dirty(this);
        }
        
        /// <summary>
        /// Dirty this node in response to a change in connectivity. Invalidate
        /// any cache in prep for an OnUpdate() followup call. 
        /// </summary>
        public virtual void OnDirty()
        {
            // Dirty all ports so they can refresh their state
            inputs.ForEach(port => port.OnDirty());
            outputs.ForEach(port => port.OnDirty());
        }

        /// <summary>
        /// Called when this node was dirtied and the UI is redrawing. 
        /// </summary>
        public virtual void OnUpdate()
        {
            // Propagate update to all ports
            inputs.ForEach(port => port.OnUpdate());
            outputs.ForEach(port => port.OnUpdate());
        }

        public override Rect GetPosition()
        {
            // The default implementation doesn't give us back a valid position until layout is resolved.
            // See: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/GraphViewEditor/Elements/Node.cs#L131
            Rect position = base.GetPosition();
            if (position.width > 0 && position.height > 0)
            {
                return position;
            }
            UpdateBinding();
            return new Rect(target.graphPosition, Vector2.one);
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            UpdateBinding();
            target.graphPosition = newPos.position;
        }
        
        protected void OnTooltip(TooltipEvent evt)
        {
            // TODO: Better implementation that can be styled
            if (evt.target == titleContainer.Q("title-label"))
            {
                var typeData = NodeReflection.GetNodeType(target.GetType());
                evt.tooltip = typeData?.tooltip;
                
                // Float the tooltip above the node title bar
                var bound = titleContainer.worldBound;
                bound.x = 0;
                bound.y = 0;
                bound.height *= -1;
                
                evt.rect = titleContainer.LocalToWorld(bound);
            }
        }
    }
}
