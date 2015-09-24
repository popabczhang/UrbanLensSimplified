using UnityEngine;
using System.Collections;

public class YZCloseLook : MonoBehaviour {

	public float closeLookDistance; //100f
	public bool pieceSelected = false;
	public int selectedPieceID;
	private YZMainScript YZMainScript;
	public GameObject testPosViz;

	// Use this for initialization
	void Start () {

		YZMainScript = GameObject.Find ("YZMainScript").GetComponent<YZMainScript> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (pieceSelected) {
			Vector3 camPos = transform.position;
			BoxCollider _bc = YZMainScript.YZPieces[selectedPieceID].emptyGameObjectForCollider.GetComponent<BoxCollider>();
			Vector3 closestPoint = _bc.ClosestPointOnBounds(camPos);
			float distance = Vector3.Distance(closestPoint, camPos);
			if (distance < closeLookDistance) {
				//Instantiate (testPosViz, closestPoint, Quaternion.identity);
				YZMainScript.YZPieces[selectedPieceID].modelPlain.SetActive (false);
				YZMainScript.YZPieces[selectedPieceID].modelDiagrammatic.SetActive (false);
				YZMainScript.YZPieces[selectedPieceID].modelClose.SetActive (true);
			} else {
				YZMainScript.YZPieces[selectedPieceID].modelPlain.SetActive (false);
				YZMainScript.YZPieces[selectedPieceID].modelDiagrammatic.SetActive (true);
				YZMainScript.YZPieces[selectedPieceID].modelClose.SetActive (false);
			}
		}

	}
}
