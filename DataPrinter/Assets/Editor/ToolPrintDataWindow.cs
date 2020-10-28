using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ToolPrintDataWindow : EditorWindow
{
    private static ToolPrintDataTree treeView;
    private static TreeViewState treeViewState;
    private static bool _canShow;
    private static int lastShowType = -1;
    private static bool needExpand;
    private static bool useOldExpand = true;

    [MenuItem(@"快速操作/---信息展示---")]
    public static void OpenWindow()
    {
        Rect rect = new Rect(0,0,640,900);
        
        GetWindowWithRect(typeof(ToolPrintDataWindow), rect, true, "DataPrint");
    }
    
    public static void SetData(ToolPrintData data)
    {
        if (treeViewState == null)
        {
            treeViewState = new TreeViewState();
        }

        if (data == null)
        {
            return;
        }

        IList<int> expandIdList = null;
        if (treeView != null)
        {
            expandIdList = treeView.GetExpanded();
        }
        
        treeView = new ToolPrintDataTree(treeViewState);
        id = 0;
        treeView.root.AddChild(GetTreeViewItem(data));
        treeView.Reload();
        if (expandIdList != null)
        {
            if (needExpand && useOldExpand)
            {
                for (int i = 0; i < expandIdList.Count; i++)
                {
                    treeView.SetExpanded(expandIdList[i], true);
                }
            }
            else
            {
                for (int i = 0; i < expandIdList.Count; i++)
                {
                    treeView.SetExpanded(expandIdList[i], false);
                }
            }
        }

        _canShow = true;
    }

    private static int id = 0;

    public static int ID
    {
        get
        {
            id += 1;
            return id;
        }
    }
    
    private static TreeViewItem GetTreeViewItem(ToolPrintData baseData)
    {
        TreeViewItem item = new TreeViewItem(0, -1, "root");
        List<TreeViewItem> itemList = GetChildItems(1, baseData.childDataList);

        if (itemList != null)
        {
            itemList.ForEach(x => item.AddChild(x));
        }
        return item;
    }

    private static List<TreeViewItem> GetChildItems(int index, List<ToolPrintData> printDatas)
    {
        if (printDatas == null)
        {
            return new List<TreeViewItem>();
        }
        List<TreeViewItem> viewItems = new List<TreeViewItem>();
        printDatas.ForEach(x =>
        {
            string title = "";
            if (x.childDataList == null || x.childDataList.Count == 0)
            {
                title = x.name + " | " + x.content;
            }
            else
            {
                title = x.name;
            }
            TreeViewItem item = new TreeViewItem(ID, 0, title);
            viewItems.Add(item);
            if (x.childDataList != null && x.childDataList.Count > 0)
            {
                List<TreeViewItem> childItems = GetChildItems(0, x.childDataList);
                childItems.ForEach(y => item.AddChild(y));
            }
        });
        return viewItems;
    }
    
    private void OnGUI()
    {
        if (GUILayout.Button("信息", GUILayout.Width(position.width)))
        {
            //SetData(ToolPrintDataUtil.PrintData(obj));
        }

        if (_canShow)
        {
            treeView.OnGUI(new Rect(0, 150, position.width, 150));
        }
    }
}
