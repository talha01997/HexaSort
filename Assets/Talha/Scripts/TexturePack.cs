using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TexturePackNew", menuName = "TexturePack/MyTexturePack", order = 1)]
public class TexturePack : ScriptableObject
{
    [System.Serializable]
    public class TexturePackInfo
    {
        public TextureInfo.TextureEnum SelectedTextureEnum = TextureInfo.TextureEnum.None;
        public Texture texture;
    }
    public List<TexturePackInfo> HexagonTextureInfo = new List<TexturePackInfo>();
    public int GetTextureEnumIndex(TextureInfo.TextureEnum ControlTextureEnum)
    {
        for (int i = 0; i < HexagonTextureInfo.Count; i++)
        {
            if (ControlTextureEnum == HexagonTextureInfo[i].SelectedTextureEnum)
            {
                return i;
            }
        }
        return 999;
    }
}