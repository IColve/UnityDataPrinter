using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ToolPrintDataUtil
{
    private static List<int> instIdList = new List<int>();

    public static ToolPrintData PrintData(object obj)
    {
        instIdList.Clear();

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start(); // 性能监测开始

        if (obj == null)
        {
            return null;
        }

        ToolPrintData baseData = new ToolPrintData();

        RecursionPrintData(baseData, obj);

        stopwatch.Stop();

        Debug.Log("print data success time is " + stopwatch.Elapsed.TotalMilliseconds + "ms");
        return baseData;
    }

    private static void RecursionPrintData(ToolPrintData parentData, object obj, string name = null)
    {
        try
        {
            Debug.Log(obj.GetType());

            if (obj is Transform tran)
            {
                if (instIdList.Contains(tran.GetInstanceID()))
                {
                    return;
                }
                else
                {
                    instIdList.Add(tran.GetInstanceID());
                }
            }
            else if (obj is GameObject o)
            {
                if (instIdList.Contains(o.GetInstanceID()))
                {
                    return;
                }
                else
                {
                    instIdList.Add(o.GetInstanceID());
                }
            }
            else if (obj is MonoBehaviour mono)
            {
                if (instIdList.Contains(mono.GetInstanceID()))
                {
                    return;
                }
                else
                {
                    instIdList.Add(mono.GetInstanceID());
                }
            }
            else if (obj is Component component)
            {
                if (instIdList.Contains(component.GetInstanceID()))
                {
                    return;
                }
                else
                {
                    instIdList.Add(component.GetInstanceID());
                }
            }

            ToolPrintData childData = new ToolPrintData();

            childData.name = name == null ? obj.GetType().ToString() : name;

            if (obj is string str)
            {
                childData.content = str;
            }
            else if (obj.GetType().IsArray)
            {
                Array array = obj as Array;

                for (int i = 0; i < array.Length; i++)
                {
                    RecursionPrintData(childData, array.GetValue(i));
                }
            }
            else if (obj is IList iList)
            {
                for (int i = 0; i < iList.Count; i++)
                {
                    RecursionPrintData(childData, iList[i]);
                }
            }
            else if (obj.GetType().IsEnum)
            {
                childData.content = obj.ToString();
            }
            else if (obj is Vector3 || obj is Vector4 || obj is Vector2 || obj is Quaternion)
            {
                childData.content = obj.ToString();
            }
            else if (obj is IDictionary iDic)
            {
                ToolPrintData keyData = new ToolPrintData();
                keyData.name = "keys";

                foreach (object key in iDic.Keys)
                {
                    RecursionPrintData(keyData, key, key.GetType().ToString());
                }
                
                ToolPrintData valueData = new ToolPrintData();
                valueData.name = "value";
                
                foreach (object value in iDic.Values)
                {
                    RecursionPrintData(valueData, value, value.GetType().ToString());
                }
                
                childData.childDataList.Add(keyData);
                childData.childDataList.Add(valueData);
            }
            else
            {
                PropertyInfo[] infos = obj.GetType().GetProperties();
                if (infos.Length == 0)
                {
                    childData.content = obj.ToString();
                }
                else
                {
                    for (int i = 0; i < infos.Length; i++)
                    {
                        RecursionPropertyInfo(infos[i], obj, childData);
                    }
                }

                MonoBehaviour[] components = null;
                if (obj is Transform childTran)
                {
                    components = childTran.GetComponents<MonoBehaviour>();
                }
                else if (obj is GameObject gameObject)
                {
                    components = gameObject.GetComponents<MonoBehaviour>();
                }

                if (components != null)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        RecursionPrintData(childData, components[i], components[i].GetType().ToString());
                    }
                }

                if (obj is MonoBehaviour)
                {
                    FieldInfo[] fileInfos = obj.GetType().GetFields();
                    for (int i = 0; i < fileInfos.Length; i++)
                    {
                        try
                        {
                            object o = fileInfos[i].GetValue(obj);
                            
                            if (o == null)
                            {
                                ToolPrintData data = new ToolPrintData();
                                data.name = fileInfos[i].Name;
                                data.content = "Null";
                                childData.childDataList.Add(data);
                            }
                            else
                            {
                                RecursionPrintData(childData, o, fileInfos[i].Name);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(fileInfos[i].Name + " Error");
                            throw;
                        }
                    }
                }
            }

            parentData.childDataList.Add(childData);
        }
        catch (Exception e)
        {
            Debug.Log("转换失败");
            throw;
        }
    }

    private static void RecursionPropertyInfo(PropertyInfo info, object obj, ToolPrintData childData)
    {
        try
        {
            if (!info.CanRead)
            {
                return;
            }

            if (info.PropertyType.ToString() == "UnityEngine.Component")
            {
                return;
            }

            object childObj = info.GetValue(obj);

            if (info.PropertyType.BaseType.FullName == "System.ValueType")
            {
                ToolPrintData data = new ToolPrintData();
                data.name = info.Name;
                data.content = childObj.ToString();
                childData.childDataList.Add(data);
            }
            else
            {
                if (childObj != null)
                {
                    RecursionPrintData(childData, childObj, info.Name);
                }
                else
                {
                    ToolPrintData data = new ToolPrintData();
                    data.name = info.Name;
                    data.content = "Null";
                    childData.childDataList.Add(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(info.PropertyType + " Error");
            throw;
        }
    }
}

public class ToolPrintData
{
    public string name;
    public string content;
    public List<ToolPrintData> childDataList;

    public ToolPrintData()
    {
        childDataList = new List<ToolPrintData>();
        content = "";
    }
}
