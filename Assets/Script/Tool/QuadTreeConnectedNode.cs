using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeConnectedNode:IRect {

    bool isOuter = true;
    public void SetIsOuter(bool b)
    {
        isOuter = b;
    }

    int level;
    float minX, maxX, minZ, maxZ;
    float centerX, centerZ;
    float Height;
    float Width;
    public QuadTreeConnectedNode(float minX, float maxX, float minZ, float maxZ,int level) {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
        this.level = level;
        this.centerX = 0.5f * (minX + maxX);
        this.centerZ = 0.5f * (minZ + maxZ);
        this.Width = maxX - minX;
        this.Height = maxZ - minZ;

        points = new Vector3[4];
        points[0] = new Vector3(minX, 0, minZ);
        points[1] = new Vector3(maxX, 0, minZ);
        points[2] = new Vector3(maxX, 0, maxZ);
        points[3] = new Vector3(minX, 0, maxZ);

        centerPoint= new Vector3(centerX, 0, centerZ); ;
    }

    public QuadTreeConnectedNode[] SplitTo4()
    {
        childs = new QuadTreeConnectedNode[4];
        var nextLevel = level + 1;
        childs[0] = new QuadTreeConnectedNode(minX, centerX, minZ, centerZ, nextLevel);//左下
        childs[1] = new QuadTreeConnectedNode(centerX,maxX, minZ, centerZ, nextLevel);//右下
        childs[2] = new QuadTreeConnectedNode(centerX, maxX, centerZ, maxZ, nextLevel);//右上
        childs[3] = new QuadTreeConnectedNode(minX, centerX, centerZ, maxZ, nextLevel);//左上
        //Debug.Log(level);

        return childs;
    }

    public bool HasChild()
    {
        return childs != null;
    }

    public bool IsContainRectVertex(IRect rect)
    {
        var testPoints = rect.GetRectInfo();
        for (var i = 0; i < testPoints.Length; ++i)
        {
            var nowPoint = testPoints[i];
            if (nowPoint.x > minX && nowPoint.x < maxX && nowPoint.z > minZ && nowPoint.z < maxZ)
                return true;
        }
        return false;
    }

    Vector3 centerPoint;
    Vector3[] points;
    public QuadTreeConnectedNode[] childs;

    QuadTreeConnectedNode[] leftLink, rightLink, upLink, downLink;

    public Vector3[] GetRectInfo()
    {
        return points;
    }

    public Vector3 GetCenter()
    {
        return centerPoint;
    }

    public void CollectDrawRect(List<IRect> list,bool outer)
    {
        if (!HasChild() &&　this.isOuter== outer)
            list.Add(this);

        if (childs == null)
            return; 

        foreach (var child in childs)
            child.CollectDrawRect(list, outer);
    }

    public class LinkInfo
    {
        public QuadTreeConnectedNode from;
        public QuadTreeConnectedNode to;
        public LinkInfo(QuadTreeConnectedNode from, QuadTreeConnectedNode to)
        {
            this.from = from;
            this.to = to;
        }
    }

    static List<LinkInfo> verticalLinks = new List<LinkInfo>();
    static List<LinkInfo> horizontalLinks = new List<LinkInfo>();
    public static void ClearLinks()
    {
        verticalLinks.Clear();
        horizontalLinks.Clear();
    }
    public static List<LinkInfo> GetVerticaLinks(){return verticalLinks;}
    public static List<LinkInfo> GetHorizontalLinks() { return horizontalLinks; }

    public void MakeConnected()
    {
        if (HasChild())
        {
            //對每個node
            //找出4組子node配對Pn
            MakeConnectedHorizontal(childs[0], childs[1]);
            MakeConnectedHorizontal(childs[3], childs[2]);
            MakeConnectedVertical(childs[1], childs[2]);
            MakeConnectedVertical(childs[0], childs[3]);

            foreach (var child in childs)
                child.MakeConnected();
        }
    }

    void MakeConnectedHorizontal(QuadTreeConnectedNode leftNode, QuadTreeConnectedNode rightNode)
    {
        //https://plus.google.com/u/0/+XiangweiChiou/posts/EET3zRE8Awz
        if (!leftNode.isOuter || !rightNode.isOuter)
            return;

        //如果Pn裡的2個node都沒有child node，就為該2個node建立連結
        if (!leftNode.HasChild() && !rightNode.HasChild())
        {
            leftNode.rightLink = new QuadTreeConnectedNode[1];
            leftNode.rightLink[0] = rightNode;

            rightNode.leftLink = new QuadTreeConnectedNode[1];
            rightNode.leftLink[0] = leftNode;

            horizontalLinks.Add(new LinkInfo(leftNode, rightNode));
        }
        else//否則，向下遞迴找出邊界上子node，並相連
        {
            var leftSideNodes = new List<QuadTreeConnectedNode>();

            if (!leftNode.HasChild())
                leftSideNodes.Add(leftNode);
            else
                GetBorderNodes(leftNode, new int[] { 1, 2 }, leftSideNodes);

            var rightSideNodes = new List<QuadTreeConnectedNode>();

            if (!rightNode.HasChild())
                rightSideNodes.Add(rightNode);
            else
                GetBorderNodes(rightNode, new int[] { 0, 3 }, rightSideNodes);

            connectTwoSide(true, leftSideNodes, rightSideNodes);
        }    
    }

    void MakeConnectedVertical(QuadTreeConnectedNode downNode, QuadTreeConnectedNode upNode)
    {
        //https://plus.google.com/u/0/+XiangweiChiou/posts/EET3zRE8Awz
        if (!downNode.isOuter || !upNode.isOuter)
           return;

        //如果Pn裡的2個node都沒有child node，就為該2個node建立連結
        if (!downNode.HasChild() && !upNode.HasChild())
        {
            downNode.upLink = new QuadTreeConnectedNode[1];
            downNode.upLink[0] = upNode;

            upNode.downLink = new QuadTreeConnectedNode[1];
            upNode.downLink[0] = downNode;

            verticalLinks.Add(new LinkInfo(downNode, upNode));
        }
        else//否則，向下遞迴找出邊界上子node，並相連
        {
            var upSideNodes = new List<QuadTreeConnectedNode>();

            if(!upNode.HasChild())
                upSideNodes.Add(upNode);
            else
                GetBorderNodes(upNode, new int[] { 0, 1 }, upSideNodes);

            var downSideNodes = new List<QuadTreeConnectedNode>();
           
            if (!downNode.HasChild())
                downSideNodes.Add(downNode);
            else
                GetBorderNodes(downNode, new int[] { 3, 2 }, downSideNodes);

            connectTwoSide(false, downSideNodes, upSideNodes);
        }
    }

    static void GetBorderNodes(QuadTreeConnectedNode targetNode,int[] borderIndex,List<QuadTreeConnectedNode> list)
    {
        if (!targetNode.HasChild())
            return;

        foreach (var index in borderIndex)
        {
            var nowChildNode = targetNode.childs[index];
            if (nowChildNode.HasChild())
                GetBorderNodes(nowChildNode, borderIndex, list);
            else
                list.Add(nowChildNode);
        }
    }

    class CompareHelper
    {
        bool isLeftRight;

        // 左/右
        // 下/上
        List<QuadTreeConnectedNode> firstSideNodes;
        List<QuadTreeConnectedNode> secondSideNodes;
        public CompareHelper(bool isLeftRight,List<QuadTreeConnectedNode> firstSideNodes, List<QuadTreeConnectedNode> secondSideNodes)
        {
            this.isLeftRight = isLeftRight;
            this.firstSideNodes = firstSideNodes;
            this.secondSideNodes = secondSideNodes;
        }

        List<QuadTreeConnectedNode> firstBuffer = new List<QuadTreeConnectedNode>();
        List<QuadTreeConnectedNode> secondBuffer = new List<QuadTreeConnectedNode>();
        float firstSideSum = 0;
        float secondSideSum = 0;

        int leftIndex = 0;
        int rightIndex = 0;
        public void AddFirstSide() {
            var node = firstSideNodes[leftIndex];
            firstBuffer.Add(node);
            firstSideSum = firstSideSum + GetValue(node);
            ++leftIndex;
        }

        public void AddSecondSide()
        {
            var node = secondSideNodes[rightIndex];
            secondBuffer.Add(node);
            secondSideSum = secondSideSum + GetValue(node);
            ++rightIndex;
        }

        float GetValue(QuadTreeConnectedNode node)
        {
            return isLeftRight ? node.Width : node.Height;
        }

        public void ClearBuffer() {
            firstBuffer.Clear();
            secondBuffer.Clear();
            firstSideSum = 0;
            secondSideSum = 0;
        }

        public void ConnectNode()
        {
            if (isLeftRight)
                ConnectVertical();
            else
                ConnectHorizontal();
        }

        int GetOuterCount(List<QuadTreeConnectedNode> buffer)
        {
            int count = 0;
            for (var i = 0; i < buffer.Count; ++i)
            {
                var rightNode = buffer[i];
                if (rightNode.isOuter)
                    ++count;
            }
            return count;
        }

        void ConnectVertical()
        {
            //1對1
            if (firstBuffer.Count == 1 && secondBuffer.Count == 1)
            {
                var leftNode = firstBuffer[0];
                var rightNode = secondBuffer[0];

                if (!leftNode.isOuter || !rightNode.isOuter)
                    return;

                leftNode.rightLink = new QuadTreeConnectedNode[1];
                rightNode.leftLink = new QuadTreeConnectedNode[1];

                leftNode.rightLink[0] = rightNode;
                rightNode.leftLink[0] = leftNode;

                horizontalLinks.Add(new LinkInfo(leftNode, rightNode));
            }

            //1對多
            if (firstBuffer.Count == 1 && secondBuffer.Count > 1)
            {
                var leftNode = firstBuffer[0];
                if (!leftNode.isOuter)
                    return;

                leftNode.rightLink = new QuadTreeConnectedNode[GetOuterCount(secondBuffer)];

                var nowIndex = 0;
                for (var i = 0; i < secondBuffer.Count; ++i)
                {
                    var rightNode = secondBuffer[i];
                    if (!rightNode.isOuter)
                        continue;

                    rightNode.leftLink = new QuadTreeConnectedNode[1];
                    rightNode.leftLink[0] = leftNode;

                    leftNode.rightLink[nowIndex] = rightNode;
                    ++nowIndex;

                    horizontalLinks.Add(new LinkInfo(leftNode, rightNode));
                }
            }

            //多對1
            if (firstBuffer.Count >1 && secondBuffer.Count== 1)
            {
                var rightNode = secondBuffer[0];
                if (!rightNode.isOuter)
                    return;

                rightNode.leftLink = new QuadTreeConnectedNode[GetOuterCount(firstBuffer)];

                var nowIndex = 0;
                for (var i = 0; i < firstBuffer.Count; ++i)
                {
                    var leftNode = firstBuffer[i];
                    if (!leftNode.isOuter)
                        continue;

                    leftNode.rightLink = new QuadTreeConnectedNode[1];
                    leftNode.rightLink[0] = rightNode;

                    rightNode.leftLink[nowIndex] = leftNode;
                    ++nowIndex;

                    horizontalLinks.Add(new LinkInfo(leftNode,rightNode));
                }
            }
        }

        void ConnectHorizontal()
        {
            //1對1
            if (firstBuffer.Count == 1 && secondBuffer.Count == 1)
            {
                var downNode = firstBuffer[0];
                var upNode = secondBuffer[0];

                if (!downNode.isOuter || !upNode.isOuter)
                    return;

                downNode.upLink = new QuadTreeConnectedNode[1];
                upNode.downLink = new QuadTreeConnectedNode[1];

                downNode.upLink[0] = upNode;
                upNode.downLink[0] = downNode;

                verticalLinks.Add(new LinkInfo(downNode, upNode));
            }

            //1對多
            if (firstBuffer.Count == 1 && secondBuffer.Count > 1)
            {
                var downNode = firstBuffer[0];
                if (!downNode.isOuter)
                    return;

                downNode.upLink = new QuadTreeConnectedNode[GetOuterCount(secondBuffer)];

                var nowIndex = 0;
                for (var i = 0; i < secondBuffer.Count; ++i)
                {
                    var upNode = secondBuffer[i];
                    if (!upNode.isOuter)
                        continue;

                    upNode.downLink = new QuadTreeConnectedNode[1];
                    upNode.downLink[0] = downNode;

                    downNode.upLink[nowIndex] = upNode;
                    ++nowIndex;

                    verticalLinks.Add(new LinkInfo(downNode, upNode));
                }
            }

            //多對1
            if (firstBuffer.Count > 1 && secondBuffer.Count == 1)
            {
                var upNode = secondBuffer[0];
                if (!upNode.isOuter)
                    return;

                upNode.downLink = new QuadTreeConnectedNode[GetOuterCount(firstBuffer)];

                var nowIndex = 0;
                for (var i = 0; i < firstBuffer.Count; ++i)
                {
                    var downNode = firstBuffer[i];
                    if (!downNode.isOuter)
                        continue;

                    downNode.upLink = new QuadTreeConnectedNode[1];
                    downNode.upLink[0] = upNode;

                    upNode.downLink[nowIndex] = downNode;
                    ++nowIndex;

                    verticalLinks.Add(new LinkInfo(downNode, upNode));
                }
            }
        }

        public bool IsFinish()
        {
            return leftIndex == firstSideNodes.Count && rightIndex == secondSideNodes.Count;
        }

        public bool IsEqual(){return Mathf.Abs(firstSideSum - secondSideSum) < float.Epsilon;}
        public bool FirstSideIsBigger() { return firstSideSum > secondSideSum; }  
}

    void connectTwoSide(bool isLeftRight, List<QuadTreeConnectedNode> firstSideNodes, List<QuadTreeConnectedNode> secondSideNodes)
    {
        Debug.Log(firstSideNodes.Count + "," + secondSideNodes.Count);
        var compareHelper = new CompareHelper(isLeftRight, firstSideNodes, secondSideNodes);

        compareHelper.AddFirstSide();
        compareHelper.AddSecondSide();
        while (true)
        {
            //當2個buffer的sum值相等，就可以ConnectNode
            if (compareHelper.IsEqual())
            {
                //Debug.Log("IsEqual");
                compareHelper.ConnectNode();
                compareHelper.ClearBuffer();

                if (compareHelper.IsFinish())
                    return;
                else
                {
                    compareHelper.AddFirstSide();
                    compareHelper.AddSecondSide();
                }
            }
            else if (compareHelper.FirstSideIsBigger())
            {
                //Debug.Log("FirstSideIsBigger");
                compareHelper.AddSecondSide();
            }
            else
            {
                //Debug.Log("SecondSideIsBigger");
                compareHelper.AddFirstSide();
            }  
        }
    }
}
