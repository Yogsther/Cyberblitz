using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

public class BuilderPipeline : Editor
{
	/*[MenuItem("Build Pipeline/Build client Windows")]
	[MenuItem("Build Pipeline/Build client macOS")]*/
	/*[MenuItem("Build Pipeline/Build server Linux")]*/
	/*[MenuItem("Build Pipeline/Build for deployment")]*/

	[MenuItem("Build Pipeline/Windows/Both")]
	public static void BuildBothWindows()
	{
		BuildClientWindows();
		BuildServerWindows();
	}

	[MenuItem("Build Pipeline/Windows/Build client")]
	public static void BuildClientWindows()
	{
		string path = "ClientWindows";
		string[] scenes = new string[] { "Assets/Scenes/Client.unity" };

		BuildGame(path, scenes, ".exe", BuildTarget.StandaloneWindows64, false);
	}


	[MenuItem("Build Pipeline/macOS/Build client")]
	public static void BuildClientMacos()
	{
		string path = "ClientmacOS";
		string[] scenes = new string[] { "Assets/Scenes/Client.unity" };

		BuildGame(path, scenes, "", BuildTarget.StandaloneOSX, false);
	}



	[MenuItem("Build Pipeline/Windows/Build server")]
	public static void BuildServerWindows()
	{
		string path = "ServerWindows";
		string[] scenes = new string[] { "Assets/Scenes/Server.unity" };

		BuildGame(path, scenes, ".exe", BuildTarget.StandaloneWindows, true);
	}


	[MenuItem("Build Pipeline/macOS/Build server")]
	public static void BuildMacosServer()
	{
		string path = "ServermacOS";
		string[] scenes = new string[] { "Assets/Scenes/Server.unity" };

		BuildGame(path, scenes, "", BuildTarget.StandaloneOSX, true);
	}

	[MenuItem("Build Pipeline/Linux/Build server")]
	public static void BuildLinuxServer()
	{
		string path = "ServerLinux";
		string[] scenes = new string[] { "Assets/Scenes/Server.unity" };

		BuildGame(path, scenes, "._x86_64", BuildTarget.StandaloneLinux64, true);
	}



	[MenuItem("Build Pipeline/Build deployment")]
	public static void BuildDeployment()
	{
		BuildClientMacos();
		BuildClientWindows();
		BuildLinuxServer();
	}


	public static void BuildGame(string path, string[] scenes, string sufix, BuildTarget target, bool server = false)
	{

		Config config = GetConfig();

		FileUtil.DeleteFileOrDirectory("Builds/" + path);

		string destination = "Builds/" + path + "/" + (server ? "CyberBlitzServer" : "CyberBlitzClient") + "_" + config.version + sufix;
		BuildPipeline.BuildPlayer(scenes, destination, target, server ? BuildOptions.EnableHeadlessMode : BuildOptions.None);

		// Does not work, should be investigaed:
		//Debug.Log("Built: " + destination);

		/*
		 * Start the process when building is done.
		 * Will implement later
		 * 
		 * Process proc = new Process();
		proc.StartInfo.FileName = path + "/BuiltGame.exe";
		proc.Start();*/
	}

	public static Config GetConfig()
	{
		string path = "Assets/Scripts/config.json";
		StreamReader reader = new StreamReader(path);
		string json = reader.ReadToEnd();
		reader.Close();
		return JsonConvert.DeserializeObject<Config>(json);
	}


}