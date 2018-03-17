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

        Gizmos.DrawLine(target.P0.position, target.P1.position);
        Gizmos.DrawLine(target.P2.position, target.P3.position);
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
