using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CellController : MonoBehaviour
{
    [Header("References")]
    public Transform HexStackParent;
    public GameObject opaqueMesh;
    public GameObject transparentMesh;
    public GameObject scoreUnlock, adUnlock;
    public TextMeshPro scoreTxt;
    [Header("Debug")]
    public bool IsAction;
    public bool isOccupied;
    public bool isOpen = true;
    public bool isLocked, lockedWithAd;
    public bool canClick, canHammer;
    public int scoreToUnlock;
    bool blastCompleted, canBlast;
    [SerializeField] Vector2 _coordinates = Vector2.zero;
    [SerializeField] Vector3 hammerOffset;
    [Header("Hexagons Related")]
    public List<HexagonController> hexagons = new List<HexagonController>();
    public List<TextureInfo.TextureEnum> contentInfo;

    private void OnEnable()
    {
        GameManager.instance.LevelEndedEvent += LevelEnd;
        UiManager.instance.HammerOn += HammerOn;
        UiManager.instance.HammerOff += HammerOff;
    }
    private void OnDisable()
    {
        GameManager.instance.LevelEndedEvent -= LevelEnd;
        UiManager.instance.HammerOn -= HammerOn;
        UiManager.instance.HammerOff -= HammerOff;
    }

    public void Starter()
    {
        SetHexagonLists();
    }

    private void OnMouseUp()
    {
        if (lockedWithAd)
        {
            adUnlock.SetActive(false);
            isLocked = false;
            isOccupied = false;
            lockedWithAd = false;
            opaqueMesh.GetComponent<MeshRenderer>().material = GridManager.instance.cellMat;
            Destroy(GetComponent<BoxCollider>());
        }
        if (canHammer && hexagons.Count>0)
        {
            GameManager.instance.hammer.DOMove(transform.position+hammerOffset, .1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                GameManager.instance.hammer.gameObject.SetActive(true);
                GameManager.instance.hammer.DOScale(1, .35f).SetDelay(.15f);
                GameManager.instance.hammer.DORotate(new Vector3(0, 90, 80), 1).SetDelay(.45f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    GameManager.instance.hammer.DOScale(.1f, .35f).OnComplete(() =>
                    {
                        GameManager.instance.hammer.DORotate(new Vector3(0, 90, -20), .1f);
                        GameManager.instance.hammer.gameObject.SetActive(false);
                        HammerSelectedCell();
                    });
                    
                });
            });
        }
    }
    void ClearList()
    {
        hexagons.Clear();
    }
    void HammerSelectedCell()
    {
        print("hammered");
        List<HexagonController> selectedHexList = new List<HexagonController>();
        selectedHexList.Add(hexagons[hexagons.Count - 1]);

        for (int i = hexagons.Count - 2; i >= 0; i--)
        {
            selectedHexList.Add(hexagons[i]);
        }

        //Update GridPlan and CellData
        for (int i = 0; i < selectedHexList.Count; i++)
        {
            hexagons.RemoveAt(hexagons.Count - 1);
            CellData ThisGridClass = GridManager.instance.GridPlan[(int)GetCoordinates().x, (int)GetCoordinates().y];
            ThisGridClass.CellContentList.RemoveAt(ThisGridClass.CellContentList.Count - 1);
        }

        foreach (var hex in selectedHexList)
        {
            hex.transform.DOScale(.1f, .25f).OnComplete(() =>
            {
                hex.DestroySelf();
                isOccupied = false;
                UiManager.instance.DeActivateHammer();
            });
        }
        hexagons.Clear();
        //SetHexagonLists();
        //IsAction = false;
    }
    public IEnumerator ControlTransfer(float StartControlDelay)
    {
        //If There is Any Hex
        if (hexagons.Count > 0)
        {
            yield return new WaitForSeconds(StartControlDelay);

            //If There is opportunity to Blast
            if (IsThereBlast() && canBlast)
            {
                //Create Blast Hex List
                List<HexagonController> selectedHexList = new List<HexagonController>();
                TextureInfo.TextureEnum topHexColor = hexagons[hexagons.Count - 1].GetTexture();
                selectedHexList.Add(hexagons[hexagons.Count - 1]);

                for (int i = hexagons.Count - 2; i >= 0; i--)
                {
                    if (hexagons[i].GetTexture() == topHexColor)
                    {
                        selectedHexList.Add(hexagons[i]);
                    }
                    else
                    {
                        break;
                    }
                }

                //Update GridPlan and CellData
                for (int i = 0; i < selectedHexList.Count; i++)
                {
                    hexagons.RemoveAt(hexagons.Count - 1);
                    CellData ThisGridClass = GridManager.instance.GridPlan[(int)GetCoordinates().x, (int)GetCoordinates().y];
                    ThisGridClass.CellContentList.RemoveAt(ThisGridClass.CellContentList.Count - 1);
                }

                //Blast Rope Group
                StartCoroutine(BlastSelectedHexList(selectedHexList));

                //Wait Blast Complete Time
                //yield return new WaitForSeconds(0.36f);
                yield return new WaitUntil(() => blastCompleted);

                SetOccupied(hexagons.Count > 0);
                StartCoroutine(ControlTransfer(0));

                GameManager.instance.CheckFailStatus();
            }
            //If No Blast
            else
            {
                TextureInfo.TextureEnum TopRopeColor = hexagons[hexagons.Count - 1].GetTexture();
                GridManager.TransferType SendOrTake = GridManager.TransferType.Take;
                List<Vector2> NeighboursCoordinateList = GridManager.instance.GetNeighboursCoordinates(GetCoordinates());
                List<Vector2> SelectedNeighbours = new List<Vector2>();
                Vector2 SelectedNeighbour = Vector2.zero;

                //Control All Finded Neighbours Cells
                for (int i = 0; i < NeighboursCoordinateList.Count; i++)
                {
                    int NeighbourPosX = (int)NeighboursCoordinateList[i].x;
                    int NeighbourPosY = (int)NeighboursCoordinateList[i].y;
                    CellData ControlNeighbourGrid = GridManager.instance.GridPlan[NeighbourPosX, NeighbourPosY];
                    CellController ControlNeighbourGridPart = GridManager.instance.GridPlan[NeighbourPosX, NeighbourPosY].CellObject.GetComponent<CellController>();

                    //If Cell Open And Have a Hexagon
                    if (ControlNeighbourGrid.isOpen && ControlNeighbourGrid.CellContentList.Count > 0)
                    {
                        //If Hexagon Colors Matched
                        if (TopRopeColor == ControlNeighbourGridPart.hexagons[ControlNeighbourGridPart.hexagons.Count - 1].GetTexture())
                        {
                            SelectedNeighbours.Add(new Vector2(NeighbourPosX, NeighbourPosY));

                            //StartCoroutine(ControlTransfer(0));
                        }
                    }
                }

                //If There Is Possible Moves
                if (SelectedNeighbours.Count > 0)
                {
                    //Set Selected Neighbours To First Finded
                    //print(SelectedNeighbour + " default selected");
                    if(SelectedNeighbours.Count == 1)
                    {
                        SelectedNeighbour = SelectedNeighbours[0];
                        //Check Selected Neighbours Pure Status
                        for (int i = 0; i < SelectedNeighbours.Count; i++)
                        {
                            print("count 1");
                            if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() && !IsPure())
                            {
                                SendOrTake = GridManager.TransferType.Send;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                            else if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() == false && !IsPure())
                            {
                                SendOrTake = GridManager.TransferType.Send;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                            else if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() == false && IsPure())
                            {
                                SendOrTake = GridManager.TransferType.Take;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                            else if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() && IsPure())
                            {
                                SendOrTake = GridManager.TransferType.Take;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                        }
                    }
                    else if (SelectedNeighbours.Count > 1)
                    {
                        foreach (var neighbour in SelectedNeighbours)
                        {
                            if (GridManager.instance.GridPlan[(int)neighbour.x, (int)neighbour.y].CellObject.GetComponent<CellController>().IsPure() == false)
                            {
                                SelectedNeighbour = neighbour;
                                break;
                            }
                            else
                            {
                                SelectedNeighbour = SelectedNeighbours[0];
                            }
                        }
                        //SelectedNeighbour = SelectedNeighbours[0];
                        //Check Selected Neighbours Pure Status
                        for (int i = 0; i < SelectedNeighbours.Count; i++)
                        {
                            print("count more than one");
                            if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure()==false && (!IsPure() || IsPure()))
                            {
                                SendOrTake = GridManager.TransferType.Take;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                            else if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() && IsPure())
                            {
                                SendOrTake = GridManager.TransferType.Take;
                                SelectedNeighbour = SelectedNeighbours[i];
                                break;
                            }
                            //else if (GridManager.instance.GridPlan[(int)SelectedNeighbours[i].x, (int)SelectedNeighbours[i].y].CellObject.GetComponent<CellController>().IsPure() == false && IsPure())
                            //{
                            //    SendOrTake = GridManager.TransferType.Take;
                            //    SelectedNeighbour = SelectedNeighbours[i];
                            //    break;
                            //}
                        }
                    }

                    //If Transfer Type is "Take" and There is Other Color Rope, Control Second Color Transfer is Possible
                    if (SendOrTake == GridManager.TransferType.Take)
                    {
                        /*
                        bool IsThereOtherColor = false;

                        for (int i = 0; i < NeighboursCoordinateList.Count; i++)
                        {
                            int NeighbourPosX = (int)NeighboursCoordinateList[i].x;
                            int NeighbourPosY = (int)NeighboursCoordinateList[i].y;
                            CellData ControlNeighbourGrid = GridManager.instance.I.GridPlan[NeighbourPosX, NeighbourPosY];
                            GridPart ControlNeighbourGridPart = GridManager.instance.I.GridPlan[NeighbourPosX, NeighbourPosY].CellObject.GetComponent<GridPart>();

                            //If Grid Open And Have a Rope
                            if (ControlNeighbourGrid.IsGridOpen && ControlNeighbourGrid.CellContentList.Count > 0)
                            {
                                //If Rope Colors Matched
                                if (topHexColor == ControlNeighbourGridPart.hexagons[ControlNeighbourGridPart.hexagons.Count - 1].GetComponent<RopePart>().HexColor)
                                {
                                    SelectedNeighbours.Add(new Vector2(NeighbourPosX, NeighbourPosY));
                                }
                            }
                        }
                        */
                    }

                    CellController SelectedGridPart = GridManager.instance.GridPlan[(int)SelectedNeighbour.x, (int)SelectedNeighbour.y].CellObject.GetComponent<CellController>();
                    CellData SelectedGridClass = GridManager.instance.GridPlan[(int)SelectedNeighbour.x, (int)SelectedNeighbour.y];
                    CellData ThisGridClass = GridManager.instance.GridPlan[(int)GetCoordinates().x, (int)GetCoordinates().y];

                    //Change Action Situations
                    IsAction = true;
                    SelectedGridPart.IsAction = true;


                    //Send
                    if (SendOrTake == GridManager.TransferType.Send)
                    {
                        //Create Send Rope List
                        List<HexagonController> WillSendRopeList = new List<HexagonController>();
                        for (int i = hexagons.Count - 1; i >= 0; i--)
                        {
                            if (hexagons[i].GetTexture() == TopRopeColor)
                            {
                                WillSendRopeList.Add(hexagons[i]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        //Update Grid Classes
                        for (int i = 0; i < WillSendRopeList.Count; i++)
                        {
                            SelectedGridClass.CellContentList.Add(ThisGridClass.CellContentList[ThisGridClass.CellContentList.Count - 1]);
                            ThisGridClass.CellContentList.RemoveAt(ThisGridClass.CellContentList.Count - 1);
                        }

                        //Move Hex Objects
                        for (int i = 0; i < WillSendRopeList.Count; i++)
                        {
                            WillSendRopeList[i].transform.SetParent(SelectedGridPart.HexStackParent);
                            //WillSendRopeList[i].transform.DOLocalJump(new Vector3(0, SelectedGridPart.hexagons.Count * GridManager.instance.VERTICAL_PLACEMENT_OFFSET, 0),1,1, 0.3f);

                            print("moved hex");
                            WillSendRopeList[i].transform.DOLocalRotate(new Vector3(180, 0, 0), .3f);
                            WillSendRopeList[i].transform.DOLocalMove(new Vector3(0, SelectedGridPart.hexagons.Count * GridManager.instance.VERTICAL_PLACEMENT_OFFSET, 0), 0.3f);
                            Vibration.VibratePop();
                            SoundManager.instance.PlaySFXSound("Bounce");
                            SelectedGridPart.hexagons.Add(WillSendRopeList[i]);
                            hexagons.RemoveAt(hexagons.Count - 1);
                            yield return new WaitForSeconds(0.06f);
                        }
                        
                        //If There Is No Hex In This Cell Set Occupation Status
                        SetOccupied(hexagons.Count > 0);
                    }
                    //Take
                    else if (SendOrTake == GridManager.TransferType.Take)
                    {
                        //Create Take Rope List
                        List<HexagonController> WillTakeRopeList = new List<HexagonController>();
                        for (int i = SelectedGridPart.hexagons.Count - 1; i >= 0; i--)
                        {
                            if (SelectedGridPart.hexagons[i].GetTexture() == TopRopeColor)
                            {
                                WillTakeRopeList.Add(SelectedGridPart.hexagons[i]);
                            }
                            else
                            {
                                break;
                            }
                        }

                        //Update Grid Classes
                        for (int i = 0; i < WillTakeRopeList.Count; i++)
                        {
                            ThisGridClass.CellContentList.Add(SelectedGridClass.CellContentList[SelectedGridClass.CellContentList.Count - 1]);
                            SelectedGridClass.CellContentList.RemoveAt(SelectedGridClass.CellContentList.Count - 1);
                        }

                        //Move Rope Objects
                        for (int i = 0; i < WillTakeRopeList.Count; i++)
                        {
                            WillTakeRopeList[i].transform.SetParent(HexStackParent);
                            //WillTakeRopeList[i].transform.DOLocalJump(new Vector3(0, hexagons.Count * GridManager.instance.VERTICAL_PLACEMENT_OFFSET, 0),1,1, 0.3f);
                            //print("moved rope");
                            WillTakeRopeList[i].transform.DOLocalRotate(new Vector3(180, 0, 0), .3f);
                            WillTakeRopeList[i].transform.DOLocalMove(new Vector3(0, hexagons.Count * GridManager.instance.VERTICAL_PLACEMENT_OFFSET, 0), 0.3f);
                            Vibration.VibratePop();
                            SoundManager.instance.PlaySFXSound("Bounce");
                            hexagons.Add(WillTakeRopeList[i]);
                            
                            SelectedGridPart.hexagons.RemoveAt(SelectedGridPart.hexagons.Count - 1);
                            yield return new WaitForSeconds(0.06f);
                        }
                        SetOccupied(hexagons.Count > 0);
                    }

                    

                    //Wait Transfer Complete Time
                    yield return new WaitForSeconds(0.36f);

                    StartCoroutine(ControlTransfer(0));
                    StartCoroutine(SelectedGridPart.ControlTransfer(0));
                }
                else
                {
                    IsAction = false;
                    GameManager.instance.CheckFailStatus();
                }
            }
        }
        else
        {
            IsAction = false;
            isOccupied = false;
            GameManager.instance.CheckFailStatus();
        }
        GameManager.instance.CheckFailStatus();
    }
    //private void Update()
    //{
    //    if (Input.GetMouseButtonUp(0) && canClick)
    //    {
    //        adUnlock.SetActive(false);
    //        isLocked = false;
    //        Destroy(GetComponent<BoxCollider>());
    //    }
    //}
    void LookAtCam()
    {
        //scoreUnlock.transform.LookAt(Camera.main.transform);
        for (int i = 0; i < scoreUnlock.transform.childCount; i++)
        {
            scoreUnlock.transform.GetChild(i).LookAt(Camera.main.transform);
        }
    }

    void HammerOn()
    {
        canHammer = true;
        if (hexagons.Count > 0)
        {
            HexStackParent.DOShakeRotation(.35f, new Vector3(0, 15, 0), 5, 90, true).SetLoops(-1, LoopType.Yoyo).SetId("CellShake");
        }
    }
    void HammerOff()
    {
        canHammer = false;
        if (hexagons.Count > 0)
        {
            DOTween.Kill("CellShake");
            //HexStackParent.DOShakeRotation(.35f, new Vector3(0, 15, 0), 5, 90, true).SetLoops(-1, LoopType.Yoyo).SetId("CellShake");
        }
    }

    void LevelEnd()
    {
        print("level end test");
        //HexStackParent.transform.localPosition = new Vector3(0, -10, 0);
        HexStackParent.DOLocalMove(new Vector3(0, -5, 0), 1f);
    }
    public bool IsThereBlast()
    {
        bool performBlast = false;

        
        if (IsPure())
        {
            if (hexagons.Count >= GameManager.instance.BlastObjectiveAmount)
            {
                performBlast = true;
                canBlast = true;
            }

        }
        if (!IsPure())
        {
            TextureInfo.TextureEnum MyTopRopeColor = hexagons[hexagons.Count - 1].GetTexture();
            List<Vector2> NeighboursCoordinateList = GridManager.instance.GetNeighboursCoordinates(GetCoordinates());

            //Control All Finded Neighbours Cells
            for (int i = 0; i < NeighboursCoordinateList.Count; i++)
            {
                int NeighbourPosX = (int)NeighboursCoordinateList[i].x;
                int NeighbourPosY = (int)NeighboursCoordinateList[i].y;
                CellData ControlNeighbourGrid = GridManager.instance.GridPlan[NeighbourPosX, NeighbourPosY];
                CellController ControlNeighbourGridPart = GridManager.instance.GridPlan[NeighbourPosX, NeighbourPosY].CellObject.GetComponent<CellController>();

                //If Cell Open And Have a Hexagon
                if (ControlNeighbourGrid.isOpen && ControlNeighbourGrid.CellContentList.Count > 0)
                {
                    //foreach (var neighbourCell in ControlNeighbourGridPart.hexagons)
                    //{
                    //    //if(MyTopRopeColor)
                    //}
                    //If Hexagon Colors Matched
                    if (MyTopRopeColor == ControlNeighbourGridPart.hexagons[ControlNeighbourGridPart.hexagons.Count - 1].GetTexture())
                    {
                        //SelectedNeighbours.Add(new Vector2(NeighbourPosX, NeighbourPosY));
                        canBlast = false;
                        return false;
                        //StartCoroutine(ControlTransfer(0));
                    }
                }
            }
        }
        
        if (hexagons.Count > 1)
        {
            int matchCount = 0;
            TextureInfo.TextureEnum TopRopeColor = hexagons[hexagons.Count - 1].GetTexture();
            for (int i = hexagons.Count - 2; i >= 0; i--)
            {
                if (hexagons[i].GetTexture() == TopRopeColor)
                {
                    matchCount++;
                }
                else
                {
                    break;
                }
            }

            if (matchCount >= GameManager.instance.BlastObjectiveAmount)
            {
                canBlast = true;
                return true;
            }
            canBlast = false;
            return false;
        }

        return performBlast;
    }
    bool IsPure()
    {
        TextureInfo.TextureEnum TopRopeColor = hexagons[hexagons.Count - 1].GetTexture();
        for (int i = hexagons.Count - 1; i >= 0; i--)
        {
            if (hexagons[i].GetTexture() != TopRopeColor)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator BlastSelectedHexList(List<HexagonController> hexList)
    {
        int hexCount = hexList.Count;
        if (hexList.Count > 10)
            hexCount = hexList.Count;
        else
            hexCount = 10;

        var hexPosition = hexList[0].transform.position;
        print(hexPosition);
        blastCompleted = false;
        for (int i = 0; i < hexList.Count; i++)
        {
            hexList[i].DestroySelf();
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(.1f);
        //CoinsManager.Instance.AnimateStar(hexPosition);
        CoinsManager.Instance.AddCoins(new Vector3(hexPosition.x, hexPosition.y + .5f, hexPosition.z), 1,hexCount);
        blastCompleted = true;
        //CanvasManager.instance.UpdateScoreText();
    }
    public void UpdateHexagonsList(List<HexagonController> hexes)
    {
        for (int i = 0; i < hexes.Count; i++)
        {
            hexagons.Add(hexes[i]);
            foreach (var item in hexagons)
            {
                item.transform.DOScale(1, .2f);
            }
            hexes[i].transform.SetParent(HexStackParent);
            GridManager.instance.GridPlan[(int)_coordinates.x, (int)_coordinates.y].CellContentList.Add(hexes[i].GetTexture());
        }
    }
    public void ToggleCellObject(out bool _isOpen)
    {
        bool status = opaqueMesh.activeSelf;
        opaqueMesh.SetActive(!status);
        transparentMesh.SetActive(status);

        isOpen = opaqueMesh.activeSelf;
        _isOpen = isOpen;
    }
    #region Getters / Setters
    public void AddHex(HexagonController hex)
    {
        if (!hexagons.Contains(hex))
            hexagons.Add(hex);
    }
    // GETTERS
    public Vector2 GetCoordinates()
    {
        return _coordinates;
    }
    public Vector3 GetCenter()
    {
        Vector3 centerPos = new Vector3(transform.position.x, transform.position.y + .2f, transform.position.z);
        return centerPos;
    }
    public Vector3 GetVerticalPosForHex()
    {
        float verticalOffset = (hexagons.Count - 1) * GridManager.instance.VERTICAL_PLACEMENT_OFFSET;
        Vector3 pos = new Vector3(0, verticalOffset, 0);

        return GetCenter() + pos;
    }

    public int GetHexListCount()
    {
        return hexagons.Count;
    }

    // SETTERS
    private void SetHexagonLists()
    {
        foreach (Transform hex in HexStackParent)
        {
            HexagonController hexagonController = hex.GetComponent<HexagonController>();
            hexagons.Add(hexagonController);
        }
    }
    public void SetCoordinates(float x, float y)
    {
        _coordinates.x = x;
        _coordinates.y = y;
    }
    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }
    #endregion
}