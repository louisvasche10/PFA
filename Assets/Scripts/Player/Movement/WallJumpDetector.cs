using UnityEngine;
public static class WallJumpDetector
{
    // Les normales verticales excluent les sols et les rampes du wall jump.
    public static bool FindWall(Vector3 origin, float distance, out Vector3 normal)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector3 direction = Quaternion.Euler(0, i * 45, 0) * Vector3.forward;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~(1 << 2), QueryTriggerInteraction.Ignore) && Mathf.Abs(hit.normal.y) < 0.2f)
            { normal = hit.normal; return true; }
        }
        normal = Vector3.zero;
        return false;
    }
}
