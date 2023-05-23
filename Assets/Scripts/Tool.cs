using UnityEditor;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void DeletePlayerPref()
    {
        PlayerPrefs.DeleteAll();
    }
}
