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
        {
            var boxCollider = boxColliderMetaInfo.boxCollider;
            var boxTransfrom = boxColliderMetaInfo.transform;

            var boxColliderSize = boxCollider.size;
            var bosLossyScale = boxTransfrom.lossyScale;
            var bosFinalSize = new Vector3(boxColliderSize.x * bosLossyScale.x, boxColliderSize.y * bosLossyScale.y, boxColliderSize.z * bosLossyScale.z);

            var boxColliderCenter = boxTransfrom.TransformPoint(boxCollider.center);

            switch (boxColliderMetaInfo.boxDirection)
            {
                case BoxDirection.Default:
                    DrawRectDefault(boxColliderCenter, boxTransfrom, bosFinalSize);
                    break;
                case BoxDirection.FromBlender:
                    DrawRectFromBlender(boxColliderCenter, boxTransfrom, bosFinalSize);
                    break;
            }
        }
    }

    static Color boxColor=Color.red;
    static void DrawRectDefault(Vector3 pos,Transform boxTransform,Vector3 size)
    {
        Vector3 xDir = boxTransform.right;
        Vector3 zDir = boxTransform.forward;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.z;
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        var point = new Vector3[4];
        point[0] = fixedPos + xDir * xLen + zDir * zLen;
        point[1] = fixedPos - xDir * xLen + zDir * zLen;
        point[2] = fixedPos - xDir * xLen - zDir * zLen;
        point[3] = fixedPos + xDir * xLen - zDir * zLen;

        Gizmos.color = boxColor;
        for (var i = 0; i <= 2; ++i)
            Gizmos.DrawLine(point[i], point[i + 1]);
        Gizmos.DrawLine(point[3], point[0]);
    }

    static void DrawRectFromBlender(Vector3 pos, Transform boxTransform, Vector3 size)
    {
        Vector3 xDir = boxTransform.right;
        Vector3 zDir = boxTransform.up;
        var xLen = 0.5f * size.x;
        var yLen = 0.5f * size.y;
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        var point = new Vector3[4];
        point[0] = fixedPos + xDir * xLen + zDir * yLen;
        point[1] = fixedPos - xDir * xLen + zDir * yLen;
        point[2] = fixedPos - xDir * xLen - zDir * yLen;
        point[3] = fixedPos + xDir * xLen - zDir * yLen;

        Gizmos.color = boxColor;
        for (var i = 0; i <= 2; ++i)
            Gizmos.DrawLine(point[i], point[i + 1]);
        Gizmos.DrawLine(point[3], point[0]);
    }
}
