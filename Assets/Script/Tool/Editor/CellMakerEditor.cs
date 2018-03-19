using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CellMaker))]
public class CellMakerEditor : Editor {

    CellMaker cellMaker;
    void OnEnable()
    {
        cellMaker = (CellMaker)target;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        if (GUILayout.Button("Get All BoxCollider And Generate Rect"))
        {
            cellMaker.GetAllBoxColliderMetaInfoInSceneAndGenerateRect();
            SceneView.RepaintAll();
            Debug.Log("Get");
        }

        if (GUILayout.Button("Generate Nav Cell"))
        {
            cellMaker.GenerateQuadTreeConnectedNode();
            cellMaker.MakeConnected();

            cellMaker.CollectOuterQuadRect();
            cellMaker.CollectInnerQuadRect();
            SceneView.RepaintAll();
            Debug.Log("Generate");
        }

        if (GUILayout.Button("TestIsIntersect"))
        {
            cellMaker.TestIsIntersect();
        }
    }
}
