using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class InitScript : MonoBehaviour {
	
	public static InitScript Instance;
	//string txtmuch = "45";
	string txtmuchx = "3";
	string txtmuchy = "5";
	string txtx = "2";
	string txty = "1";
	string txtsay = "Your gambols, your songs, your flashes of merriment, that were wont to set the table on a roar? Not one now to mock your own grinning? Quite chop-fallen.  Now get you to my lady's chamber, and tell her, let her paint an inch thick, to this favour she must come. Make her laugh at that.";
	string txtforward = "3";
	
	public GameObject prefabobj;
	public GameObject prefabchar;
	public GameObject prefabmark;
	//public Material prefabarm;
	
	// for mode dropdowns
	private Vector2 scrollViewVector = Vector2.zero;
	private Vector2 fileViewVector = Vector2.zero;
	private Vector2 scriptViewVector = Vector2.zero;
	private Vector2 actionViewVector = Vector2.zero;
	private Vector2 charViewVector = Vector2.zero;
	private Vector2 targetViewVector = Vector2.zero;
	private Rect dropDownRect = new Rect(775, 25, 125, 200);
	private Rect dropDownFileRect = new Rect(125, 65, 125, 200); // Choose character input file
	private Rect dropDownScriptRect = new Rect(450,25,125,200); // Choose script file
	private Rect dropDownActionRect = new Rect(450, 200, 125, 200); // Choose action 
	private Rect dropDownCharRect = new Rect(125, 200, 125, 200); // Choose actor or character 125, 390, 100, 30
	private Rect dropDownTargetRect = new Rect(775, 200, 125, 200); // Choose target pawn, mark, or char
    private static string[] mlist = {"Choose Mode:","Baseline", "Random", "NLP", "Rules", "FDG"};
	private static string[] filelist = {};
	private static string[] fullfilelist = {};
	private static string[] actionlist = {"Choose Action:", "Walk", "Look", "Pickup", "Putdown", "Speak", "Point", "WalkToPt", "Print", "Follow"};
	private static string[] charlisttext = {};
	private static string[] targetlisttext = {};
	private static GameObject[] charlist = {};
	private static GameObject[] targetlist = {};
	static string logFile;
    static int indexNumber;
	static int fileindexNumber;
	static int scriptindexNumber;
	static int actionindexNumber;
	static int charindexNumber;
	static int targetindexNumber;
    bool show = false;
	bool fileshow = false;
	bool scriptshow = false;
	bool actionshow = false;
	bool charshow = false;
	bool targetshow = false;
	public enum playmodes { baseline, random, nlp, rules, fdg };
	public static playmodes mode = playmodes.baseline;
	public static bool runshort = false;
	
	float timer = 0.0f;
	float timerMax = 1.0f; // reset to 5 when working
	bool starting = false;
	
	// for file reading
	static char quote = System.Convert.ToChar (34);
	//StreamWriter[] charFiles = null;
	public static bool started = false;
	static string path = @"";
	static string inputFileName = Application.dataPath + @"//Files//InputFile.txt";
	static string bmlFileName = Application.dataPath + @"//Files//BMLFile.txt";
	static string miniinputFileName = Application.dataPath + @"//Files//miniInputFile.txt";
	static string minibmlFileName = Application.dataPath + @"//Files//miniBMLFile.txt";
	static StreamReader inputFile = null;
	
	// variables for legend
	public  Texture hamletT;
	public  Texture horatioT;
	public  Texture gravediggerT;
	public  Texture gravediggertwoT;
	public  Texture lanternT;
	public  Texture shovelT;
	public  Texture skull1T;
	public  Texture skull2T;
	public  Texture legendBkgrd;
	
	public static Texture legendTexture;
	
	public float startx1 = 5f;
	public float startx2 = 110f;
	public float starty = 80f;
	public float widthtext = 90f;
	public float widthimg = 85f;
	public float heighttext = 30f;
	public float heightimg = 135f;
	public float startximg1 = 5f;
	public float startximg2 = 100f;
	public float startyimg = 100f;
	public float spacing;
	public float linex = .7f;
	public float liney = .5f;
	public Material mat;
	
	static bool intermission = false;
	static Color mycolor;
	static float itimer = 0.0f;
	static float itimerMax = 7.0f;
	static int inum = -1;
	static Texture2D mytexture;
	static Texture2D mytexture2;
	static Texture2D mytexture3;
	static GUIStyle newstyle;
	
	static bool wait = false;
	static float wtimer = 0.0f;
	static float wtimerMax = 1.0f;
	
	static bool alldone = false;
	static float atimer = 0.0f;
	static float atimerMax = 4.0f;
	
	static float pauseamt = 0.0f;
	static float pausemax = 5.0f;
	static float pauseforces = 30.0f;
	
	static bool movementfound = false;
	
	void Awake() {
		Instance = this;
		WWW www3 = new WWW("file://" + Application.dataPath + "/Textures/blanklegend.png");
				        //yield return www3;
				        legendTexture = www3.texture;
		/* #if UNITY_EDITOR
            Material m = AssetDatabase.LoadAssetAtPath("Assets/Materials/BLUEMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/PURPLEMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/REDMat.mat", typeof(Material)) as Material;
            m = AssetDatabase.LoadAssetAtPath("Assets/Materials/GREENMat.mat", typeof(Material)) as Material;
			#endif*/
		
	}
	// Use this for initialization
	void Start () {
		spacing = heightimg+heighttext+5f;
		mycolor = GUI.backgroundColor;
		mytexture = new Texture2D(Screen.width, Screen.height); // orange
		int y = 0;
        while (y < mytexture.height) {
            int x = 0;
            while (x < mytexture.width) {
                //Color color = ((x & y) ? Color.white : Color.gray);
                mytexture.SetPixel(x, y, new Color(255f/255f, 127f/255f, 0f/255f));//new Color(51f/255f, 178f/255f, 146f/255f)); // turquoise
                ++x;
            }
            ++y;
        }
        mytexture.Apply();
		mytexture2 = new Texture2D(Screen.width, Screen.height); // brown
		y = 0;
        while (y < mytexture2.height) {
            int x = 0;
            while (x < mytexture2.width) {
                //Color color = ((x & y) ? Color.white : Color.gray);
                mytexture2.SetPixel(x, y, new Color(89f/255f, 64f/255f, 39f/255f));//new Color(51f/255f, 178f/255f, 146f/255f)); // turquoise
                ++x;
            }
            ++y;
        }
        mytexture2.Apply();
		
		mytexture3 = new Texture2D(Screen.width, Screen.height); // yellow
		y = 0;
		while (y < mytexture3.height) {
			int x=0;
			while (x < mytexture3.width) {
				mytexture3.SetPixel(x, y, Color.yellow);
				++x;
			}
			++y;
		}
		mytexture3.Apply();
		
		newstyle = new GUIStyle();
		newstyle.normal.background = mytexture;
		newstyle.fontSize = 30;
		newstyle.normal.textColor = Color.black;
		
		
		// initialize list of files for loading characters and pawns
		fullfilelist = System.IO.Directory.GetFiles(Application.dataPath + @"//Files//");
		
		filelist = new string[fullfilelist.Length+1];
		filelist[0] = "Select File:";
		// remove extra filesystem info from list - remember that the fullfilelist will use fileindex-1
		for (var i=0;  i<fullfilelist.Length; i++) {
			filelist[i+1] = fullfilelist[i].Replace(Application.dataPath + @"//Files/", "");
			//Debug.Log (fullfilelist[i]);
			//Debug.Log (filelist[i+1]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (starting) {
			
			timer += Time.deltaTime;
			if (timer >= timerMax) {
				// ready to start
				RunPlay ();
				timer = 0.0f;
				starting = false;
			}
		}
		//if (started && GlobalObjs.globalQueue.Count == 0) {
		//	callNextStep();
		//}
		if (intermission) {
			itimer += Time.deltaTime;
			if (itimer > itimerMax) {
				intermission = false;
				wait = true;
				itimer = 0.0f;
				//Debug.Log ("Removing inum="+inum);
				//GlobalObjs.removeOne(inum);
				//inum = -1;
			}
		}
		if (wait) {
			wtimer +=Time.deltaTime;
			if (wtimer > wtimerMax) {
				Debug.Log ("Removing inum="+inum);
				GlobalObjs.removeOne(inum);
				inum = -1;	
				wtimer = 0.0f;
				wait = false;
			}
		}
		if (alldone) {
			atimer +=Time.deltaTime;
			if (atimer > atimerMax) {
				GlobalObjs.removeOne (inum);
				inum = -1;
				atimer = 0.0f;
				alldone = false;
				started = false;
			}
		}
	}
	
	void OnGUI() {
		//Debug.Log ("INITIALIZE:"+dropDownRect.x+","+dropDownRect.y);
		if (intermission) {
			// show blue screen for intermission with text
			//GUIStyle newstyle = new GUIStyle();
			//newstyle.normal.background = new Texture2D(Screen.width, Screen.height);
			//GUI.backgroundColor = Color.blue;
			//GUI.Box (new Rect(0,0,Screen.width, Screen.height), "", newstyle);
			if (indexNumber == 1 || indexNumber == 0) {
				GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), mytexture, ScaleMode.ScaleToFit, false, 0);
				GUI.Label (new Rect((Screen.width/2) - 130, (Screen.height/2) + 60, Screen.width, 50), "This screen is orange", newstyle);
			} else if (indexNumber == 2) {
				GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), mytexture3, ScaleMode.ScaleToFit, false, 0);
				GUI.Label (new Rect((Screen.width/2) - 130, (Screen.height/2) + 60, Screen.width, 50), "This screen is yellow", newstyle);
			} else {
				GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), mytexture2, ScaleMode.ScaleToFit, false, 0);
				GUI.Label (new Rect((Screen.width/2) - 120, (Screen.height/2) + 60, Screen.width, 50), "This screen is brown", newstyle);
			}
			GUI.Label (new Rect((Screen.width/2) - 85, (Screen.height/2) - 120, Screen.width, 50), "INTERMISSION", newstyle);
			
			
		} else {
			GUI.backgroundColor = mycolor;
			// legend
			GUI.BeginGroup(new Rect(1200, -3, 200, 900));
				GUI.Box (new Rect(0,-3, 200,900), legendBkgrd);
				GUI.Label (new Rect(10, 50, 200, 900 ), new GUIContent(legendTexture));
				GUIStyle mystyle = new GUIStyle();
				mystyle.fontSize = 30;
				mystyle.normal.textColor = Color.white;
				GUI.Label (new Rect(20, startx1+20, widthtext*2, heighttext*2), "LEGEND", mystyle);
			/*
				GUI.Label (new Rect(startx2, starty, widthtext, heighttext), "Hamlet:");
				GUI.Label(new Rect(startximg2, startyimg,widthimg,heightimg), new GUIContent(hamletT));
				
				GUI.Label (new Rect(startx2, starty+(spacing*1), widthtext, heighttext), "Horatio:");
				GUI.Label(new Rect(startximg2, startyimg+(spacing*1),widthimg,heightimg), new GUIContent(horatioT));
			
				GUI.Label (new Rect(startx1, starty, widthtext, heighttext), "GraveDigger 1:");
				GUI.Label(new Rect(startximg1, startyimg,widthimg,heightimg), new GUIContent(gravediggerT));
			
				GUI.Label (new Rect(startx1, starty+(spacing*1), widthtext, heighttext), "GraveDigger 2:");
				GUI.Label(new Rect(startximg1, startyimg+(spacing*1),widthimg,heightimg), new GUIContent(gravediggertwoT));
			
				GUI.Label (new Rect(startx1, starty+(spacing*2), widthtext*2, heighttext*2), "--------------------------------------------");
			
				GUI.Label (new Rect(startx1, starty+30+(spacing*2), widthtext, heighttext), "Shovel:");
				GUI.Label(new Rect(startximg1, startyimg+30+(spacing*2),widthimg,heightimg*2), new GUIContent(shovelT));
			
				GUI.Label (new Rect(startx1, starty+30+(spacing*2.5f), widthtext, heighttext), "Lantern:");
				GUI.Label(new Rect(startximg1, startyimg+30+(spacing*2.5f),widthimg,heightimg*2), new GUIContent(lanternT));
			
				GUI.Label (new Rect(startx2, starty+30+(spacing*2), widthtext, heighttext), "Skull 1:");
				GUI.Label(new Rect(startximg2, startyimg+30+(spacing*2),widthimg,heightimg), new GUIContent(skull1T));
			
				GUI.Label (new Rect(startx2, starty+30+(spacing*2.5f), widthtext, heighttext), "Skull 2:");
				GUI.Label(new Rect(startximg2, startyimg+30+(spacing*2.5f),widthimg,heightimg), new GUIContent(skull2T));
			*/
			
			GUI.EndGroup();
			//GUI.DrawTexture(new Rect(100,60, 50,50), hamletT, ScaleMode.ScaleToFit, true, 0);
			// end legend
			
			if (started) {
				// show nothing
			} else {
				
				// choose file to generate characters and pawns
				// pre-load file using this button
				if(GUI.Button(new Rect((dropDownFileRect.x - 100), dropDownFileRect.y, dropDownFileRect.width, 25), ""))
		        {
		            if(!fileshow)
		            {
		                fileshow = true;
		            }
		            else
		            {
		                fileshow = false;
		            }
		        }
		        if(fileshow)
		        {
		             
					fileViewVector = GUI.BeginScrollView(new Rect((dropDownFileRect.x - 100), (dropDownFileRect.y + 25), dropDownFileRect.width, dropDownFileRect.height),fileViewVector,new Rect(0, 0, dropDownFileRect.width, Mathf.Max(dropDownFileRect.height, (filelist.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownFileRect.width, Mathf.Max(dropDownFileRect.height, (filelist.Length*25))), "");           
		            for(int index = 0; index < filelist.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownFileRect.height, 25), ""))
		                {
		                    fileshow = false;
		                    fileindexNumber = index;
							Debug.Log("FileIndex="+fileindexNumber);
							
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownFileRect.height, 25), filelist[index]);               
		            }
		            GUI.EndScrollView();   
				}
		        else
		        {
					//Debug.Log ("in file show else");
		            GUI.Label(new Rect((dropDownFileRect.x - 95), dropDownFileRect.y, 300, 25), filelist[fileindexNumber]);
		        }
				
				if (GUI.Button (new Rect(25,25,150,30), "Load character file")) {
					LoadChars();
					
				}
				// end of choosing file to generate characters and pawns
				
				
				// choose file for script
				if(GUI.Button(new Rect((dropDownScriptRect.x - 100), dropDownScriptRect.y, dropDownScriptRect.width, 25), ""))
		        {
		            if(!scriptshow)
		            {
		                scriptshow = true;
		            }
		            else
		            {
		                scriptshow = false;
		            }
		        }
		        if(scriptshow)
		        {
		             
					scriptViewVector = GUI.BeginScrollView(new Rect((dropDownScriptRect.x - 100), (dropDownScriptRect.y + 25), dropDownScriptRect.width, dropDownScriptRect.height),scriptViewVector,new Rect(0, 0, dropDownScriptRect.width, Mathf.Max(dropDownScriptRect.height, (filelist.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownScriptRect.width, Mathf.Max(dropDownScriptRect.height, (filelist.Length*25))), "");           
		            for(int index = 0; index < filelist.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownScriptRect.height, 25), ""))
		                {
		                    scriptshow = false;
		                    scriptindexNumber = index;
							Debug.Log("FileIndex="+scriptindexNumber);
							
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownScriptRect.height, 25), filelist[index]);               
		            }
		            GUI.EndScrollView();   
				}
		        else
		        {
		            GUI.Label(new Rect((dropDownScriptRect.x - 95), dropDownScriptRect.y, 300, 25), filelist[scriptindexNumber]);
		        }
				
				// choose character to run
				if(GUI.Button(new Rect((dropDownCharRect.x - 100), dropDownCharRect.y, dropDownCharRect.width, 25), ""))
		        {
		            if(!charshow)
		            {
		                charshow = true;
		            }
		            else
		            {
		                charshow = false;
		            }
		        }
		        if(charshow)
		        {
		             
					charViewVector = GUI.BeginScrollView(new Rect((dropDownCharRect.x - 100), (dropDownCharRect.y + 25), dropDownCharRect.width, dropDownCharRect.height),charViewVector,new Rect(0, 0, dropDownCharRect.width, Mathf.Max(dropDownCharRect.height, (charlisttext.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownCharRect.width, Mathf.Max(dropDownCharRect.height, (charlisttext.Length*25))), "");           
		            for(int index = 0; index < charlisttext.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownCharRect.height, 25), ""))
		                {
		                    charshow = false;
		                    charindexNumber = index;
							Debug.Log("FileIndex="+charindexNumber);
							
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownCharRect.height, 25), charlisttext[index]);               
		            }
		            GUI.EndScrollView();   
				}
		        else
		        {

		            GUI.Label(new Rect((dropDownCharRect.x - 95), dropDownCharRect.y, 300, 25), (charlisttext.Length>0?charlisttext[charindexNumber]:"Choose Char:"));
		        }
				
				
				// choose action
				if(GUI.Button(new Rect((dropDownActionRect.x - 100), dropDownActionRect.y, dropDownActionRect.width, 25), ""))
		        {
		            if(!actionshow)
		            {
		                actionshow = true;
		            }
		            else
		            {
		                actionshow = false;
		            }
		        }
		        if(actionshow)
		        {
		             
					actionViewVector = GUI.BeginScrollView(new Rect((dropDownActionRect.x - 100), (dropDownActionRect.y + 25), dropDownActionRect.width, dropDownActionRect.height),actionViewVector,new Rect(0, 0, dropDownActionRect.width, Mathf.Max(dropDownActionRect.height, (actionlist.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownActionRect.width, Mathf.Max(dropDownActionRect.height, (actionlist.Length*25))), "");           
		            for(int index = 0; index < actionlist.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownActionRect.height, 25), ""))
		                {
		                    actionshow = false;
		                    actionindexNumber = index;
							Debug.Log("FileIndex="+actionindexNumber);
							
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownActionRect.height, 25), actionlist[index]);               
		            }
		            GUI.EndScrollView();   
				}
		        else
		        {
		            GUI.Label(new Rect((dropDownActionRect.x - 95), dropDownActionRect.y, 300, 25), actionlist[actionindexNumber]);
		        }
				
				
				// choose target
				if(GUI.Button(new Rect((dropDownTargetRect.x - 100), dropDownTargetRect.y, dropDownTargetRect.width, 25), ""))
		        {
		            if(!targetshow)
		            {
		                targetshow = true;
		            }
		            else
		            {
		                targetshow = false;
		            }
		        }
		        if(targetshow)
		        {
		             
					targetViewVector = GUI.BeginScrollView(new Rect((dropDownTargetRect.x - 100), (dropDownTargetRect.y + 25), dropDownTargetRect.width, dropDownTargetRect.height),targetViewVector,new Rect(0, 0, dropDownTargetRect.width, Mathf.Max(dropDownTargetRect.height, (targetlisttext.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownTargetRect.width, Mathf.Max(dropDownTargetRect.height, (targetlisttext.Length*25))), "");           
		            for(int index = 0; index < targetlisttext.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownTargetRect.height, 25), ""))
		                {
		                    targetshow = false;
		                    targetindexNumber = index;
							Debug.Log("FileIndex="+targetindexNumber);
							
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownTargetRect.height, 25), targetlisttext[index]);               
		            }
		            GUI.EndScrollView();   
				}
		        else
		        {
		            GUI.Label(new Rect((dropDownTargetRect.x - 95), dropDownTargetRect.y, 300, 25), (targetlisttext.Length>0?targetlisttext[targetindexNumber]:"Choose target"));
		        }
				
				// enter x
				// enter y
				GUI.Label (new Rect(925, 310, 250, 30), "Enter target coordinates:");
				txtx = GUI.TextField (new Rect(1100, 310, 40, 30), txtx, 4);
				txty = GUI.TextField (new Rect(1150, 310, 40, 30), txty, 4);
				
				// enter speech text
				GUI.Label (new Rect(925,350, 750, 30), "Enter speech text:");
				txtsay = GUI.TextArea (new Rect(925, 400, 300, 100), txtsay);
				
				// run action
				if (GUI.Button (new Rect(825, 200, 200, 30), "Click to do Action")) {
					RunAction();	
				}
/*				
				//txtmuch = GUI.TextField(new Rect(780, 30, 40, 30), txtmuch, 4);
				txtmuchx = GUI.TextField (new Rect(780, 30, 40, 30), txtmuchx, 4);
				txtmuchy = GUI.TextField (new Rect(830, 30, 40, 30), txtmuchy, 4);
				if (GUI.Button(new Rect(500,30,250,30),"Click to Rotate Hamlet")) {
					float howmuchx;
					float howmuchy;
					bool success = float.TryParse(txtmuchx, out howmuchx);
					success = float.TryParse (txtmuchy, out howmuchy);
					GlobalObjs.HamletFunc.doRotate(howmuchx, howmuchy, null);
					Debug.Log("Clicked the button to rotate");	
				}
				txtx = GUI.TextField (new Rect(780, 70, 40, 30), txtx, 4);
				txty = GUI.TextField (new Rect(830, 70, 40, 30), txty, 4);
				if (GUI.Button (new Rect(500, 70, 250, 30), "Click to Move Hamlet FWD")) {
					float x;
					float y;
					bool success = float.TryParse (txtx, out x);
					success = float.TryParse (txty, out y);
					GlobalObjs.HamletFunc.doWalk(x, y, null, false);
					Debug.Log ("Clicked the button to walk");
				}
				txtsay = GUI.TextField (new Rect(780, 110, 100, 30), txtsay, 100);
				if (GUI.Button (new Rect(500, 110, 250, 30), "Speak")) {
					GlobalObjs.HamletFunc.doSpeak(txtsay);
					Debug.Log ("Said something");
				}*/
				if (GUI.Button (new Rect(25, 545, 100, 30), "Stop")) {
					foreach(CharFuncs c in GlobalObjs.listOfChars) {
						c.doStopAll ();
					}
					//GlobalObjs.HamletFunc.doStopAll();
					Debug.Log ("Stopped everything");
				}
/*
				txtforward = GUI.TextField (new Rect(780, 190, 100, 30), txtforward, 4);
				if (GUI.Button (new Rect(500, 190, 250, 30), "Move Forward")) {
					float thisamt;
					bool success = float.TryParse (txtforward, out thisamt);
					GlobalObjs.HamletFunc.doForward(thisamt);
					Debug.Log ("Moved forward "+thisamt);
				}
				if (GUI.Button (new Rect(125, 150, 100, 30), "Pickup")) {
					Debug.Log ("Pickup");
					//shrinking = true;
					GlobalObjs.HamletFunc.doPickup(GlobalObjs.Lantern);//.animation.Play("Shrink");
				}
				if (GUI.Button (new Rect(125, 190, 100, 30), "Putdown")) {
					Debug.Log ("Putdown");
					GlobalObjs.HamletFunc.doPutDown(GlobalObjs.Lantern);
				}
				if (GUI.Button (new Rect(125, 230, 100, 30), "Follow")) {
					Debug.Log ("Following");
					GlobalObjs.GraveDiggerFunc.doWalk (GlobalObjs.Grave.transform.position.x, GlobalObjs.Grave.transform.position.z, GlobalObjs.Grave, false);
					GlobalObjs.GraveDiggerTwoFunc.doWalk (GlobalObjs.GraveDigger.transform.position.x, GlobalObjs.GraveDigger.transform.position.z, GlobalObjs.GraveDigger, true);
				}
				if (GUI.Button (new Rect(125, 270, 100, 30), "Point")) {
					Debug.Log ("Pointing");
					GlobalObjs.HamletFunc.doPoint (GlobalObjs.Skull1);
				}*/
				/*if (GUI.Button(new Rect(25, 595, 100, 30), "Check Visible")) {
					Debug.Log ("Checking if Grave is visible");
					GlobalObjs.HamletFunc.moveTo = GlobalObjs.Grave.transform.position;
					Debug.Log ("Grave="+GlobalObjs.Grave.transform.position+", Hamlet="+GlobalObjs.Hamlet.transform.position);
					Debug.Log (GlobalObjs.HamletFunc.isVisible());
				}*/
/*
				if (GUI.Button (new Rect(125, 350, 100, 30), "Look at")) {
					Debug.Log ("Look at Grave");
					GlobalObjs.GraveDiggerFunc.doRotate(GlobalObjs.Grave.transform.position.x, GlobalObjs.Grave.transform.position.z, GlobalObjs.Grave);
				}
				*/
				if (GUI.Button (new Rect(25, 635, 100, 30), "Intermission")) {
					Debug.Log ("Start Intermission");
					intermission = true;
					QueueObj temp = new QueueObj(null, null, new Vector3(0,0,0), QueueObj.actiontype.intermission);
					inum = temp.msgNum;
					GlobalObjs.globalQueue.Add(temp);
					Debug.Log ("Starting inum="+inum);
				}
				/*
				if (GUI.Button (new Rect(125, 470, 100, 30), "Run Short Version")) {
					runshort = !runshort;
					Debug.Log ("Run Short="+runshort);
				}
				if (GUI.Button (new Rect(125, 430, 100, 30), "Long Speech")) {
					Debug.Log ("Saying long message");
					GlobalObjs.HamletFunc.doSpeak("He hath borne me on his back a thousand times,and now how abhorred in my imagination it is--my gorge rises at it. Here hung those lips that I have kissed I know not how oft.Where be your gibes now? Your gambols, your songs, your flashes of merriment, that were wont to set the table on a roar? No tone now to mock your own grinning? Quite chop-fallen.");
					Debug.Log ("Done long message");
				}*/
				
				//bool useBML = GUI.Toggle(new Rect(500, 30, 100, 30), BML, "Use BML File?");
				if (GUI.Button (new Rect(825, 25, 100, 30), "Start Play")) {
					Debug.Log ("Starting Play "+Time.time);	
					//RunPlay();
					starting = true;
					timer = 0.0f;
				}
				
				
				// shows dropdown to choose what to run
				//Debug.Log ("button1:"+(dropDownRect.x-100)+","+dropDownRect.y);
				if(GUI.Button(new Rect((dropDownRect.x - 100), dropDownRect.y, dropDownRect.width, 25), ""))
		        {
		            if(!show)
		            {
		                show = true;
		            }
		            else
		            {
		                show = false;
		            }
		        }
		        if(show)
		        {
					//Debug.Log ("scrollviewvect:"+scrollViewVector);
					//Debug.Log ("scroll:"+(dropDownRect.x-100)+","+(dropDownRect.y+25));
		            scrollViewVector = GUI.BeginScrollView(new Rect((dropDownRect.x - 100), (dropDownRect.y + 25), dropDownRect.width, dropDownRect.height),scrollViewVector,new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (mlist.Length*25))));
		            GUI.Box(new Rect(0, 0, dropDownRect.width, Mathf.Max(dropDownRect.height, (mlist.Length*25))), "");           
		            for(int index = 0; index < mlist.Length; index++)
		            {               
		                if(GUI.Button(new Rect(0, (index*25), dropDownRect.height, 25), ""))
		                {
		                    show = false;
		                    indexNumber = index;
							Debug.Log("Index="+indexNumber);
							if (index == 0 || index == 1) {
								newstyle.normal.background = mytexture;
								newstyle.normal.textColor = Color.black;
							} else {
								newstyle.normal.background = mytexture2;
								newstyle.normal.textColor = Color.white;
							}
		                }
		                GUI.Label(new Rect(5, (index*25), dropDownRect.height, 25), mlist[index]);               
		            }
		            GUI.EndScrollView();   
		        }
		        else
		        {
					//Debug.Log ("selected:"+(dropDownRect.x-95)+","+dropDownRect.y);
		            GUI.Label(new Rect((dropDownRect.x - 95), dropDownRect.y, 300, 25), mlist[indexNumber]);
		        }
			}
		}
       
	}
	
	public static void LogPositions() {
		
		// uses logfilename & character name to append to a logfile initiated at runplay
		
		DateTime timeSpan = System.DateTime.Now;
 		string timeText = timeSpan.ToString();//string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		
		foreach(GameObject c in GlobalObjs.listOfCharObj) {
			using(System.IO.StreamWriter myFile = new System.IO.StreamWriter(Application.dataPath + @"//Logs//"+logFile+c.name+".log", true)) {
				
				myFile.WriteLine(timeText+"\t"+c.transform.position.x+"\t"+c.transform.position.z+"\t"+c.transform.eulerAngles.y);
					
			}
				
		}
	}

	
	void RunPlay() {
		// check Mode & run based on that setting
		// use indexNumber, where 1=Baseline, 2=BML, etc
		
		Debug.Log ("Run in mode #"+indexNumber);
		starting = false;
		started = true;
		inputFile = File.OpenText (fullfilelist[scriptindexNumber-1]);
		
		DateTime timeSpan = System.DateTime.Now;
 		string timeText = timeSpan.ToString("MM-dd-yy-HH-mm-ss-fff");
		logFile = timeText+"-"+filelist[scriptindexNumber]+"-";
		
		Debug.Log ("Filename="+fullfilelist[scriptindexNumber-1]);
		// make sure a file was loaded and a file was selected for the script
		
		// need to add logic to do different actions based on mode chosen!!
		switch (indexNumber) {
		case 0: // baseline -- by default if don't choose or if click choose mode

			mode = playmodes.baseline;
			newstyle.normal.background = mytexture;
			newstyle.normal.textColor = Color.black;
			logFile = logFile + "baseline-";
			/*if (runshort) {
				inputFile = File.OpenText (miniinputFileName);
				GlobalObjs.Hamlet.transform.position = new Vector3(4.2f, 0f, 37f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, -1f, 0f, 0f);
				GlobalObjs.Horatio.transform.position = new Vector3(.9f, 0f, 35.3f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, .9f, 0f, -.5f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(16f, 0f, 34f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, -1f, 0f, -.1f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(54f, 0f, 49.9f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, .7f, 0f, -.7f);
				
				GlobalObjs.GraveDiggerTwoFunc.doPickup(GlobalObjs.Lantern);
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
			} else {
				inputFile = File.OpenText(inputFileName);
			}*/
			break;
		case 2: // random
			mode = playmodes.random;
			newstyle.normal.background = mytexture3;
			newstyle.normal.textColor = Color.black;
			logFile = logFile + "random-";
			/*if (runshort) {
				// place chars randomly
				GlobalObjs.Hamlet.transform.position = new Vector3(-7.9f, 0f, .8f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, -1f, 0f, -1f);
				GlobalObjs.Horatio.transform.position = new Vector3(-33f, 0f, 44.8f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, .8f, 0f, .6f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(18.1f, 0f, 4.5f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, -.5f, 0f, .9f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(6f, 0f, -4f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, 1f, 0f, .1f);
				
				GlobalObjs.GraveDiggerFunc.doPickup(GlobalObjs.Skull2);
				GlobalObjs.HamletFunc.doPickup(GlobalObjs.Skull1);
				
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
				inputFile = File.OpenText (minibmlFileName);
			} else {
				inputFile = File.OpenText (bmlFileName);
			}*/
			break;
		case 1: //baseline
			mode = playmodes.baseline;
			newstyle.normal.background = mytexture;
			newstyle.normal.textColor = Color.black;
			logFile = logFile + "baseline-";
			/*if (runshort) {
				// place chars
				GlobalObjs.Hamlet.transform.position = new Vector3(4.2f, 0f, 37f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, -1f, 0f, 0f);
				GlobalObjs.Horatio.transform.position = new Vector3(.9f, 0f, 35.3f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, .9f, 0f, -.5f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(16f, 0f, 34f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, -1f, 0f, -.1f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(54f, 0f, 49.9f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, .7f, 0f, -.7f);
				
				GlobalObjs.GraveDiggerTwoFunc.doPickup(GlobalObjs.Lantern);
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
				inputFile = File.OpenText(miniinputFileName);
			} else {
				inputFile = File.OpenText (inputFileName);
			}*/
			break;
		case 3: // nlp
			mode = playmodes.nlp;
			newstyle.normal.background = mytexture2;
			newstyle.normal.textColor = Color.white;
			logFile = logFile + "nlp-";
			/*if (runshort) {
				// place chars
				GlobalObjs.Hamlet.transform.position = new Vector3(-6.8f, 0f, 42.4f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, .6f, 0f, -.8f);
				GlobalObjs.Horatio.transform.position = new Vector3(-5f, 0f, 39.1f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, 1f, 0f, .2f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(14.3f, 0f, 34.1f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, .8f, 0f, .6f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(49.1f, 0f, 50.2f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, -.6f, 0f, .8f);
				
				GlobalObjs.GraveDiggerTwoFunc.doPickup(GlobalObjs.Lantern);
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
				inputFile = File.OpenText(minibmlFileName);
			} else {
				inputFile = File.OpenText (bmlFileName);
			}*/
			break;
		case 4:
			mode = playmodes.rules;
			logFile = logFile + "rules-";
			/*if (runshort) {
				// place chars
				GlobalObjs.Hamlet.transform.position = new Vector3(-6.8f, 0f, 42.4f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, .6f, 0f, -.8f);
				GlobalObjs.Horatio.transform.position = new Vector3(-5f, 0f, 39.1f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, 1f, 0f, .2f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(14.3f, 0f, 34.1f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, .8f, 0f, .6f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(49.1f, 0f, 50.2f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, -.6f, 0f, .8f);
				
				GlobalObjs.GraveDiggerTwoFunc.doPickup(GlobalObjs.Lantern);
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
				inputFile = File.OpenText(minibmlFileName);
			} else {
				inputFile = File.OpenText (bmlFileName);
			}*/
			break;
		case 5:
			mode = playmodes.fdg;
			logFile = logFile + "fdg-";
			/*if (runshort) {
				// place chars
				GlobalObjs.Hamlet.transform.position = new Vector3(-6.8f, 0f, 42.4f);
				GlobalObjs.Hamlet.transform.rotation = new Quaternion(0f, .6f, 0f, -.8f);
				GlobalObjs.Horatio.transform.position = new Vector3(-5f, 0f, 39.1f);
				GlobalObjs.Horatio.transform.rotation = new Quaternion(0f, 1f, 0f, .2f);
				GlobalObjs.GraveDigger.transform.position = new Vector3(14.3f, 0f, 34.1f);
				GlobalObjs.GraveDigger.transform.rotation = new Quaternion(0f, .8f, 0f, .6f);
				GlobalObjs.GraveDiggerTwo.transform.position = new Vector3(49.1f, 0f, 50.2f);
				GlobalObjs.GraveDiggerTwo.transform.rotation = new Quaternion(0f, -.6f, 0f, .8f);
				
				GlobalObjs.GraveDiggerTwoFunc.doPickup(GlobalObjs.Lantern);
				GlobalObjs.Shovel.transform.position = GlobalObjs.Grave.transform.position + new Vector3(0f, .5f, 0f);
				//GlobalObjs.Shovel.transform.position.y = .5f;
				
				inputFile = File.OpenText(minibmlFileName);
			} else {
				inputFile = File.OpenText (bmlFileName);
			}*/
			
			// set everyone's prior rotateTo for their audience facing position
			foreach(CharFuncs c in GlobalObjs.listOfChars) {
				c.lastrotateTo = new Vector3(c.thisChar.transform.position.x, 0, 90);
				// set everyone's prior target?
				//c.moveTo = c.thisChar.transform.position;
			}
			
			
			Forces.createForceGraph ();
			
			Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Forces & movement found");
			movementfound = false;
			Forces.setupForces ();
			Forces.g.printall();
			Forces.recalculate ();
			// apply forces to forcequeue
		//}
		//if (mode == playmodes.fdg) { // does this have to fire if no movement was found??  I say no...???
			// apply all forcequeue items to character funcs
			Forces.unsetupForces();
			Forces.applyChanges();
			
			//pausesome ();
			
			break;
		}
		// pause
		//started = true;
		pausesome ();
//		callNextStep ();
	}
	
	public static void pausesome() {
		Debug.Log ("pausing");
		if (pauseamt >= pausemax) {
			Debug.Log ("pauseamt enough");
			callNextStep();
		} else {
			pauseamt += Time.deltaTime;
			//pausesome ();
		}
	}
	
	public static void callNextStep() {
		
		LogPositions();
		
		string curLine = null;// = inputFile.ReadLine ();
		string[] parsedLine = null;
		bool firstiteration = true;
		
		
		while (firstiteration || (curLine != null && parsedLine[0] != "N")) {
			firstiteration = false;
       		curLine = inputFile.ReadLine ();
			Debug.Log ("*****"+curLine);
	        if (curLine != null) {
	           
	//            currentMessageNum++;
	            parsedLine = curLine.Split ('\t');
	            Debug.Log ("CJT LINE="+curLine);
				//Debug.Log ("First item=" +parsedLine[0]);
	            switch (parsedLine [1]) {
	                case "MOVE":
	                    //Debug.Log ("CJT MESSAGE="+parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
	                    //vhmsg.SendVHMsg ("vrExpress", parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
		                //Debug.Log ("Doing movement for "+parsedLine[2]+" doing:"+parsedLine[4]);	
						if (mode == playmodes.random) {
							doRandomMvmt(parsedLine[2], parsedLine[4]);
						} else {
							parseMovement(parsedLine[2], parsedLine[4]);    
						}
						break;
	                case "SPEAK":
	                    //if (parsedLine [1] == actor) {
	                        // find the speech tags & display only that text, start listening for enter key or mouse click?   
	                    //    showtext = findSpeech (parsedLine [3]);
	                    //} else {
	                        // else send the message to be spoken by the character
	                    //Debug.Log ("CJT MESSAGE="+parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
						
						CharFuncs who = GlobalObjs.getCharFunc(parsedLine[2]);
						string saywhat = findSpeech(parsedLine[4]);
						Debug.Log (who.name+" says: "+saywhat);
						who.doSpeak (saywhat);
						if (mode == playmodes.random) {
							int temp = UnityEngine.Random.Range (0,2);
							if (temp == 1) {
								doRandomMvmt(parsedLine[2], parsedLine[4]);
							}
						}
	                    //vhmsg.SendVHMsg ("vrSpeak", parsedLine [1] + " " + parsedLine [2] + " CJT" + currentMessageNum + " " + parsedLine [3]);
	                    //}
	                    if (mode == playmodes.rules || mode == playmodes.fdg) {
	                    	// add rule to get everyone to look at the speaker
							Debug.Log ("Adding Look at Speaker "+who.thisChar.name);
	                    	foreach (CharFuncs c in GlobalObjs.listOfChars) {
	                    		if (c.onstage() && c != who) {
	                    			c.doRotate(who.thisChar.transform.position.x, who.thisChar.transform.position.z, who.gameObject);
	                    		}
	                    	}
	                    }
	                    break;
					case "BREAK":
						Debug.Log ("Start Intermission");
						intermission = true;
						QueueObj temp = new QueueObj(null, null, new Vector3(0,0,0), QueueObj.actiontype.intermission);
						inum = temp.msgNum;
						GlobalObjs.globalQueue.Add(temp);
						Debug.Log ("Starting inum="+inum);
						break;
					case "PRINT":
						Debug.Log (Time.time);
						Debug.Log ("Coordinates:");
						foreach(GameObject g in GlobalObjs.listOfCharObj) {
							Debug.Log (g.name+"="+g.transform.position+","+g.transform.rotation);
							if (g.transform.childCount != 0) {
								Debug.Log (g.name+" children=");
								for (int i=0; i< g.transform.childCount; i++) {
									Debug.Log (g.transform.GetChild(i).name);
								}
								Debug.Log ("End "+g.name+" children");
							}
						}
						/*Debug.Log ("Hamlet="+GlobalObjs.Hamlet.transform.position+","+GlobalObjs.Hamlet.transform.rotation);
						Debug.Log ("Horatio="+GlobalObjs.Horatio.transform.position+","+GlobalObjs.Horatio.transform.rotation);
						Debug.Log ("GraveDigger="+GlobalObjs.GraveDigger.transform.position+","+GlobalObjs.GraveDigger.transform.rotation);
						Debug.Log ("GraveDigger2="+GlobalObjs.GraveDiggerTwo.transform.position+","+GlobalObjs.GraveDiggerTwo.transform.rotation);
						if (GlobalObjs.Hamlet.transform.childCount != 0) {
							Debug.Log ("Hamlet children=");
							for (int i=0; i< GlobalObjs.Hamlet.transform.childCount; i++) {
								Debug.Log (GlobalObjs.Hamlet.transform.GetChild(i).name);
							}
							Debug.Log ("End Hamlet children");
						}
						if (GlobalObjs.Horatio.transform.childCount != 0) {
							Debug.Log ("Horatio children=");
							for (int i=0; i< GlobalObjs.Horatio.transform.childCount; i++) {
								Debug.Log (GlobalObjs.Horatio.transform.GetChild(i).name);
							}
							Debug.Log ("End Horatio children");
						}
						if (GlobalObjs.GraveDigger.transform.childCount != 0) {
							Debug.Log ("GraveDigger children=");
							for (int i=0; i< GlobalObjs.GraveDigger.transform.childCount; i++) {
								Debug.Log (GlobalObjs.GraveDigger.transform.GetChild(i).name);
							}
							Debug.Log ("End GraveDigger children");
						}
						if (GlobalObjs.GraveDiggerTwo.transform.childCount != 0) {
							Debug.Log ("GraveDiggerTwo children=");
							for (int i=0; i< GlobalObjs.GraveDiggerTwo.transform.childCount; i++) {
								Debug.Log (GlobalObjs.GraveDiggerTwo.transform.GetChild(i).name);
							}
							Debug.Log ("End GraveDiggerTwo children");
						}*/
						break;
	                default:
	                    // bad command, ignore
					Debug.Log ("Bad command");
	                    break;
	            }
	            //curLine = null;
	            //parsedLine = null;
	        } else {
	            // exit - nothing left to do
	            Debug.Log ("CJT MESSAGE=DONE!!");
	            inputFile.Close ();
				QueueObj temp = new QueueObj(null, null, new Vector3(0,0,0), QueueObj.actiontype.intermission);
						inum = temp.msgNum;
						GlobalObjs.globalQueue.Add(temp);
				alldone = true;
//	            started = false;
	            inputFile = null;
	            //currentMessageNum = 0;
	           // Application.Quit ();
	        }

		} //while (curLine != null && parsedLine[0] != "N");
		
		if (mode == playmodes.fdg && movementfound) {
			Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Forces & movement found");
			movementfound = false;
			Forces.setupForces ();
			Forces.g.printall();
			Forces.recalculate ();
			// apply forces to forcequeue
		//}
		//if (mode == playmodes.fdg) { // does this have to fire if no movement was found??  I say no...???
			// apply all forcequeue items to character funcs
			Forces.unsetupForces();
			Forces.applyChanges();
		}
		
	}
	
	
	static string findSpeech(string xml) {
        string myText = null;
        int startPos = 0;
        int endPos = 0;
        startPos = xml.IndexOf("application/ssml+xml"+quote+">");
        endPos = xml.IndexOf("</speech>");
        myText = xml.Substring(startPos+22,endPos-startPos-22);
        return myText;
    }
	
	static void doRandomMvmt(string name, string xmltxt) {
		// do a random movement for the current character
		CharFuncs who = GlobalObjs.getCharFunc(name);
		Debug.Log (who.thisChar.name);
		
		float targetx = UnityEngine.Random.Range(-50,51); // x position
			
		float targety = UnityEngine.Random.Range(-5, 70); // y position
		
		int whichfunction = UnityEngine.Random.Range(0, 5); // which character function to run
		
		int objnum;
		if (whichfunction == 2 || whichfunction == 3) {
			objnum = UnityEngine.Random.Range(0, GlobalObjs.listOfPawnObj.Count); // can only pick up or put down one of these objects
		} else {
			objnum = UnityEngine.Random.Range(0, GlobalObjs.listOfAllObj.Count); // which person or object or location to look, rotate or point to
		}
		float temp = Mathf.Floor (UnityEngine.Random.Range(0,2)); 
		bool isobject = (temp == 1)?(true):(false); // whether to use the object or the position for target
		
		float temp2 = Mathf.Floor (UnityEngine.Random.Range(0,2)); // whether char is following or not -- only needed if isobject = true & object is a char
		bool following = (temp2 == 1)?(true):(false);
		bool ischar = false;
		GameObject whichobj = null;
		Debug.Log ("Objnum="+objnum+", whichfunc="+whichfunction+", isobj="+isobject);
		
		if (isobject || whichfunction >=2) {
			if (whichfunction ==2 || whichfunction ==3) {
				whichobj = GlobalObjs.listOfPawnObj[objnum];
				ischar = false;
				following = false;
			} else {
				whichobj = GlobalObjs.listOfAllObj[objnum];
				foreach (GameObject g in GlobalObjs.listOfCharObj) {
					if (g.name == whichobj.name) {
						ischar = true;
						break;
					}
				}
			}
			/*switch (objnum) {
				case 0:
					ischar = true;
					whichobj = GlobalObjs.Hamlet;
					break;
				case 1:
					ischar = true;
					whichobj = GlobalObjs.Horatio;
					break;
				case 2:
						ischar = true;
					whichobj = GlobalObjs.GraveDigger;
					break;
				case 3:
					ischar = true;
					whichobj = GlobalObjs.GraveDiggerTwo;
					break;
				case 4:
					ischar = false;
					whichobj = GlobalObjs.Skull1;
					following = false;
					break;
				case 5:
					ischar = false;
					whichobj = GlobalObjs.Skull2;
					following = false;
					break;
				case 6:
					ischar = false;
					whichobj = GlobalObjs.Lantern;
				following = false;
					break;
				case 7:
					ischar = false;
					whichobj = GlobalObjs.Shovel;
				following = false;
					break;
				case 8:
					ischar = false;
					whichobj = GlobalObjs.Center;
				following = false;
					break;
				case 9:
					ischar = false;
					whichobj = GlobalObjs.CenterRight;
				following = false;
					break;
				case 10:
					ischar = false;
					whichobj= GlobalObjs.CenterBackStage;
				following = false;
					break;
				case 11:
					ischar = false;
					whichobj = GlobalObjs.DownStage;
				following = false;
					break;
				case 12:
					ischar = false;
					whichobj = GlobalObjs.Grave;
				following = false;
					break;
				
				case 13:
					ischar = false;
					whichobj = GlobalObjs.StageLeft;
				following = false;
					break;
				case 14:
					ischar = false;
					whichobj = GlobalObjs.StageRight;
				following = false;
					break;
				case 15:
					ischar = false;
					whichobj = GlobalObjs.Steps;
				following = false;
					break;
				case 16:
					ischar = false;
					whichobj = GlobalObjs.Stool;
				following = false;
					break;
				case 17:
					ischar = false;
					whichobj = GlobalObjs.UpStage;
				following = false;
					break;
				default:
					ischar = false;
					whichobj = null;
				following = false;
					break;				
			}*/
		}
		Debug.Log ("Whichobj="+((whichobj == null)?("NULL"):(whichobj.name)));
				
		switch (whichfunction) { // check which function to call
			case 0:
				// walk
				if (isobject) {
					who.doWalk(whichobj.transform.position.x, whichobj.transform.position.z, whichobj, following);
				} else {
					who.doWalk(targetx, targety, null, following);
				}
				break;
			case 1:
						// rotate
				if (isobject) {
					who.doRotate(whichobj.transform.position.x, whichobj.transform.position.z, whichobj);
				} else {
					who.doRotate(targetx, targety, null);
				}		
				break;
			case 2:
				who.doPickup(whichobj);
						// pickup
				break;
			case 3:
						// putdown
				who.doPutDown(whichobj);
				break;
			case 4:
					// point
				who.doPoint(whichobj);
				break;
			
		}
	}
	
	static void parseMovement(string name, string xmltxt) {
		CharFuncs who = GlobalObjs.getCharFunc(name);
		Debug.Log (who.thisChar.name);
		//string action;
		float targetx = -1;
		float targety = -1;
		float targetx2 = -1;
		float targety2 = -1;
		float targetx3 = -1;
		float targety3 = -1;
		float targetx4 = -1;
		float targety4 = -1;
		GameObject target = null;
		bool following = false;
		
		string myText = null;
		int startPos = 0;
		int endPos = 0;
		string targetstr;
		bool hasrel = false;
		string relstr = null;
		
		if (xmltxt.Contains ("follow=")) {
			startPos = xmltxt.IndexOf ("follow="+quote);
			following = true;
		} else {
			startPos = xmltxt.IndexOf ("target="+quote);
		}
		Debug.Log (xmltxt);
		Debug.Log ("startPos="+startPos);
		//Debug.Log ("start="+startPos);
		//Debug.Log (xmltxt.Substring (startPos));
		endPos = xmltxt.Substring (startPos+8).IndexOf (quote);
		//Debug.Log (xmltxt.Substring(startPos)[7]);
		//Debug.Log (xmltxt.Substring(startPos)[7] == quote);
		//Debug.Log ("end="+endPos);
		targetstr = xmltxt.Substring(startPos+8, endPos);
		
		
		Debug.Log("Parsed target="+targetstr);
		if (targetstr.IndexOf ("/") >= 0) { // relative position to calculate
			string[] reltarget = targetstr.Split (' ');
			// reltarget[0] = relative position
			// reltarget[1] = true target to lookup
			if (reltarget.Length > 1) {
				target = GlobalObjs.getObject (reltarget[1]);
			} else {
				target = null;
			}
			relstr = reltarget[0];
			hasrel = true;
			
		} else {
			if (targetstr.IndexOf(" ") > 0) {
				// this is a vector position, not an object
				string[] position = targetstr.Split (' ');
				bool success = float.TryParse(position[0], out targetx);
				success = float.TryParse (position[1], out targety);
				if (position.Length > 2) {
					success = float.TryParse(position[2], out targetx2);
					success = float.TryParse(position[3], out targety2);
					if (position.Length > 4) {
						success = float.TryParse (position[4], out targetx3);
						success = float.TryParse (position[5], out targety3);
						if (position.Length > 6) {
							success = float.TryParse(position[6], out targetx4);
							success = float.TryParse(position[7], out targety4);
						}
					}
				}
				
			} else {
				// this is an object
				target = GlobalObjs.getObject(targetstr);
				
			}
		}
		
		// find out what action to take
		if (xmltxt.Contains ("pick-up") || xmltxt.Contains ("PICK-UP")) {
			Debug.Log ("Action=pickup");
			if (mode == playmodes.rules || mode == playmodes.fdg) {
            	// check if close enough to pick up the object, else add a locomotion before the pickup
            	if (who.getDist(target.transform.position) > 1) {
					Debug.Log ("Adding move to object for "+who.thisChar.name+ " to "+target.name);
            		who.doWalk(target.transform.position.x, target.transform.position.z, target, false);
            	}
            }
			who.doPickup(target);	
			if (hasrel) {
				// add additional logic for what the relative thing is
				if (relstr == "/FORWARD") { // change to switch & add more!
					// calculate moving towards audience 20 paces
					Vector3 dir = Quaternion.Euler (Vector3.forward) * new Vector3(0,0,20) + who.transform.position; 
					Debug.Log ("Current rotation="+who.transform.rotation+", New dir="+dir+": cur postn="+who.transform.position);
					
					who.doWalk(dir.x, dir.z, null, false);
					who.doPutDown(target);
				}
			}
			
		} else if (xmltxt.Contains("put-down") || xmltxt.Contains ("PUT-DOWN")) {
			Debug.Log ("Action=putdown");
			who.doPutDown(target);
		} else if (xmltxt.Contains ("locomotion") || xmltxt.Contains ("LOCOMOTION")) {
			Debug.Log ("Action=move");
			if(mode == playmodes.fdg) {
				movementfound = true;
			}
			if (target != null) {
				if (hasrel) { // check for relative walking
					if (relstr == "/AROUND") { // add more & use switch!!
						// calculate new positions - do relative to the object? 5 points from object 
						Int16 i = -1;
						Int16 j = -1;
						if (who.transform.position.x > target.transform.position.x) {
							i = -1;
						} else {
							i = 1;
						}
						/*if (who.transform.position.z > target.transform.position.z) {
							j = -1;
						} else {
							j = 1;
						}*/
						who.doWalk (target.transform.position.x, target.transform.position.z + 8, null, false);
						who.doWalk (target.transform.position.x+8*i, target.transform.position.z, null, false);
						who.doWalk (target.transform.position.x, target.transform.position.z - 8, null, false);
						who.doWalk (target.transform.position.x -8*i, target.transform.position.z, null, false);
//						who.doWalk (who.transform.position.x - (2*(who.transform.position.x - target.transform.position.x)), who.transform.position.z - (2*(who.transform.position.z - target.transform.position.z)), null, following);
//						who.doWalk (who.transform.position.x, who.transform.position.z, null, following);
						
						// make char look at audience since cannot figure out where was looking? 
						GameObject audience = GlobalObjs.getObject("AUDIENCE");
						who.doRotate(audience.transform.position.x, audience.transform.position.z, audience);
					
					} else if (relstr == "/LEFT") {
						Vector3 dir = (Vector3.left*20) + who.transform.position;//Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,90,0))) * new Vector3(0,0,50) + who.transform.position;
						who.doWalk(dir.x, dir.z, null, false);
					} else if (relstr == "/OPPOSITE") {
						Vector3 towhere = new Vector3(-1.0f*who.transform.position.x, 0, who.transform.position.z);
						who.doWalk (towhere.x, towhere.z, null, false);
					}
				} else {
					if (mode == playmodes.rules || mode == playmodes.fdg) {
						checkUpstaging(who, target.transform.position.x, target.transform.position.z, target, following);
					} else {
						who.doWalk (target.transform.position.x, target.transform.position.z, target, following);
					}
				}
			} else {
				if (hasrel) { // check for relative walking
					if (relstr == "/LEFT") {
						Vector3 dir = (Vector3.left*20)  + who.transform.position;//Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,90,0))) * new Vector3(0,0,50) + who.transform.position;
						who.doWalk(dir.x, dir.z, null, false);
					} else if (relstr == "/FORWARD") {
						Vector3 dir = (Vector3.forward*20) + who.transform.position;
						who.doWalk (dir.x, dir.z, null, false);
					}
				} else {
					if (mode == playmodes.rules || mode == playmodes.fdg) {
						if (GlobalObjs.listOfChars.Count > 2) { // only worry about upstaging when there is more than 2 characters
							checkUpstaging(who, targetx, targety, null, following);
						} else {
							who.doWalk (targetx, targety, null, following);
						}
					} else {
						who.doWalk (targetx, targety, null, following);
					}
				}
				if (targetx2 != -1) {
					if (mode == playmodes.rules || mode == playmodes.fdg) {
						checkUpstaging(who, targetx2, targety2, null, following);
					} else {
						who.doWalk (targetx2, targety2, null, following); // this one should get queued
					}
					if (targetx3 != -1) {
						
						if (mode == playmodes.rules || mode == playmodes.fdg) {
							checkUpstaging(who, targetx3, targety3, null, following);
						} else {
							who.doWalk (targetx3, targety3, null, following); // this one should get queued
						}
						
						if (targetx4 != -1) {
							if (mode == playmodes.rules || mode == playmodes.fdg) {
								checkUpstaging(who, targetx4, targety4, null, following);
							} else {
								who.doWalk (targetx4, targety4, null, following); // this one should get queued
							}
						}
					}
				}
			}
		} else if (xmltxt.Contains ("gaze") || xmltxt.Contains ("GAZE")) {
			Debug.Log ("Action=turn");
			if (target != null) {
				who.doRotate(target.transform.position.x, target.transform.position.z, target);
			} else {
				if (hasrel) { // calculate relative positions
					if (relstr == "/LEFT") { // add more & use switch!!
						//Vector3 dir = who.transform.position + ((
						//Vector3 endPos = Quaternion.AngleAxis(90, Vector3.up)*50*Vector3.forward;
							Vector3 dir = Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,-90,0))) * new Vector3(0,0,50) + who.transform.position; 
						Debug.Log ("Current rotation="+who.transform.rotation+", New dir="+dir+": cur postn="+who.transform.position);
						who.doRotate (dir.x, dir.z, null);
					} else {
						if (relstr == "/RIGHT") { // add more & use switch!!
							//Vector3 dir = who.transform.position + ((
							//Vector3 endPos = Quaternion.AngleAxis(90, Vector3.up)*50*Vector3.forward;
								Vector3 dir = Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,90,0))) * new Vector3(0,0,50) + who.transform.position; 
							Debug.Log ("Current rotation="+who.transform.rotation+", New dir="+dir+": cur postn="+who.transform.position);
							who.doRotate (dir.x, dir.z, null);
						}
					}
				} else {
					who.doRotate(targetx, targety, null);
				}
			}
		} else if (xmltxt.Contains ("POINT") || xmltxt.Contains ("point")) {
			Debug.Log ("Action=point");
			if (target != null) {
				if (mode == playmodes.rules || mode == playmodes.fdg) {
	            	// add rule to get everyone to look at the point to target
					Debug.Log ("Adding Everyone Look at pointed object "+target.name);
	            	foreach (CharFuncs c in GlobalObjs.listOfChars) {
	            		if (c.onstage() && c != who) {
	            			c.doRotate(target.transform.position.x, target.transform.position.z, target);
	            		}
	            	}
	            } else {
					who.doPoint(target);
				}
			} else {
				Debug.Log ("Error no target");
			}
		} else {
			Debug.Log ("Error - unknown command");
		}
		
		
	}

	public static void checkUpstaging(CharFuncs who, float targetx2, float targety2, GameObject g, bool following) {
		// check if my target is closer to audience 
		// 		checkmovetarget for move -- if upstage, adjust based on precedence -- check all chars movements, not just current position!!
		foreach (CharFuncs c in GlobalObjs.listOfChars) {
			if (c.onstage() && c.thisChar.name != who.thisChar.name) { // only check onstage chars and non-same chars
				switch (who.compareImportance(c)) {
					case "More":
						// make sure who is closer to audience -- consider target, not current loc
						if (targety2 < c.getLastMovePostn().z) {
							// change targety2
							Debug.Log ("Moving "+who.thisChar.name+" closer to audience");
							targety2 = c.getLastMovePostn().z + 1;
						}
						break;
					case "Less":
						// make sure who is further from audience -- consider target, not current loc
						if (targety2 > c.getLastMovePostn().z) {
							// change targety2
							Debug.Log ("Moving "+who.thisChar.name+" farther from audience");
							targety2 = c.getLastMovePostn().z - 1;
						}
						break;
					default:
						// error, leave as-is
						break;
				}
			} // if not onstage, do nothing
		}
		// check to be sure not too close to another character first
		foreach (CharFuncs c in GlobalObjs.listOfChars) {
			if (c.onstage() && c.thisChar.name != who.thisChar.name) {
				if (who.getDist(c.getLastMovePostn()) < 3) { // is this really the distance???
					// move apart
					Debug.Log ("NEED TO SEPARATE!!!");
				}
			}
		}
		// go ahead and walk to new target
		who.doWalk(targetx2, targety2, g, following);
		
		
		// for each char onstage
		// 	if char = actor do nothing
		// 	else
		// 	if actor precedence < char precedence & char diet to aud < actor diet to aud
		// 		move actor target closer to audience
		// 	if actor precedence > char precedence & actor diet < char diet to aud
		// 		move actor target farther from audience
		// end for
		// for each char onstage
		// 	if char = actor do nothing
		// 	else
		// 	if diet from actor to char < 3 feet, then move apart in opposite path of where they are
		// end for
		// move actor
		// should I be tracking this for the non-actors too??
		
	}
	
	public static void LoadChars() {
		if (fileindexNumber == 0) {
			Debug.Log ("ERROR - no file chosen");
		} else {
			Debug.Log ("Loading file:" + fullfilelist[fileindexNumber-1]);	
			// Be sure to check that a file is selected!!!
			
			// Read file, then generate pawns and marks and characters & setup global variables
			// format: Type    Speed    Name    X    Z    R1    R2    R3    R4    Holding    Color    Importance    Voice
			// Type: C or P or M for type of object - S for speed
			// Speed: Slow, Med, Fast (only for S)
			// Name: uppercase name with no spaces
			// Start X Position
			// Start Z Position
			// Rotation 4 components
			// Holding Object: define prior to the character!!
			// Color: blue, purple, red, green, yellow, orange, brown, white
			// Importance: 1 to 8 for chars only saying 1 = highest priority char to lowest - only chars
			// Voice: Alex, Ralph, Bruce, Fred, more? Kathy, Vicki, Victoria, Agnes, Princess, Junior
			
			StreamReader charfile = File.OpenText (fullfilelist[fileindexNumber-1] );
			string curLine = charfile.ReadLine ();
			string[] parsedLine = null; 
			string name = null;
			float startx;
			float startz;
			float rotation1;
			float rotation2;
			float rotation3;
			float rotation4;
			string objectheld;
			int priority;
			Color pcolor;
			bool success = false;
			//Debug.Log (curLine);
			
			GlobalObjs.priorityList.Capacity = 15;
			charlist = new GameObject[15];
			charlisttext = new string[15];
			targetlist = new GameObject[75];
			targetlisttext = new string[75];
			int cindex = 0;
			int oindex = 0;
			
			while (curLine != null) { // read each line
				parsedLine = curLine.Split ('\t');
				Debug.Log (curLine);
				Debug.Log (parsedLine[0]);
				switch (parsedLine[0]) {
					
					case "C":
						// define character
						name = parsedLine[2];
						success = float.TryParse (parsedLine[3], out startx);
						success = float.TryParse (parsedLine[4], out startz);
						success = float.TryParse (parsedLine[5], out rotation1);
						success = float.TryParse (parsedLine[6], out rotation2);
						success = float.TryParse (parsedLine[7], out rotation3); 
						success = float.TryParse (parsedLine[8], out rotation4);
						objectheld = parsedLine[9];
						// set color voice importance
						// need to know how many people so create right size char array for importance?
						
						GameObject person = (GameObject)Instantiate (InitScript.Instance.prefabchar, new Vector3(startx, 0, startz), new Quaternion(rotation1, rotation2, rotation3, rotation4));
						person.name = name;
						// get charfuncs & fire pickup if objectheld is defined
						CharFuncs personfunc = (CharFuncs) person.GetComponent (typeof(CharFuncs));
						
						
						// set voice
						personfunc.voice = parsedLine[12];
						// set material
					Debug.Log ("x"+parsedLine[10]+"x");
						personfunc.bodycolor = parsedLine[10];
						//personfunc.bodycolor = "BoxPersonColorOnly-GREEN";
/*						foreach(Material myMaterial in  Resources.FindObjectsOfTypeAll(typeof(Material))) {
				            //Debug.Log ("Material="+myMaterial.name);
				            if (myMaterial.name == parsedLine[10]) {
				                person.gameObject.renderer.material = myMaterial;
				                Debug.Log ("Found "+parsedLine[10]);
				            }
						}*/
						
						personfunc.armcolor = parsedLine[10].Replace ("Mat", "");
						Debug.Log ("y"+personfunc.armcolor+"y");
						// set myicon variable to image of appropriately colored icon from texture folder
						personfunc.myicon = (Texture) Resources.LoadAssetAtPath ("Assets/Textures/"+personfunc.armcolor+"icon.tiff", typeof(Texture));
//        prefab = (GameObject) Resources.LoadAssetAtPath("Assets/Artwork/mymodel.fbx", typeof(GameObject))
					
        
						GlobalObjs.listOfChars.Add (personfunc);
						GlobalObjs.listOfCharObj.Add(person);
						GlobalObjs.listOfAllObj.Add(person);
					
						targetlisttext[oindex] = name;
						targetlist[oindex] = person;
						oindex++;
						// add to priority array
						success = int.TryParse (parsedLine[11], out priority);
						
						// check how big current priorityList is
						/*if (priority+1 > GlobalObjs.priorityList.Count) {
							// make bigger & save what is in there now
							
						}*/
					
						GlobalObjs.priorityList.Insert(priority, person);
						//person.gameObject.tag = "Person"; // don't need this?
					
						/*if (objectheld != null && objectheld != "null") {
							GameObject objfound = GlobalObjs.getObject(objectheld);
							personfunc.doPickup(objfound);
						}*/ // TODO - fix how to get them to pickup for initialization
							
						charlisttext[cindex] = name;
						charlist[cindex] = person;
						cindex++;
						Debug.Log ("Created char="+name);
						break;
					case "P":
						// define pawn
						name = parsedLine[2];
						success = float.TryParse (parsedLine[3], out startx);
						success = float.TryParse (parsedLine[4], out startz);
							
					
						GameObject pawn = (GameObject)Instantiate (InitScript.Instance.prefabobj, new Vector3(startx, 0.5f, startz), new Quaternion(0,0,0,0));
						pawn.name = name;
						// TODO set material?
						switch(parsedLine[5]) {
							case "Blue":
								pcolor = Color.blue;
								break;
							case "Yellow":
								pcolor = Color.yellow;
								break;
							case "Orange":
								pcolor = new Color(255.0f/255.0f, 153.0f/255.0f, 51.0f/255.0f);
						Debug.Log ("ORANGE!!!");
								break;
							case "Green":
								pcolor = Color.green;
						Debug.Log ("GREEN!");
								break;
							case "Pink":
								pcolor = Color.magenta;
								break;
							case "Cyan":
								pcolor = Color.cyan;
								break;
							case "Grey":
							case "Gray":
								pcolor = Color.gray;
								break;
							case "Red":
								pcolor = Color.red;
								break;
							case "Purple":
								pcolor = new Color(76.0f/255.0f, 0.0f/255.0f, 153.0f/255.0f);//76 0 153
								break;
							case "Brown":
								pcolor = new Color(102.0f/255.0f, 51.0f/255.0f, 0.0f/255.0f);
								break;
							case "White":
								pcolor = Color.white;
								break;
						
							default:
								pcolor = Color.white;
								break;
							
						}
					//Debug.Log (parsedLine[5]);
					pawn.renderer.material.color = pcolor;
//11					pawn.renderer.material.shader = Shader.Find ("Diffuse");
						//rend.material.shader = Shader.Find("Specular");
       // rend.material.SetColor("_SpecColor", Color.red);
//11						pawn.renderer.material.SetColor("_Color", pcolor);
//						color = pcolor;
					
						
						GlobalObjs.listOfPawnObj.Add (pawn);
						GlobalObjs.listOfAllObj.Add (pawn);
						//pawn.gameObject.tag = "Pawn"; // don't need this?
						
						targetlisttext[oindex] = name;
						targetlist[oindex] = pawn;
						oindex++;
						Debug.Log ("Created pawn="+name);
						break;
					case "M":
						// define mark
						name = parsedLine[2];
						success = float.TryParse (parsedLine[3], out startx);
						success = float.TryParse (parsedLine[4], out startz);	
					
						GameObject mark = (GameObject)Instantiate (InitScript.Instance.prefabmark, new Vector3(startx, -0.5f, startz), new Quaternion(0,0,0,0));
						mark.name = name;
				
						GlobalObjs.listOfAllObj.Add (mark);
						//mark.gameObject.tag = "Mark"; // don't need this?
						mark.renderer.material.color = Color.clear;
						
						targetlisttext[oindex] = name;
						targetlist[oindex] = mark;
						oindex++;
						Debug.Log ("Created mark="+name);
						break;
					case "S":
						// define speed of script
						switch (parsedLine[1]) {
							case "Slow": // TODO make slower
								CharFuncs.mspeed = 5;
								CharFuncs.timerMax = 2.0f;
								CharFuncs.rspeed = 50;
								CharFuncs.sspeed = 20f;
								CharFuncs.pointertimerMax = 2.0f;
								break;
							case "Med":
								CharFuncs.mspeed = 5;
								CharFuncs.timerMax = 2.0f;
								CharFuncs.rspeed = 50;
								CharFuncs.sspeed = 20f;
								CharFuncs.pointertimerMax = 2.0f;
								break;
							case "Fast": // TODO make faster
								CharFuncs.mspeed = 5;
								CharFuncs.timerMax = 2.0f;
								CharFuncs.rspeed = 50;
								CharFuncs.sspeed = 20f;
								CharFuncs.pointertimerMax = 2.0f;
								break;
							default:
								// do nothing
								break;
							
						}
						
						Debug.Log ("Set speed="+parsedLine[1]);
						break;
					case "T":
						WWW www3 = new WWW("file://" + Application.dataPath + "/Textures/"+parsedLine[1]);
				        //yield return www3;
				        legendTexture = www3.texture;
						//legendTexture = parsedLine[1];
						break;
					default:
						// nothing
						Debug.Log ("ERROR parsing file");
						break;
				}
				
				
				curLine = charfile.ReadLine ();
			}
			/*
			charlist = new GameObject[5];
			charlist[0] = null;
			charlist[1] = GlobalObjs.Hamlet;
			charlist[2] = GlobalObjs.Horatio;
			charlist[3] = GlobalObjs.GraveDigger;
			charlist[4] = GlobalObjs.GraveDiggerTwo;
			
			charlisttext = new string[5];
			charlisttext[0] = "Choose Character:";
			for (int i = 1; i < charlist.Length; i++) {
				charlisttext[i] = charlist[i].name;
			}
			
			targetlist = new GameObject[20];
			targetlist[0] = null;
			targetlist[1] = GlobalObjs.Hamlet;
			targetlist[2] = GlobalObjs.Horatio;
			targetlist[3] = GlobalObjs.GraveDigger;
			targetlist[4] = GlobalObjs.GraveDiggerTwo;
			targetlist[5] = GlobalObjs.Skull1;
			targetlist[6] = GlobalObjs.Skull2;
			targetlist[7] = GlobalObjs.Shovel;
			targetlist[8] = GlobalObjs.Lantern;
			//targetlist[8] = GlobalObjs.Box;
			targetlist[9] = GlobalObjs.Audience;
			targetlist[10] = GlobalObjs.Grave;
			targetlist[11] = GlobalObjs.StageRight;
			targetlist[12] = GlobalObjs.CenterBackStage;
			targetlist[13] = GlobalObjs.Center;
			targetlist[14] = GlobalObjs.CenterRight;
			targetlist[15] = GlobalObjs.DownStage;
			targetlist[16] = GlobalObjs.StageLeft;
			targetlist[17] = GlobalObjs.Steps;
			targetlist[18] = GlobalObjs.Stool;
			targetlist[19] = GlobalObjs.UpStage;
			
			targetlisttext = new string[20];
			targetlisttext[0] = "Choose Target:";
			for (int i = 1; i < targetlist.Length; i++) {
				targetlisttext[i] = targetlist[i].name;
			}
			*/
			charfile.Close ();
			GlobalObjs.priorityList.TrimExcess(); // removes blank areas
			
			targetlisttext[oindex] = "/LEFT";
						//targetlist[oindex] = mark;
						oindex++;
			targetlisttext[oindex] = "/FORWARD";
			oindex++;
			
			targetlisttext[oindex] = "/AROUND "+targetlisttext[0];
			oindex++;
			
			
		}
	}
	
	public void RunAction() {
		// use the charlist, charindexNumber, actionlist, actionindexNumber, targetlist, targetindexNumber, txtx, txty, txtsay
		Debug.Log ("Running Action for "+charlist[charindexNumber].name);
		CharFuncs who = (CharFuncs) charlist[charindexNumber].GetComponent (typeof(CharFuncs));
		CharFuncs target = targetlist[targetindexNumber] != null?(CharFuncs) targetlist[targetindexNumber].GetComponent (typeof(CharFuncs)):null;
		float x;
		float y;
		bool success = float.TryParse (txtx, out x);
		success = float.TryParse (txty, out y);
		
		
		switch (actionindexNumber) {
		case 0:
		case 1:
			if (targetlisttext[targetindexNumber] == "/LEFT") {
				
				Vector3 dir = (Vector3.left*20)  + who.transform.position;//Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,90,0))) * new Vector3(0,0,50) + who.transform.position;
				Debug.Log("WALKING LEFT:"+dir);
				who.doWalk(dir.x, dir.z, null, false);
			} else if (targetlisttext[targetindexNumber] == "/FORWARD") {
				
				Vector3 dir = (Vector3.forward*20)  + who.transform.position;//Quaternion.Euler (who.transform.TransformDirection(new Vector3(0,90,0))) * new Vector3(0,0,50) + who.transform.position;
				Debug.Log("WALKING FORWARD:"+dir);
				who.doWalk(dir.x, dir.z, null, false);
			} else if (targetlisttext[targetindexNumber] == "/AROUND TABLE") {
				Int16 i = -1;
				GameObject ptarget = targetlist[0];
						
						if (who.transform.position.x > ptarget.transform.position.x) {
							i = -1;
						} else {
							i = 1;
						}
					
						who.doWalk (ptarget.transform.position.x, ptarget.transform.position.z + 8, null, false);
						who.doWalk (ptarget.transform.position.x+8*i, ptarget.transform.position.z, null, false);
						who.doWalk (ptarget.transform.position.x, ptarget.transform.position.z - 8, null, false);
						who.doWalk (ptarget.transform.position.x -8*i, ptarget.transform.position.z, null, false);						
			} else {
				who.doWalk(x, y, targetlist[targetindexNumber], (target == null?false:true));
			}
			break;
		case 2:
			who.doRotate(x, y, targetlist[targetindexNumber]);
			break;
		case 3:
			who.doPickup (targetlist[targetindexNumber]);
			break;
		case 4:
			who.doPutDown (targetlist[targetindexNumber]);
			break;
		case 5:
			who.doSpeak(txtsay);
			break;
		case 6:
			who.doPoint (targetlist[targetindexNumber]);
			break;
		case 7:
			who.doWalk (x, y, null, false);
			break;
		case 8:
			foreach(GameObject g in GlobalObjs.listOfCharObj) {
				Debug.Log (g.name+"="+g.transform.position+","+g.transform.rotation);
				if (g.transform.childCount != 0) {
					Debug.Log (g.name+" children=");
					for (int i=0; i< g.transform.childCount; i++) {
						Debug.Log (g.transform.GetChild(i).name);
					}
					Debug.Log ("End "+g.name+" children");
				}
			}
			break;
		case 9:
			who.doWalk(x, y, targetlist[targetindexNumber], true);
			break;
			
			
			
			//"Choose Action:", "Walk", "Look", "Pickup", "Putdown", "Speak", "Point"
			
		}
		
				
	}
	
	
}
