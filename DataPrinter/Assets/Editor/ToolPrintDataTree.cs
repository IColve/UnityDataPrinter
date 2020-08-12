using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ToolPrintDataTree : TreeView
{
    public TreeViewItem root;

    public ToolPrintDataTree(TreeViewState state) : base(state)
    {
        root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
    }

    protected override TreeViewItem BuildRoot()
    {
        SetupDepthsFromParentsAndChildren(root);
        return root;
    }
}
