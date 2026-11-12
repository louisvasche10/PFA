using UnityEngine;
public static class FirstPersonLook
{
    public static float Rotate(Transform player, Transform camera, Vector2 delta, float sensitivity, float pitch)
    {
        player.Rotate(Vector3.up * delta.x * sensitivity);
        pitch = Mathf.Clamp(pitch - delta.y * sensitivity, -85f, 85f);
        camera.localRotation = Quaternion.Euler(pitch, 0, 0);
        return pitch;
    }
}
