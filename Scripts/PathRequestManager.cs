using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour {

	Queue<PersonController> pathRequestQueue = new Queue<PersonController>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    PathFinding pathfinding;

    bool isProcessingPath;

    void Awake(){
        instance = this;
        pathfinding = GetComponent<PathFinding>();
    }

	public static void RequestPath(PersonController obj, Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
		//Debug.Log ("Path request made");
		obj.request = new PathRequestManager.PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(obj);
        instance.TryProcessNext();
    }

	public static void RemoveRequest(PersonController obj) {
		Queue<PersonController> tempQueue = new Queue<PersonController> ();
		while (instance.pathRequestQueue.Count > 0) {
			PersonController temp = instance.pathRequestQueue.Dequeue ();
			if (temp != obj) {
				tempQueue.Enqueue (temp);
			}
		}
		instance.pathRequestQueue = tempQueue;
	}

    void TryProcessNext() {
        if (!isProcessingPath && pathRequestQueue.Count > 0) {
			PersonController tempPerson = pathRequestQueue.Dequeue ();
			if (tempPerson != null) {
				currentPathRequest = tempPerson.request;
				if (currentPathRequest != null) {
					isProcessingPath = true;
					pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
				} else {
					TryProcessNext ();
				}
			} else {
				TryProcessNext ();
			}
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    public class PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
