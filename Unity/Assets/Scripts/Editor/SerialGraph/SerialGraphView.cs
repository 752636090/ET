using ET.NodeDefine;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace ET
{
    public class SerialGraphView : GraphView
    {
        public override List<Port> GetCompatiblePorts(Port startViewPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            SerialGraph serialGraph = SerialGraphEditor.Instance.EditorSerialGraph.SerialGraph;
            SerialPort startPort = (SerialPort)startViewPort.userData;
            SerialNode startNode = serialGraph.NodeDict[startPort.NodeId];
            if (startNode.GetType().GetMember(startPort.Name).Length == 0)
            {
                Debug.LogError($"{startNode.GetType()} {startPort.Name}");
            }
            MemberInfo startMenberInfo = startNode.GetType().GetMember(startPort.Name)[0];
            bool startIsInput = startMenberInfo.GetCustomAttribute<PortAttribute>() is InputAttribute;
            //TypeConstraint startTypeConstraint = startMenberInfo.GetCustomAttribute<PortAttribute>().TypeConstraint;
            foreach (Port port in ports)
            {
                SerialPort targetPort = (SerialPort)port.userData;
                SerialNode targetNode = serialGraph.NodeDict[targetPort.NodeId];
                MemberInfo targetMemberInfo = targetNode.GetType().GetMember(targetPort.Name)[0];
                if (startIsInput && (targetMemberInfo.GetCustomAttribute<PortAttribute>() is InputAttribute))
                {
                    continue;
                }
                if (!startIsInput && (targetMemberInfo.GetCustomAttribute<PortAttribute>() is OutputAttribute))
                {
                    continue;
                }
                //Debug.Log(targetNode.GetType());
                //Debug.Log(targetMenberInfo.Name);
                //TypeConstraint targetTypeConstraint = targetMenberInfo.GetCustomAttribute<PortAttribute>().TypeConstraint;
                if (CanConnect(startIsInput ? startMenberInfo : targetMemberInfo, startIsInput ? targetMemberInfo : startMenberInfo))
                {
                    compatiblePorts.Add(port);
                }
            }
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

        }

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
        }

        private bool CanConnect(MemberInfo inputMenberInfo, MemberInfo outputMemberInfo)
        {
            TypeConstraint inputTypeConstraint = inputMenberInfo.GetCustomAttribute<PortAttribute>().TypeConstraint;
            TypeConstraint outputTypeConstraint = outputMemberInfo.GetCustomAttribute<PortAttribute>().TypeConstraint;
            Type inputType = inputMenberInfo.GetReturnType();
            Type outputType = outputMemberInfo.GetReturnType();
            // If there isn't one of each, they can't connect
            if (inputMenberInfo == null || outputMemberInfo == null) return false;
            // Check input type constraints
            if (inputTypeConstraint == TypeConstraint.Inherited && !inputType.IsAssignableFrom(outputType)) return false;
            if (inputTypeConstraint == TypeConstraint.Strict && inputType != outputType) return false;
            if (inputTypeConstraint == TypeConstraint.InheritedInverse && !outputType.IsAssignableFrom(inputType)) return false;
            // Check output type constraints
            if (outputTypeConstraint == TypeConstraint.Inherited && !inputType.IsAssignableFrom(outputType)) return false;
            if (outputTypeConstraint == TypeConstraint.Strict && inputType != outputType) return false;
            if (outputTypeConstraint == TypeConstraint.InheritedInverse && !outputType.IsAssignableFrom(inputType)) return false;
            // Success
            return true;
        }
    }
}
