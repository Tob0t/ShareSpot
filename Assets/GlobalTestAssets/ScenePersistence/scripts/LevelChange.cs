using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelChange : MonoBehaviour
{
	public void ChangeLevel(string theLevelName)
	{
		Application.LoadLevel(theLevelName);
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		//SceneManager.LoadScene (theLevelName);
	}
}
