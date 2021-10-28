using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This config is loaded by both the ServerHost and ClientHost to help them establish
/// a connection. 
/// 
/// version: is used to match the versions between client and server and make sure that the connection
/// will be compatible. It is also ready by the deployment system to name builds. 
/// 
/// live: if the client is connection to the live server or localhost
/// 
/// stable: (only applies if live is true) If stable is true it will connection to stable.cyberblitz, if it's false
/// it will connect to nightly.cyberblitz
/// </summary>

public class Config {
	public int port;
	public string version;
	public bool live;
	public bool stable;
}