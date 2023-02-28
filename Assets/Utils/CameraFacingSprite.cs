using UnityEngine;

public class CameraFacingSprite : MonoBehaviour
{
    private void OnGUI()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
