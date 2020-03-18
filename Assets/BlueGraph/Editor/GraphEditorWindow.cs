
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace BlueGraph.Editor
{
    /// <summary>
    /// Build a basic window container for the BlueGraph canvas
    /// </summary>
    public class GraphEditorWindow : EditorWindow
    {
        Graph m_Graph;
        CanvasView m_Canvas;
        public Func<Graph, SerializedObject, VisualElement> make_side_menu;
    
        /// <summary>
        /// Load a graph asset in this window for editing
        /// </summary>
        public void Load(Graph graph)
        {
            m_Graph  = graph;
            var serializedGraph = new SerializedObject(graph);
            
            m_Canvas = new CanvasView(this);
            m_Canvas.Load(graph, serializedGraph);
            var side_menu = make_side_menu(m_Graph, serializedGraph);
            if (side_menu != null) {
                var container = new VisualElement();
                container.style.flexDirection = FlexDirection.Row;
                container.Add(side_menu);
                container.Add(m_Canvas);
                container.style.flexGrow = new StyleFloat(1.0f);
                rootVisualElement.Add(container);
            } else {
                rootVisualElement.Add(m_Canvas);
            }
        
            titleContent = new GUIContent(graph.name);
            Repaint();
        }

        private void Update()
        {
            m_Canvas.Update();
        }

        /// <summary>
        /// Restore an already opened graph after a reload of assemblies
        /// </summary>
        private void OnEnable()
        {
            if (m_Graph)
            {
                Load(m_Graph);
            }
        }
    }
}
