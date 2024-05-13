using DG.Tweening;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    [SerializeField] ColorInfo.ColorEnum color;
    [SerializeField] TextureInfo.TextureEnum texture;

    public void Initialize(TextureInfo.TextureEnum textureEnum, Material material)
    {
        //color = colorEnum;
        texture = textureEnum;
        GetComponentInChildren<Renderer>().material = material;
    }

    public ColorInfo.ColorEnum GetColor()
    {
        return color;
    }
    public TextureInfo.TextureEnum GetTexture()
    {
        return texture;
    }
    public void DestroySelf()
    {
        transform.DOScale(Vector3.zero, .2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            Destroy(gameObject,.05f);
        });
        //CoinsManager.Instance.AddCoins(transform.position, 1);
        //ParticlesAnimationHandler.instance.AnimateParticles(transform.position);
    }
}