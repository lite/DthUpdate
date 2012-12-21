using UnityEngine;
using System.Collections;

public class GameLoader : MonoBehaviour {
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 200, 100, 30), "Start")){
			Application.LoadLevel(1);
		}
		
		if(GUI.Button(new Rect(10, 300, 100, 30), "Update")){   
			using (AndroidJavaClass unity_player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				AndroidJavaObject current_activity = unity_player.GetStatic<AndroidJavaObject>("currentActivity");
		
				AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent",
																current_activity,
																new AndroidJavaClass("com.unity3d.plugin.downloader.UnityDownloaderActivity"));
		
				int Intent_FLAG_ACTIVITY_NO_ANIMATION = 0x10000;
				intent.Call<AndroidJavaObject>("addFlags", Intent_FLAG_ACTIVITY_NO_ANIMATION);
				intent.Call<AndroidJavaObject>("putExtra", "name", "Downloading...");   

//				intent.Call<AndroidJavaObject>("putExtra", "unityplayer.Activity", 
//															current_activity.Call<AndroidJavaObject>("getClass").Call<string>("getName"));
				current_activity.Call("startActivity", intent);
		
				if (AndroidJNI.ExceptionOccurred() != System.IntPtr.Zero)
				{
					Debug.LogError("Exception occurred while attempting to start DownloaderActivity - is the AndroidManifest.xml incorrect?");
					AndroidJNI.ExceptionDescribe();
					AndroidJNI.ExceptionClear();
				}
			}
    	}
	}
	
	void Update ()   
    {   
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home) )   
        {   
            Application.Quit();   
        }   
    }   
}
