using UnityEngine;
using System.Collections;

public class GameMenu : MonoBehaviour
{
    void OnGUI()
	{
		if (GUI.Button(new Rect(10, 150, 100, 30), "One")){
			Application.LoadLevel(2);
		}
		if (GUI.Button(new Rect(10, 200, 100, 30), "Two")){
			Application.LoadLevel(3);
		}
		if (GUI.Button(new Rect(10, 250, 100, 30), "Three")){
			Application.LoadLevel(4);
		}
		if (GUI.Button(new Rect(10, 300, 100, 30), "Back")){
			Application.LoadLevel(0);
		}	
	}
}