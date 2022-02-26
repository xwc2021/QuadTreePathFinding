using UnityEngine;
public enum BoxDirection { Default, FromBlender }

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderMetaInfo : MonoBehaviour, IRect
{
    public BoxCollider boxCollider;
    public BoxDirection boxDirection = BoxDirection.Default;

    [SerializeField]
    Vector3[] points;
    Vector3 centerPoint;

    public Vector3[] GetRectInfo()
    {
        return points;
    }

    public Vector3 GetCenter()
    {
        return centerPoint;
    }

    public void GenerateRectInfo()
    {
        points = new Vector3[4];

        var boxColliderSize = boxCollider.size;
        var bosLossyScale = transform.lossyScale;
        var boxFinalSize = new Vector3(boxColliderSize.x * bosLossyScale.x, boxColliderSize.y * bosLossyScale.y, boxColliderSize.z * bosLossyScale.z);

        var boxColliderCenter = transform.TransformPoint(boxCollider.center);

        switch (boxDirection)
        {
            case BoxDirection.Default:
                GenerateRectDefault(boxColliderCenter, boxFinalSize);
                break;
            case BoxDirection.FromBlender:
                GenerateFromBlender(boxColliderCenter, boxFinalSize);
                break;
        }

        centerPoint = 0.25f * (points[0] + points[1] + points[2] + points[3]);
    }

    void GenerateRectDefault(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.forward;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.z;
        GeometryTool.Generate(pos, xDir, zDir, xLen, zLen, points);
    }

    void GenerateFromBlender(Vector3 pos, Vector3 size)
    {
        Vector3 xDir = transform.right;
        Vector3 zDir = transform.up;
        var xLen = 0.5f * size.x;
        var zLen = 0.5f * size.y;
        GeometryTool.Generate(pos, xDir, zDir, xLen, zLen, points);
    }
}