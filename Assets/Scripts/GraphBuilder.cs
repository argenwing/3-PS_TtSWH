using System;
using UnityEngine;

/// <summary>
/// Builds the graph
/// </summary>
public class GraphBuilder : MonoBehaviour
{
    static Graph<Waypoint> graph;

    #region Constructor

    // Uncomment the code below after copying this class into the console
    // app for the automated grader. DON'T uncomment it now; it won't
    // compile in a Unity project

    /// <summary>
    /// Constructor
    /// 
    /// Note: The GraphBuilder class in the Unity solution doesn't 
    /// use a constructor; this constructor is to support automated grading
    /// </summary>
    /// <param name="gameObject">the game object the script is attached to</param>
    //public GraphBuilder(GameObject gameObject) :
    //    base(gameObject)
    //{
    //}

    #endregion

    /// <summary>
    /// Awake is called before Start
    ///
    /// Note: Leave this method public to support automated grading
    /// </summary>
    public void Awake()
    {
        // add nodes (all waypoints, including start and end) to graph
        Graph<Waypoint> graph = new Graph<Waypoint>();
        GameObject[] tempGraph = GameObject.FindGameObjectsWithTag("Waypoint");


        for (int i = 0; i < tempGraph.Length; i++)
        {
            //graph.AddNode(tempGraph[i]);
            Waypoint waypoint = tempGraph[i].GetComponent<Waypoint>();
            graph.AddNode(waypoint);
        }
        GameObject start = GameObject.FindGameObjectWithTag("Start");
        Waypoint startWaypoint = start.GetComponent<Waypoint>();
        graph.AddNode(startWaypoint);
        graph.AddNode(GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>());
        Array.Clear(tempGraph, 0, tempGraph.Length);

        // add neighbors for each node in graph

        for (int i = 0; i < graph.Count; i++)
        {
            for (int j = 0; j < graph.Count; j++)
            {
                float weightX = Math.Abs(graph.Nodes[i].Value.Position.x - graph.Nodes[j].Value.Position.x);
                float weightY = Math.Abs(graph.Nodes[i].Value.Position.y - graph.Nodes[j].Value.Position.y);
                if (weightX <= 3.5f && weightY <= 3.0f && weightX + weightY != 0)
                {
                    float weight = (float)Math.Sqrt(Math.Pow(weightX, 2.0f) + Math.Pow(weightY, 2.0f));
                    graph.Nodes[i].AddNeighbor(graph.Nodes[j], weight);
                }
            }
        }
        Graph = graph;
    }

    /// <summary>
    /// Gets and sets the graph
    /// 
    /// CAUTION: Set should only be used by the autograder
    /// </summary>
    /// <value>graph</value>
    public static Graph<Waypoint> Graph
    {
        get { return graph; }
        set { graph = value; }
    }

    public Graph<Waypoint> BuildedGraph
    {
        get { return graph; }
    }
}
