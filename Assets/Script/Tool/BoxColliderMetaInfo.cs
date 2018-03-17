using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxDirection { Default,FromBlender}

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderMetaInfo : MonoBehaviour {

    public BoxCollider boxCollider;
    public BoxDirection boxDirection = BoxDirection.Default;

    [SerializeField]
    Vector3[] point;

    public void GetEdge(int index ,out Vector3 from, out Vector3 to)
    {
        switch (index)
        {
            case 0:
                from = point[0];
                to = point[1];
                break;
            case 1:
                from = point[1];
                to = point[2];
                break;
            case 2:
                from = point[2];
                to = point[3];
                break;
            default:
                from = point[3];
                to = point[0];
                break;
        }
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
        Generate(pos, xDir, zDir, xLen, zLen);
    }

    void GenerateFromBlender(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.up;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.y;
        Generate(pos, xDir, zDir, xLen, zLen);
    }

    void Generate(Vector3 pos, Vector3 xDir, Vector3 zDir, float xLen, float zLen)
    {
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        point[0] = fixedPos + xDir * xLen + zDir * zLen;
        point[1] = fixedPos - xDir * xLen + zDir * zLen;
        point[2] = fixedPos - xDir * xLen - zDir * zLen;
        point[3] = fixedPos + xDir * xLen - zDir * zLen;
    }

    public static bool IsIntersect(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3,Transform c1, Transform c2)
    {
        Vector3 v0 = p1 - p0;
        Vector3 v1 = p3 - p2;

        float a = v0.x;
        float b = v0.z;
        float c = v1.x;
        float d = v1.z;
        float e = p2.x - p0.x;
        float f = p2.z - p0.z;

        float temp = (a * d - b * c);
        if (temp == 0)
            return false;

        float t = (e * d - c * f) / temp ;
        float s = -(a * f - e * b) / temp;

        Vector3 crossPointT = p0 + t * v0;
        float dotValueT=Vector3.Dot(crossPointT - p0, crossPointT - p1);

        Vector3 crossPointS = p2 + s * v1;
        float dotValueS = Vector3.Dot(crossPointS - p2, crossPointS - p3);

        c1.position = crossPointT;
        c2.position = crossPointS;

        print(dotValueT + "," + dotValueS);
        return dotValueT < 0 && dotValueS < 0;
    }

    public static bool IsIntersect(BoxColliderMetaInfo a, BoxColliderMetaInfo b)
    {
        //如果任2邊相交


        return true;
    }

    //b是不是被a包住
    public static bool IsContain(BoxColliderMetaInfo a, BoxColliderMetaInfo b)
    {

        return true;
    }
}
