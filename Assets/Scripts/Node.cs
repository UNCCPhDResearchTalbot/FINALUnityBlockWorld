using System;
using UnityEngine;

public class Node
{
	public String calledby;
	public String name;
	public String type;
	public float x;
	public float y;
	//int rank;
	//bool onstage;
	//float lastangle;
	public bool ismoveable;
	public Vector2 disp;
	public Vector2 pos;
	public Vector2 orig;
	public Node aud;
	public FVertex fvert;
	public CharFuncs origObj;
	
	public Node (String n, String m, String t, float a, float b, FVertex f) : this(n,m,t,a,b,f,null) {}
	public Node (String n, String m, String t, float a, float b) : this(n,m,t,a,b,null, null) {}
	public Node (String n, String m, String t, float a, float b, FVertex f, CharFuncs g)
	{
		name = n;
		calledby = m;
		type = t;
		x = a;
		y = b;
		if (f == null) {
			fvert = new FVertex(this);
		} else {
			fvert = f;
		}
		origObj = g;
		switch (type) 
		{
		case "char":
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = true;
			// add to char list & all
			//Forces.g.characters.Add (fvert); // how do I get this vertex to add here?
			Forces.g.AddVertex(fvert); // really should have this as the vertex - maybe put the logic to add to char list when do vertex?
			// add audience
			FVertex aud2 = Forces.g.AddVertex (new Node(this.name+"A", this.name+"A", "audience", this.x, Forces.L+5, null));
			aud = aud2.Data;
			// add audience link
			Forces.g.AddEdge (fvert, aud2,"audience");
			
			// add links to all other chars
			Forces.LinkToAllChars(fvert);
			// create center or link to center
			if (Forces.g.center == null && (Forces.g.characters.Count + Forces.g.humans.Count >=2)) {
				Debug.Log("CREATED CENTER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				new Node("CENTER", "CENTER", "center", this.x, this.y, null);
				Forces.LinkToAllChars(Forces.g.center);
			} else if (Forces.g.center != null) {
				Forces.g.AddEdge (Forces.g.center, fvert, "center");
			}
				
			// recalculate center
			Forces.recalcCenter();
			break;
		case "target":
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = false;
			// add to all
			Forces.g.AddVertex (fvert);
			break;
		case "human":
			calledby = "HUMAN";
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = false;
			// capture original as x&y?
			orig.x = x;
			orig.y = y;
			// add to all
			Forces.g.AddVertex (fvert);
			// add to humans - done above
			//add audience
			FVertex haud = Forces.g.AddVertex (new Node(this.name+"A", this.name+"A", "audience", this.x, Forces.L+5, null));
			aud = haud.Data;
			// add audience link
			Forces.g.AddEdge (fvert, haud,"audience");
			
			
			// add links to all other chars
			Forces.LinkToAllChars(fvert);
			// create center or link to center
			if (Forces.g.center == null && Forces.g.characters.Count + Forces.g.humans.Count >=2) {
				new Node("CENTER", "CENTER", "center", this.x, this.y, null);
				Forces.LinkToAllChars(Forces.g.center);
			} else if (Forces.g.center != null) {
				Forces.g.AddEdge (Forces.g.center, fvert, "center");
			}
				
			// recalculate center
			Forces.recalcCenter();
			break;
		case "center":
			// calculate x & y using chars and humans (sum them then divide by count of two groups)
			calledby = "CENTER";
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = true;
			// add to all
			Forces.g.AddVertex (fvert); // duplicates the center vertex - don't do!!
			Forces.g.center = fvert;
			// add to chars???
			// create link to self for all chars & humans
			// create audience
			FVertex caud = Forces.g.AddVertex (new Node(this.name+"A", this.name+"A", "audience", this.x, Forces.L+5, null));
			aud = caud.Data;
			// create link audience
			Forces.g.AddEdge (fvert, caud, "audience");
			
			
			break;
		case "audience":
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = false;
			// add to all
			//Forces.g.AddVertex (fvert);
			break;
		default:
			disp = new Vector2(0f,0f);
			pos = new Vector2(x,y);
			ismoveable = false;
			break;
		}
		
		
	}
	
	public void doupdate(float x, float y) {
		if (type == "center" || type == "target" || type == "human") {
			this.x = x;
			this.y = y;
		}
	}
	
	public void updpos(float x, float y) {
		if (type == "char" || type == "center") {
			
			pos.x = x;
			pos.y = y;
			aud.x = x;
			aud.pos.x = x;
		}
		if (type == "center") {
			//Debug.Log("center updated");
			doupdate (x,y);
		}
		if (type == "char") {
			Forces.recalcCenter();
		}	
	}
	
	public void upddisp(float x, float y) {
		if (type == "char" || type == "center") {
			disp.x = x;
			disp.y = y;
			aud.disp.x = x;
		}
	}
	
	public void upddisp(Vector2 vec) {
		if (type == "char" || type == "center") {
			disp.x = vec.x;
			disp.y = vec.y;
			aud.disp.x = vec.x;
		}
	}
	
	public void dodelete() {// need more thought on this for the diff types!!!
		//Forces.g.RemoveVertex ();
		Forces.g.RemoveVertex(this);
	}
	
}


