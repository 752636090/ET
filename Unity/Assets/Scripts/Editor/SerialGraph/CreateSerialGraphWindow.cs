using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public class CreateSerialGraphWindow : OdinEditorWindow
    {
        [HorizontalGroup("1")]
        [LabelText("文件名")]
        [OnValueChanged("OnNameChanged")]
        public string GraphName;
        [HorizontalGroup("1")]
        [LabelText("ID")]
        public int GraphId;

        public static void ShowWindow()
        {
            Rect wr = new Rect(0, 0, 300, 100);
            CreateSerialGraphWindow window = (CreateSerialGraphWindow)GetWindowWithRect(typeof(CreateSerialGraphWindow), wr, true);
            window.titleContent = new GUIContent("新增图配置");
            window.Show();
        }

        [Button("创建")]
        public void Create()
        {
            if (SerialGraphEditor.Instance == null)
            {
                EditorUtility.DisplayDialog("错误", "不要关掉原来的界面", "确定");
                Close();
                return;
            }

            if (string.IsNullOrEmpty(GraphName))
            {
                EditorUtility.DisplayDialog("错误", "文件名为空", "确定");
                return;
            }

            if (SerialGraphEditor.Instance.CreateGraph(GraphName, GraphId))
            {
                Close();
            }
        }

        private void OnNameChanged()
        {
            int iStart = -1;
            int iEnd = -1;
            for (int i = 0; i < GraphName.Length; i++)
            {
                if (iStart == -1)
                {
                    if (char.IsDigit(GraphName[i]))
                    {
                        iStart = i; 
                    }
                }
                else
                {
                    if (!char.IsDigit(GraphName[i]))
                    {
                        iEnd = i - 1;
                        break;
                    }
                }
            }
            if (iStart >= 0)
            {
                if (iEnd == -1)
                {
                    iEnd = GraphName.Length - 1;
                }
                GraphId = int.Parse(GraphName.Substring(iStart, iEnd - iStart + 1));
            }
        }
    }
}
