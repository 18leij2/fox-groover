using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PermanentUI : MonoBehaviour
{
    //Player Stats
    public int cherries = 0;
    public int health = 6;
    public int gems = 0;
    public int finalCherry;
    public int finalGem;

    public static PermanentUI perm;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //Singleton
        if (!perm)
        {
            perm = this;
        }
        else
            Destroy(gameObject);
    }
}
