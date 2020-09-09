using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ToolPrintDataUtil
{
    //使用 GetProperties().length 来判断object是否可tostring直接转换string
    //string类型的 Properties.length 不为0 所以需单独判断string类型进行处理
    //array and list 需单独处理

    private static List<int> hashList;
    
    public static ToolPrintData PrintData(object obj)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start(); // 性能监测开始

        if (obj == null)
        {
            return null;
        }

        if (hashList == null)
        {
            hashList = new List<int>();
        }
        
        ToolPrintData baseData = new ToolPrintData();

        RecursionPrintData(baseData, obj);
        
        hashList.Clear();
        
        stopwatch.Stop();

        return baseData;
        Debug.Log("print data success time is " + stopwatch.Elapsed.TotalMilliseconds + "ms");
    }
    
    private static void RecursionPrintData(ToolPrintData parentData, object obj)
    {
        try
        {
            if (obj != null)
            {
                int hash = obj.GetHashCode();
                if (hash != 0 && hashList.Contains(hash))
                {
                    return;
                }

                if (hash != 0)
                {
                    hashList.Add(hash);
                }
            }

            ToolPrintData childData = new ToolPrintData();
            if (obj != null)
            {
                childData.name = obj.GetType().ToString();
            }
            if (obj == null)
            {
                childData.content = "null";
            }
            else if (obj is string str)
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
            else if (obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                IList iList = obj as IList;
                for (int i = 0; i < iList.Count; i++)
                {
                    RecursionPrintData(childData, iList[i]);
                }
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
                        RecursionPrintData(childData, infos[i], obj);
                    }
                }
            }
            
            parentData.childDataList.Add(childData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static void RecursionPrintData(ToolPrintData parentData, PropertyInfo info, object obj)
    {
        try
        {
            ToolPrintData childData = new ToolPrintData();
            childData.name = info.Name;

            bool hasAdd = false;
            //处理unity类型
            if (!(obj is string) && !obj.GetType().IsArray && (!obj.GetType().IsGenericType || obj.GetType().GetGenericTypeDefinition() != typeof(List<>)))
            {
                if (info.PropertyType.BaseType.FullName == "System.ValueType")
                {
                    childData.content = info.GetValue(obj).ToString();
                    hasAdd = true;
                }
                else if (info.PropertyType.FullName == "UnityEngine.Component")
                {
                    childData.content = info.GetType().ToString();
                    hasAdd = true;
                }
                else
                {
                    object o = info.GetValue(obj);
                    if (o != null)
                    {
                        int hash = o.GetHashCode();
                        if (hashList.Contains(hash))
                        {
                            return;
                        }
                        hashList.Add(hash);
                    }
                }
            }

            if (hasAdd)
            {
                
            }
            else if (obj == null || info == null)
            {
                childData.content = "null";
            }
            else if (obj is string str)
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
            else if(obj.GetType().IsGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                IList iList = obj as IList;
                for (int i = 0; i < iList.Count; i++)
                {
                    RecursionPrintData(childData, iList[i]);
                }
            }
            else
            {
                object valueObj = info.GetValue(obj, null);
                if (valueObj == null)
                {
                    childData.content = "null";
                }
                else
                {
                    PropertyInfo[] infos = valueObj.GetType().GetProperties();
                    if (infos.Length == 0)
                    {
                        childData.content = valueObj.ToString();
                    }
                    else
                    {
                        for (int i = 0; i < infos.Length; i++)
                        {
                            RecursionPrintData(childData, infos[i], valueObj);
                        }
                    }
                }

            }

            parentData.childDataList.Add(childData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string CheckConvertStr(object obj)
    {
        if (obj is String str)
        {
            return str;
        }
        // else if
        return null;
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
