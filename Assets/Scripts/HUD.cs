using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The HUD
/// </summary>
public class HUD : MonoBehaviour
{
    [SerializeField]
    Text pathLengthText;

    const string pathLength = "Path Length: ";

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
	{
        EventManager.AddPathFoundListener(SetPathLength);
    }

    /// <summary>
    /// Sets the path length in the hud
    /// </summary>
    /// <param name="length">path length</param>
    void SetPathLength(float length)
    {
        pathLengthText.text = pathLength + length.ToString();
    }
}
