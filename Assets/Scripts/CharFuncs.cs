using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharFuncs : MonoBehaviour {
	
	public GUIStyle mystyle;
	public Texture myicon;
	public Texture bubbleTexture;
	
	// GameObject this function is tied to!!
	public GameObject thisChar;
	public CharacterController thisCharController;
	public string voice;
	
	//variables for speech
	System.Diagnostics.Process myProcess;
	
	// variables for the rotate
	Vector3 rotateTo; // the destination point to turn to
	public Vector3 lastrotateTo; // last position looked at
	GameObject rotateToObj; // the destination object to turn to
	public GameObject lastrotateToObj; // last object looked at
	int rotateDir; // 1 to turn clockwise, -1 to turn counterclockwise
	bool rotating; // true if currently rotating
	//Queue rotateQueue; // if already rotating, additional rotateInfo - stored in Vector2 or GameObject
	public static float rspeed = 50;//30;
	
	// to keep from triggering too many times for collision
	bool collided = false;
	
	// variables for the move
	public Vector3 moveTo;
	public GameObject moveToObj;
	public Vector3 prevpostn;
	public Vector3 prevpostn2;
	public List<Vector3> priorpostns = new List<Vector3>();
	int which; // x position
	bool moving;
	
	public GameObject lastmoveToObj;
	public bool forcedmove = false;
	
	//Queue moveQueue;
	public static float mspeed = 5;
	bool following;
	public CharFuncs moveToObjFunc;
	public bool beingfollowed = false;
	float timer = 0.0f;
	public static float timerMax = 3.0f; // for following delay?
	bool left = false; // used for pointing on the left vs right side of character
	Queue<miniQueueObj> genQueue;
	
	//variables for the pickup/putdown
	bool shrinking = false;
	bool growing = false;
	GameObject manipObj;
	GameObject extraObj;
	public static float sspeed = 20f;
	bool pickup = true;
	static float carrydropheight = 0.5f;
	Vector3 curscalesize;
	
	//variables for pointing
	bool pointing = false;
	GameObject pointtarget;
	float pointertimer = 0.0f;
	public static float pointertimerMax = 2.0f;
	public GameObject prefabarm;
	Material armmat;
	public string armcolor;
	GameObject arm;
	public int pointnum = -1;
	public string bodycolor;
	bool bodycolorset = false;
	
	// static constants
	static Vector3 nullVector = new Vector3(0,0,0);
	float curHeight;
	float halfHeight;
	float holdwhere = 1.5f;
	
	public int workingNum = -1;
	public int speakNum = -1;
	bool speaking = false;
	string saywhat = "";
	float stimer = 0.0f;
	float stimerMin = 3.0f;
	
	void Awake() {
		thisChar = gameObject;
		thisCharController = thisChar.GetComponent<CharacterController>();
	}
	
	void initPostns() { // assumes 5 last postns to check
		priorpostns = new List<Vector3>();
		for (int i=0; i<5; i++) {
			priorpostns.Add (new Vector3(-99, -99, -99));	
		}
	}
	
	bool isInPrior( Vector3 checkpostn) { // looks for exact matches - might look for "close" instead
		bool myreturn = false;
		checkpostn.x = (float)System.Math.Round (checkpostn.x, 2);
		checkpostn.y = 0f;
		checkpostn.z = (float)System.Math.Round (checkpostn.z, 2);
		//System.Math.Round(val,2);
		if (priorpostns.IndexOf(checkpostn) != -1) {
			myreturn = true;
		}
		return myreturn;
	}
	
	int howmanyprior(Vector3 checkpostn) {
		// count how many times find match
		int count = 0;
		checkpostn.x = (float)System.Math.Round (checkpostn.x, 2);
		checkpostn.y = 0f;
		checkpostn.z = (float)System.Math.Round (checkpostn.z, 2);
		foreach (Vector3 v in priorpostns) {
			if (checkpostn == v) {
				count++;
			}
		}
		return count;
	}
	
	void insertcurpostn(Vector3 curpostn) { // add to end of list & remove first
		priorpostns.RemoveAt(0);
		curpostn.x = (float)System.Math.Round (curpostn.x, 2);
		curpostn.y = 0f;
		curpostn.z = (float)System.Math.Round (curpostn.z, 2);
		priorpostns.Add(curpostn);
	}
	
	// Use this for initialization of vars
	void Start () {
		mystyle.normal.textColor = Color.white;
		mystyle.alignment = TextAnchor.MiddleCenter;
		thisChar = gameObject;
		thisCharController = thisChar.GetComponent<CharacterController>();
		//speechfunc = (SpeechBubble)thisChar.GetComponent(typeof(SpeechBubble));
		speaking=false;
		armmat = GlobalObjs.getMaterial(armcolor);
		Debug.Log ("armcolor="+armcolor);
		Debug.Log ("voice for "+thisChar.name+" ="+voice+"XX");
		/*switch(this.name) {
			case "Hamlet":
				voice = "Alex";
				break;
			case "GraveDigger":
				voice="Ralph";
				break;
			case "GraveDiggerTwo":
				voice="Bruce";
				break;
			case "Horatio":
				voice="Fred";
				break;
		}*/
		
		
		
		rotateTo = nullVector;
		rotateToObj = null;
		rotating = false;
		rotateDir = 1;
		
		moveTo = nullVector;
		moveToObj = null;
		moving = false;
		following = false;
		which = -1;
		initPostns();
		
		genQueue = new Queue<miniQueueObj>();
		
		growing = false;
		shrinking = false;
		manipObj = null;
		pickup = true;
		
		curHeight = thisChar.transform.localScale.y;
		halfHeight = curHeight/2f;
		
		pointing = false;
		pointtarget = null;
		pointertimer = 0.0f;
	
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit) {
		// when character collider hits, make them shift a little
		//int which = UnityEngine.Random.Range(0,2); // x position
		float amtdist = Random.Range(1.0f, 5.0f);
		
		// check if what I hit is a character or not - ignore if not character
		Vector3 howmuch = thisChar.transform.right*amtdist*3f*Time.deltaTime;//Vector3.left*3*Time.deltaTime;
		howmuch.y = 0f;
		
		if (howmuch.x == 0f && howmuch.z == 0f) {
			howmuch.x += amtdist*3f*Time.deltaTime;
			
		}
		
		
		// maybe let this run every time a collision happens?  need to make sure it is another char, not the floor
		//if (!collided && GlobalObjs.isChar(hit.gameObject)) {
		if (GlobalObjs.isChar (hit.gameObject)) {
			Debug.Log ("COLLISION!!!!--STOP EVERYTHING!!!!--CAN USE ARRAY HERE?"+thisChar.name + " hit "+hit.gameObject.name);
		
			//if (which == 0) {
			Debug.Log ("Moved char by "+howmuch);
				thisCharController.Move(howmuch);// ((Vector3.left)*2*Time.deltaTime);
			//} else {
			//	thisCharController.Move(howmuch);// ((Vector3.right)*2*Time.deltaTime);
			//}
			Debug.Log (thisChar.name+"--collided!"+hit.gameObject.name);
			collided = true;
		}
		if (thisChar.transform.position.y != 0) {
			//Debug.Log (thisChar.name+" -- NOT ZERO!! at end of collider");
			thisChar.transform.position = new Vector3(thisChar.transform.position.x, 0.0f, thisChar.transform.position.z);
		}
		//Debug.Log ("**********"+thisChar.name+" GOT STUCK "+((which==0)?("left"):("right"))+"*************");
	}
	
	// Update is called once per frame - when action occurring, do something
	void Update () {
		/*if (thisChar.transform.position.y != 0) {
			Debug.Log (thisChar.name+" -- NOT ZERO!! at beginning of update");
		}*/
		collided = false;
		if (armmat == null) {
			armmat = GlobalObjs.getMaterial(armcolor);
			Debug.Log ("armcolorupdate="+armcolor);
		}
		
		if (thisCharController == null) {
			thisCharController = thisChar.GetComponent<CharacterController>();
		}
		if (speaking) {
			stimer += Time.deltaTime;
		}
		if (myProcess != null && myProcess.WaitForExit(1000) && stimer > stimerMin) {
			// tell everyone I'm done speaking
			Debug.Log ("Done Speaking at "+Time.time+" for "+thisChar.name);
			myProcess.Close ();
			myProcess = null;
			//speechfunc.showbubble = false;
			speaking=false;
			saywhat = "";
			stimer = 0.0f;
			GlobalObjs.removeOne(speakNum);
			//speakNum = -1;
		}
		if (pointing) {
			pointertimer += Time.deltaTime;
			if (pointertimer >= pointertimerMax) {
				// done pointing
				pointertimer = 0.0f;
				pointing = false;
				// detach arm
				arm.transform.parent = null;
				// delete arm
				Destroy (arm);
				pointtarget = null;
				arm = null;
				GlobalObjs.removeOne(pointnum);
			} else {
				// update arm as needed to point to target if char or target moved
				Vector3 relativePoint = thisChar.transform.InverseTransformPoint(pointtarget.transform.position);
				if (relativePoint.x < 0.0f) {
					left = true;
				} else {
					left = false;
				}
				Vector3 correctedstart;
				if (!left) {
					correctedstart = thisChar.transform.position + thisChar.transform.right.normalized;
				} else {
					correctedstart = thisChar.transform.position - thisChar.transform.right.normalized;
				}
				Vector3 newstart = new Vector3(correctedstart.x, 3.5f, correctedstart.z);
				Vector3 offset = pointtarget.transform.position - newstart;
				//Vector3 scale = new Vector3(0.5f, 2.0f, 0.5f);
				Vector3 position = newstart + (offset / 2.0f);
				//GameObject myarm = Instantiate (prefabarm, position, Quaternion.identity) as GameObject;
				//arm.transform.localScale = new Vector3(.5f, 1f, .5f);
				arm.transform.position = newstart+ offset.normalized;
				arm.transform.up = offset;
				//myarm.renderer.material = armmat;
				// need to attach to character so can move with character
				//myarm.transform.parent = thisChar.transform;
				//myarm.transform.localScale = scale;
			}
		}
		
		// begin exclusive commands!
		bool shortenmove = false;
		bool shortenrotate = false;
		if (following) {
			// do nothing for 1 second	
			timer += Time.deltaTime;
			if (timer >= timerMax) {
				// ready to start
				timer = 0.0f;
				following = false;
			}
			
		} else if (shrinking) {
			//Debug.Log ("SHRINKING!!");
			// scale char down
			float samt = Mathf.Min (Time.deltaTime*sspeed, thisChar.transform.localScale.y - halfHeight); // so don't go past 10f shrinking
			
			if (!pickup) {
				manipObj.transform.parent = null;
			}
			if (extraObj != null) {
				extraObj.transform.parent = null;
			}
			
			thisChar.transform.localScale += new Vector3(0f, -1*samt, 0f);
			
			if (!pickup) { // if putting down, handle object while shrinking also - be sure to put down current object if picking up a new one
				//manipObj.transform.localScale = new Vector3(1f,1f,1f);
			//	manipObj.transform.localScale += new Vector3(0f, 1*samt, 0f);
				//Debug.Log ("Shrink size="+thisChar.renderer.bounds.size.y);
				manipObj.transform.position = new Vector3(manipObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, manipObj.transform.position.z);
				manipObj.transform.parent = thisChar.transform;
			}
			if (extraObj != null) {
				extraObj.transform.position = new Vector3(extraObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, extraObj.transform.position.z);
				extraObj.transform.parent = thisChar.transform;
			}
			
			if (thisChar.transform.localScale.y <= halfHeight) {
				Debug.Log ("Done shrinking "+thisChar.name);
				// move object, attach later -- need to figure out the right position better here
				Vector3 temp = thisChar.transform.position + thisChar.transform.right.normalized*-1*holdwhere;
				manipObj.transform.position = new Vector3 (temp.x, carrydropheight, temp.z);//thisChar.transform.position + thisChar.transform.right.normalized*holdwhere;
				//new Vector3(thisChar.transform.position.x+.35f, 0, thisChar.transform.position.z);
				//manipObj.transform.position.y = carrydropheight;
				manipObj.transform.rotation = thisChar.transform.rotation;
				shrinking = false; // done shrinking
				if (pickup) { // attach object
					manipObj.transform.parent = thisChar.transform;
					curscalesize = manipObj.transform.localScale;
					//manipObj.transform.localPosition=new Vector3(.5f, 0f, 0f); // put it to char's right side
					//manipObj.transform.localRotation=Quaternion.identity; // keep same rotation as char
					Debug.Log ("Attached object");
				} else { // detach object
					manipObj.transform.parent = null;
					Debug.Log ("Detached object");
					//manipObj.transform.parent = null;
				}
				if (extraObj != null) {
					extraObj.transform.position = new Vector3(temp.x, carrydropheight, temp.z);
					extraObj.transform.rotation = thisChar.transform.rotation;
					extraObj.transform.parent = null;
					extraObj = null;
				}
				growing = true; // start growing
				//Debug.Log ("Ready to grow");
			}
		} else if (growing) {
			float gamt = Mathf.Min (Time.deltaTime*sspeed, curHeight - thisChar.transform.localScale.y); // so don't grow past 20f
			if (pickup) {
				manipObj.transform.parent = null;
			}
			thisChar.transform.localScale += new Vector3(0f, 1*gamt, 0f);
			
			if (pickup) { // if picking up, handle object while growing also
			//	manipObj.transform.localScale += new Vector3(0f, -1*gamt, 0f);
				//manipObj.transform.localScale = new Vector3(1f,1f,1f);
				//Debug.Log ("Size="+thisChar.renderer.bounds.size.y);
				manipObj.transform.position = new Vector3(manipObj.transform.position.x, (.5f + thisChar.renderer.bounds.size.y) - 3f, manipObj.transform.position.z);
				manipObj.transform.parent = thisChar.transform;
			}
			if (thisChar.transform.localScale.y >= curHeight) {
				Debug.Log ("Done growing "+thisChar.name);
				// attach if picking up
				//if (pickup) {
				//	manipObj.transform.parent = thisChar.transform;
				//}
				growing = false; // done growing
				manipObj = null;
				GlobalObjs.removeOne(workingNum);
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				
				}
				//workingNum = -1;
				//Debug.Log ("Reset working num in update if growing to "+workingNum+ " for "+thisChar.name);
			}
		} else if (rotating) {
			// re-update direction in case target moved
			if (rotateToObj == null) {
				// no need to change
			} else {
				rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
				rotateDir = getDirection (rotateTo);
			}
			float howmuch = rotateDir*Time.deltaTime*rspeed;
			float diff = getAngle(rotateTo);
			//Debug.Log ("Howmuch="+howmuch+", diff="+diff+" for "+thisChar.name+" rotating");
			if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) {
				howmuch = rotateDir*diff;
				//Debug.Log ("Smaller distance!"+diff+" for "+thisChar.name+"rotating");
				shortenrotate = true;
			}
			/*if (workingNum == 190) {
				Debug.Log ("190 rotateobj="+rotateToObj.name+" for "+thisChar.name);//+" parent="+rotateToObj.transform.parent.name);
				Debug.Log ("Check value="+(rotateToObj != null && rotateToObj != thisChar && rotateToObj.transform.parent != thisChar));
				Debug.Log ("Check value name="+(rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)));//(rotateToObj != null && rotateToObj.name != thisChar.name && rotateToObj.transform.parent.name != thisChar.name));
				Debug.Log ("Check parent="+(rotateToObj.transform.parent != thisChar));
				//Debug.Log ("Check parent name="+(rotateToObj.transform.parent.name != thisChar.name));
				Debug.Log ("Shortenrotate="+shortenrotate);
			}*/
			if ((rotateToObj != null && rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)) || rotateToObj == null) { // if ask to look at self, don't do anything OR child object
				//Debug.Log ("Distance="+getDist (rotateTo));
				if (getDist(rotateTo) < 1.0f) {
					shortenrotate = true;
				} else {
					thisChar.transform.Rotate (Vector3.up * howmuch);	
				}
			} else {
				shortenrotate = true;
			}
			//Debug.Log ("Rotated " + howmuch + " for " + thisChar.name);
			if (Mathf.RoundToInt(getAngle(rotateTo)*10) == 0 || shortenrotate) { 
				//Debug.Log ("In finish rotating on update and rotate="+rotating+" moving="+moving+" for "+thisChar.name);
				// remove from global queue!
				shortenrotate = false;
				rotating = false;
				lastrotateTo = rotateTo;
				lastrotateToObj = rotateToObj;
				rotateTo = nullVector;
				//rotateToObj = null;
				rotateDir = 1;
				GlobalObjs.removeOne(workingNum);
				Debug.Log ("Done Rotating for " + thisChar.name);
				// check to see if need to do a move now!!
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				
				}
			}
		} else if (moving) {
			prevpostn.x = thisChar.transform.position.x;
			prevpostn.z = thisChar.transform.position.z;
			insertcurpostn(thisChar.transform.position);
			if (thisChar.transform.position.y != 0) {
				//Debug.Log (thisChar.name+" -- NOT ZERO!! at beginning of moving");
			}
			
			//Debug.Log (thisChar.name + " - coords:"+thisChar.transform.position.x+","+thisChar.transform.position.z+"--last:"+prevpostn.x+","+prevpostn.z);
			if (moveToObj == null) {
				// no need to change
			} else {
				moveTo = calculateObjPostn(moveToObj);
			}
			// turn towards target before moving if not facing target
			//Debug.Log ("Angle="+getAngle(moveTo));
			if (moveToObj != null && (moveToObj == thisChar || moveToObj.transform.parent == thisChar)) {
				// don't do the move - break out
				shortenmove = true;
			} else {
				
				if (Mathf.RoundToInt(getAngle (moveTo)*10) != 0) { 
					//Debug.Log ("Need to turn while walking");
					float howmuch = getDirection (moveTo)*Time.deltaTime*rspeed;
					float diff = getAngle(moveTo);
					//Debug.Log ("Howmuch="+howmuch+", diff="+diff+" for "+thisChar.name+" rotating");
					if (Mathf.Abs (diff) < Mathf.Abs (howmuch)) { // so don't turn too much
						howmuch = getDirection(moveTo)*diff;
					}
					//Debug.Log ("Turning "+howmuch);
					thisChar.transform.Rotate (Vector3.up * howmuch);	
				}
				if (isVisible()) { // only start walking if can see target
					Vector3 dir = thisChar.transform.position - moveTo;
					dir = dir.normalized;
					dir = -1f*dir.normalized; // to make them move forward
					dir *= mspeed;
					// need to check if distance is more than how far I am from my target
					Vector3 mymove = dir*Time.deltaTime;
					mymove.y = 0;
					thisCharController.Move (mymove);
					Debug.Log (thisChar.name + " - coords:"+thisChar.transform.position.x+","+thisChar.transform.position.z+"--last:"+prevpostn.x+","+prevpostn.z);
					
					if (thisChar.transform.position.y != 0) {
						//Debug.Log (thisChar.name+" -- NOT ZERO!! after normal move");
						thisChar.transform.position = new Vector3(thisChar.transform.position.x, 0f, thisChar.transform.position.z);
					}
					
					
					// check if "stuck" - if so, do a random move to left or right
					if (which == -1) {
						
						which = UnityEngine.Random.Range(0,2); // x position
					}
					float amtdist = Random.Range(1.0f, 5.0f);
					Vector3 howmuchr = thisChar.transform.right*amtdist*5f*Time.deltaTime;//Vector3.left*10*Time.deltaTime;
					howmuchr.y = 0f;
					Vector3 howmuch = -thisChar.transform.right*amtdist*5f*Time.deltaTime;//Vector3.left*10*Time.deltaTime;
					howmuch.y = 0f;
					
					
					if (howmuchr.x == 0f && howmuchr.z == 0f) {
						howmuchr.x += amtdist*5f*Time.deltaTime;
						
					}
					if (howmuch.x == 0f && howmuch.z == 0f) {
						howmuch.x += amtdist*5f*Time.deltaTime;
					}
			
					//if (thisChar.transform.position.x == prevpostn.x && thisChar.transform.position.z == prevpostn.z) {
					if (isInPrior(thisChar.transform.position)) {
						Debug.Log ("Current postn="+thisChar.transform.position);
						if (howmanyprior(thisChar.transform.position) > 2) {
							Debug.Log ("Switching direction to adjust");
							if (which == 0) {
								which = 1;
							} else {
								which = 0;
							}
						}
						if (which == 0) {
							thisCharController.Move(howmuch);// ((Vector3.left)*3*Time.deltaTime);
							Debug.Log ("ADJUSTING LEFT:"+howmuch);//((Vector3.left)*3*Time.deltaTime));
	
						} else {
							thisCharController.Move(howmuchr);// ((Vector3.left)*3*Time.deltaTime);
							Debug.Log ("ADJUSTING RIGHT:"+howmuchr);//((Vector3.left)*3*Time.deltaTime));
	
						}
						Debug.Log ("New postn="+thisChar.transform.position);
						//Debug.Log ("ADJUSTING LEFT OR RIGHT:"+howmuch);//((Vector3.left)*3*Time.deltaTime));
					}
				}
			}
			//Debug.Log ("Distance="+getDist (moveTo));
			//Debug.Log ("Cur Postn="+thisChar.transform.position);
			//Debug.Log ("Move To Postn="+moveTo);
			//Debug.Log ("Dir="+dir.normalized);
			// only stop moving if not following someone and close to target OR if following and target is still moving
			//Debug.Log (thisChar.name+" following="+following+", getDist="+getDist (moveTo)+", beingfollowed="+(moveToObjFunc != null?moveToObjFunc.beingfollowed.ToString():"doesn't exist"));
			/*if ((prevpostn.x == thisChar.transform.position.x && prevpostn.z == thisChar.transform.position.z)||(prevpostn2.x == thisChar.transform.position.x && prevpostn2.z == thisChar.transform.position.z)) {
				// move character one position away from audience
				thisCharController.Move ((Vector3.left)*Time.deltaTime);
				//thisChar.transform.position = new Vector3(thisChar.transform.position.x, 0.0000000000f, thisChar.transform.position.z);
//				thisChar.transform.position.z = thisChar.transform.position.z -1;
			}
			prevpostn2.x = prevpostn.x;
			prevpostn2.z = prevpostn.z;
			*/
			
			bool conflictingperson = checkothers(moveTo);
			if ((conflictingperson && getDist(moveTo) < 3f) ||shortenmove || (getDist (moveTo) < 1f && !following) || (following && getDist (moveTo) < 2f && moveToObjFunc != null && moveToObjFunc.beingfollowed == false)) {
				moving = false;
				which = -1;
				Debug.Log ("Done Moving"+thisChar.transform.position);
				if (!forcedmove) {
					lastmoveToObj = moveToObj;
					forcedmove = false;
				}
				moveTo = nullVector;
				moveToObj = null;
				beingfollowed = false;
				GlobalObjs.removeOne(workingNum);
				if (genQueue.Count > 0) {
					getNextInMiniQueue();
				}
			}
		} else {
			// keep looking at our rotateToObj if not null -- may have to check how close I am first?
			if ((rotateToObj != null && rotateToObj.name != thisChar.name && ((rotateToObj.transform.parent != null && rotateToObj.transform.parent.name != thisChar.name) || rotateToObj.transform.parent == null)) ) {//rotateToObj != null && getDist (new Vector3(rotateToObj.transform.position.x, 0f, rotateToObj.transform.position.z)) >= 1.0f) {
				if (getDist (new Vector3(rotateToObj.transform.position.x, 0f, rotateToObj.transform.position.z)) >= 1.0f) {
					int myrotateDir = getDirection (new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z));
					float myhowmuch = getAngle(new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z));
					thisChar.transform.Rotate (Vector3.up*myhowmuch*myrotateDir);
				}
			}
		}
		if (thisChar.transform.position.y != 0) {
			//Debug.Log (thisChar.name+" -- NOT ZERO!! at end of update");
		}	
	
	}
	
	
	public bool checkothers(Vector3 moveToPt) {
		
		foreach(CharFuncs c in GlobalObjs.listOfChars) {
			if (((moveToObj!= null && moveToObj.name != c.thisChar.name)||(moveToObj == null)) && c.getDist(moveToPt) < 1f) {
				return true;
			}
		}
		return false;
		
	}
	
	// functions for the character
	
	public void doRotate(float towherex, float towherey, GameObject towhatobj) {
		// add to global queue!!!
		// add to global queue
		GlobalObjs.printQueue("Start Rotate "+thisChar.name);
		//Debug.Log ("Rotate="+rotating+" Move="+moving+" for "+thisChar.name);
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(towherex, 0, towherey)):(towhatobj.transform.position), QueueObj.actiontype.rotate);
		GlobalObjs.globalQueue.Add(temp);
		
		if (rotating || moving || shrinking || growing) {
			// wait & try again when done rotating	
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(towherex, towherey, towhatobj, temp.msgNum, false, miniQueueObj.actiontype.rotate);
		} else {
			workingNum = temp.msgNum;
			//Debug.Log ("Changed working num in doRotate to "+workingNum+" for "+thisChar.name);
			rotating = true;
			
			// set RotateDir as appropriate
			if (towhatobj == null) {
				rotateTo = new Vector3(towherex, 0, towherey); 
				rotateToObj = null;
				rotateDir = getDirection (rotateTo);
			} else {
				rotateTo = new Vector3(towhatobj.transform.position.x, 0, towhatobj.transform.position.z);
				rotateToObj = towhatobj;
				rotateDir = getDirection (rotateTo);
			}
			Debug.Log ("Starting rotation to " + towhatobj + " for " + this.name);
		}
		//Debug.Log ("END Rotate with Rotate="+rotating+" Move="+moving+" for "+thisChar.name+" msg="+temp.msgNum);
		GlobalObjs.printQueue("End Rotate "+thisChar.name);
		
	}
	
	public void doWalk(float x, float y, GameObject towhatobj, bool tofollow) {
		// add to global queue
		GlobalObjs.printQueue("Start Walk "+thisChar.name);
		QueueObj temp = new QueueObj(thisChar, towhatobj, (towhatobj == null)?(new Vector3(x, 0, y)):(towhatobj.transform.position), QueueObj.actiontype.move);
		GlobalObjs.globalQueue.Add(temp);
		Debug.Log ("*********************Added "+temp.msgNum+" for "+thisChar.name);
		// do something
		Debug.Log("In doWalk for "+thisChar.name);
		if (moving || rotating || shrinking || growing) {
			// wait & try again when done moving
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(x, y, towhatobj, temp.msgNum, tofollow, miniQueueObj.actiontype.move);
		} else {
			rotateToObj = null;
			workingNum = temp.msgNum;
			moving = true;
			following = tofollow;
			if (towhatobj == null) {
				moveToObj = null;
				moveTo = new Vector3(x, 0, y);
				moveToObjFunc = null;
			} else {
				moveToObj = towhatobj;
				// if moving to an object that is being held, then go to the char holding the object instead
				if (GlobalObjs.isPawn(towhatobj) && towhatobj.transform.parent != null) {
					Debug.Log ("Changed target from "+towhatobj.name+" to "+towhatobj.transform.parent.gameObject.name);
					moveToObj = towhatobj.transform.parent.gameObject;
					towhatobj = towhatobj.transform.parent.gameObject;
				} else {
					Debug.Log ("Didn't change target for walking");
				}
				moveTo = calculateObjPostn(towhatobj);
				if (tofollow) {
					moveToObjFunc = (CharFuncs)moveToObj.GetComponent(typeof(CharFuncs));
					moveToObjFunc.beingfollowed = true;
				} else {
					moveToObjFunc = null;
				}
			}
			Debug.Log ("Starting walk to " + towhatobj + " for " + this.name);
		}

		GlobalObjs.printQueue("End Walk "+thisChar.name);
	}
	
	public void doStopAll() {
		rotating = false;
		rotateDir = 1;
		rotateTo = nullVector;
		rotateToObj = null;
		if (genQueue != null) {
			while (genQueue.Count > 0) {
				genQueue.Dequeue();
			}
		}
		moving = false;
		moveTo = nullVector;
		moveToObj = null;
		following = false;
		timer = 0.0f;
		left = false;
		shrinking = false;
		growing = false;
		manipObj = null;
		pickup = true;
		pointing = false;
		pointtarget = null;
		pointertimer = 0.0f;
		pointnum = -1;
		workingNum = -1;
		speakNum = -1;
		speaking = false;
		saywhat = "";
		GlobalObjs.globalQueue.RemoveRange(0, GlobalObjs.globalQueue.Count);
		Debug.Log ("Stopped Everything in method for "+thisChar.name);
	}
	
	public void doSpeak(string toSay) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, null, nullVector, QueueObj.actiontype.speak);
		GlobalObjs.globalQueue.Add(temp);
		speakNum = temp.msgNum;
		//speechfunc.showbubble = true;
		saywhat = toSay;
		//Debug.Log ("Said:"+toSay);	
		// clean up all ' and " to be /' and /"
		toSay = toSay.Replace("'", "");//"\\'");
		toSay = toSay.Replace ("\"", " ");
		toSay = toSay.ToLower ();
		//Debug.Log ("Cleaned said:"+toSay);
		speaking=true;
		myProcess = System.Diagnostics.Process.Start ("say", "-v "+voice + " \"" + toSay+"\"");
		if (!onstage()) {
			saywhat = "(offstage) "+saywhat;
		}
		
		/*if (InitScript.mode == InitScript.playmodes.rules) {
			// get everyone else that is onstage to look at this person
			Debug.Log ("Adding everyone look at "+thisChar.name);
			foreach(CharFuncs a in GlobalObjs.listOfChars) {
//			for(CharFuncs a : GlobalObjs.listOfChars) {
				if (a.onstage()) {
					a.doRotate(thisChar.transform.position.x, thisChar.transform.position.z, thisChar);
				}
			}
		}*/
		
	}
	
	public void doForward(float amt) { // used only for testing in the UI
		rotateToObj = null;
		//moveToObj = GlobalObjs.Grave;
		moveToObj = GlobalObjs.listOfPawnObj[0];
		moveTo = calculateObjPostn(moveToObj);
		moving = true;

	}
	
	private int getDirection(Vector3 target) {
		int dir = 1;
		
		float crossprod = Vector3.Cross(thisChar.transform.forward, new Vector3(thisChar.transform.position.x - target.x, 0, thisChar.transform.position.z - target.z)).y;
		//Debug.Log ("CrossProd="+crossprod);
		if (crossprod > 0) {
			// turn clockwise
			dir = 1;
		} else {
			// turn counter clockwise
			dir = -1;
		}
		
		return dir;
	}
	
	private float getAngle(Vector3 target) {
		Vector3 targetvector = new Vector3(thisChar.transform.position.x - target.x, 0, thisChar.transform.position.z - target.z);
		float result = Vector3.Angle(thisChar.transform.forward, targetvector);
		return result;
	}
	
	public float getDist(Vector3 target) {
		float result = 	thisChar.transform.position.x - target.x;
		result = result*result;
		result = result + ((thisChar.transform.position.z - target.z)*(thisChar.transform.position.z - target.z));
		result = Mathf.Sqrt (result);
		//Debug.Log ("Dist="+result);
		return result;
	}
	
	public void doPickup(GameObject obj) {
		Debug.Log ("picking up "+obj.name);
		// need to check against all pawns
		
		if (!GlobalObjs.isPawn(obj)) {
//		if (obj.name.ToLower() != "skull1" && obj.name.ToLower() != "skull2" && obj.name.ToLower() != "lantern" && obj.name.ToLower () != "shovel") {
			// not valid command - ignore
			Debug.Log ("bad object");
		} else {
			// add to global queue
			QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.pickup);
			GlobalObjs.globalQueue.Add(temp);
			if (moving || rotating || shrinking || growing) {
				// wait & try again when done moving
				Debug.Log ("Already doing something for "+thisChar.name);
				putInMiniQueue(-1, -1, obj, temp.msgNum, false, miniQueueObj.actiontype.pickup);
			} else {
				rotateToObj = null;
				workingNum = temp.msgNum;
				Debug.Log ("Changed working num in doPickup to "+workingNum+ " for "+thisChar.name);
				shrinking = true;
				manipObj = obj;
				pickup = true;
				curscalesize = obj.transform.localScale;
				switch (thisChar.transform.childCount) {
					case 0:
						break;
					case 1:
						if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (0).gameObject;
						}
						break;
					case 2:
						if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (0).gameObject;
						} else if (thisChar.transform.GetChild (1).gameObject.name != "ArmPrefab") {
							extraObj = thisChar.transform.GetChild (1).gameObject;
						}
						break;
					default:
						Debug.Log ("ERROR - too many children");
						break;
					
				}
			}
		}
	}
	
	public void doPutDown(GameObject obj) {
		// add to global queue
		QueueObj temp = new QueueObj(thisChar, obj, obj.transform.position, QueueObj.actiontype.putdown);
		GlobalObjs.globalQueue.Add(temp);
		if (moving || rotating || shrinking || growing) {
			// wait & try again when done moving
			Debug.Log ("Already doing something for "+thisChar.name);
			putInMiniQueue(-1, -1, obj, temp.msgNum, false, miniQueueObj.actiontype.putdown);
		} else {
			rotateToObj = null;
			if (thisChar.transform.GetChildCount () == 0) {
				// do nothing since not holding anything
				GlobalObjs.removeOne(temp.msgNum);
			} else {

				// figure out what object we are carrying
				if (thisChar.transform.childCount == 1) {
					if (thisChar.transform.GetChild(0).gameObject.name == "ArmPrefab") {
						// no children
						Debug.Log ("Only child is an arm!");
						GlobalObjs.removeOne(temp.msgNum);
					} else {
						if (thisChar.transform.GetChild (0).gameObject.name == obj.name) {
							workingNum = temp.msgNum;
							//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
							shrinking = true;
							manipObj = thisChar.transform.GetChild (0).gameObject;
						} else {
							// don't have the object right now
							GlobalObjs.removeOne(temp.msgNum);
						}
					}
				} else {
					// assume if there is two, one is definitely the object to putdown
					workingNum = temp.msgNum;
					//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
					shrinking = true;
					if (thisChar.transform.GetChild (0).gameObject.name == "ArmPrefab") {
						if (thisChar.transform.GetChild (1).gameObject.name == obj.name) {
							manipObj = thisChar.transform.GetChild (1).gameObject;
						} else {
							shrinking = false;
							GlobalObjs.removeOne(temp.msgNum); // don't have the right object
						}
					} else {
						if (thisChar.transform.GetChild (0).gameObject.name == obj.name) {
							manipObj = thisChar.transform.GetChild (0).gameObject;
						} else {
							shrinking = false;
							GlobalObjs.removeOne(temp.msgNum); // don't have right object
						}
					}
				}
				
				//manipObj = thisChar.transform.GetChild(0).gameObject;
				pickup = false;
				if (manipObj != null) {
					Vector3 curpostn = manipObj.transform.position;
					Quaternion currot = manipObj.transform.rotation;
					//manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;

					manipObj.transform.rotation = currot;//thisChar.transform.rotation;
					curscalesize = manipObj.transform.localScale;
				}
				
			}
		}
	}
	
	Vector3 calculateObjPostn(GameObject o) {
		Vector3 heading = o.transform.position - thisChar.transform.position;
		heading.y = 0;
		float distance = heading.magnitude;
		Vector3 direction = heading/distance;
		float minusamt = 0f; // if not pawn or char object
		if (GlobalObjs.isPawn(o)) {
				minusamt = 1.4f;
		}
		if (GlobalObjs.isChar (o)) {
				minusamt = 2.8f;
		}
		
		/*switch (o.name) {
		case "Lantern":
		case "Shovel":
		case "Skull1":
		case "Skull2":
			minusamt = 1.4f;
			break;
		case "Hamlet":
		case "Horatio":
		case "GraveDigger":
		case "GraveDiggerTwo":
			minusamt = 2.8f;
			break;
		default:
			minusamt = 0f;
			break;
		}*/
		//if (thisChar.name == "GraveDiggerTwo" && o.name == "GraveDigger") {
		//	Debug.Log ("Position G2="+thisChar.transform.position+" G1="+o.transform.position+" distance="+distance+" direction="+direction+" minusamt="+ minusamt+" postn="+(thisChar.transform.position + (direction *(distance - minusamt))));
		//}
		return thisChar.transform.position + (direction * (distance - minusamt));
	}
	
	void OnGUI() {
		// show text when speaking
		if(speaking) {
			GUI.Label (new Rect(150, 630, 1000, 30), new GUIContent(saywhat, myicon), mystyle);	
			//Debug.Log (saywhat);
			Vector3 ptheight = Camera.main.WorldToScreenPoint(thisChar.transform.position + new Vector3(0f, 40f, 0f));
			Vector3 ptheightabove = Camera.main.WorldToScreenPoint(thisChar.transform.position + new Vector3(0f, 41f, 0f));
			Vector3 pt = Camera.main.WorldToScreenPoint(thisChar.transform.position);
			//Debug.Log ("ph="+ptheight+", pha="+ptheightabove+", p="+pt);
			float heightdiff = ptheight.y - pt.y;
			float width = (2f/40f) * heightdiff;
			float bubbleheight = 1.15f * width;
			float bubblewidth = width;
			float startbubbley = pt.y +(bubbleheight/2);//ptheight.y;//Screen.height - ptheightabove.y -25f;
			float startbubblex = ptheight.x - (.4f*width);
			GUI.DrawTexture(new Rect(startbubblex, startbubbley, bubblewidth, bubbleheight), bubbleTexture, ScaleMode.ScaleToFit, true, 0f);

		}
	}
	
	public void doPoint(GameObject target) {
		QueueObj temp = new QueueObj(thisChar, target, target.transform.position, QueueObj.actiontype.point);
		GlobalObjs.globalQueue.Add(temp);
		pointnum = temp.msgNum;
		pointing = true;
		pointertimer = 0.0f;
		pointtarget = target;
		Vector3 relativePoint = thisChar.transform.InverseTransformPoint(pointtarget.transform.position);
		if (relativePoint.x < 0.0f) {
			left = true;
		} else {
			left = false;
		}
		Vector3 correctedstart;
		if (!left) {
			correctedstart = thisChar.transform.position + thisChar.transform.right.normalized;
		} else {
			correctedstart = thisChar.transform.position - thisChar.transform.right.normalized;
		}
		Vector3 newstart = new Vector3(correctedstart.x, 3.5f, correctedstart.z);
		Vector3 offset = target.transform.position - newstart;
		//Vector3 scale = new Vector3(0.5f, 2.0f, 0.5f);
		Vector3 position = newstart + (offset / 2.0f);
		GameObject myarm = Instantiate (prefabarm, position, Quaternion.identity) as GameObject;
		arm = myarm;
		myarm.transform.localScale = new Vector3(.5f, 1f, .5f);
		myarm.transform.position = newstart+ offset.normalized;
		myarm.transform.up = offset;
		Debug.Log ("Material name="+armmat.name);
		myarm.renderer.material = armmat;
		// need to attach to character so can move with character
		myarm.transform.parent = thisChar.transform;
		//myarm.transform.localScale = scale;
	}
	
	public void getNextInMiniQueue() {
		miniQueueObj pulled = (miniQueueObj)genQueue.Dequeue();
		workingNum = pulled.msgnum;
		Debug.Log ("*********************Dequeued "+workingNum+" for "+thisChar.name);
		switch (pulled.action) {
			case miniQueueObj.actiontype.move:
				moving = true;
				rotateToObj = null;
				following = pulled.following;
				if (pulled.getTargetType() == "Vector3") {
					moveTo = pulled.targetpt; 
					moveToObj = null;
					moveToObjFunc = null;
				} else {
					moveToObj = pulled.target;
					if (GlobalObjs.isPawn(pulled.target) && pulled.target.transform.parent != null) {
						Debug.Log ("Changed target from "+pulled.target.name+" to "+pulled.target.transform.parent.gameObject.name);
						moveToObj = pulled.target.transform.parent.gameObject;
					} else {
						Debug.Log ("Didn't change target for walking");
					}
					moveTo = calculateObjPostn(moveToObj);
					if (following) {
						moveToObjFunc = (CharFuncs)moveToObj.GetComponent(typeof(CharFuncs));
						moveToObjFunc.beingfollowed = true;
					} else {
						moveToObjFunc = null;
					}
				}
				break;
			case miniQueueObj.actiontype.rotate:
				rotating = true;
				if (pulled.getTargetType() == "Vector3") {
					rotateTo = pulled.targetpt; 
					rotateToObj = null;
					rotateDir = getDirection (rotateTo);
				} else {
					rotateToObj = pulled.target;
					rotateTo = new Vector3(rotateToObj.transform.position.x, 0, rotateToObj.transform.position.z);
					rotateDir = getDirection(rotateTo);
				}		
				break;
			case miniQueueObj.actiontype.pickup:
				if (!GlobalObjs.isPawn(pulled.target)) {
//				if (pulled.target.name.ToLower() != "skull1" && pulled.target.name.ToLower() != "skull2" && pulled.target.name.ToLower() != "lantern" && pulled.target.name.ToLower () != "shovel") {
					// not valid command - ignore
					GlobalObjs.removeOne(pulled.msgnum);
				} else {
					shrinking = true;
					rotateToObj = null;
					manipObj = pulled.target;
					pickup = true;
					curscalesize = pulled.target.transform.localScale;
					switch (thisChar.transform.childCount) {
						case 0:
							break;
						case 1:
							if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (0).gameObject;
							}
							break;
						case 2:
							if (thisChar.transform.GetChild (0).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (0).gameObject;
							} else if (thisChar.transform.GetChild (1).gameObject.name != "ArmPrefab") {
								extraObj = thisChar.transform.GetChild (1).gameObject;
							}
							break;
						default:
							Debug.Log ("ERROR - too many children");
							break;
						
					}
				}
				break;
			case miniQueueObj.actiontype.putdown:
				rotateToObj = null;
				if (thisChar.transform.GetChildCount () == 0) {
					// do nothing since not holding anything
					GlobalObjs.removeOne(pulled.msgnum);
				} else {
	
					// figure out what object we are carrying
					if (thisChar.transform.childCount == 1) {
						if (thisChar.transform.GetChild(0).gameObject.name == "ArmPrefab") {
							// no children
							Debug.Log ("Only child is an arm!");
							GlobalObjs.removeOne(pulled.msgnum);
						} else {
							//workingNum = pulled.msgnum;
							//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
							if (thisChar.transform.GetChild (0).gameObject.name == pulled.target.name) {
								shrinking = true;
								manipObj = thisChar.transform.GetChild (0).gameObject;
							} else {
								GlobalObjs.removeOne(pulled.msgnum);
							}
						}
					} else {
						// assume if there is two, one is definitely the object to putdown
						//workingNum = pulled.msgnum;
						//Debug.Log ("Changed working num in doPutDown to "+workingNum+ " for " +thisChar.name);
						shrinking = true;
						if (thisChar.transform.GetChild (0).gameObject.name == "ArmPrefab") {
							if (thisChar.transform.GetChild (1).gameObject.name == pulled.target.name) {
								manipObj = thisChar.transform.GetChild (1).gameObject;
							} else {
								// invalid object
								shrinking = false;
								GlobalObjs.removeOne(pulled.msgnum);
							}
						} else {
							if (thisChar.transform.GetChild (0).gameObject.name == pulled.target.name) {
								manipObj = thisChar.transform.GetChild (0).gameObject;
							} else {
								shrinking = false;
								GlobalObjs.removeOne(pulled.msgnum);
							}
						}
					}
					
					//manipObj = thisChar.transform.GetChild(0).gameObject;
					pickup = false;
					if (manipObj != null) {
						Vector3 curpostn = manipObj.transform.position;
						Quaternion currot = manipObj.transform.rotation;
						//manipObj.transform.position = new Vector3(curpostn.x, carrydropheight, curpostn.z);//curpostn;

						manipObj.transform.rotation = currot;//thisChar.transform.rotation;
						curscalesize = manipObj.transform.localScale;
					}					
				}
				break;
			default:
				// do nothing
				break;
		}
		
	}
	
	public void putInMiniQueue(float x, float y, GameObject t, int n, bool f, miniQueueObj.actiontype a) {
		if (t == null) {
			genQueue.Enqueue (new miniQueueObj(new Vector3(x, 0, y), null, n, f, a));
		} else {
			genQueue.Enqueue (new miniQueueObj(new Vector3(t.transform.position.x, 0, t.transform.position.z),t, n, f, a));
		}
	}
	
	public bool isVisible() {
		// checks if the current move target is in field of view of the character or not
		// update moveTo if Obj is not null
		if (moveToObj != null) {
			moveTo = calculateObjPostn(moveToObj);
		}
		Vector3 totarget = moveTo - thisChar.transform.position; 
		totarget.y = 0;
		Vector3 myforward = -1f*thisChar.transform.forward; // forward is actually backwards
		myforward.y = 0;
		float myprod = Vector3.Dot(myforward.normalized, totarget.normalized);
		if (myprod > Mathf.Cos (Mathf.Deg2Rad*45)) {
			//Debug.Log ("Myprod="+myprod+", Cos60="+Mathf.Cos (Mathf.Deg2Rad*60));
			return true;
		}
		//Debug.Log ("Myprod="+myprod+", Cos60="+Mathf.Cos (Mathf.Deg2Rad*60));
		return false;
		
	}
	
	public bool onstage() {
		if (thisChar.transform.position.z > 0 && thisChar.transform.position.z < 110) {
			if (thisChar.transform.position.x > -45 && thisChar.transform.position.x < 45) {
				return true;
			}
		}
		return false;
	}
	
	public string compareImportance(CharFuncs other) {
		// Hamlet is most important, GraveDigger1, GraveDigger2, Horatio
		int mypriority = 0;
		int otherpriority = 0;
		
		foreach(GameObject g in GlobalObjs.listOfCharObj) {
			if (g.name == thisChar.name) {
				mypriority = GlobalObjs.listOfCharObj.IndexOf (g);
			}
			if (g.name == other.name) {
				otherpriority = GlobalObjs.listOfCharObj.IndexOf(g);
			}
		}
		if (mypriority < otherpriority) {
			return "More";
		} else if (mypriority > otherpriority) {
			return "Less";
		} else {
			Debug.Log ("Error - invalid priority");
			return "Other";
		}
/*		
		switch (thisChar.name) {
			case "Hamlet":
				return "More";
				break;
			case "Horatio":
				return "Less";
				break;
			case "GraveDigger1":
				switch (other.thisChar.name) {
					case "Hamlet":
						return "Less";
						break;
					default:
						return "More";
						break;
				}
				break;
			case "GraveDigger2":
				switch (other.thisChar.name) {
					case "Horatio":
						return "More";
						break;
					default:
						return "Less";
						break;
				}
				break;
			default:
				return "Other";
				break;
		}*/
	}
	
	public Vector3 getLastMovePostn() {
		// go through genQueue until get last movement target (if any)
		Vector3 result = thisChar.transform.position;
		if (moveToObj != null) {
			result = moveToObj.transform.position;
		} else if (moveTo != nullVector) {
			result = moveTo;
		}
		miniQueueObj r = null;
		foreach(miniQueueObj m in genQueue) {
			if (m.action == miniQueueObj.actiontype.move) {
				r = m;
				if (m.target != null) {
					result = m.target.transform.position;
				} else {
					result = m.targetpt;
				}
			}
		}
		
		// check if following someone & if so, adopt that person's target
		if (r != null && r.following) {
			// get r.target's last position
			result = GlobalObjs.getCharFunc(r.target.name).getLastMovePostn();
		}
		
		return result;
	}
	
	public bool hasLookTarget() {
		//Vector3 result = lastrotateTo;
		Debug.Log ("checking if has look target-"+thisChar.name);
		foreach(miniQueueObj m in genQueue) {
			if(m.action == miniQueueObj.actiontype.rotate) {
				Debug.Log ("Found look target-"+thisChar.name);
				return true;
			}
		}
		return false;
		
	}
	
	public GameObject getLastTarget() {
		GameObject target = moveToObj;
		miniQueueObj r = null;
		foreach(miniQueueObj m in genQueue) {
			if (m.action == miniQueueObj.actiontype.move) {
				target = m.target;
				r = m;
			}
		}
		if (r != null && r.following) {
			target = GlobalObjs.getCharFunc(r.target.name).getLastTarget ();
		}
		return target;
	}
	
	public void updateLastPostn(float a, float b) {
		bool foundmvmt = false;
		miniQueueObj lastmvmt = null;
		// find last movement for character & update position to what is in the graph
			foreach(miniQueueObj m in genQueue) {
				if (m.action == miniQueueObj.actiontype.move) {
					foundmvmt = true;
					lastmvmt = m;
				}
			}
		
		// if no movement for the character, then add one for the new position - be sure to set the new messagenum for it!!! and add to queue
		if (!foundmvmt) {
			
			// check if had a movement originally
			if (moveToObj != null && following) {
				Debug.Log("**********************FOLLOWING WALK-"+thisChar.name+" ("+a+", "+b+")");
				// do nothing??
			} else 
			if (moveTo != nullVector) {
				forcedmove = true;
				lastmoveToObj = moveToObj;
				Debug.Log("**********************UPDATING WALK-"+thisChar.name+" ("+a+", "+b+")");
				moveTo = new Vector3(a, 0, b);
				moveToObj = null;
				
			} else {
				forcedmove = true;
				Debug.Log("**********************ADDING WALK-"+thisChar.name+" ("+a+", "+b+")");
				doWalk(a, b, null, false);
				// Add a rotate to re-look at whatever they were looking at before the move since we added a movement for them
				if (!hasLookTarget()) {
					Debug.Log ("ADDING A LOOK-"+thisChar.name);
					if (lastrotateTo == null) { // if just starting, look at audience
						doRotate (thisChar.transform.position.x, 90, null);
					} else {
						doRotate (lastrotateTo.x , lastrotateTo.z, lastrotateToObj);
					}
				}
			}
		} else {
			Debug.Log("**********************FOUND MVMT-"+thisChar.name+" ("+a+", "+b+")");
			if (lastmvmt.target != null) {
				forcedmove = true;
				lastmoveToObj = lastmvmt.target;
				lastmvmt.target = null;
				lastmvmt.targetpt = new Vector3(a, 0, b);
			} else {
				lastmvmt.targetpt = new Vector3(a, 0, b);
			}
		}
		
	}
	
}

