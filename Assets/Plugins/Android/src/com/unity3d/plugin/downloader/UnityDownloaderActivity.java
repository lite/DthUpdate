package com.unity3d.plugin.downloader;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.view.LayoutInflater;
import android.widget.CompoundButton;
import android.widget.FrameLayout;
import android.widget.ToggleButton;
import com.unity3d.player.UnityPlayerActivity;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;

public class UnityDownloaderActivity extends UnityPlayerActivity
{
    private static final String TAG = "UnityDownloaderActivity";

    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);

        int layoutID = getResources().getIdentifier("main", "layout", getPackageName());
        int buttonID = getResources().getIdentifier("toggleButton", "id", getPackageName());

        FrameLayout layout = (FrameLayout) LayoutInflater.from(this).inflate(layoutID, null);
        ToggleButton button = (ToggleButton) layout.findViewById(buttonID);

        addContentView(layout, new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MATCH_PARENT, FrameLayout.LayoutParams.MATCH_PARENT));

        button.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton buttonView,
                                         boolean isChecked) {
                new Thread(downloadRun).start();
            }
        });
    }

    Runnable downloadRun = new Runnable(){
        @Override
        public void run() {
            String update_url = getString(getResources().getIdentifier("update_xml", "string", getPackageName()));
            checkVersion(update_url);
        }
    };

    private void checkVersion(String update_xml){
        Log.d(TAG, "checkVersion: " + update_xml);

        try {
            URL url = new URL(update_xml);
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setConnectTimeout(5000);
            UpdateInfo updateInfo = parseXml(connection.getInputStream());
            Log.d(TAG, "version:" + updateInfo.getVersion());
            Log.d(TAG, "url:" + updateInfo.getUrl());
            Log.d(TAG, "desc:" + updateInfo.getDescription());

            String versionName = this.getPackageManager().getPackageInfo(getPackageName(), 0).versionName;
            Log.d(TAG, "versionName:" + versionName);

            if(updateInfo.getVersion() == versionName){
                Log.d(TAG, "This is the latest version.");
                return;
            }

            File parent = new File(Environment.getExternalStorageDirectory(), updateInfo.getPath());
            parent.mkdirs();
            String filename = parent.getPath() + updateInfo.getFile();
            Log.d(TAG, "filename:" + filename);

            updateGame(updateInfo.getUrl(), filename);
            installGame(filename);
        } catch (Exception e) {
            Log.e(TAG, e.toString());
        }
    }

    private void installGame(String filename) {
        Log.d(TAG, "installGame: " + filename);
        Intent intent = new Intent();
        intent.setAction(Intent.ACTION_VIEW);
        intent.setDataAndType(Uri.fromFile(new File(filename)), "application/vnd.android.package-archive");
        startActivity(intent);
    }

    private void updateGame(String fileurl, String filename) {
        Log.d(TAG, "UpdateGame: " + fileurl + " path: " + filename);

        try{
            URL url = new URL(fileurl);
            HttpURLConnection conn =  (HttpURLConnection) url.openConnection();
            conn.setConnectTimeout(5000);

            InputStream is = conn.getInputStream();
            File file = new File(filename);
            FileOutputStream fos = new FileOutputStream(file);
            BufferedInputStream bis = new BufferedInputStream(is);
            byte[] buffer = new byte[1024];
            int len ;
            while((len = bis.read(buffer))!=-1){
                fos.write(buffer, 0, len);
            }
            fos.close();
            bis.close();
            is.close();
            Log.d(TAG, "UpdateGame done.");
        } catch (Exception e) {
            Log.e(TAG, e.toString());
        }

    }

    private UpdateInfo parseXml(InputStream in){
        UpdateInfo updateInfo = new UpdateInfo();
        DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
        try {
            DocumentBuilder db = dbf.newDocumentBuilder();
            Document doc = db.parse(in);
            Element root = doc.getDocumentElement();
            NodeList resultNode = root.getElementsByTagName("info");
            for(int i = 0; i < resultNode.getLength();i++){
                Element res = (Element)resultNode.item(i);
                updateInfo.setVersion(res.getElementsByTagName("version").item(0).getFirstChild().getNodeValue());
                updateInfo.setUrl(res.getElementsByTagName("url").item(0).getFirstChild().getNodeValue());
                updateInfo.setPath(res.getElementsByTagName("path").item(0).getFirstChild().getNodeValue());
                updateInfo.setFile(res.getElementsByTagName("file").item(0).getFirstChild().getNodeValue());
                updateInfo.setDescription(res.getElementsByTagName("description").item(0).getFirstChild().getNodeValue());
            }
        } catch (Exception e) {
            Log.e(TAG, e.toString());
        }
        return updateInfo;
    }
}
