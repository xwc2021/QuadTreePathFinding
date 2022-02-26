using System.Collections.Generic;
using UnityEngine;
using DiyAStar;
public class CellMaker : MonoBehaviour
{
    public float nodeSize = 0.25f;
    public float modifyNodeSize = 0.125f;

    [SerializeField]
    Transform from;

    [SerializeField]
    Transform destination;

    public IGraphNode[] GetRawPath()
    {
        return rawNodes.ToArray();
    }

    public Vector3[] GetModifyPath()
    {
        return modifyNodes.ToArray();
    }

    List<IGraphNode> rawNodes = new List<IGraphNode>();
    List<Vector3> modifyNodes = new List<Vector3>();
    public void TestPathFind()
    {
        rawNodes = FindPath(from.position, destination.position);
        modifyNodes = ModifyPath(rawNodes);
        Debug.Log("rawNodes" + rawNodes.Count);
    }

    AStarPathFinder pathFinder = new AStarPathFinder();

    public List<IGraphNode> FindPath(Vector3 from, Vector3 destination)
    {
        AStarPathFinder.resetIGraphNode(outerQuadNode);

        IGraphNode nodeFrom = null, nodeDestination = null;
        quadTreeConnectedNode.GetGraphNode(from, ref nodeFrom);
        quadTreeConnectedNode.GetGraphNode(destination, ref nodeDestination);

        return pathFinder.findPath(nodeFrom, nodeDestination);
    }

    Vector3 GetCrossPoint(IGraphNode p0, IGraphNode p1)
    {
        var n0 = p0 as QuadTreeConnectedNode;
        var n1 = p1 as QuadTreeConnectedNode;

        //判斷是水平還是垂直
        var vec = n1.GetPosition() - n0.GetPosition();

        bool isHorizontal = Mathf.Abs(vec.x) > Mathf.Abs(vec.z);
        float ratio = isHorizontal ? n0.halfWidth / Mathf.Abs(vec.x) : n0.halfHeight / Mathf.Abs(vec.z);

        //相似三角形的1邊ratio會=其他邊ratio
        return n0.GetPosition() + vec * ratio;
    }

    //路徑修剪
    //https://plus.google.com/u/0/+XiangweiChiou/posts/X12EPrwgtPM
    List<Vector3> ModifyPath(List<IGraphNode> rawNodes)
    {
        var collect = new List<Vector3>();
        for (var i = 0; i < rawNodes.Count - 1; ++i)
        {
            var point = GetCrossPoint(rawNodes[i], rawNodes[i + 1]);
            collect.Add(point);
        }

        return collect;
    }

    public bool onlyLeafNode = true;
    public bool showNodeLink = true;

    public BoxColliderMetaInfo[] boxColliderMetaInfoList;
    public IRect[] colliderRects;

    [SerializeField]
    Transform border;

    public Vector3 GetDirX() { return transform.right; }
    public Vector3 GetDirZ() { return transform.forward; }

    public Vector3 GetOrigin() { return transform.position; }
    public Vector3 GetBorder() { return border.position; }

    public void GetAllBoxColliderMetaInfoInSceneAndGenerateRect()
    {
        boxColliderMetaInfoList = GameObject.FindObjectsOfType<BoxColliderMetaInfo>();

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
    int maxSplitLevel = 5;
    public int GetMaxSplitLeve() { return maxSplitLevel; }

    QuadTreeConnectedNode quadTreeConnectedNode;

    public void CollectDrawRect(List<IRect> list, bool outer)
    {
        if (quadTreeConnectedNode != null)
            quadTreeConnectedNode.CollectDrawRect(list, outer, onlyLeafNode);
    }

    public void CollectDrawRectOnlyLeafNode(List<IRect> list, bool outer)
    {
        if (quadTreeConnectedNode != null)
            quadTreeConnectedNode.CollectDrawRect(list, outer, true);
    }

    //除了是四叉樹，所有葉節點彼此還會相連
    public void GenerateQuadTreeConnectedNode()
    {
        var origin = GetOrigin();
        var border = GetBorder();
        quadTreeConnectedNode = new QuadTreeConnectedNode(origin.x, border.x, origin.z, border.z, 0);

        //第1次直接split
        var nowTestNodes = quadTreeConnectedNode.SplitTo4();
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
                {
                    //就把node分成4塊，並加入下一輪的測試清單
                    //有分裂的話outer=true(預設值)
                    nextTestNodes.AddRange(node.SplitTo4());
                }
                else
                    SetIsOuter(node, colliderRects);
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

    public void MakeConnected()
    {
        QuadTreeConnectedNode.ClearLinks();
        quadTreeConnectedNode.MakeConnected();
    }

    public void GenerateGraphNodeData()
    {
        quadTreeConnectedNode.GenerateGraphNodeData();
    }

    void SetIsOuter(QuadTreeConnectedNode node, IRect[] colliderRects)
    {
        //如果node的中心點在rect裡面
        if (NodeCenterIsInColliderRects(node, colliderRects))
            node.SetIsOuter(false);
        else
            node.SetIsOuter(true);
    }

    bool NodeIsIntersectWithColliderRects(QuadTreeConnectedNode node, IRect[] colliderRects)
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
            if (GeometryTool.IsContainCenterPoint(rect, node))
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

    IGraphNode[] outerQuadNode;
    List<IRect> outerQuadRect = new List<IRect>();

    public void CollectOuterQuadRect()
    {
        // (1)收集尋路用的
        outerQuadRect.Clear();
        CollectDrawRectOnlyLeafNode(outerQuadRect, true);

        //從IRect轉成IGraphNode
        outerQuadNode = new IGraphNode[outerQuadRect.Count];
        for (var i = 0; i < outerQuadRect.Count; ++i)
            outerQuadNode[i] = outerQuadRect[i] as IGraphNode;

        // (2)顯示用的 黃色Rect
        outerQuadRect.Clear();
        CollectDrawRect(outerQuadRect, true);
        print("Outer Node Count : " + outerQuadRect.Count);
    }
    public List<IRect> GetOuterQuadRect() { return outerQuadRect; }

    List<IRect> innerQuadRect = new List<IRect>();
    public void CollectInnerQuadRect()
    {
        // 顯示用的 紅色Rect
        innerQuadRect.Clear(); ;
        CollectDrawRect(innerQuadRect, false);
        print("Inner Node Count : " + innerQuadRect.Count);
    }
    public List<IRect> GetInnerQuadRect() { return innerQuadRect; }
}