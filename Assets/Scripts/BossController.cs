using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [SerializeField] private GameObject bossWall;
    [SerializeField] private GameObject bossStart;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject HPBar;

    [SerializeField] private Image blackOutSquare;

    private bool endGame = false;

    private int debounce = 0;

    private void Update()
    {
        if (player.GetComponent<PlayerController>().bossStart == true && debounce == 0)
        {
            debounce = 1;
            bossWall.SetActive(true);
            HPBar.SetActive(true);
            boss.GetComponent<BossScript>().enabled = true;
        }

        if (BossScript.instance.health <= 0 && endGame == false)
        {
            endGame = true;
            PermanentUI.perm.finalCherry = PermanentUI.perm.cherries;
            PermanentUI.perm.finalGem = PermanentUI.perm.gems;
            PermanentUI.perm.cherries = 0;
            PermanentUI.perm.gems = 0;
            StartCoroutine(fadeBlackOutSquare());
        }
    }

    private IEnumerator fadeBlackOutSquare(int fadeSpeed = 1)
    {
        yield return new WaitForSeconds(3);
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while (blackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        SceneManager.LoadScene("GameWin");
    }
}
