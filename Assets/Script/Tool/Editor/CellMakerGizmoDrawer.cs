using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellMakerGizmoDrawer {

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(CellMaker target, GizmoType gizmoType)
    {
        var boxMetaInfoList = target.boxColliderMetaInfoList;
        foreach (var boxColliderMetaInfo in boxMetaInfoList)
            DrawRect(boxColliderMetaInfo);
    }

    static Color boxColor=Color.white;
    static void DrawRect(BoxColliderMetaInfo boxColliderMetaInfo)
    {
        var point = boxColliderMetaInfo.GetRectInfo();
        if (point == null)
            return;

        Gizmos.color = boxColor;
        for (var i = 0; i <= 2; ++i)
            Gizmos.DrawLine(point[i], point[i + 1]);
        Gizmos.DrawLine(point[3], point[0]);
    }

}
