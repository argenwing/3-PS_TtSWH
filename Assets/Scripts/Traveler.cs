using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A traveler
/// </summary>
public class Traveler : MonoBehaviour
{
	#region Fields

	[SerializeField]
	GameObject prefabExplosion;

	// needed for the PathLength property
	float pathLength = 0;
	
    // events fired by class
    PathFoundEvent pathFoundEvent = new PathFoundEvent();
    PathTraversalCompleteEvent pathTraversalCompleteEvent = new PathTraversalCompleteEvent();

	// added
	const float ImpluseForceMagnitude = 2.0f;

	Rigidbody2D rgbd2D;
	Waypoint startWaypoint;
	Waypoint endWaypoint;
	GraphBuilder graphBuilder;

	LinkedList<Waypoint> allPath;
	LinkedListNode<Waypoint> currentTarget;

	#endregion

	#region Constructor

	// Uncomment the code below after you copy this class into the console
	// app for the automated grader. DON'T uncomment it now; it won't
	// compile in a Unity project

	/// <summary>
	/// Constructor
	/// 
	/// Note: The Traveler class in the Unity solution doesn't 
	/// use a constructor; this constructor is to support automated grading
	/// </summary>
	/// <param name="gameObject">the game object the script is attached to</param>
	//public Traveler(GameObject gameObject) :
	//    base(gameObject)
	//{
	//}

	#endregion

	#region Properties

	/// <summary>
	/// Gets the length of the final path
	/// 
	/// NOTE: This property should only be accessed after the
	/// Start method has been called (which is always the case
	/// in Unity)
	/// </summary>
	public float PathLength
    {
        get { return pathLength; }
    }

    #endregion
	
	#region Unity methods

	/// <summary>
	/// Use this for initialization
	/// 
	/// Note: Leave this method public to support automated grading
	/// </summary>
	public void Start()
	{
		EventManager.AddPathFoundInvoker(this);
		EventManager.AddPathTraversalCompleteInvoker(this);

		Vector3 position = transform.position;
		startWaypoint = GameObject.FindGameObjectWithTag("Start").GetComponent<Waypoint>();
		transform.position = startWaypoint.transform.position;

		rgbd2D = GetComponent<Rigidbody2D>();
		endWaypoint = GameObject.FindGameObjectWithTag("End").GetComponent<Waypoint>();
		graphBuilder = GameObject.FindGameObjectWithTag("Start").GetComponent<GraphBuilder>();
		Graph<Waypoint> graph = graphBuilder.BuildedGraph;
		//allPath = Search(startWaypoint, endWaypoint, GraphBuilder.Graph);
		allPath = Search(startWaypoint, endWaypoint, graph);

		currentTarget = allPath.First;

		GoToNextWaypoint();
	}

	#endregion

	#region Public methods


	void OnTriggerEnter2D(Collider2D other)
	{
		// check waypoint and target
		if (other.gameObject == currentTarget.Value.gameObject)
		{
			GoToNextWaypoint();
		}
	}

    /// <summary>
    /// Adds the given listener for the PathFoundEvent
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddPathFoundListener(UnityAction<float> listener)
    {
        pathFoundEvent.AddListener(listener);
    }

    /// <summary>
    /// Adds the given listener for the PathTraversalCompleteEvent
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddPathTraversalCompleteListener(UnityAction listener)
    {
        pathTraversalCompleteEvent.AddListener(listener);
    }

	/// <summary>
	/// Does a search for a path from start to end on
	/// graph
	/// 
	/// Note: Leave this method public to support automated grading
	/// </summary>
	/// <param name="start">start value</param>
	/// <param name="finish">finish value</param>
	/// <param name="graph">graph to search</param>
	/// <returns>string for path or empty string if there is no path</returns>
	public LinkedList<Waypoint> Search(Waypoint start, Waypoint end,
        Graph<Waypoint> graph)
    {
		// Create a search list (a sorted linked list) of search nodes 
		// (I provided a SearchNode class, which you should instantiate 
		// with Waypoint. I also provided a SortedLinkedList class)
		SortedLinkedList<SearchNode<Waypoint>> searchList =
			new SortedLinkedList<SearchNode<Waypoint>>();

		// Create a dictionary of search nodes keyed by the corresponding 
		// graph node. This dictionary gives us a very fast way to determine 
		// if the search node corresponding to a graph node is still in the 
		// search list
		Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>> pathNode =
			new Dictionary<GraphNode<Waypoint>, SearchNode<Waypoint>>();

		// Save references to the start and end graph nodes in variables
		GraphNode<Waypoint> startNode = graph.Find(start);
		GraphNode<Waypoint> endNode = graph.Find(end);

		// for each graph node in the graph
		foreach (GraphNode<Waypoint> graphNode in graph.Nodes)
		{
			// Create a search node for the graph node (the constructor I 
			// provided in the SearchNode class initializes distance to the max
			// float value and previous to null)
			SearchNode<Waypoint> searchNode = new SearchNode<Waypoint>(graphNode);

			// If the graph node is the start node
			if (graphNode == startNode)
			{
				// Set the distance for the search node to 0
				searchNode.Distance = 0;
			}
			// Add the search node to the search list 
			searchList.Add(searchNode);

			// Add the search node to the dictionary keyed by the graph node
			pathNode.Add(graphNode, searchNode);
		}

		// While the search list isn't empty
		while (searchList.Count > 0)
		{
			// Save a reference to the current search node (the first search 
			// node in the search list) in a variable. We do this because the
			// front of the search list has the smallest distance
			SearchNode<Waypoint> currentSearchNode = searchList.First.Value;

			// Remove the first search node from the search list
			searchList.RemoveFirst();

			// Save a reference to the current graph node for the current search
			// node in a variable
			GraphNode<Waypoint> currentGraphNode = currentSearchNode.GraphNode;

			// Remove the search node from the dictionary (because it's no 
			// longer in the search list)
			pathNode.Remove(currentGraphNode);

			// If the current graph node is the end node
			if (currentGraphNode == endNode)
			{
				// Display the distance for the current search node as the path 
				// length in the scene (Hint: I used the HUD and the event 
				// system to do this)
				pathFoundEvent.Invoke(currentSearchNode.Distance);

				// Return a linked list of the waypoints from the start node to 
				// the end node. Uncomment the line of code below, replacing
				// the argument with the name of your current search node
				// variable; you MUST do this for the autograder to work
				return BuildWaypointPath(currentSearchNode);
			}
			// For each of the current graph node's neighbors
			foreach (GraphNode<Waypoint> neighbor in currentGraphNode.Neighbors)
			{

				// If the neighbor is still in the search list (use the 
				// dictionary to check this)*********
				if (pathNode.ContainsKey(neighbor))
				{
					// Save the distance for the current graph node + the weight 
					// of the edge from the current graph node to the current 
					// neighbor in a variable**********
					float distant = currentSearchNode.Distance +
						currentGraphNode.GetEdgeWeight(neighbor);

					// Retrieve the neighor search node from the dictionary
					// using the neighbor graph node
					SearchNode<Waypoint> neighborSearchNode;
					neighborSearchNode = pathNode[neighbor];

					// If the distance you just calculated is less than the 
					// current distance for the neighbor search node*******
					if (distant < neighborSearchNode.Distance)
					{

						// Set the distance for the neighbor search node to 
						// the new distance
						neighborSearchNode.Distance = distant;

						// Set the previous node for the neighbor search node 
						// to the current search node
						neighborSearchNode.Previous = currentSearchNode;

						// Tell the search list to Reposition the neighbor 
						// search node. We need to do this because the change 
						// to the distance for the neighbor search node could 
						// have moved it forward in the search list
						searchList.Reposition(neighborSearchNode);
					}
				}
			}
		}
		// didn't find a path from start to end nodes
		return null;
	}

	#endregion
	
	#region Private methods
	
    /// <summary>
    /// Builds a waypoint path from the start node to the given end node
    /// Side Effect: sets the pathLength field
    /// 
    /// CAUTION: Do NOT change this method; if you do, you'll break the autograder
    /// </summary>
    /// <returns>waypoint path</returns>
    /// <param name="endNode">end node</param>
    LinkedList<Waypoint> BuildWaypointPath(SearchNode<Waypoint> endNode)
    {
        LinkedList<Waypoint> path = new LinkedList<Waypoint>();
        path.AddFirst(endNode.GraphNode.Value);
        pathLength = endNode.Distance;
        SearchNode<Waypoint> previous = endNode.Previous;
        while (previous != null)
        {
            path.AddFirst(previous.GraphNode.Value);
            previous = previous.Previous;
        }

        return path;
    }

	void Explode()
	{
		// remove start and end nodes
		allPath.RemoveFirst();
		allPath.RemoveLast();

		// start explosion
		LinkedListNode<Waypoint> currentWaypoint = allPath.First;
		while (currentWaypoint != null)
		{
			Instantiate(prefabExplosion, currentWaypoint.Value.transform.position,
				Quaternion.identity);
			Destroy(currentWaypoint.Value.gameObject);
			currentWaypoint = currentWaypoint.Next;
		}
	}

	void GoToNextWaypoint()
	{
		currentTarget = currentTarget.Next;
		if (currentTarget != null)
		{
			Vector2 direction = new Vector2(
				currentTarget.Value.transform.position.x - transform.position.x,
				currentTarget.Value.transform.position.y - transform.position.y);
			direction.Normalize();

			// control speed
			rgbd2D.velocity = Vector2.zero;
			rgbd2D.AddForce(direction * ImpluseForceMagnitude, ForceMode2D.Impulse);
		}
        else
        {
			rgbd2D.velocity = Vector2.zero;
			pathTraversalCompleteEvent.Invoke();
			Explode();
		}
	}

	#endregion
}
