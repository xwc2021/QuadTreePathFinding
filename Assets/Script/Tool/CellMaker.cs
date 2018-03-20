using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiyAStar;

public class CellMaker : MonoBehaviour {

    public float nodeSize = 0.25f;

    [SerializeField]
    Transform from;

    [SerializeField]
    Transform destination;

    public IGraphNode[] GetPathList()
    {
        return pathNodes.ToArray();
    }

    List<IGraphNode> pathNodes=new List<IGraphNode>();
    public void TestPathFind()
    {
        pathNodes =FindPath(from.position, destination.position);
        Debug.Log("pathNodes"+pathNodes.Count);
    }

    AStarPathFinder pathFinder =new AStarPathFinder();

    public List<IGraphNode> FindPath(Vector3 from ,Vector3 destination)
    {
        AStarPathFinder.resetIGraphNode(outerQuadNode);

        IGraphNode nodeFrom=null, nodeDestination = null;
        quadTreeConnectedNode.GetGraphNode(from,ref nodeFrom);
        quadTreeConnectedNode.GetGraphNode(destination, ref nodeDestination);

        Debug.Log(nodeFrom);
        Debug.Log(nodeDestination);

        return pathFinder.findPath(nodeFrom, nodeDestination);
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
    public int GetMaxSplitLeve() { return maxSplitLevel; }

    QuadTreeConnectedNode quadTreeConnectedNode;

    public void CollectDrawRect(List<IRect> list,bool outer)
    {
        if(quadTreeConnectedNode!=null)
            quadTreeConnectedNode.CollectDrawRect(list,outer, onlyLeafNode);
    }

    public void CollectDrawRectOnlyLeafNode(List<IRect> list, bool outer)
    {
        if (quadTreeConnectedNode != null)
            quadTreeConnectedNode.CollectDrawRect(list, outer, true);
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

    IGraphNode[] outerQuadNode;
    List<IRect> outerQuadRect = new List<IRect>();
    public void CollectOuterQuadRect()
    {
        outerQuadRect.Clear();
        CollectDrawRectOnlyLeafNode(outerQuadRect, true);

        //從IRect轉成IGraphNode
        outerQuadNode = new IGraphNode[outerQuadRect.Count];
        for (var i = 0; i < outerQuadRect.Count; ++i)
            outerQuadNode[i] = outerQuadRect[i] as IGraphNode;

        outerQuadRect.Clear();
        CollectDrawRect(outerQuadRect, true);
    }
    public List<IRect> GetOuterQuadRect() { return outerQuadRect; }

    List<IRect> innerQuadRect =new List<IRect>();
    public void CollectInnerQuadRect()
    {
        innerQuadRect.Clear(); ;
        CollectDrawRect(innerQuadRect, false);
    }
    public List<IRect> GetInnerQuadRect() { return innerQuadRect; }
}
