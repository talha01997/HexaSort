using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] GameObject profileParent;
    [SerializeField] Text nameTxt;
    [SerializeField] Image avatarImg;
    [SerializeField] List<Image> buttonImages;
    [SerializeField] List<Sprite> playerIcons;
    [SerializeField] Image myAvatar;
    [SerializeField] InputField nameField;
    public int avatarNum;
    public string userName;
    

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(.3f);
        for (int i = 0; i < buttonImages.Count; i++)
        {
            playerIcons.Add(buttonImages[i].sprite);
        }
        DisableSelectedOutline();
        if (!Data.instance.firstPlay)
        {
            SetProfileData(Data.instance.avatarNum,Data.instance.userName);
        }
    }

    public void OnClickAvatarBtn(int num)
    {
        DisableSelectedOutline();
        buttonImages[num].transform.GetChild(0).gameObject.SetActive(true);
        myAvatar.sprite = buttonImages[num].sprite;
        avatarImg.sprite = buttonImages[num].sprite;
        
        avatarNum = num;
    }
    void SetProfileData(int num, string user)
    {
        DisableSelectedOutline();
        buttonImages[num].transform.GetChild(0).gameObject.SetActive(true);
        myAvatar.sprite = buttonImages[num].sprite;
        avatarImg.sprite = buttonImages[num].sprite;
        nameTxt.text = user;
        nameField.text = user;
        avatarNum = num;
    }
    public void OnClickCloseBtn()
    {
        Data.instance.firstPlay = false;
        Data.instance.avatarNum = avatarNum;
        userName = nameField.text;
        nameTxt.text = userName;
        Data.instance.userName = userName;
        Data.instance.SaveMyData();
        profileParent.SetActive(false);
    }

    void DisableSelectedOutline()
    {
        foreach (var btn in buttonImages)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
