using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Object并非C#基础中的Object，而是 UnityEngine.Object
using Object = UnityEngine.Object;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEditor;
using ET;

//使其能在Inspector面板显示，并且可以被赋予相应值
[Serializable]
public class ReferenceCollectorData
{
    [HideLabel]
    public string key;
    //Object并非C#基础中的Object，而是 UnityEngine.Object
    [OnValueChanged("OnComObjChange")]
    [HideLabel]
    public Object obj;
#if UNITY_EDITOR
    [OnValueChanged("OnTypeChange")]
    [HideLabel]
    public ReferenceType type;
    private int m_InstanceId;
    private static readonly (string, string)[] namespaces = {
        ("UnityEngine", "UnityEngine.dll"),
        ("UnityEngine.UI", "UnityEngine.UI.dll") };

    private void OnComObjChange()
    {
        if (obj == null)
        {
            key = "";
            type = ReferenceType.None;
            m_InstanceId = 0;
            return;
        }

        Transform trans = null;
        //Transform[] arr = Selection.transforms[0].GetComponentsInChildren<Transform>(true);
        Transform[] arr = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
        if (null != arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].gameObject.GetInstanceID() == obj.GetInstanceID())
                {
                    trans = arr[i];
                    m_InstanceId = arr[i].gameObject.GetInstanceID();
                    break;
                }
            }
        }

        if (trans != null)
        {
            key = trans.name;

            obj = GetObj(trans, type);
            if (obj == null)
            {
                obj = GetObj(trans, null);
                Array rcTypes = Enum.GetValues(typeof(ReferenceType));
                for (int i = 0; i < rcTypes.Length; i++)
                {
                    if (rcTypes.GetValue(i).ToString() == obj.GetType().Name)
                    {
                        type = (ReferenceType)rcTypes.GetValue(i);
                        break;
                    }
                }
            }
        }
    }

    private void OnTypeChange()
    {
        if (obj == null)
        {
            key = "";
            type = ReferenceType.None;
            return;
        }

        Transform trans = null;
        //Transform[] arr = Selection.transforms[0].GetComponentsInChildren<Transform>(true);
        Transform[] arr = Selection.activeGameObject.GetComponentsInChildren<Transform>(true);
        if (null != arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].gameObject.GetInstanceID() == m_InstanceId)
                {
                    trans = arr[i];
                    break;
                }
            }
        }

        Object tempObj = GetObj(trans, type);

        if (tempObj != null)
        {
            obj = tempObj;
        }
        else
        {
            type = ReferenceType.None;
            Debug.LogError("选择的类型在当前组件上不存在");
        }
    }

    private Object GetObj(Transform trans, ReferenceType? type)
    {
        string[] rcTypeNames = Enum.GetNames(typeof(ReferenceType));
        for (int i = rcTypeNames.Length - 1; i >= 0; i--)
        {
            string rcTypeName = rcTypeNames[i];
            if (rcTypeName == "None"
                || (type != null
                    && type.ToString() != rcTypeName
                    )
                ) continue;

            if (type == ReferenceType.GameObject)
            {
                return trans.gameObject;
            }
            Type rcType = null;
            foreach ((string, string) ns in namespaces)
            {
                rcType = Type.GetType($"{ns.Item1}.{rcTypeName}", false, false);
                if (rcType == null)
                {
                    rcType = Type.GetType($"{ns.Item1}.{rcTypeName}, {ns.Item2}", false, false);
                }
                if (rcType != null) break;
            }
            if (rcType != null)
            {
                MethodInfo getTargetComponent = typeof(Transform).GetMethod("GetComponent", new Type[] { }).MakeGenericMethod(rcType);
                Object tempObj = getTargetComponent.Invoke(trans, null) as Object;
                if (tempObj != null) return tempObj;
            }
            else
            {
                Debug.LogError($"RC类型错误 {rcTypeName}");
            }
        }

        return null;
    }

#endif
}
//继承IComparer对比器，Ordinal会使用序号排序规则比较字符串，因为是byte级别的比较，所以准确性和性能都不错
public class ReferenceCollectorDataComparerByName : IComparer<ReferenceCollectorData>
{
    public int Compare(ReferenceCollectorData x, ReferenceCollectorData y)
    {
        return string.Compare(x.key, y.key, StringComparison.Ordinal);
    }
}
public class ReferenceCollectorDataComparerByType : IComparer<ReferenceCollectorData>
{
    public int Compare(ReferenceCollectorData x, ReferenceCollectorData y)
    {
        return x.type.CompareTo(y.type);
    }
}

//继承ISerializationCallbackReceiver后会增加OnAfterDeserialize和OnBeforeSerialize两个回调函数，如果有需要可以在对需要序列化的东西进行操作
//ET在这里主要是在OnAfterDeserialize回调函数中将data中存储的ReferenceCollectorData转换为dict中的Object，方便之后的使用
//注意UNITY_EDITOR宏定义，在编译以后，部分编辑器相关函数并不存在
public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
{
    //用于序列化的List
    [OnValueChanged("OnChanged")]
    [TableList(ShowIndexLabels = false, AlwaysExpanded = true)]
    public List<ReferenceCollectorData> data = new List<ReferenceCollectorData>();
    //Object并非C#基础中的Object，而是 UnityEngine.Object
    private readonly Dictionary<string, Object> dict = new Dictionary<string, Object>();

#if UNITY_EDITOR
    //添加新的元素
    public void Add(string key, Object obj)
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        //根据PropertyPath读取数据
        //如果不知道具体的格式，可以右键用文本编辑器打开一个prefab文件（如Bundles/UI目录中的几个）
        //因为这几个prefab挂载了ReferenceCollector，所以搜索data就能找到存储的数据
        UnityEditor.SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        //遍历data，看添加的数据是否存在相同key
        for (i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                break;
            }
        }
        //不等于data.Count意为已经存在于data List中，直接赋值即可
        if (i != data.Count)
        {
            //根据i的值获取dataProperty，也就是data中的对应ReferenceCollectorData，不过在这里，是对Property进行的读取，有点类似json或者xml的节点
            UnityEditor.SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            //对对应节点进行赋值，值为gameobject相对应的fileID
            //fileID独一无二，单对单关系，其他挂载在这个gameobject上的script或组件会保存相对应的fileID
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
        else
        {
            //等于则说明key在data中无对应元素，所以得向其插入新的元素
            dataProperty.InsertArrayElementAtIndex(i);
            UnityEditor.SerializedProperty element = dataProperty.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("gameObject").objectReferenceValue = obj;
        }
        //应用与更新
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }
    //删除元素，知识点与上面的添加相似
    public void Remove(string key)
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        UnityEditor.SerializedProperty dataProperty = serializedObject.FindProperty("data");
        int i;
        for (i = 0; i < data.Count; i++)
        {
            if (data[i].key == key)
            {
                break;
            }
        }
        if (i != data.Count)
        {
            dataProperty.DeleteArrayElementAtIndex(i);
        }
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    [Button("全部清除"), ResponsiveButtonGroup("Handle")]
    public void Clear()
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        //根据PropertyPath读取prefab文件中的数据
        //如果不知道具体的格式，可以直接右键用文本编辑器打开，搜索data就能找到
        var dataProperty = serializedObject.FindProperty("data");
        dataProperty.ClearArray();
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    [Button("删除空引用"), ResponsiveButtonGroup("Handle")]
    public void DelNullReference()
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        var dataProperty = serializedObject.FindProperty("data");
        for (int i = dataProperty.arraySize - 1; i >= 0; i--)
        {
            var gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("obj");
            if (gameObjectProperty.objectReferenceValue == null)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
                EditorUtility.SetDirty(this);
                serializedObject.ApplyModifiedProperties();
                serializedObject.UpdateIfRequiredOrScript();
            }
        }
    }

    enum SortType { None, Key名, 类型 } // 如果不想用中文，可以用[LableText("")]显示中文
    [HorizontalGroup("Split")]
    [VerticalGroup("Split/Left")]
    [ShowInInspector]
    [LabelText("排序方式")]
    private SortType sortType = SortType.None;

    [HorizontalGroup("Split")]
    [VerticalGroup("Split/Right")]
    [Button("排序")]
    public void Sort()
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        IComparer<ReferenceCollectorData> comparer;
        switch (sortType)
        {
            case SortType.Key名:
                comparer = new ReferenceCollectorDataComparerByName();
                break;
            case SortType.类型:
                comparer = new ReferenceCollectorDataComparerByType();
                break;
            default:
                return;
        }
        data.Sort(comparer);
        sortType = SortType.None;
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    private void OnChanged()
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }
#endif
    //使用泛型返回对应key的gameobject
    public T Get<T>(string key) where T : class
    {
        Object dictGo;
        if (!dict.TryGetValue(key, out dictGo))
        {
            return null;
        }
        return dictGo as T;
    }

    public Object GetObject(string key)
    {
        Object dictGo;
        if (!dict.TryGetValue(key, out dictGo))
        {
            return null;
        }
        return dictGo;
    }

    public void OnBeforeSerialize()
    {
    }
    //在反序列化后运行
    public void OnAfterDeserialize()
    {
        dict.Clear();
        foreach (ReferenceCollectorData referenceCollectorData in data)
        {
            if (!dict.ContainsKey(referenceCollectorData.key))
            {
                dict.Add(referenceCollectorData.key, referenceCollectorData.obj);
            }
        }
    }
}

#if UNITY_EDITOR
public enum ReferenceType
{
    None,
    GameObject,
    Transform,
    RectTransform,
    Image,
    //ZImage,
    Text,
    //ZText,
    RawImage,
    Button,
    InputField,
    Scrollbar,
    ScrollRect,
    //LoopListView2
}
#endif