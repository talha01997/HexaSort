using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
//using GameAssets.GameSet.GameDevUtils.Managers;

public class CoinsManager : MonoBehaviour
{

    public static CoinsManager Instance;
    //References
    [Header("UI references")]
    //[SerializeField]public Text coinUIText;
    [SerializeField]public GameObject animatedCoinPrefab, objectInWorld;
    [SerializeField] public Transform target;
    public RectTransform canvasRect;
    public Camera mainCam;

    [Space]
    [Header("Available coins : (coins to pool)")]
    [SerializeField] int maxCoins;
    Queue<GameObject> coinsQueue = new Queue<GameObject>();


    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 0.9f)] float minAnimDuration;
    [SerializeField] [Range(0.9f, 2f)] float maxAnimDuration;

    [SerializeField] Ease easeType;
    [SerializeField] float spread;

    Vector3 targetPosition;


    private int _c = 0;

    public int Coins
    {
        get { return _c; }
        set
        {
            _c = value;
            //update UI text whenever "Coins" variable is changed
            //coinUIText.text = Coins.ToString();
        }
    }

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        targetPosition = target.position - new Vector3(0, 100, 0);
        // Coins = PlayerPrefs.GetInt("Coin");
        //	_c              = CurrencyManager.Instance.TotalCurrencyFor("Coins");
        //  coinUIText.text = Coins.ToString();
        //prepare pool
        PrepareCoins();
    }

    void PrepareCoins()
    {
        GameObject coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoinPrefab);
            coin.transform.parent = canvasRect;
            coin.SetActive(false);
            coinsQueue.Enqueue(coin);
        }
    }

    IEnumerator Animate(Vector3 collectedCoinPosition, int amount, int score)
    {
        //yield return new WaitForSeconds(1);
        for (int i = 0; i < amount; i++)
        {
            //check if there's coins in the pool
            if (coinsQueue.Count > 0)
            {
                //extract a coin from the pool
                GameObject coin = coinsQueue.Dequeue();
                coin.SetActive(true);

                //move coin to the collected coin pos
                var rectTransform = coin.GetComponent<RectTransform>();
                rectTransform.localPosition = collectedCoinPosition + new Vector3(Random.Range(-spread, spread), 0f, 0f);

                //animate coin to target position
                float duration = Random.Range(minAnimDuration, maxAnimDuration);
                SoundManager.instance.PlaySFXSound("Star");
                coin.transform.DOMove(targetPosition, duration)
                .SetEase(easeType)/*.OnUpdate(() => { coin.transform.DOScale(0.08f, 1); })*/
                .OnComplete(() =>
                {
                    //executes whenever coin reach target position
                    coin.SetActive(false);
                    coinsQueue.Enqueue(coin);
                    CanvasManager.instance.UpdateScoreText(score);
                    //coinUIText.text = Coins.ToString();
                    // Coins++;
                    //CurrencyManager.Instance.PlusCurrencyValue("Coins", 1);
                });
            }
        }
        yield return null;
    }

    public void AddCoins(Vector3 collectedCoinPosition, int amount, int score)
    {
        print("animated");
        StartCoroutine(Animate(WordPointToCanvasPoint(Camera.main, collectedCoinPosition, canvasRect), amount,score));
    }

    public Vector2 WordPointToCanvasPoint(Camera camera, Vector3 worldPoint, RectTransform canvasRect)
    {
        Vector2 viewportPosition = camera.WorldToViewportPoint(worldPoint);
        Vector2 screenPosition = new Vector2(((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));
        return screenPosition;
    }
    public void AnimateStar(Vector3 position)
    {
        var myObj = Instantiate(objectInWorld, position, Quaternion.identity);
        myObj.transform.DOJump(new Vector3(myObj.transform.position.x, myObj.transform.position.y + .25f, myObj.transform.position.z), 1, 1, .15f).SetEase(Ease.OutBounce);
        Destroy(myObj, .15f);
    }
    //public void AddCoins(int amount)
    //{
    //    //Coins += amount;
    //    //PlayerPrefs.SetInt("Coin", Coins);
    //    //DataBase.Instance.cash += Coins;
    //    //coinUIText.text = DataBase.Instance.cash.ToString();
    //    //DataBase.Instance.WriteData();

    //}
}
