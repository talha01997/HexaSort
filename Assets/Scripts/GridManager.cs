using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Unity.VisualScripting;

public class GridManager : MonoSingleton<GridManager>
{
    public enum TransferType
    {
        Send, Take
    };

    [Header("References")]
    [SerializeField] GameObject CellPrefab;
    [SerializeField] HexagonController hexagonBlockPrefab;
    public Material BlockMaterial, lockedMaterial, cellMat;
    public TexturePack texturePack;
    public LayerMask CellLayer;

    [Header("Configuration")]
    [SerializeField] int _gridSizeX = 0;
    [SerializeField] int _gridSizeY = 0;

    [Header("Debug")]
    public CellData[,] GridPlan;
    private const float CELL_HORIZONTAL_OFFSET = 0.85f;
    private const float CELL_VERTICAL_OFFSET = 0.95f;
    public float VERTICAL_PLACEMENT_OFFSET = 0.2f;
    [SerializeField] List<StartInfo> startInfos = new List<StartInfo>();
    public List<CellController> scoreLockedCells, adLockedCells;

    [Space(125)]
    public GridInfoAssigner CurrentGridInfo;

    public void Start()
    {
        startInfos = new();
     
        for (int i = 0; i < InfoManager.instance.GetCurrentInfo().startInfos.Count; i++)
        {
            startInfos.Add(InfoManager.instance.GetCurrentInfo().startInfos[i]);
        }
        CurrentGridInfo = InfoManager.instance.GetCurrentInfo();
        GenerateGrid();
    }
    public void GenerateGrid()
    {
        if (GridPlan != null)
        {
            DestroyPreviousGrid();

        }

        GridPlan = new CellData[_gridSizeX, _gridSizeY];

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                GridPlan[x, y] = new CellData();
                

                int index;
                GridPlan[x, y].PosX = x;
                GridPlan[x, y].PosY = y;

                if (ContainsInStartInfo(x, y, out index))
                {
                    GridPlan[x, y].isOpen = startInfos[index].isOpen;
                    GridPlan[x, y].isLocked = startInfos[index].isLocked;
                    GridPlan[x, y].unlockWithAd = startInfos[index].unlockWithAd;
                    GridPlan[x, y].unlockWithScore = startInfos[index].unlockWithScore;
                    GridPlan[x, y].scoreToUnlock = startInfos[index].scoreToUnlock;

                    List<TextureInfo.TextureEnum> CE = new List<TextureInfo.TextureEnum>();
                    
                    for (int i = 0; i < startInfos[index].ContentInfo.Count; i++)
                    {
                        CE.Add(startInfos[index].ContentInfo[i]);
                    }
                    GridPlan[x, y].CellContentList = CE;
                }
                else
                {
                    GridPlan[x, y].isOpen = true;
                    GridPlan[x, y].CellContentList = new();
                }

                if (GridPlan[x, y].isOpen)
                {
                    GameObject cloneCellGO = Instantiate(CellPrefab, Vector3.zero, CellPrefab.transform.rotation, transform);
                    
                    cloneCellGO.transform.position =
                        new Vector3(x * CELL_HORIZONTAL_OFFSET, 0,
                        -(((x % 2) * (CELL_VERTICAL_OFFSET / 2)) + y * CELL_VERTICAL_OFFSET));

                    GridPlan[x, y].CellObject = cloneCellGO;

                    cloneCellGO.name = x.ToString() + "," + y.ToString();
                    CellController cellController = cloneCellGO.GetComponent<CellController>();
                    cellController.SetCoordinates(x, y);
                    if (GridPlan[x, y].isLocked)
                    {
                        LockCells(GridPlan, x, y, cellController);
                    }
                }
                // locking codeeee
                //if (GridPlan[x, y].isLocked)
                //{
                //    GameObject cloneCellGO = Instantiate(CellPrefab, Vector3.zero, CellPrefab.transform.rotation, transform);

                //    cloneCellGO.transform.position =
                //        new Vector3(x * CELL_HORIZONTAL_OFFSET, 0,
                //        -(((x % 2) * (CELL_VERTICAL_OFFSET / 2)) + y * CELL_VERTICAL_OFFSET));

                //    GridPlan[x, y].CellObject = cloneCellGO;

                //    cloneCellGO.name = x.ToString() + "," + y.ToString();

                //    CellController cellController = cloneCellGO.GetComponent<CellController>();
                //    cellController.SetCoordinates(x, y);
                //    cellController.opaqueMesh.GetComponent<MeshRenderer>().material = lockedMaterial;
                //    cellController.isLocked = true;
                //    cellController.scoreToUnlock = GridPlan[x, y].scoreToUnlock;
                //    LockCells(GridPlan, x, y, cellController);
                //}

                if (GridPlan[x, y].CellContentList.Count != 0 && GridPlan[x, y].isOpen)
                {
                    CellController cellParent = GridPlan[x, y].CellObject.GetComponent<CellController>();
                    for (int i = 0; i < GridPlan[x, y].CellContentList.Count; i++)
                    {
                        TextureInfo.TextureEnum texture = GridPlan[x, y].CellContentList[i];
                        Material mat = new(BlockMaterial);
                        
                        //mat.color = texturePack.HexagonTextureInfo[texturePack.GetTextureEnumIndex(texture)].HexColor;
                        mat.SetTexture("_MainTex", texturePack.HexagonTextureInfo[texturePack.GetTextureEnumIndex(texture)].texture);
                        SpawnHexagon(i,
                           cellParent.transform.position,
                           cellParent.HexStackParent,
                            mat,
                            texture);
                    }

                    cellParent.SetOccupied(true);
                }
                if (GridPlan[x, y].isOpen)
                    GridPlan[x, y].CellObject.GetComponent<CellController>().Starter();
            }
        }
    }
    private void DestroyPreviousGrid()
    {
        for (int x = 0; x < GridPlan.GetLength(0); x++)
        {
            for (int y = 0; y < GridPlan.GetLength(1); y++)
            {
                Destroy(GridPlan[x, y].CellObject);
            }
        }
    }
    public void SpawnHexagon(int index, Vector3 gridPos, Transform parent, Material mat, TextureInfo.TextureEnum texture)
    {
        float verticalPos = (index + 1) * VERTICAL_PLACEMENT_OFFSET;
        Vector3 spawnPos = gridPos + new Vector3(0, verticalPos, 0);
        HexagonController cloneBlock = Instantiate(hexagonBlockPrefab, spawnPos, Quaternion.identity, parent);
        cloneBlock.Initialize(texture, mat);
    }

    public void LockCells(CellData[,] cellData, int x, int y, CellController cellController)
    {
        cellController.opaqueMesh.GetComponent<MeshRenderer>().material = lockedMaterial;
        cellController.isLocked = true;
        cellController.isOccupied = true;
        if (cellData[x, y].unlockWithAd)
        {
            cellController.lockedWithAd = cellData[x, y].unlockWithAd;
            cellController.adUnlock.SetActive(true);
            //cellController.AddComponent<BoxCollider>();
        }
        else if (cellData[x, y].unlockWithScore)
        {
            cellController.scoreToUnlock = cellData[x, y].scoreToUnlock;
            scoreLockedCells.Add(cellController);
            cellController.scoreUnlock.SetActive(true);
            cellController.scoreTxt.text = cellData[x, y].scoreToUnlock.ToString();
        }
    }

    public void CheckLockedCells(int score)
    {
        foreach (CellController cell in scoreLockedCells.ToList())
        {
            if (score >= cell.scoreToUnlock)
            {
                cell.scoreUnlock.SetActive(false);
                cell.isLocked = false;
                cell.isOccupied = false;
                cell.opaqueMesh.GetComponent<MeshRenderer>().material = cellMat;
                GridPlan[(int)cell.GetCoordinates().x, (int)cell.GetCoordinates().y].CellContentList = new();
                scoreLockedCells.Remove(cell);
                print(GridPlan);
            }
        }
    }
    bool ContainsInStartInfo(int x, int y, out int index)
    {
        for (int i = 0; i < startInfos.Count; i++)
        {
            StartInfo startInfo = startInfos[i];
            if (startInfo.Coordinates.x == x && startInfo.Coordinates.y == y)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public List<Vector2> GetNeighboursCoordinates(Vector2 controlGridCoordinate)
    {
        List<Vector2> neighbourList = new List<Vector2>();

        bool isEvenRow = ((int)controlGridCoordinate.x % 2 == 0);

        Vector2[] offsetsEvenRow = new Vector2[] {
            new Vector2(0, -1), // Top
            new Vector2(+1, -1), // Top Right
            new Vector2(+1, 0), // Bottom Right
            new Vector2(0, +1), // Bottom
            new Vector2(-1, 0), // Bottom Left
            new Vector2(-1, -1) // Top Left
            };

        Vector2[] offsetsOddRow = new Vector2[] {
            new Vector2(0, -1), // Top
            new Vector2(+1, 0), // Top Right
            new Vector2(+1, +1), // Bottom Right
            new Vector2(0, +1), // Bottom
            new Vector2(-1, +1), // Bottom Left
            new Vector2(-1, 0) // Top Left
            };

        Vector2[] offsets = isEvenRow ? offsetsEvenRow : offsetsOddRow;

        foreach (Vector2 offset in offsets)
        {
            Vector2 neighbour = new Vector2(controlGridCoordinate.x + offset.x, controlGridCoordinate.y + offset.y);

            if (IsCoordinateValidAndOpen(neighbour))
            {
                neighbourList.Add(neighbour);
            }
        }

        bool IsCoordinateValidAndOpen(Vector2 coord)
        {
            bool isValid = coord.x >= 0 && coord.x < GridPlan.GetLength(0) &&
                           coord.y >= 0 && coord.y < GridPlan.GetLength(1);

            return isValid && GridPlan[(int)coord.x, (int)coord.y].isOpen && !GridPlan[(int)coord.x, (int)coord.y].CellObject.GetComponent<CellController>().IsAction;
        }

        return neighbourList;
    }


    [System.Serializable]
    public class StartInfo
    {
        public Vector2Int Coordinates;
        public List<TextureInfo.TextureEnum> ContentInfo;
        public bool isOpen;
        public bool isLocked, unlockWithAd, unlockWithScore;
        public int scoreToUnlock;
    }
}