using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRect
{
    Vector3[] GetRectInfo();
}

public class GeometryTool
{
    //2線段是否相交
    public static bool IsIntersect(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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

        float t = (e * d - c * f) / temp;
        float s = -(a * f - e * b) / temp;

        Vector3 crossPointT = p0 + t * v0;
        float dotValueT = Vector3.Dot(crossPointT - p0, crossPointT - p1);

        Vector3 crossPointS = p2 + s * v1;
        float dotValueS = Vector3.Dot(crossPointS - p2, crossPointS - p3);

        //print(dotValueT + "," + dotValueS);
        return dotValueT < 0 && dotValueS < 0;
    }

    public static bool IsIntersect(IRect a, IRect b)
    {
        //如果任2邊相交
        for (var i = 0; i < 4; ++i)
        {
            Vector3 p0, p1;
            GetEdge(a, i, out p0, out p1);
            for (var k = 0; k < 4; ++k)
            {
                Vector3 p2, p3;
                GetEdge(b, k, out p2, out p3);
                if (GeometryTool.IsIntersect(p0, p1, p2, p3))
                    return true;
            }
        }

        return false;
    }

    //b是不是被a包住
    //(用在a的軸向沒有和世界對齊的情況)
    //(另1種作法是把b的4個點轉換到a的local，之後就可以用軸對齊的判定)
    public static bool IsContainCenterPoint(IRect a, IRect b)
    {
        var point = a.GetRectInfo();
        var p1 = point[1];
        var p2 = point[2];
        var p3 = point[3];

        var xDir = (p3 - p2);
        var zDir = (p1 - p2);

        var width = xDir.magnitude;
        var height = zDir.magnitude;

        var xNormalDir = xDir/width;
        var zNormalDir = zDir/height;

        var testPoint = b.GetRectInfo();

        //只判斷中心點
        var center=Vector3.zero;
        foreach (var tPoint in testPoint)
            center = center+ tPoint;
        center = center* 0.25f;

        var vec = center - p2;
        var xValue = Vector3.Dot(vec, xNormalDir);
        var zValue = Vector3.Dot(vec, zNormalDir);

        bool test = xValue > 0 && xValue < width && zValue > 0 && zValue < height;
        return test;

        /*
        foreach (var tPoint in testPoint)
        {
            var vec = tPoint - p2;
            var xValue = Vector3.Dot(vec, xNormalDir);
            var zValue = Vector3.Dot(vec, zNormalDir);

            bool test = xValue > 0 && xValue < width && zValue > 0 && zValue < height;
            if (!test)
                return false;
        }

        return true;
        */
    }

    public static void GetEdge(IRect rect, int index, out Vector3 from, out Vector3 to)
    {
        var point = rect.GetRectInfo();
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

    public static void Generate(Vector3 pos, Vector3 xDir, Vector3 zDir, float xLen, float zLen, Vector3[] point)
    {
        //口 頂點順序(從右上角開始逆時鐘方法)
        //10
        //23
        var fixedPos = new Vector3(pos.x, 0, pos.z);
        point[0] = fixedPos + xDir * xLen + zDir * zLen;
        point[1] = fixedPos - xDir * xLen + zDir * zLen;
        point[2] = fixedPos - xDir * xLen - zDir * zLen;
        point[3] = fixedPos + xDir * xLen - zDir * zLen;
    }
}
