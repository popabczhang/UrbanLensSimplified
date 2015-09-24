using UnityEngine;
using System.Collections;

public class YZClick : MonoBehaviour {


	public GameObject cameraMain;
	public GameObject particle;
	public Vector3 checkMousePosition;
	public bool firstTimeClick = true;
	public int previousPieceID;
	public int currentPieceID;
	private YZMainScript YZMainScript;
	private YZCloseLook YZCloseLook;
	private CamMaxMouse CamMaxMouse;

	// Use this for initialization
	void Start () {
		
		//CSStructures gridStructures = (CSStructures)GameObject.Find("CSGrid").GetComponent<CSStructures>();
		//private YZMainScript YZMainScript = (YZMainScript)GameObject.Find("YZMainScript").GetComponent<YZMainScript>();
		//private YZCloseLook YZCloseLook = (YZCloseLook)GameObject.Find("Main Camera").GetComponent<YZCloseLook>();
		YZMainScript = GameObject.Find ("YZMainScript").GetComponent<YZMainScript> ();
		YZCloseLook = GameObject.Find("Main Camera").GetComponent<YZCloseLook>();
		CamMaxMouse = GameObject.Find("Main Camera").GetComponent<CamMaxMouse>();
	
	}
	
	// Update is called once per frame
	void Update () {


		//check mouse up & down in same position
		if (Input.GetMouseButtonDown(0)) { checkMousePosition = Input.mousePosition; }
		if (Input.GetMouseButtonUp(0) && Input.mousePosition == checkMousePosition) {
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
			if (hit) {
				//Debug.Log("Hit " + hitInfo.transform.gameObject.name);
				GameObject focusedGameObject = hitInfo.transform.gameObject;
				if (focusedGameObject.tag == "clickable") {
					//Debug.Log ("It's working!");
					currentPieceID = focusedGameObject.GetComponent<YZPieceInfo>().PieceArrayID;
					//relocate the main cam target - uncomfortable for now
					//CamMaxMouse.target.transform.position = YZMainScript.YZPieces[currentPieceID].emptyGameObjectForCollider.transform.position;
					if (firstTimeClick) {
						YZMainScript.YZPieces[currentPieceID].modelPlain.SetActive (false);
						YZMainScript.YZPieces[currentPieceID].modelDiagrammatic.SetActive (true);
						previousPieceID = currentPieceID;
						firstTimeClick = false;
						YZCloseLook.pieceSelected = true;
						YZCloseLook.selectedPieceID = currentPieceID;
					} else if(currentPieceID != previousPieceID) {
						YZMainScript.YZPieces[previousPieceID].modelPlain.SetActive (true);
						YZMainScript.YZPieces[previousPieceID].modelDiagrammatic.SetActive (false);
						YZMainScript.YZPieces[previousPieceID].modelClose.SetActive (false);
						YZMainScript.YZPieces[currentPieceID].modelPlain.SetActive (false);
						YZMainScript.YZPieces[currentPieceID].modelDiagrammatic.SetActive (true);
						previousPieceID = currentPieceID;
						YZCloseLook.pieceSelected = true;
						YZCloseLook.selectedPieceID = currentPieceID;
					} else if(currentPieceID == previousPieceID){
						YZMainScript.YZPieces[currentPieceID].modelPlain.SetActive (true);
						YZMainScript.YZPieces[currentPieceID].modelDiagrammatic.SetActive (false);
						YZMainScript.YZPieces[currentPieceID].modelClose.SetActive (false);
						firstTimeClick = true;
						YZCloseLook.pieceSelected = false;
					}
				} else {
					//Debug.Log ("nopz");
					if (previousPieceID!=null){
						YZMainScript.YZPieces[previousPieceID].modelPlain.SetActive (true);
						YZMainScript.YZPieces[previousPieceID].modelDiagrammatic.SetActive (false);
						YZMainScript.YZPieces[previousPieceID].modelClose.SetActive (false);
						firstTimeClick = true;
						YZCloseLook.pieceSelected = false;
					}
				}
			} else {
				//Debug.Log("No hit");
				if (previousPieceID!=null){
					YZMainScript.YZPieces[previousPieceID].modelPlain.SetActive (true);
					YZMainScript.YZPieces[previousPieceID].modelDiagrammatic.SetActive (false);
					YZMainScript.YZPieces[previousPieceID].modelClose.SetActive (false);
					firstTimeClick = true;
					YZCloseLook.pieceSelected = false;
				}
			}
		}
		
	}


}
