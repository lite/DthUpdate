using UnityEngine;
using System.Collections;
using System.Reflection;

public class GameLoader : MonoBehaviour {
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 200, 100, 30), "Start")){
			loadBytes();
			
			Application.LoadLevel("GameMenu");
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
	
	private bool loadBytes()
	{
		//string link = "http://192.168.1.21:8080/scripts.unity3d";
		string link = "file://" + Application.dataPath + "/bundles/scripts.unity3d";
		WWW download = WWW.LoadFromCacheOrDownload(link, 1);
	    Debug.Log("start load bytes from" + link + "---"+Time.fixedTime );
	
		if (download.error != null)
	    {
	        Debug.LogError(download.error);
	        return false;
	    }
	
		AssetBundle bundleBytes = download.assetBundle;
		Debug.Log("end load bytes-------------------------------"+Time.fixedTime);

		// Load the TextAsset object
		Object [] objs = bundleBytes.LoadAll(typeof(TextAsset));
		foreach(TextAsset o in objs){
			Debug.Log(o.name);
			Assembly assembly = Assembly.Load(o.bytes);
    		System.Type type = assembly.GetType("GameMenu");
			GameObject go = new GameObject();
			go.AddComponent(type);
//			GameObject cam = GameObject.Find("Main Camera");
//			cam.AddComponent(type);
//			Camera.main.AddComponent( "GameMenu" );
			
//			MethodInfo dump = type.GetMethod("Dump", new System.Type[0]);
//			if (dump != null)
//		    {
//				dump.Invoke(assembly, null);
//		    }
        }
		
	    return true;
	}

	private bool loadScenes()
	{
		//string link = "http://192.168.1.21:8080/scenes.unity3d";
		string link = "file://" + Application.dataPath + "/bundles/scenes.unity3d";
		WWW download = WWW.LoadFromCacheOrDownload(link, 1);
	    Debug.Log("start load scenes from" + link + "---"+Time.fixedTime );
	
		if (download.error != null)
	    {
	        Debug.LogError(download.error);
	        return false;
	    }
	
		Debug.Log("end load scenes-------------------------------"+Time.fixedTime);
	
	    return true;
	}

}
