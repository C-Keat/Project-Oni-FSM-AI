using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will store the path towards a location the AI wants to move towards
public class Path
{
    Vector3[] pathNodes;
    public int currentPathIndex = 0;

    public Path(Vector3[] pathNodes)
    {
        this.pathNodes = pathNodes;
    }



    public Vector3[] getPathNodes()
    {
        return pathNodes;
    }

    public Vector3 GetNextNode()
    {
        if (currentPathIndex < pathNodes.Length)
        {
            return pathNodes[currentPathIndex];
        }

        return Vector3.negativeInfinity; //we reached the end/ there is no path to follow

    }

    public bool reachedEndNode()
    {
        return (currentPathIndex == pathNodes.Length); //returns true if we have reached the end of our path
    }
}
