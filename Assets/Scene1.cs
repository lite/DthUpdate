using UnityEngine;
using System.Collections;

public class Scene1 : MonoBehaviour {
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 100, 100, 20), "Scene1" );
		if (GUI.Button(new Rect(10, 300, 100, 30), "Back")){
			Application.LoadLevel(1);
		}	
	}
}

