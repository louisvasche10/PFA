using UnityEngine;
namespace PFACombat
{
 public static class WeaponRecoil
 {
     // Animation locale : le Canvas enfant suit naturellement le recul.
     public static void Apply(Transform model, Vector3 position, Quaternion rotation, float amount)
     {
         model.localPosition = position + new Vector3(0, -0.025f * amount, -0.075f * amount);
         model.localRotation = rotation * Quaternion.Euler(-12 * amount, 0, 0);
     }
 }
}
