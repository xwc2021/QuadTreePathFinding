using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxDirection { Default,FromBlender}

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderMetaInfo : MonoBehaviour, IRect
{

    public BoxCollider boxCollider;
    public BoxDirection boxDirection = BoxDirection.Default;

    [SerializeField]
    Vector3[] point;

    Vector3[] IRect.GetRectInfo()
    {
        return point;
    }

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

    void GenerateRectDefault(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.forward;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.z;
        GeometryTool.Generate(pos, xDir, zDir, xLen, zLen,point);
    }

    void GenerateFromBlender(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.up;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.y;
        GeometryTool.Generate(pos, xDir, zDir, xLen, zLen, point);
    }

    
}
