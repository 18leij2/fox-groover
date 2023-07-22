using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
 
public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loginMenu;
    public GameObject AddressWindow;
    public TextMeshProUGUI address;
    public GameObject BalanceWindow;
    public TextMeshProUGUI balance;
    public GameObject TransactionWindow;
    public GameObject VerificationWindow;
    public InputField VerificationInput;
    public InputField userInput;
    public InputField passInput;
    public GameObject TXNWindow;

    public InputField txnID;
    public string txn = "";

    public GameObject keyCheck;
    public InputField keyInput;
    private string key;

    // [SerializeField] private string user;
    // [SerializeField] private string pass;

    private string TFCIP = "http://52.59.241.179:8011";

    public void playGame()
    {
        // VerificationWindow.SetActive(true);
        SceneManager.LoadScene("GameScene");
    }

    public void playGameOk()
    {
        Example json = JsonUtility.FromJson<Example>(txn);
        Verify verify = new Verify(
            json.data.tx.from,
            json.data.tx.to,
            json.data.tx.amount,
            json.data.tx.id,
            "20210203"
        );
        string json2 = JsonUtility.ToJson(verify);
        print(json2);
        StartCoroutine(POSTRequest(TFCIP + "/verification", json2, (payload) =>
            {
                print(payload);
                if (payload != null)
                {
                    VerificationWindow.SetActive(false);
                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    print("ERROR");
                }
            }));
    }

    public void logIn()
    {
        mainMenu.SetActive(false);
        loginMenu.SetActive(true);
    }

    public class Verify
    {
        public string from;
        public string to;
        public string amount;
        public string txid;
        public string date;

        public Verify(string a, string b, int amt, string id, string timestamp)
        {
            from = a;
            to = b;
            amount = amt.ToString();
            txid = id;
            date = timestamp.ToString();
        }
    }

    [System.Serializable]
    public class Data
    {
        public string pubKey;
        public string privKey;
        public string address;
    }

    public class JSON
    {
        public int code;
        public Data data;
        public string msg;
    }

    [System.Serializable]
    public class GetData
    {
        public string activeTFC;
        public string lockedTFC;
    }

    public class GetJSON
    {
        public int code;
        public GetData data;
        public string msg;
    }

    public class TransferTx
    {
        public string from;
        public string to;
        public int amount;
        public string auxdata;
        public int carryFee;

        public TransferTx(string a, string b, int amt, string data, int fee)
        {
            from = a;
            to = b;
            amount = amt;
            auxdata = data;
            carryFee = fee;
        }
    }

    [System.Serializable]
    public class Tx
    {
        public string id;
        public int timestamp;
        public string from;
        public int accountNonce;
        public string to;
        public int amount;
        public int carryFee;
        public string payload;
        public int txType;
        public string signature;
    }

    [System.Serializable]
    public class txData
    {
        public Tx tx;
    }

    public class Example
    {
        public int code;
        public txData data;
        public string msg;
    }

    IEnumerator POSTRequest(string uri, string json, System.Action<string> callback)
    {
        var uwr = new UnityWebRequest(uri, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("POST Error: " + uwr.error);
        }
        else
        {
            Debug.Log("POST Response:\n" + uwr.downloadHandler.text);
            callback(uwr.downloadHandler.text);
        }
    }

    IEnumerator GETRequest(string uri, System.Action<string> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(uri))
        {
            yield return uwr.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(pages[page] + ": GET Error: " + uwr.error);
            }
            else
            {
                Debug.Log(pages[page] + ": GET Response:\n" + uwr.downloadHandler.text);
                callback(uwr.downloadHandler.text);
            }
        }
    }

    public void getAddress()
    {
        StartCoroutine(POSTRequest(TFCIP + "/account", "{}", (payload) =>
            {
                JSON json = JsonUtility.FromJson<JSON>(payload);
                address.text = json.data.address;
            }
        ));
        AddressWindow.SetActive(true);
    }

    public void getAddressOk()
    {
        AddressWindow.SetActive(false);
    }

    public void getBalance()
    {
        StartCoroutine(GETRequest(TFCIP + "/balance/" + address.text, (payload) => // "1E37wEQrNev7E5SKS2v5qFiGFaisMHk9c2"
        	{
                GetJSON json = JsonUtility.FromJson<GetJSON>(payload);
                balance.text = json.data.activeTFC;
        	}));
        BalanceWindow.SetActive(true);
    }

    public void getBalanceOk()
    {
        BalanceWindow.SetActive(false);
    }

    public void openCheck()
    {
        keyCheck.SetActive(true);
    }

    public void sendTFC()
    {
        TransferTx transferTx = new TransferTx(
            address.text,
            "1E37wEQrNev7E5SKS2v5qFiGFaisMHk9c2",
            5,
            "testing",
            0
            );
        string json = JsonUtility.ToJson(transferTx);
        // print(json);
        StartCoroutine(POSTRequest(TFCIP + "/transferTx", json, (payload) =>
            {
                txn = payload;
            }));
        TransactionWindow.SetActive(true);
    }

    public void getTFC()
    {
        TransferTx transferTx = new TransferTx(
            "1E37wEQrNev7E5SKS2v5qFiGFaisMHk9c2",
            address.text,
            5,
            "testing",
            0
            );
        string json = JsonUtility.ToJson(transferTx);
        // print(json);
        StartCoroutine(POSTRequest(TFCIP + "/transferTx", json, (payload) =>
            {
                txn = payload;
            }));
        TransactionWindow.SetActive(true);
    }

    public void confirmTXN()
    {
        TXNWindow.SetActive(true);
    }

    public void confirmTXNOk()
    {
        string sig = txnID.text;
        Example json = JsonUtility.FromJson<Example>(txn);
        json.data.tx.signature = sig;
        string json2 = JsonUtility.ToJson(json.data.tx);
        StartCoroutine(POSTRequest(TFCIP + "/signedTx", json2, (payload) =>
        	{
        		return;
        	}));
        TXNWindow.SetActive(false);
    }
    public void sendTFCOk()
    {
        TransactionWindow.SetActive(false);
    }

    public void back()
    {
        mainMenu.SetActive(true);
        loginMenu.SetActive(false);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}