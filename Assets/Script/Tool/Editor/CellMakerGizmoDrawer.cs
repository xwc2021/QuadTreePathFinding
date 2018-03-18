using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CellMakerGizmoDrawer {

    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmoFor(CellMaker target, GizmoType gizmoType)
    {
        Gizmos.DrawLine(target.P0.position, target.P1.position);
        Gizmos.DrawLine(target.P2.position, target.P3.position);

        var colliderRects = target.colliderRects;
        if (colliderRects != null)
        {
            foreach (var rect in colliderRects)
                DrawRect(rect, colliderColor);
        }

        var drawQuadRectList = new List<IRect>();
        target.CollectDrawRect(drawQuadRectList);

        foreach (var rect in drawQuadRectList)
            DrawRect(rect, quadTreeColor);
    }

    static Color colliderColor=Color.green;
    static Color quadTreeColor = Color.yellow;
    static void DrawRect(IRect rect,Color color)
    {
        var point = rect.GetRectInfo();
        if (point == null)
            return;

        Gizmos.color = color;
        for (var i = 0; i <= 2; ++i)
            Gizmos.DrawLine(point[i], point[i + 1]);
        Gizmos.DrawLine(point[3], point[0]);
    }

    static void DrawQuadTree()
    {
    }

}
