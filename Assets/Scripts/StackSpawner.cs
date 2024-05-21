using System;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using static UnityEditor.Progress;
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
    [SerializeField] List<int> generatedTextures;
    [SerializeField] List<TextureInfo.TextureEnum> myTexturesList;
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
        //TextureInfo.TextureEnum randomTexture = GetRandomTexture();
        //int typeOfSpawn = Random.Range(0, 2);
        //if (typeOfSpawn == 0)
        //{
        //    randomTexture = GetRandomTexture();
        //}
        //else if (typeOfSpawn == 1)
        //{

        //}
        for (int s = 0; s < stacks.Count; s++)
        {
            //generatedTextures.Clear();
            int randomHexCount = GetRandomAmount(1, 4);
            randomHex = randomHexCount;
            TextureInfo.TextureEnum texture = GetVariedTexture();
            
            for (int i = 0; i < randomHexCount; i++)
            {
                //Material mat = new Material(GridManager.instance.BlockMaterial);
                Material mat = new(GridManager.instance.BlockMaterial);
                //mat.color = GridManager.instance.texturePack.HexagonTextureInfo[GridManager.instance.texturePack.GetTextureEnumIndex(color)].HexColor;
                mat.SetTexture("_MainTex", GridManager.instance.texturePack.HexagonTextureInfo[GridManager.instance.texturePack.GetTextureEnumIndex(myTexturesList[i])].texture);
                HexagonController hex = Instantiate(hexagonPrefab, Vector3.zero, Quaternion.identity, stacks[s]);

                float verticalPos = i * GridManager.instance.VERTICAL_PLACEMENT_OFFSET;
                Vector3 spawnPos = new Vector3(0, verticalPos, 0);
                hex.transform.localPosition = spawnPos;
                hex.Initialize(myTexturesList[i], mat);
            }
        }
    }
    int randomHex;
    bool CheckValidSpawnTexture(List<TextureInfo.TextureEnum> textureEnums)
    {
        if (textureEnums.Count <= 1)
            return true;
        else
        {
            for (int i = 0; i < textureEnums.Count; i++)
            {
                //if (textureEnums[i+1])
                if (textureEnums[i] == textureEnums[i + 1])
                {
                    return true;
                }
                else if (textureEnums[i] != textureEnums[i + 1])
                {
                    return false;
                }
            }
        }
        return true;
    }
    #region GETTERS

    TextureInfo.TextureEnum GetRandomTexture()
    {
        int randomIndex = Random.Range(1, maxColorVarierty + 1);

        return (TextureInfo.TextureEnum)randomIndex;
    }
    TextureInfo.TextureEnum GetVariedTexture()
    {
        UniqueSequenceGenerator.instance.GenerateUniqueSequence(1, maxColorVarierty + 1, 4);

        foreach (var item in UniqueSequenceGenerator.instance.sequence)
        {
            myTexturesList.Add((TextureInfo.TextureEnum)item);
            return (TextureInfo.TextureEnum)item;
        }
        return (TextureInfo.TextureEnum)0;
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
