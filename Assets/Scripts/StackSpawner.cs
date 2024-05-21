using System;
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
    void SpawnStacks()
    {
        for (int i = 0; i < _count; i++)
        {
            int spawnPosIndex = i + 1; // Because when use this extension "GetComponentsInChildren" it adds this transform itself to the array too
            PickableStack cloneStack = Instantiate(stackPrefab, spawnPoints[spawnPosIndex].position, Quaternion.identity);
            stacks.Add(cloneStack.transform);

            foreach (var stack in stacks)
            {
                stack.DOScale(1, .3f).SetEase(Ease.OutBounce);
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
