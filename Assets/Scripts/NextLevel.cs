using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PermanentUI.perm.finalCherry = PermanentUI.perm.cherries;
            PermanentUI.perm.finalGem = PermanentUI.perm.gems;
            SceneManager.LoadScene(sceneName);
        }
    }
}
