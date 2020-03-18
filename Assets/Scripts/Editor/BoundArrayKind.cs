using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SOTU.Editor {
    // Pulled from unity forums by uMathiu
    // https://forum.unity.com/threads/propertydrawer-with-uielements-changes-in-array-dont-refresh-inspector.747467/
    public class ArrayInspectorElement : BindableElement, INotifyValueChanged<int>
    {
        private readonly SerializedObject bound_object;
        private readonly string array_path;
    
        public Func<string, int, VisualElement> make_item { get; set; }
        public Action<string, int> on_add_item { get; set; }
    
        public override VisualElement contentContainer => base_container;
    
        private readonly VisualElement base_container;
    
        public ArrayInspectorElement(SerializedProperty arrayProperty, Func<string, int,  VisualElement> makeItem, Action<string, int> on_add_item = null)
        {
            var header = new VisualElement();
        
            header.Add(new Label(arrayProperty.displayName));
        
            var addButton = new Button(() =>
            {
                Debug.Log("PRESSED BUTTON!");
                var size = arrayProperty.arraySize;
                arrayProperty.InsertArrayElementAtIndex(size);
                arrayProperty.serializedObject.ApplyModifiedProperties();
                if (on_add_item != null) {
                    on_add_item($"{arrayProperty.propertyPath}.Array.data[{size}]", size);
                }
                arrayProperty.serializedObject.ApplyModifiedProperties();
            });
            addButton.text = "+";
            this.on_add_item = on_add_item;
            header.Add(addButton);
    
            // This belongs in uss
            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.SpaceBetween;
    
        
            //We use a content container so that array size = child count      
            // And the child management becomes easier
            base_container = new VisualElement() {name = "array-contents"};
            this.hierarchy.Add(header);
            this.hierarchy.Add(base_container);
        
            array_path = arrayProperty.propertyPath;
            bound_object = arrayProperty.serializedObject;
            this.make_item = makeItem;
    
            var property = arrayProperty.Copy();
            var endProperty = property.GetEndProperty();
    
            //We prefill the container since we know we will need this
            property.NextVisible(true); // Expand the first child.
            do
            {
                if (SerializedProperty.EqualContents(property, endProperty))
                    break;
                if (property.propertyType == SerializedPropertyType.ArraySize)
                {
                    arraySize = property.intValue;
                    bindingPath = property.propertyPath;
                    break;
                }
            }
            while (property.NextVisible(false)); // Never expand children.
    
            UpdateCreatedItems();
            //we assume we don't need to Bind here
        }
    
        VisualElement AddItem(string propertyPath, int index)
        {
            VisualElement child;
            if (make_item != null)
            {
                child = make_item(propertyPath, index);
            }
            else
            {
                var pf = new PropertyField();
                pf.bindingPath = propertyPath;
                child = pf;
            }
    
            Add(child);
            return child;
        }
    
        bool UpdateCreatedItems()
        {
            int currentSize = childCount;
    
            int targetSize = this.arraySize;
    
            if (targetSize < currentSize)
            {
                for (int i = currentSize-1; i >= targetSize; --i)
                {
                    RemoveAt(i);
                }
            }else if (targetSize > currentSize)
            {
                for (int i = currentSize; i < targetSize; ++i)
                {
                    AddItem($"{array_path}.Array.data[{i}]", i);
                }
    
                return true; //we created new Items
            }
    
            return false;
        }
    
        private int arraySize = 0;
        public void SetValueWithoutNotify(int newSize)
        {
            this.arraySize = newSize;
    
            if (UpdateCreatedItems())
            {
                //We rebind the array
                this.Bind(bound_object);
            }
        }
    
        public int value
        {
            get => arraySize;
            set
            {
                if (arraySize == value) return;
            
                if (panel != null)
                {
                    using (ChangeEvent<int> evt = ChangeEvent<int>.GetPooled(arraySize, value))
                    {
                        evt.target = this;
                    
                        // The order is important here: we want to update the value, then send the event,
                        // so the binding writes and updates the serialized object
                        arraySize = value;
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
}
