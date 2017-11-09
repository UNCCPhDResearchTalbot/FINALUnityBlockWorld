using UnityEngine;
using System.Collections;

public class FQueueObj : MonoBehaviour {
	
	public GameObject actorObj;
	public CharFuncs actorFunc;
	public GameObject target;
	public Vector3 targetpt;
	public int msgnum;
	public bool following;
	public actiontype action;
	public enum actiontype {
		move,
		rotate,
		pickup,
		putdown,
		intermission,
		other
	};
	
	public FQueueObj(Vector3 p, GameObject t, int n, bool f, actiontype a, GameObject ac) {
		target = t;
		targetpt = p;
		msgnum = n;
		following = f;
		action = a;
		actorObj = ac;
		if (a != actiontype.intermission) {
			actorFunc = GlobalObjs.getCharFunc(actorObj);
		}
	}
	
	public FQueueObj(Vector3 p, GameObject t, int n, bool f, GameObject ac) {
		target = t;
		targetpt = p;
		msgnum = n;
		following = f;
		actorObj = ac;
	}
	
	public string getTargetType() {
		if (target == null) {
			return "Vector3";
		} else {
			return "GameObject";
		}
	}
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
