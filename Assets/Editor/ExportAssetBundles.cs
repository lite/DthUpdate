// C# Example
// Builds an asset bundle from the selected objects in the project view.
// Once compiled go to "Menu" -> "Assets" and select one of the choices
// to build the Asset Bundle

using UnityEngine;
using UnityEditor;
using System.IO;


public class ExportAssetBundles {
    [MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource () {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets,BuildTarget.Android);
            Selection.objects = selection;
        }
    }
    [MenuItem("Assets/Build AssetBundle From Selection - No dependency tracking")]
    static void ExportResourceNoTrack () {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // Build the resource file from the active selection.
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path);
        }
    }
	[@MenuItem ("Assets/BuildAndroidStreamed")]
	static void BuildScene(){
		string[] levels  = {"Assets/ProfabsEditor.unity"};
		BuildPipeline.BuildStreamedSceneAssetBundle(levels, "Streamed-Level1.unity3d", BuildTarget.Android);
	}
	[@MenuItem("Assets/ForBuildNpc")]
	static void ForBuildNpc(){
		int dirs=Selection.objects.Length;
		string unity3dFileName;
		for(int i=0;i<dirs;i++){
			string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
			if(path.Length!=0){
				int pos=path.LastIndexOf("/");
				if (pos<0) pos = 0;
				unity3dFileName = path.Substring(pos);
				ExportAssetBundles exp=new ExportAssetBundles();
				string[] fileEntries = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);//仅本级目
				Object[] objects = new Object[fileEntries.Length];
				for (int k = 0; k < fileEntries.Length; k++){
					string filepath=fileEntries[k];
					string localpath=filepath.Replace("\\","/");
					objects[k]=AssetDatabase.LoadMainAssetAtPath(localpath);
				}
				string str=path.Substring(0,path.IndexOf("/"))+"/npcpackage" + unity3dFileName + ".unity3d";
				BuildPipeline.BuildAssetBundle(objects[0],objects,str,BuildAssetBundleOptions.CollectDependencies
				                               | BuildAssetBundleOptions.CompleteAssets,BuildTarget.Android);
				
			}
		}
		
	}
	
	
}