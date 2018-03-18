using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeConnectedNode:IRect {

    int level;
    float minX, maxX, minZ, maxZ;
    float centerX, centerZ;
    public QuadTreeConnectedNode(float minX, float maxX, float minZ, float maxZ,int level) {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
        this.level = level;
        this.centerX = 0.5f * (minX + maxX);
        this.centerZ = 0.5f * (minZ + maxZ);

        points = new Vector3[4];
        points[0] = new Vector3(minX, 0, minZ);
        points[1] = new Vector3(maxX, 0, minZ);
        points[2] = new Vector3(maxX, 0, maxZ);
        points[3] = new Vector3(minX, 0, maxZ);
    }

    public QuadTreeConnectedNode[] SplitTo4()
    {
        childs = new QuadTreeConnectedNode[4];
        var nextLevel = level + 1;
        childs[0] = new QuadTreeConnectedNode(minX, centerX, minZ, centerZ, nextLevel);//左下
        childs[1] = new QuadTreeConnectedNode(centerX,maxX, minZ, centerZ, nextLevel);//右下
        childs[2] = new QuadTreeConnectedNode(centerX, maxX, centerZ, maxZ, nextLevel);//右上
        childs[3] = new QuadTreeConnectedNode(minX, centerX, centerZ, maxZ, nextLevel);//左上
        Debug.Log(level);

        return childs;
    }

    public bool HasChild()
    {
        return childs != null;
    }

    public bool IsContainRect(IRect rect)
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

    Vector3[] points;
    public QuadTreeConnectedNode[] childs;

    QuadTreeConnectedNode[] leftLink, rightLink, upLink, downLink;

    Vector3[] IRect.GetRectInfo()
    {
        return points;
    }

    public void CollectDrawRect(List<IRect> list)
    {
        if (!HasChild())
            list.Add(this);

        if (childs == null)
            return; 

        foreach (var child in childs)
            child.CollectDrawRect(list);
    }
}
