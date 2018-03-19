using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMaker : MonoBehaviour {

    public BoxColliderMetaInfo[] boxColliderMetaInfoList;
    public IRect[] colliderRects;

    //分割的臨界值
    [SerializeField]
    float smallestX=1;

    [SerializeField]
    float smallestZ=1;

    [SerializeField]
    Transform border;

    [SerializeField]
    int cellCountX = 10;

    [SerializeField]
    int cellCountZ = 10;

    public int GetCellCountX() { return cellCountX; }
    public int GetCellCountZ() { return cellCountZ; }

    public Vector3 GetDirX() { return transform.right; }
    public Vector3 GetDirZ() { return transform.forward; }

    public Vector3 GetOrigin() { return transform.position; }
    public Vector3 GetBorder() { return border.position; }

    public void GetAllBoxColliderMetaInfoInSceneAndGenerateRect()
    {
        boxColliderMetaInfoList=GameObject.FindObjectsOfType<BoxColliderMetaInfo>();

        foreach (var boxColliderMetaInfo in boxColliderMetaInfoList)
        {
            var boxCollider = boxColliderMetaInfo.GetComponent<BoxCollider>();
            boxColliderMetaInfo.boxCollider = boxCollider;
            boxColliderMetaInfo.GenerateRectInfo();
        }

        colliderRects = new IRect[boxColliderMetaInfoList.Length];
        for (var i = 0; i < boxColliderMetaInfoList.Length; ++i)
            colliderRects[i] = boxColliderMetaInfoList[i] as IRect;
    }

    [SerializeField]
    int maxSplitLevel=5;
    QuadTreeConnectedNode quadTreeConnectedNode;

    public void CollectDrawRect(List<IRect> list,bool outer)
    {
        if(quadTreeConnectedNode!=null)
            quadTreeConnectedNode.CollectDrawRect(list,outer);
    }

    //除了是四叉樹，所有葉節點彼此還會相連
    public void GenerateQuadTreeConnectedNode()
    {
        var origin=GetOrigin();
        var border = GetBorder();
        quadTreeConnectedNode = new QuadTreeConnectedNode(origin.x, border.x, origin.z, border.z,0);

        //第1次直接split
        var nowTestNodes =quadTreeConnectedNode.SplitTo4();
        var nextTestNodes = new List<QuadTreeConnectedNode>();

        //進行分裂
        int count = maxSplitLevel - 1;
        for (var i = 1; i <= count; ++i)
        {
            foreach (var node in nowTestNodes)
            {
                var a = NodeIsIntersectWithColliderRects(node, colliderRects);
                var b = NodeIsContainColliderRectsVertex(node, colliderRects);
                if (a || b)//如果有rect和node相交或是頂點在node裡面
                    nextTestNodes.AddRange(node.SplitTo4());//就把node分成4塊，並加入下一輪的測試清單
                else
                {
                    SetIsOuter(node, colliderRects);
                }
            }
            nowTestNodes = nextTestNodes.ToArray();
            nextTestNodes.Clear();
        }

        //分裂完後，還要測式1次
        foreach (var node in nowTestNodes)
        {
            SetIsOuter(node, colliderRects);
        }
    }

    void SetIsOuter(QuadTreeConnectedNode node, IRect[] colliderRects)
    {
        //如果node的中心點在rect裡面
        if (NodeCenterIsInColliderRects(node, colliderRects))
            node.SetIsOuter(false);
        else
            node.SetIsOuter(true);
    }

    bool NodeIsIntersectWithColliderRects(QuadTreeConnectedNode node,IRect[] colliderRects)
    {
        for (var i = 0; i < colliderRects.Length; ++i)
        {
            var rect = colliderRects[i];
            if (GeometryTool.IsIntersect(node, rect))
                return true;
        }
        return false;
    }

    bool NodeIsContainColliderRectsVertex(QuadTreeConnectedNode node, IRect[] colliderRects)
    {
        for (var i = 0; i < colliderRects.Length; ++i)
        {
            var rect = colliderRects[i];
            if (node.IsContainRectVertex(rect))
                return true;
        }
        return false;
    }

    bool NodeCenterIsInColliderRects(QuadTreeConnectedNode node, IRect[] colliderRects)
    {
        for (var i = 0; i < colliderRects.Length; ++i)
        {
            var rect = colliderRects[i];
            if (GeometryTool.IsContainCenterPoint(rect,node))
                return true;
        }
        return false;
    }

    //debug用
    public Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3;

    public void TestIsIntersect()
    {
        Vector3 p0 = P0.position;
        Vector3 p1 = P1.position;
        Vector3 p2 = P2.position;
        Vector3 p3 = P3.position;
        bool result = GeometryTool.IsIntersect(p0, p1, p2, p3);
        Debug.Log(result);
    }
}
