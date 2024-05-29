using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class StackSpawner : MonoSingleton<StackSpawner>
{
    [Header("References")]
    [SerializeField] PickableStack stackPrefab;
    [SerializeField] HexagonController hexagonPrefab;
    [SerializeField] Transform spawnPoint;
    [Header("References")]
    [SerializeField] List<int> scoreTresholds;

    [Header("Debug")]
    [SerializeField] int maxColorVarierty;
    [SerializeField] int tresholdIndex;
    [Tooltip("Only for demonstration, do not modify this region")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] List<Transform> stacks = new List<Transform>();
    const int _count = 3;

    protected override void Awake()
    {
        base.Awake();
        maxColorVarierty = 3;
        spawnPoints = GetComponentsInChildren<Transform>();

        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    spawnPoints.Add(transform.GetChild(i));
        //}
    }

    private void Start()
    {
        SpawnStacks();

        InputManager.instance.StackPlacedOnGridEvent += OnStackPlaced;
        CanvasManager.instance.ScoreUpdatedEvent += OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        if (score > scoreTresholds[tresholdIndex])
        {
            // color enum count, -1 because exlude NONE satus
            if (Enum.GetNames(typeof(ColorInfo.ColorEnum)).Length - 1 > maxColorVarierty)
            {
                if (tresholdIndex < scoreTresholds.Count - 1)
                    tresholdIndex++;

                maxColorVarierty++;
            }
        }

        if (maxColorVarierty == 5) CanvasManager.instance.ScoreUpdatedEvent -= OnScoreUpdated;
    }

    private void OnStackPlaced(PickableStack stackToRemove)
    {
        stacks.Remove(stackToRemove.transform);

        if (CheckCanSpawn()) SpawnStacks();
    }

    bool CheckCanSpawn()
    {
        return stacks.Count == 0;
    }
    [ContextMenu("SpawnStacks")]
    void SpawnStacks()
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < _count; i++)
        {
            int spawnPosIndex = i + 1; // Because when use this extension "GetComponentsInChildren" it adds this transform itself to the array too
            //PickableStack cloneStack = Instantiate(stackPrefab, spawnPoints[spawnPosIndex].position, Quaternion.identity);
            PickableStack cloneStack = Instantiate(stackPrefab, spawnPoint.position, Quaternion.identity);
            cloneStack._startPos = spawnPoints[spawnPosIndex].position;
            stacks.Add(cloneStack.transform);

            //foreach (var stack in stacks)
            //{
            //    sequence.Join(stack.DOScale(1, .2f).SetEase(Ease.OutBounce));
            //}

            for (int j = 0; j < stacks.Count; j++)
            {
                sequence.Prepend(stacks[j].DOMove(spawnPoints[j+1].position, .2f).SetEase(Ease.OutBack));
            }
        }

        SpawnHex();
    }
    void SpawnHex()
    {
        for (int s = 0; s < stacks.Count; s++)
        {
            int randomHexCount = GetRandomAmount(1, 5);
            List<int> sequenceList = UniqueSequenceGenerator.instance.GenerateUniqueSequence(1, maxColorVarierty + 1, randomHexCount);

            for (int i = 0; i < randomHexCount; i++)
            {
                //TextureInfo.TextureEnum texture = GetRandomTexture(randomHexCount, i);
                TextureInfo.TextureEnum texture = (TextureInfo.TextureEnum)sequenceList[i];
                //Material mat = new Material(GridManager.instance.BlockMaterial);
                Material mat = new(GridManager.instance.BlockMaterial);
                //mat.color = GridManager.instance.texturePack.HexagonTextureInfo[GridManager.instance.texturePack.GetTextureEnumIndex(color)].HexColor;
                mat.SetTexture("_MainTex", GridManager.instance.texturePack.HexagonTextureInfo[GridManager.instance.texturePack.GetTextureEnumIndex(texture)].texture);
                HexagonController hex = Instantiate(hexagonPrefab, Vector3.zero, Quaternion.identity, stacks[s]);

                float verticalPos = i * GridManager.instance.VERTICAL_PLACEMENT_OFFSET;
                Vector3 spawnPos = new Vector3(0, verticalPos, 0);
                hex.transform.localPosition = spawnPos;
                hex.Initialize(texture, mat);
            }
        }
    }

    public void RespawnStack()
    {
        foreach (var item in stacks)
        {
            Destroy(item.gameObject);
        }
        stacks.Clear();

        SpawnStacks();
    }

    #region GETTERS

    TextureInfo.TextureEnum GetRandomTexture(int listCount, int index)
    {
        List<int> sequenceList = UniqueSequenceGenerator.instance.GenerateUniqueSequence(1, maxColorVarierty + 1, listCount);
        //int randomIndex = Random.Range(1, maxColorVarierty + 1);
        return (TextureInfo.TextureEnum)sequenceList[index];
        //return (TextureInfo.TextureEnum)randomIndex;
    }
    int GetRandomAmount(int min, int max)
    {
        return Random.Range(min, max);
    }

    #endregion
}

[Serializable]
public class ContentInfo
{
    public ColorInfo.ColorEnum color;
    public int amount;
}
