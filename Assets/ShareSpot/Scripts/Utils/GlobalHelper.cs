﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHelper{


	public static int MaxClients = 10;  ///< Maximal allowed clients to connect at the same time.
	public static int MaxRepetitions = 10;	///< Amount of repetions in the game mode. // TODO: using or deleting?

	/// <summary>
	/// Defining the area of the possible location of a pickup.
	/// </summary>
	private static float minX = 100f;
	private static float maxX = 1820f;
	private static float minY = 80f;
	private static float maxY = 120f;
	private static float minZ = 100f;
	private static float maxZ = 980f;

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
