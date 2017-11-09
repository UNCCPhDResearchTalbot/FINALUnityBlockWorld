using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class FGraph
{
	
		#region Globals

	public List<FVertex> graphVertices;
	public List<FEdge> graphEdges;
	public List<FVertex> characters;
	public List<FVertex> humans;
	public FVertex center;
	

		#endregion

		#region Construction
        
	/// <summary>
	/// Initializes a new instance of the <see cref="Graph&lt;T&gt;"/> class.
	/// </summary>
	/// <param name="isDirected">if set to <c>true</c> [is directed].</param>
	public FGraph ()
	{
		graphVertices = new List<FVertex> ();
		graphEdges = new List<FEdge> ();
		characters = new List<FVertex>();
		humans = new List<FVertex>();
		center = null;
	}

		#endregion
		
	public bool IsEmpty {
		get {
			return this.VertexCount == 0;
		}
	}
	
	public int Size {
		get {
			return this.VertexCount;
		}
	}
		
	public void Clear ()
	{
		/*foreach (FEdge e in graphEdges) {
			Destroy(e.FromVertex.Data);
			Destroy(e.FromVertex);
			Destroy(e.ToVertex.Data);
			Destroy(e.ToVertex);
			Destroy(e);
		}
		foreach (FVertex f in graphVertices) {
			Destroy(f.Data);
			Destroy(f);
		}*/
		graphVertices.Clear ();
		graphVertices = new List<FVertex>();
		graphEdges.Clear ();
		graphEdges = new List<FEdge>();
		characters.Clear ();
		characters = new List<FVertex>();
		humans.Clear ();
		humans = new List<FVertex>();
		//Destroy(center.Data);
		//Destroy(center);
		center = null;
	}
	
	public bool RemoveVertex (FVertex vertex)
	{
		if (vertex == null) {
			throw new ArgumentNullException ("vertex");
		}

		if (!graphVertices.Remove (vertex)) {
			return false;
		} else {
			// Delete all the edges in which this vertex forms part of
			List<FEdge> list = vertex.EdgeList;
				
			while (list.Count > 0) {
				RemoveEdge (list [0]);
			}

			return true;
		}
	}
		
	public bool RemoveVertex (Node item) 
	{
		for (int i = 0; i < graphVertices.Count; i++) {
			if (graphVertices [i].Data.name ==  (item.name)) {
				RemoveVertex (graphVertices [i]);
				return true;
			}
		}

		return false;
	}
	
	public int EdgeCount {
		get {
			return graphEdges.Count;
		}
	}
		
	public int VertexCount {
		get {
			return graphVertices.Count;
		}
	}
		
	public bool RemoveEdge (FEdge edge)
	{
		//CheckEdgeNotNull (edge);

		if (!graphEdges.Remove (edge)) {
			return false;
		}

		edge.FromVertex.RemoveEdge (edge);
		edge.ToVertex.RemoveEdge (edge);

		return true;
	}
		
	public bool RemoveEdge (FVertex from, FVertex to)
	{
		if (from == null) {
			throw new ArgumentNullException ("from");
		}

		if (to == null) {
			throw new ArgumentNullException ("to");
		}

			
		for (int i = 0; i < graphEdges.Count; i++) {
			if (((graphEdges [i].FromVertex == from) && (graphEdges [i].ToVertex == to)) ||
						((graphEdges [i].FromVertex == to) && (graphEdges [i].ToVertex == from))) {
				RemoveEdge (graphEdges [i]);
				return true;
			}
		}
			

		return false;
	}
		
	public void AddEdge (FEdge edge)
	{
		//CheckEdgeNotNull (edge);

		if ((!graphVertices.Contains (edge.FromVertex)) || (!graphVertices.Contains (edge.ToVertex))) {
			throw new ArgumentException ("Vertex could not be found");
		}
            
		if (edge.FromVertex.HasEdgeTo (edge.ToVertex)) {
			throw new ArgumentException ("Edge already exists");
		}

		graphEdges.Add (edge);
		AddEdgeToVertices (edge);
	}
		
	public void AddVertex (FVertex vertex)
	{
		if (graphVertices.Contains (vertex)) {
			throw new ArgumentException ("Vertex already exists");
		}
		if (vertex.Data.type == "char") {
			characters.Add (vertex);
		}
		if (vertex.Data.type == "human") {
			humans.Add (vertex);
		}

		graphVertices.Add (vertex);
	}
	
	public FVertex AddVertex (Node item)
	{
		FVertex vertex = new FVertex (item);
		if (vertex.Data.type == "char") {
			characters.Add (vertex);
		}
		if (vertex.Data.type == "human") {
			humans.Add (vertex);
		}
		graphVertices.Add (vertex);
		return vertex;
	}
		
	public FEdge AddEdge (FVertex from, FVertex to, String type)
	{
		FEdge edge = new FEdge (from, to, type);
		AddEdge (edge);
		return edge;
	}
		
	public void AddEdge (FVertex from, FVertex to, double weight, String type)
	{
		FEdge edge = new FEdge (from, to, weight, type);
		AddEdge (edge);
	}
		
	public IEnumerator<FVertex> Vertices {
		get {
			return graphVertices.GetEnumerator ();
		}
	}
		
	public IEnumerator<FEdge> Edges {
		get {
			return graphEdges.GetEnumerator ();
		}
	}
		
	public FEdge GetEdge (FVertex from, FVertex to)
	{
		return from.GetEdgeTo (to);
	}
		
	private void AddEdgeToVertices (FEdge edge)
	{
			#region Asserts

		System.Diagnostics.Debug.Assert (edge != null);
		System.Diagnostics.Debug.Assert (edge.FromVertex != null);
		System.Diagnostics.Debug.Assert (edge.ToVertex != null);

			#endregion

		edge.FromVertex.AddEdge (edge);
		edge.ToVertex.AddEdge (edge);
	}
		
		
	public FEdge GetEdge(int i) {
		return graphEdges[i];
	}
	
	public FVertex GetVertex(int i) {
		return graphVertices[i];
	}
	
	public void printall() {
		UnityEngine.Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!**************PRINTING GRAPH-start");
		UnityEngine.Debug.Log("--Vertex count="+graphVertices.Count+", Edge count="+graphEdges.Count);
		foreach(FVertex f in graphVertices) {
			UnityEngine.Debug.Log("Vertex-"+f.Data.name+", "+f.Data.type+", ("+f.Data.x+", "+f.Data.y+")");//, aud=("+f.Data.aud.x+","+f.Data.aud.y+")");
		}
		foreach(FEdge e in graphEdges) {
			UnityEngine.Debug.Log("Edge-"+e.FromVertex.Data.name+" to "+e.ToVertex.Data.name+", type="+e.GetType());
		}
		if (center !=null) {
			UnityEngine.Debug.Log("Center-"+center.Data.x+", "+center.Data.y);
		}	
		UnityEngine.Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!**************PRINTING GRAPH-end");
	}
	
	
		
}


