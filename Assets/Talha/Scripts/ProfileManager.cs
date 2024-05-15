using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] List<Image> buttonImages;
    [SerializeField] List<Sprite> playerIcons;
    [SerializeField] Image myAvatar;
    [SerializeField] InputField nameField;
    public string userName;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < buttonImages.Count; i++)
        {
            playerIcons.Add(buttonImages[i].sprite);
        }
        DisableSelectedOutline();
    }

    public void OnClickAvatarBtn(int num)
    {
        DisableSelectedOutline();
        buttonImages[num].transform.GetChild(0).gameObject.SetActive(true);
        myAvatar.sprite = buttonImages[num].sprite;
    }

    public void OnClickCloseBtn()
    {

    }

    void DisableSelectedOutline()
    {
        foreach (var btn in buttonImages)
        {
            btn.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
