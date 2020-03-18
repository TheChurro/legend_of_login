
using BlueGraph;
using BlueGraph.Editor;

namespace BlueGraphExamples
{
    [CustomNodeView(typeof(FloatOperationNode))]
    class ExampleCustomNodeView : NodeView
    {
        protected override void OnInitialize()
        {
            // Custom initialization logic goes here.
        }
    }
}
