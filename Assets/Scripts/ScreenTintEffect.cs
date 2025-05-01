using UnityEngine;
using UnityEngine.UI;

public class ScreenTintEffect : MonoBehaviour
{
    public Allomancer allomancer;
    public RawImage tintImage;

    void Update()
    {
        if (allomancer != null && tintImage != null)
        {
            bool shouldShow = allomancer.IsBurning(MetalType.Tin);
            tintImage.enabled = shouldShow;
        }
    }
}