using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject scoreboard;
    [SerializeField] private Image cherry;
    [SerializeField] private Image gem;
    [SerializeField] private int cherryMult;
    [SerializeField] private Text cherryMultText;
    [SerializeField] private Text cherries;
    [SerializeField] private Text cherryScore;
    [SerializeField] private int gemMult;
    [SerializeField] private Text gemMultText;
    [SerializeField] private Text gems;
    [SerializeField] private Text gemScore;
    [SerializeField] private Text cherryTimes;
    [SerializeField] private Text cherryEquals;
    [SerializeField] private Text gemTimes;
    [SerializeField] private Text gemEquals;
    [SerializeField] private Text totalScore;
    [SerializeField] private Text totalScoreText;

    private bool next = false;
    public GameObject blackOutSquare;

    private void Start()
    {
        cherries.text = PermanentUI.perm.finalCherry.ToString();
        gems.text = PermanentUI.perm.finalGem.ToString();
        cherryScore.text = (PermanentUI.perm.finalCherry * cherryMult).ToString();
        gemScore.text = (PermanentUI.perm.finalGem * gemMult).ToString();
        totalScore.text = ((PermanentUI.perm.finalCherry * cherryMult) + (PermanentUI.perm.finalGem * gemMult)).ToString();
        StartCoroutine(menuSwitch());
    }

    private void Update()
    {
        if (next == true && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(fadeBlackOutSquare());
        }
    }

    private IEnumerator fadeBlackOutSquare(int fadeSpeed = 5)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while (blackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
    private IEnumerator menuSwitch()
    {
        yield return new WaitForSeconds(5);
        gameOver.SetActive(false);
        scoreboard.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        cherry.gameObject.SetActive(true);
        cherries.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        cherryTimes.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        cherryMultText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        cherryEquals.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        cherryScore.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2);
        gem.gameObject.SetActive(true);
        gems.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        gemTimes.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        gemMultText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        gemEquals.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        gemScore.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);
        totalScoreText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        totalScore.gameObject.SetActive(true);

        next = true;
    }
}
