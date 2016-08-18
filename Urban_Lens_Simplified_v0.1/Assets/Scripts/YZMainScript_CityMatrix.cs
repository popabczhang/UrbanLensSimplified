using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System; //This allows the IComparable Interface
//using Newtonsoft;


public class YZMainScript_CityMatrix : MonoBehaviour {


	public GameObject player;
	public GameObject bullet;
	public int intPlayerCount = 0;
	//public redBox tempRedBox;

	public List<redBox> redBoxes;
	
	public GameObject[] structurePrefabsPlain; //no textures
	public GameObject[] structurePrefabsTexture; // materials and textures
	public GameObject[] structurePrefabsDiagrammatic; //diagramatic with program name on it
	public GameObject[] structurePrefabsClose; //close look
    public GameObject[] structurePrefabsCityMatrix; //RZ    linked to building height

	public List<YZStructure> YZStructures;
	public List<YZPiece> YZPieces;

	public int totalStructureNum = 0; //21
	public int gridNumX = 0; //19
	public int gridNumY = 0; //19
	public float gridSize = 0.0f; //32.6f

	public float[] boxColliderHeightRatios;

	public bool readPiecesFromFile = true;
	public string colortizerFilePath;
	public string colortizerTextResult = "";


	// Use this for initialization
	void Start () {

		redBoxes = new List<redBox> ();
		YZStructures = new List<YZStructure> ();
		YZPieces = new List<YZPiece> ();

		// create YZStructures list
		for (int i=0; i<=totalStructureNum-1; i++) {

			YZStructure tmpStructure = 	new YZStructure();
			tmpStructure.id = i;
			tmpStructure.floorNumber = UnityEngine.Random.Range (4,20);
			tmpStructure.totalArea = UnityEngine.Random.Range (800.0f,1200.0f) * tmpStructure.floorNumber;
			YZStructures.Add (tmpStructure);

		}

		// read pieces from colortizerFilePath & create YZPieces list
		if (readPiecesFromFile) {

			try {
				colortizerTextResult = System.IO.File.ReadAllText (colortizerFilePath);
			} catch (Exception ex) {
				print ("Error in parsing file Colorizer in loadColortizer-CSGrid");
			}

			int delimeter = 10;
			// 9  ==> \t 
			// 10 ==> \n
			// 13 ==> \r
			char _delimeter = Convert.ToChar (delimeter);
			string[] rows = colortizerTextResult.Split (_delimeter);
			//print ("colorizer data rows [0]: " + rows [0]);

			int delimeterT = 9;
			char _delimeterT = Convert.ToChar (delimeterT);

			
			int pieceCount = 0;
			for (int i =0; i<rows.Length; i++) {
				float getValue = -1.0f;
				try {
					string[] rowValues = rows [i].Split (_delimeterT);
					getValue = float.Parse (rowValues [0]);
					if (rowValues.Length > 2) {
						if (int.Parse(rowValues[0]) < YZStructures.Count) {
							
							YZPiece tmpPiece = new YZPiece ();
							tmpPiece.id = pieceCount;
							tmpPiece.fullString = rows [i];

							if (int.Parse(rowValues[0]) > -1) {
								//print ("NEW piece found!!!!!");
								//Debug.Log ("Piece ID:" + rows [i]);
								tmpPiece.structureID = int.Parse(rowValues [0]);
							}else{
								//print ("Empty(-1) piece found!!!!!");
								//Debug.Log ("Piece ID:" + rows [i]);
								tmpPiece.structureID = 16;
							}

							tmpPiece.xStep = int.Parse (rowValues [1]);
							tmpPiece.yStep = int.Parse (rowValues [2]);
							tmpPiece.rotation = int.Parse (rowValues [3]);
							
							Vector3 tmpVector3 = new Vector3 (tmpPiece.xStep * gridSize, 0f, tmpPiece.yStep * gridSize);
							int tmpStructureID = tmpPiece.structureID;

							//instantiate
							tmpPiece.modelPlain = (GameObject)Instantiate (structurePrefabsPlain [tmpStructureID], tmpVector3, Quaternion.identity);
							tmpPiece.modelDiagrammatic = (GameObject)Instantiate (structurePrefabsDiagrammatic [tmpStructureID], tmpVector3, Quaternion.identity);
							tmpPiece.modelClose = (GameObject)Instantiate (structurePrefabsClose [tmpStructureID], tmpVector3, Quaternion.identity);
							tmpPiece.emptyGameObjectForCollider = new GameObject ("emptyGameObjectForCollider");
							tmpPiece.emptyGameObjectForCollider.transform.position = tmpVector3;

							//rotate
							tmpPiece.modelPlain.transform.RotateAround (tmpVector3, Vector3.up, tmpPiece.rotation+180); //add 180 to correct
							tmpPiece.modelDiagrammatic.transform.RotateAround (tmpVector3, Vector3.up, tmpPiece.rotation+180); //add 180 to correct
							tmpPiece.modelClose.transform.RotateAround (tmpVector3, Vector3.up, tmpPiece.rotation+180); //add 180 to correct
							tmpPiece.emptyGameObjectForCollider.transform.RotateAround (tmpVector3, Vector3.up, tmpPiece.rotation+180); //add 180 to correct

							//activate
							tmpPiece.emptyGameObjectForCollider.tag = "clickable";
							//Attach Piece Information script for ID 
							YZPieceInfo newPieceInfo = tmpPiece.emptyGameObjectForCollider.AddComponent<YZPieceInfo> ();
							newPieceInfo.PieceArrayID = pieceCount;
							tmpPiece.modelDiagrammatic.SetActive (false);
							tmpPiece.modelClose.SetActive (false);

							//box collider
							BoxCollider tmpBoxCollider = (BoxCollider)tmpPiece.emptyGameObjectForCollider.AddComponent (typeof(BoxCollider));
							tmpBoxCollider.size = new Vector3 (gridSize, gridSize * boxColliderHeightRatios [tmpStructureID], gridSize);
							tmpBoxCollider.center = new Vector3 (0f, gridSize * boxColliderHeightRatios [tmpStructureID] / 2f, 0f);

							//structure
							tmpPiece.structure = YZStructures [tmpStructureID];

							//print (tmpPiece.structure.name);
							//print (tmpPiece.rotation);
							
							YZPieces.Add (tmpPiece);
							pieceCount ++;
						} else {

						}
					}
				} catch (Exception ex) {
					print("Error in parsing file Colorizer in CSGrid");
				}
			}
		}
		
		// create YZPieces list - random
		if (!readPiecesFromFile) {

			int pieceCount = 0;
			for (int i=0; i<=gridNumX-1; i++) {

				for (int j=0; j<=gridNumX-1; j++) {

					YZPiece tmpPiece = new YZPiece ();
					tmpPiece.id = pieceCount;
					Vector3 tmpVector3 = new Vector3 (i * gridSize, 0f, j * gridSize);
					int tmpStructureID = UnityEngine.Random.Range (0, totalStructureNum - 1);

					tmpPiece.modelPlain = (GameObject)Instantiate (structurePrefabsPlain [tmpStructureID], tmpVector3, Quaternion.identity);
					tmpPiece.modelDiagrammatic = (GameObject)Instantiate (structurePrefabsDiagrammatic [tmpStructureID], tmpVector3, Quaternion.identity);
					tmpPiece.modelClose = (GameObject)Instantiate (structurePrefabsClose [tmpStructureID], tmpVector3, Quaternion.identity);
					tmpPiece.emptyGameObjectForCollider = new GameObject ("emptyGameObjectForCollider");
					tmpPiece.emptyGameObjectForCollider.transform.position = tmpVector3;

					tmpPiece.emptyGameObjectForCollider.tag = "clickable";
					//Attach Piece Information script for ID 
					YZPieceInfo newPieceInfo = tmpPiece.emptyGameObjectForCollider.AddComponent<YZPieceInfo> ();
					newPieceInfo.PieceArrayID = pieceCount;
					tmpPiece.modelDiagrammatic.SetActive (false);
					tmpPiece.modelClose.SetActive (false);

					BoxCollider tmpBoxCollider = (BoxCollider)tmpPiece.emptyGameObjectForCollider.AddComponent (typeof(BoxCollider));
					tmpBoxCollider.size = new Vector3 (gridSize, gridSize * boxColliderHeightRatios [tmpStructureID], gridSize);
					tmpBoxCollider.center = new Vector3 (0f, gridSize * boxColliderHeightRatios [tmpStructureID] / 2f, 0f);

					tmpPiece.structure = YZStructures [tmpStructureID];

					YZPieces.Add (tmpPiece);
					pieceCount ++;

				}
			
			}

		}

	}//end Start()




	// Update is called once per frame
	void Update () {

		/*
		//adding red box
		if (Input.GetMouseButtonDown (0)) {

			//Instantiate (player, new Vector3( UnityEngine.Random.Range(-5.0F, 5.0F), 0.0F, UnityEngine.Random.Range(-5.0F, 5.0F) ), Quaternion.identity);

			redBox tempRedBox = new redBox();
			tempRedBox.box = new GameObject();
			tempRedBox.box = (GameObject)Instantiate (player, new Vector3( UnityEngine.Random.Range (-5.0f,5.0f), 0.0f, UnityEngine.Random.Range (-5.0f,5.0f) ), Quaternion.identity );
			tempRedBox.id = intPlayerCount;
			redBoxes.Add( tempRedBox );

			intPlayerCount ++;

		} else if (Input.GetMouseButtonDown (1)) {

			Instantiate (bullet, new Vector3( UnityEngine.Random.Range(-5.0F, 5.0F), 0.0F, UnityEngine.Random.Range(-5.0F, 5.0F) ), Quaternion.identity);
			foreach (redBox tempRedBox in redBoxes) {
				Destroy (tempRedBox.box);
			}
			Debug.Log ("");
		}
		*/

	}//end Update()




	//public class defination: 
	public class redBox {
		
		// Properties:
		public int id;
		public float blood;
		public GameObject box;
		
		// Instance Constructor. 
		public redBox() {
			
			id = 0; 
			blood = 1.0f;
				
		}
		
	}//end of class redBox

	
	public class YZStructure {
		
		// Properties:
		public int id;
		public int floorNumber;
		public float totalArea;
		
		// Instance Constructor. 
		public YZStructure() {
			
			id = 0;
			floorNumber = 0;
			totalArea = 0.0f;
			
		}
		
	}//end of class YZStructure

	public class YZPiece {
		
		// Properties:
		public int id;
		public YZStructure structure;
		public GameObject modelPlain;
		public GameObject modelDiagrammatic;
		public GameObject modelClose;
		public GameObject emptyGameObjectForCollider;

		public string fullString;
		public int structureID;
		public int xStep;
		public int yStep;
		public int rotation;
		
		// Instance Constructor. 
		public YZPiece() {

			id = 0; 
			fullString = "";
			structureID = 0;
			xStep = 0;
			yStep = 0;
			rotation = 0;
			
		}
		
	}//end of class YZPiece




}//End of YZMainScript
