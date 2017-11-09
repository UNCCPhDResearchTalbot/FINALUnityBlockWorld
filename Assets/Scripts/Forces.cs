using UnityEngine;
using System.Collections;

public class Forces : MonoBehaviour {
	
	public static bool recalc = false;
	public static float W = 90f; // update these values!!!!
	public static float L = 70f;
	public static float avgdist = 20f;// was 15f - trying to fix chars too close to move between //6f + (2f/3f); // was 150 with W=1060, L=916, bufferW=50, bufferL=10 // 4ft in old = 150 units, so 4ft here is 1+1/3 grid squares.  One grid square = 5 units => 6 2/3 units = 4ft
	public static int numiterations = 300;
	public static float area;
	public static int t;
	public static FGraph g;

	// Use this for initialization
	void Start () {
		area = W*L;
		t = numiterations;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void createForceGraph() {
		g = new FGraph();
	}
	
	public static void purgeGraph() {
		g.Clear ();
	}
	
	// call this to run my forces, then retrieve results & update/add character locomotiontargets to queue
	public static void recalculate() { // finish

			recalc = true;
			//cleanupxys();
			if (g.characters != null && ((g.characters.Count + g.humans.Count) > 1)) {
				Debug.Log("RECALCULATING");
				for (int cj = 0; cj < g.characters.Count; cj++) {
					Debug.Log(g.characters[cj].Data.name + ":" + g.characters[cj].Data.pos.x + "," + g.characters[cj].Data.pos.y);
				}
				//printAll();
				Debug.Log("READY");
				for (int i = 1; i < numiterations; i++) {// do 100 iterations?
					// calculate repulsive forces
					if (g.characters != null) {// initialize
						for (int vchar = 0; vchar < g.characters.Count; vchar++) {
							if (g.characters[vchar].Data.ismoveable) {//} && characters[vchar].onstage) {
								g.characters[vchar].Data.upddisp(0, 0);
							}
							//                characters[vchar].disp.x = 0;
							//                characters[vchar].disp.y = 0;
						}
						// calculate repulsive forces
						//console.log("repulsive");
						if (g.EdgeCount > 0) {
							//console.log("edges not null");
							for (int e = 0; e < g.EdgeCount; e++) {
								//if(i < 2) {console.log("--");}
								Vector2 delta = vectdiff(g.GetEdge(e).FromVertex.Data.pos, g.GetEdge(e).ToVertex.Data.pos);
								//[vchar].pos, characters[uchar].pos);
								if (delta.x == 0 && delta.y == 0) {
									// do nothing
								} else {
									//if(i < 2) {console.log(delta);}
									Vector2 temp1 = new Vector2(
										delta.x / vectsize(delta) * fr(vectsize(delta), g.GetEdge(e).GetType(), g.GetEdge(e)),
										delta.y / vectsize(delta) * fr(vectsize(delta), g.GetEdge(e).GetType(), g.GetEdge(e))
									);
									//if(i < 2) {console.log(temp1);}
									if (g.GetEdge (e).ToVertex.Data.ismoveable ){//}&& edges[e].point2.onstage) {
										g.GetEdge(e).ToVertex.Data.upddisp(vectdiff(g.GetEdge(e).ToVertex.Data.disp, temp1));
									}
									//if(i < 2) {console.log(edges[e].point2);}
									if (g.GetEdge(e).FromVertex.Data.ismoveable){// && edges[e].point1.onstage) {
										g.GetEdge(e).FromVertex.Data.upddisp(vectsum(g.GetEdge(e).FromVertex.Data.disp, temp1));
									}
									//if(i < 2) {console.log(edges[e].point1);}
								}
							}
						}
		//console.log("attractive");
						// calculate attractive forces
						if (g.EdgeCount > 0) {
							//console.log("edges not null");
							for (int e = 0; e < g.EdgeCount; e++) {
								//if(i < 2) {console.log("--");}
								Vector2 delta = vectdiff(g.GetEdge(e).FromVertex.Data.pos, g.GetEdge(e).ToVertex.Data.pos);
								//if(i < 2) {console.log(delta);}
								if (delta.x == 0 && delta.y ==0) {
									// do nothing
								} else {
									Vector2 temp2 = new Vector2(
										delta.x / vectsize(delta) * fa(vectsize(delta), g.GetEdge (e).GetType(), g.GetEdge(e)),
										delta.y / vectsize(delta) * fa(vectsize(delta), g.GetEdge (e).GetType(), g.GetEdge (e))
									);
									//if(i < 2) {console.log(temp2);}
									if (g.GetEdge(e).ToVertex.Data.ismoveable ){//&& edges[e].point2.onstage) {
										g.GetEdge(e).ToVertex.Data.upddisp(vectsum(g.GetEdge(e).ToVertex.Data.disp, temp2));
									}
									//if(i < 2) {console.log(edges[e].point2);}
									if (g.GetEdge(e).FromVertex.Data.ismoveable ){//&& edges[e].point1.onstage) {
										g.GetEdge(e).FromVertex.Data.upddisp(vectdiff(g.GetEdge(e).FromVertex.Data.disp, temp2));
									}
									//if(i < 2) {console.log(edges[e].point1);}
								}
							}
						}
						// {console.log("====");}
						// limit max displacement to temperature and prevent placement offstage
						for (int v = 0; v < g.characters.Count; v++) {
							Vector2 temp3 = new Vector2(
								g.characters[v].Data.disp.x / vectsize(g.characters[v].Data.disp),
								g.characters[v].Data.disp.y / vectsize(g.characters[v].Data.disp)
							);
							//if(i<2) {console.log(temp3);}
							float temp4 = Mathf.Min(vectsize(g.characters[v].Data.disp), t);
							//if(i<2) {console.log(temp4);}
							Vector2 temp5 = vectsum(g.characters[v].Data.pos, new Vector2(
								temp3.x * temp4,
								temp3.y * temp4
							));
							//if(i<2) {console.log(temp5);}
							if (vectsize(temp5) < (avgdist)) {
								// do nothing
							} else {//if (characters[v].onstage) {
								if (!float.IsNaN(temp5.x) && !float.IsNaN (temp5.y) )
								{
									g.characters[v].Data.updpos(Mathf.Min(W, Mathf.Max(0, temp5.x)), Mathf.Min(L, Mathf.Max(0, temp5.y)));
								}
							//} else {
								// do nothing
							}
						}
		
						t = numiterations - i;
						for (int cj = 0; cj < g.characters.Count; cj++) {
							//Debug.Log("FORCES POSITION UPDATED!! "+g.characters[cj].Data.name + ":" + g.characters[cj].Data.pos.x + "," + g.characters[cj].Data.pos.y);
						}
					//Debug.Log("FORCES POSITION UPDATED-new CENTER=("+g.center.Data.x+", "+g.center.Data.y+"), pos=("+g.center.Data.pos.x+", "+g.center.Data.pos.y+")");
					}
				}
				// reset everything to defaults
		
				t = numiterations;
				for (int a = 0; a < g.characters.Count; a++) {
					g.characters[a].Data.x = g.characters[a].Data.pos.x;
					g.characters[a].Data.y = g.characters[a].Data.pos.y;
					if (g.characters[a].Data.ismoveable ){//&& characters[a].onstage) {
						g.characters[a].Data.upddisp(0, 0);
						// = {
					}
					//      x : 0,
					//     y : 0
					//   };
				}
				posrecalcCenter();
			
			
			  	Debug.Log("===================AFTER RECALCULATING===========");
				 for (int a = 0; a < g.Size; a++) {
				 Debug.Log(g.GetVertex(a).Data.type+": "+g.GetVertex(a).Data.name + " at (" + g.GetVertex(a).Data.x + "," + g.GetVertex(a).Data.y + ")");
				 }
			 }
			 /*for (var b = 0; b < edges.length; b++) {
			 console.log(edges[b].type+" EDGE: "+edges[b].point1.name + "-" + edges[b].point2.name + ", dist=" + getLength(edges[b].point1, edges[b].point2));
			 }*/
			recalc = false;
			//reversexys();
		
	}
	
	public static float fa (float x, string t, FEdge e) { // finish
		switch(t) {
		case "audience":
			//...
			if (e.ToVertex.Data.name == "CENTERA") {
				return (x*x) - ((L/4)*(L/4));
			} else {
				return (x*x) - ((L/2)*(L/2));
			}			
			break;
		case "center":
		case "char":
			return (x*x) - (avgdist * avgdist);
			break;
		case "target":
			return (x*x) * 6;
			break;
		case "human":
			return (x*x*.5f) - (avgdist * avgdist);
			break;
		default:
			return (x*x) - (avgdist * avgdist);
		}
		//return 0f;
	}
	
	public static float fr (float x, string t, FEdge e) { // finish
		switch(t) {
		case "audience":
			//...
			if (e.ToVertex.Data.name == "CENTERA") {
				return 0;
			} else {
				return (-1*x*x) + ((L/2)*(L/2));
			}			
			break;
		case "center":
		case "char":
			return (-1*x*x) + (avgdist * avgdist);
			break;
		case "target":
			return 0;
			break;
		case "human":
			return 0;
			break;
		default:
			return (-1*x*x) + (avgdist * avgdist);
		}
		//return 0f;
	}
	
	public static Vector2 vectsum(Vector2 A, Vector2 B) {
		return new Vector2(A.x + B.x, A.y + B.y);
	}
	
	public static float vectsize(Vector2 A) {
		return Mathf.Sqrt ((A.x * A.x) + (A.y * A.y));
	}
	
	public static Vector2 vectdiff(Vector2 A, Vector2 B) {
		return new Vector2(A.x - B.x, A.y - B.y);
	}
	
	public static void recalcCenter() { // finish
		if (g.center != null) {
			float summedx = 0;
			float summedy = 0;
			for (int i = 0; i < g.characters.Count; i++) {
				summedx = summedx + g.characters[i].Data.x;
				summedy = summedy + g.characters[i].Data.y;
			}
			if (g.humans.Count > 0) {
				summedx = summedx + g.humans[0].Data.x;
				summedy = summedy + g.humans[0].Data.y;
			}
			g.center.Data.updpos(summedx/(g.characters.Count + g.humans.Count), 15 + (summedy / (g.characters.Count + g.humans.Count)));
			//Debug.Log("UPDATED CENTER--char="+g.characters.Count+", hum="+g.humans.Count+", ("+summedx/(g.characters.Count + g.humans.Count)+", "+ summedy / (g.characters.Count + g.humans.Count)+")");
		}
	}
	
	public static void posrecalcCenter() { // finish
		if (g.center != null) {
			float summedx = 0;
			float summedy = 0;
			for (int i = 0; i < g.characters.Count; i++) {
				summedx = summedx + g.characters[i].Data.pos.x;
				summedy = summedy + g.characters[i].Data.pos.y;
			}
			if (g.humans.Count > 0) {
				summedx = summedx + g.humans[0].Data.x;
				summedy = summedy + g.humans[0].Data.y;
			}
			g.center.Data.updpos(summedx/(g.characters.Count + g.humans.Count), 15 + (summedy / (g.characters.Count + g.humans.Count)));
			//Debug.Log("UPDATED CENTER--char="+g.characters.Count+", hum="+g.humans.Count+", ("+summedx/(g.characters.Count + g.humans.Count)+", "+ summedy / (g.characters.Count + g.humans.Count)+")");
		}
	}
	
	public static void translateToForces(Node n) {
		n.x = 45 - n.x;
		n.pos.x = n.x;
	}
	
	public static void translateToUI(Node n) {
		n.x = 45 - n.x;
	}
	
	public static void setupForces() {
		// setup force graph based on current character info from queues and settings
		Node newNode;
		Node newTarget;
		// go through each character
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!---Start setup forces---");
		foreach (CharFuncs c in GlobalObjs.listOfChars) {
			
			// check if last movement puts them onstage or not, if no movement, then current position
			Vector3 lastPostn = c.getLastMovePostn();
			Debug.Log ("Last posn="+lastPostn);
			if (positionOnStage(lastPostn)) {
			
				// if onstage, add to graph & make moveable or not based on pickup location & type
				newNode = new Node(c.name, c.name, "char", lastPostn.x, lastPostn.z, null, c);
			
				// add targets if exist for the last movement
				GameObject lastTarget = c.getLastTarget();
				if (lastTarget != null && !(isAChar (lastTarget.name))) { // !(lastTarget.name == "Hamlet" || lastTarget.name == "Horatio" || lastTarget.name == "GraveDigger" || lastTarget.name == "GraveDiggerTwo")) {
						// add target to graph
						newTarget = new Node(lastTarget.name, lastTarget.name, "target", lastTarget.transform.position.x, lastTarget.transform.position.z, null);
						g.AddEdge(newNode.fvert, newTarget.fvert, "target");
				}
				if(lastPostn == c.thisChar.transform.position) {
					if (c.lastmoveToObj == null) {
						// isn't moving, so add target as their current position so they don't move too far
						newTarget = new Node(c.name+"-current postn", c.name+"-current postn", "target", lastPostn.x, lastPostn.z, null);
						g.AddEdge (newNode.fvert, newTarget.fvert, "target");
					} else {
						// had a previous target, so keep it
						newTarget = new Node(c.name+"-old target", c.name+"-old target", "target", c.lastmoveToObj.transform.position.x, c.lastmoveToObj.transform.position.z, null);
						g.AddEdge (newNode.fvert, newTarget.fvert, "target");
					}
				}
				Debug.Log("          "+c.name+", cur=("+c.transform.position.x+", "+c.transform.position.z+"), last=("+lastPostn.x+", "+lastPostn.z+") target="+lastTarget);
			} else {
				Debug.Log ("*********DID NOT ADD CHAR TO FORCE GRAPH**********");
			}
			
			
		}
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!---Stop setup forces---");
		// use the translateToForces to make the x's match after all nodes are setup
		foreach (FVertex v in g.graphVertices) {
			//Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++befor="+v.Data.x);
			translateToForces (v.Data);
			Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++after="+v.Data.name+":"+v.Data.x);
		}
	}
	
	public static bool isAChar(string objname) {
		foreach (CharFuncs c in GlobalObjs.listOfChars) {
			if (c.thisChar.name == objname) {
				return true;
			}
		}
		return false;
	}
	
	public static void unsetupForces() {
		// use the translateToUI to make the x's match after all calculations are done
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!---Start translate forces---");
		foreach (FVertex v in g.graphVertices) {
			//Debug.Log("          befor"+v.Data.name+", "+v.Data.type+", ("+v.Data.x+", "+v.Data.y+")");
			translateToUI (v.Data);
			Debug.Log("          after"+v.Data.name+", "+v.Data.type+", ("+v.Data.x+", "+v.Data.y+")");
		}
		Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!---End translate forces---");
	}
	
	public static void applyChanges() {
		// update all character positions based on output from forces -- be careful with pickup movements & avoid changing human characters
		
		
		// go through each character in force graph
		foreach(FVertex f in g.characters) {
			
		
			// find last movement for character & update position to what is in the graph
			
		
			// if no movement for the character, then add one for the new position - be sure to set the new messagenum for it!!! and add to queue
			f.Data.origObj.updateLastPostn(f.Data.x, f.Data.y);
		}
		// clear out graph
		g.Clear();
	}
	
	public static void LinkToAllChars(FVertex f) {
		// go through list of chars, check if same as n, if not, add edge
		foreach (FVertex curChar in g.characters) {
			if (curChar.Data.name != f.Data.name) {
				Forces.g.AddEdge (f, curChar, "character");
			}
		}
	}
			
	public static bool positionOnStage(Vector3 v) {
		if (v.z > 0 && v.z < 110) {
			if (v.x > -45 && v.x < 45) {
				return true;
			}
		}
		return false;
	}
	
}
