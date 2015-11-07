//by Bob Berkebile : Pixelplacement : http://www.pixelplacement.com

using UnityEngine;
using System.Collections.Generic;

public class iTweenPath : MonoBehaviour
{
    public string pathName = "";
    public Color pathColor = Color.cyan;
    public bool isRelative = true;
    public bool keepToGround = false;
    [SerializeField][HideInInspector]public float distanceToGround = 1f;//Player.m_height/2f;
    [SerializeField][HideInInspector]public List<Vector3> nodes = new List<Vector3>() { Vector3.zero, Vector3.zero };
    [SerializeField][HideInInspector]public int nodeCount;
    //[SerializeField][HideInInspector]public static Dictionary<string, iTweenPath> paths = new Dictionary<string, iTweenPath>();
    [SerializeField][HideInInspector]public bool initialized = false;
    [SerializeField][HideInInspector]public string initialName = "";
    
    //void OnEnable()
    //{
    //    if (!paths.ContainsKey(pathName.ToLower()))
    //        paths.Add(pathName.ToLower(), this);
    //    else
    //        paths[pathName.ToLower()] = this;
    //}

    void OnDrawGizmosSelected()
    {
        if (enabled)
        { // dkoontz
            if (nodes.Count > 0)
            {
                Vector3[] nodesArray = nodes.ToArray();
                if(isRelative)
                    for (int i=0; i < nodesArray.Length; i++)
                        nodesArray[i] = transform.TransformPoint(nodesArray[i]);
                iTween.DrawPath(nodesArray, pathColor);
            }
        } // dkoontz
    }

    //public static Vector3[] GetPath(string requestedName)
    //{
    //    requestedName = requestedName.ToLower();
    //    if (paths.ContainsKey(requestedName))
    //    {
    //        Vector3[] nodesArray = paths[requestedName].nodes.ToArray();
    //        if (paths[requestedName].isRelative)
    //        {
    //            Transform trans = paths[requestedName].transform;
    //            for (int i = 0; i < nodesArray.Length; i++)
    //                nodesArray[i] = trans.TransformPoint(nodesArray[i]);
    //        }
    //        return nodesArray;
    //    }
    //    else
    //    {
    //        Debug.Log("No path with that name exists! Are you sure you wrote it correctly?");
    //        return null;
    //    }
    //}

    public Vector3[] GetPath()
    {
        Vector3[] nodesArray = nodes.ToArray();
        if (isRelative)
        {
            Transform trans = transform;
            for (int i = 0; i < nodesArray.Length; i++)
                nodesArray[i] = trans.TransformPoint(nodesArray[i]);
        }
        return nodesArray;
    }
}

