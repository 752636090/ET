using ET.NodeDefine;
using MongoDB.Bson.Serialization.Attributes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    [CreateAssetMenu(fileName = "EditorSerialGraph", menuName = "Temp/EditorSerialGraph")]
    public class EditorSerialGraph : SerializedScriptableObject
    {
        //// 和对应的行为树Id一致,最好文件名就是id,方便查找和加载
        //public int Id;
        // 对应节点的备注信息
        // 如果不喜欢自由拖放位置,所有节点位置由运行时的
        // 偏移,缩放+父子关系计算出来,那么这个数据可以不要
        public Dictionary<int, EditorSerialNodeInfo> EditorNodeInfoDict = new();

        public SerialGraph SerialGraph = new();

        public string BsonPath
        {
            get
            {
                return $"Assets/Bundles/Graphs/{SerialGraph.Type}/{SerialGraph.Id}.bson";
            }
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
        }

        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            SerialGraph.AfterDeserialize();
        }

        public void Export()
        {
            FileHelper.CreateFile(BsonPath, MongoHelper.Serialize(SerialGraph));
            AssetDatabase.Refresh();
            Debug.Log($"保存成功：{BsonPath}");
        }

        public EditorSerialNode AddNode(Type type, Vector2 position)
        {
            SerialNode newNode = (SerialNode)Activator.CreateInstance(type);
            newNode.Id = GetMaxNodeId() + 1;
            SerialGraph.Nodes.Add(newNode);
            SerialGraph.NodeDict[newNode.Id] = newNode;

            int maxPortId = GetMaxPortId();
            foreach (MemberInfo member in type.GetMembers())
            {
                // 下面这三行注释的代码忘记为啥写了
                //if (member.GetCustomAttribute<BsonIgnoreAttribute>() == null)
                //{
                //    continue;
                //}
                PortAttribute portAttribute = member.GetCustomAttribute<PortAttribute>(true);
                if (portAttribute == null)
                {
                    continue;
                }

                SerialPort port = new SerialPort()
                {
                    Id = ++maxPortId,
                    NodeId = newNode.Id,
                    Name = member.Name,
                };
                SerialGraph.Ports.Add(port);
                SerialGraph.PortDict[port.Id] = port;
                newNode.PortDict[member.Name] = port;
            }
            SerialGraphEditor.Instance.EditorSerialGraph.EditorNodeInfoDict[newNode.Id] = new EditorSerialNodeInfo()
            {
                Position = position,
            };
            EditorSerialNode editorNode = AddNodeToView(newNode);
            SerialGraphEditor.Instance.SetDirty();
            return editorNode;
        }

        public int GetMaxNodeId()
        {
            int maxNodeId = 0;
            foreach (SerialNode node in SerialGraph.Nodes)
            {
                maxNodeId = Mathf.Max(node.Id, maxNodeId);
            }
            return maxNodeId;
        }

        public int GetMaxPortId()
        {
            int maxPortId = 0;
            foreach (SerialPort port in SerialGraph.Ports)
            {
                maxPortId = Mathf.Max(port.Id, maxPortId);
            }
            return maxPortId;
        }

        public EditorSerialNode AddNodeToView(SerialNode node)
        {
            return new EditorSerialNode(node, EditorNodeInfoDict[node.Id].Position);
        }

        public void RemoveNode(SerialNode node)
        {
            foreach (SerialPort port in node.PortDict.Values)
            {
                foreach (int targetPortId in port.TargetIds)
                {
                    SerialPort targetPort = SerialGraph.PortDict[targetPortId];
                    targetPort?.TargetIds.Remove(port.Id);
                }

                SerialGraph.Ports.Remove(port);
                SerialGraph.PortDict.Remove(port.Id);
            }

            SerialGraph.Nodes.Remove(node);
            SerialGraph.NodeDict.Remove(node.Id);
            EditorNodeInfoDict.Remove(node.Id);
        }

        public void Connect(SerialPort port1, SerialPort port2)
        {
            if (!port1.TargetIds.Contains(port2.Id))
            {
                port1.TargetIds.Add(port2.Id); 
            }
            if (!port2.TargetIds.Contains(port1.Id))
            {
                port2.TargetIds.Add(port1.Id); 
            }
        }

        public void Disconnect(SerialPort port1, SerialPort port2)
        {
            port1.TargetIds.Remove(port2.Id);
            port2.TargetIds.Remove(port1.Id);
        }
    }
}
