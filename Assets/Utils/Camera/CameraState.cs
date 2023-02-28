using UnityEngine;

[CreateAssetMenu(fileName = "New Camera State", menuName = "Camera State", order = 49)]
public class CameraState : ScriptableObject
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public void SetState()
    {
        Transform mainCamera = Camera.main.transform;
        position = mainCamera.position;
        rotation = mainCamera.rotation;
        scale = mainCamera.localScale;
    }
}
