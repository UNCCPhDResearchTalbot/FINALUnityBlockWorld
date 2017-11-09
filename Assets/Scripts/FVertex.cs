using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

public class FVertex
{
	private Node vertexData;
	private List<FEdge> edges;
		
	public FVertex(Node data)
	{
		this.vertexData = data;
		this.vertexData.fvert = this;
		edges = new List<FEdge>();
	}

	public Node Data
	{
		get
		{
			return vertexData;
		}
		set
		{
			vertexData = value;
		}
	}
	
	public int EdgesCount
	{
		get
		{
			return edges.Count;
		}
	}
	
	
	
	public bool HasEdgeTo(FVertex toVertex)
	{
		for (int i = 0; i < edges.Count; i++)
		{
			
				if ((edges[i].ToVertex == toVertex) || ((edges[i].FromVertex == toVertex)))
				{
					return true;
				}
			
		}

		return false;
	}
	
	public FEdge GetEdgeTo(FVertex toVertex)
	{
		for (int i = 0; i < edges.Count; i++)
		{
							
				if ((edges[i].FromVertex == toVertex) || (edges[i].ToVertex == toVertex))
				{
					return edges[i];
				}
			
		}

		return null;
	}
	
	internal List<FEdge> EdgeList
	{
		get
		{
			return edges;
		}
	}
	
	internal void RemoveEdge(FEdge edge)
	{
		#region Asserts

		Debug.Assert(edge != null);

		#endregion

		RemoveEdgeFromVertex(edge);
	}

	internal void AddEdge(FEdge edge)
	{
		#region Asserts

		Debug.Assert(edge != null);

		#endregion

		
			edges.Add(edge);
		
	}
	
	private void RemoveEdgeFromVertex(FEdge edge)
        {
            #region Asserts

            Debug.Assert(this.edges.Remove(edge), "Edge not found on vertex in RemoveEdgeFromVertex.");

            #endregion

            this.edges.Remove(edge);

			
				edges.Remove(edge);
			
		}

		
}


