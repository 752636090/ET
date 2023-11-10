// 用于防报错

using Sirenix.OdinInspector;
using System;
using System.Diagnostics;
using UnityEngine;

namespace ET
{
//#if DOTNET
//    [AttributeUsage(AttributeTargets.Field)]
//    public class SerializeReferenceAttribute : Attribute
//    {

//    }
//#endif

    namespace NodeDefine
    {
        public abstract class PortAttribute : Attribute
        {
            public TypeConstraint TypeConstraint;
            public Capacity Capacity;

            /// <summary> Mark a serializable field as an input port. You can access this through <see cref="GetInputPort(string)"/> </summary>
            /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
            /// <param name="connectionType">Should we allow multiple connections? </param>
            /// <param name="typeConstraint">Constrains which input connections can be made to this port </param>
            /// <param name="dynamicPortList">If true, will display a reorderable list of inputs instead of a single port. Will automatically add and display values for lists and arrays </param>
            public PortAttribute(TypeConstraint typeConstraint = TypeConstraint.None, Capacity capacity = Capacity.Multi)
            {
                TypeConstraint = typeConstraint;
                Capacity = capacity;
            }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class InputAttribute : PortAttribute
        {
            public InputAttribute(TypeConstraint typeConstraint = TypeConstraint.None, Capacity capacity = Capacity.Multi) : base(typeConstraint, capacity) { }
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class OutputAttribute : PortAttribute
        {
            public OutputAttribute(TypeConstraint typeConstraint = TypeConstraint.None, Capacity capacity = Capacity.Multi) : base(typeConstraint, capacity) { }
        }

        //
        // 摘要:
        //     Graph element orientation.
        public enum Orientation
        {
            //
            // 摘要:
            //     Horizontal orientation used for nodes and connections flowing to the left or
            //     right.
            Horizontal,
            //
            // 摘要:
            //     Vertical orientation used for nodes and connections flowing up or down.
            Vertical
        }

        //
        // 摘要:
        //     Port direction (in or out).
        public enum Direction
        {
            //
            // 摘要:
            //     Port is an input port.
            Input,
            //
            // 摘要:
            //     Port is an output port.
            Output
        }

        public enum Capacity
        {
            //
            // 摘要:
            //     Port can only have a single connection.
            Single,
            //
            // 摘要:
            //     Port can have multiple connections.
            Multi
        }

        public enum TypeConstraint
        {
            /// <summary> Allow all types of input</summary>
            None,
            /// <summary> Allow connections where input value type is assignable from output value type (eg. ScriptableObject --> Object)</summary>
            Inherited,
            /// <summary> Allow only similar types </summary>
            Strict,
            /// <summary> Allow connections where output value type is assignable from input value type (eg. Object --> ScriptableObject)</summary>
            InheritedInverse,
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        [Conditional("UNITY_EDITOR")]
        public class NodeTintAttribute : BaseAttribute
        {
            public Color Color;

            /// <summary> Specify a color for this node type </summary>
            /// <param name="r"> Red [0.0f .. 1.0f] </param>
            /// <param name="g"> Green [0.0f .. 1.0f] </param>
            /// <param name="b"> Blue [0.0f .. 1.0f] </param>
            public NodeTintAttribute(float r, float g, float b)
            {
                Color = new Color(r, g, b);
            }

            /// <summary> Specify a color for this node type </summary>
            /// <param name="hex"> HEX color value </param>
            public NodeTintAttribute(string hex)
            {
                ColorUtility.TryParseHtmlString(hex, out Color);
            }

            /// <summary> Specify a color for this node type </summary>
            /// <param name="r"> Red [0 .. 255] </param>
            /// <param name="g"> Green [0 .. 255] </param>
            /// <param name="b"> Blue [0 .. 255] </param>
            public NodeTintAttribute(byte r, byte g, byte b)
            {
                Color = new Color32(r, g, b, byte.MaxValue);
            }
        }

        /// <summary> Specify a width for this node type </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        [Conditional("UNITY_EDITOR")]
        public class NodeWidthAttribute : BaseAttribute
        {
            public int Width;
            /// <summary> Specify a width for this node type </summary>
            /// <param name="width"> Width </param>
            public NodeWidthAttribute(int width)
            {
                Width = width;
            }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        [Conditional("UNITY_EDITOR")]
        public class NodeNameAttribute : BaseAttribute
        {
            public string Name;

            public NodeNameAttribute(string name)
            {
                Name = name;
            }
        }
    }


}
