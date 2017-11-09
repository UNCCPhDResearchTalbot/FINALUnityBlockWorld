using System;
using System.Collections.Generic;
using System.Text;

public class FEdge
{
	FVertex from;
	FVertex to;
	double edgeWeight;
	String type;
		
	public FEdge (FVertex fromVertex, FVertex toVertex, String t) : this(fromVertex, toVertex, 0, t)
	{
	}
		
	public FEdge (FVertex fromVertex, FVertex toVertex, double weight, String t)
	{
		if (fromVertex == null) {
			throw new ArgumentNullException ("fromVertex");
		}

		if (toVertex == null) {
			throw new ArgumentNullException ("toVertex");
		}

		from = fromVertex;
		to = toVertex;
		edgeWeight = weight;
		type = t;
	}

	public FVertex FromVertex {
		get {
			return from;
		}
	}

	public FVertex ToVertex {
		get {
			return to;
		}
	}
		
	public double Weight {
		get {
			return edgeWeight;
		}
	}

	public FVertex GetPartnerVertex (FVertex vertex)
	{
		if (from == vertex) {
			return to;
		} else if (to == vertex) {
			return from;
		} else {
			throw new ArgumentException ("Vertex not part of edge");
		}
	}
	
	public String GetType() {
		return type;
	}
}


