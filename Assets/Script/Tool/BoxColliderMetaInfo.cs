using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxDirection { Default,FromBlender}

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderMetaInfo : MonoBehaviour {

    public BoxCollider boxCollider;
    public BoxDirection boxDirection = BoxDirection.Default;

    Vector3[] point;
    public void GenerateRectInfo()
    {
        point = new Vector3[4];

        var boxColliderSize = boxCollider.size;
        var bosLossyScale = transform.lossyScale;
        var bosFinalSize = new Vector3(boxColliderSize.x * bosLossyScale.x, boxColliderSize.y * bosLossyScale.y, boxColliderSize.z * bosLossyScale.z);

        var boxColliderCenter = transform.TransformPoint(boxCollider.center);

        switch (boxDirection)
        {
            case BoxDirection.Default:
                GenerateRectDefault(boxColliderCenter, bosFinalSize);
                break;
            case BoxDirection.FromBlender:
                GenerateFromBlender(boxColliderCenter, bosFinalSize);
                break;
        }
    }

    public Vector3[] GetRectInfo()
    {
        return point;
    }

    void GenerateRectDefault(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.forward;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.z;
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        point[0] = fixedPos + xDir * xLen + zDir * zLen;
        point[1] = fixedPos - xDir * xLen + zDir * zLen;
        point[2] = fixedPos - xDir * xLen - zDir * zLen;
        point[3] = fixedPos + xDir * xLen - zDir * zLen;

    }

    void GenerateFromBlender(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.up;
        var xLen = 0.5f * size.x;
        var yLen = 0.5f * size.y;
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        point[0] = fixedPos + xDir * xLen + zDir * yLen;
        point[1] = fixedPos - xDir * xLen + zDir * yLen;
        point[2] = fixedPos - xDir * xLen - zDir * yLen;
        point[3] = fixedPos + xDir * xLen - zDir * yLen;
    }
}
