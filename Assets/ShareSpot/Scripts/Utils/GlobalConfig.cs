using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfig{


	public static int MaxClients = 10;  ///< Maximal allowed clients to connect at the same time.
	public static int MaxRepetitions = 10;	///< Amount of repetions in the game mode.
	public static int ShowChallengesRate = 2;	///< Static var for inidicating how many pickups are needed until a challenge is shown.
	public static float ShootingSpeed = 0.1f;	///< The basic shoothing speed of a file.

	/// <summary>
	/// Defining the area of the possible location of a pickup.
	/// </summary>
	private static float minX = 100f;
	private static float maxX = 1388;
	private static float minY = 200f;
	private static float maxY = 500f;
	private static float minZ = 100f;
	private static float maxZ = 1100f;

	/// <summary>
	/// List of all available colors for collecting the pickups.
	/// </summary>
	private static List<Color> collectingColors = new List<Color>{
		Color.red
		,Color.green
		,Color.blue
		,Color.yellow
		,Color.magenta
		,Color.cyan
		,Color.white
		,Color.black
		,Color.gray
	};


	public static Color GetColorForPlayerId(int playerId){
		return collectingColors[playerId];
	}

	public static Vector3 GetPickupAreaMinValues(){
		return new Vector3 (minX, minY, minZ);
	}

	public static Vector3 GetPickupAreaMaxValues(){
		return new Vector3 (maxX, maxY, maxZ);
	}
}

