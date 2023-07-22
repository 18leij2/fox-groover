using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    [SerializeField] private GameObject level;
    [SerializeField] private PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(levelShow());        
    }

    IEnumerator levelShow()
    {
        yield return new WaitForSeconds(3);
        level.SetActive(false);
        controller.enabled = true;
    }
}
