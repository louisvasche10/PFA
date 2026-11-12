using UnityEngine;
namespace PFACombat
{
 public static class EnemyPresentation
 {
 public static void Build(Transform transform, Material red, Material dark, Material light, GameObject[] healthMarks, out Transform weapon, out Vector3 weaponRest)
 {
            ArenaVisuals.Part("Torso", transform, PrimitiveType.Cube, new Vector3(0, 1.22f, 0), new Vector3(0.78f, 0.72f, 0.42f), red);
            ArenaVisuals.Part("Helmet", transform, PrimitiveType.Cube, new Vector3(0, 1.83f, 0), new Vector3(0.5f, 0.46f, 0.48f), red);
            ArenaVisuals.Part("Visor - front", transform, PrimitiveType.Cube, new Vector3(0, 1.86f, 0.25f), new Vector3(0.38f, 0.12f, 0.04f), light);
            ArenaVisuals.Part("Left leg", transform, PrimitiveType.Cube, new Vector3(-0.22f, 0.43f, 0), new Vector3(0.28f, 0.86f, 0.3f), dark);
            ArenaVisuals.Part("Right leg", transform, PrimitiveType.Cube, new Vector3(0.22f, 0.43f, 0), new Vector3(0.28f, 0.86f, 0.3f), dark);
            ArenaVisuals.Part("Left arm", transform, PrimitiveType.Capsule, new Vector3(-0.53f, 1.2f, 0), new Vector3(0.22f, 0.38f, 0.22f), red);
            ArenaVisuals.Part("Right arm", transform, PrimitiveType.Capsule, new Vector3(0.53f, 1.2f, 0.12f), new Vector3(0.22f, 0.38f, 0.22f), red);
            for (int i = 0; i < healthMarks.Length; i++)
                healthMarks[i] = ArenaVisuals.Part("Health pip " + (i + 1), transform, PrimitiveType.Cube, new Vector3((i - 1) * 0.18f, 2.2f, 0), Vector3.one * 0.12f, light);
            weapon = new GameObject("Melee baton").transform;
            weapon.SetParent(transform, false);
            weapon.localPosition = new Vector3(0.58f, 1.12f, 0.42f);
            weaponRest = weapon.localPosition;
            ArenaVisuals.Part("Handle", weapon, PrimitiveType.Cylinder, Vector3.zero, new Vector3(0.1f, 0.23f, 0.1f), dark);
            ArenaVisuals.Part("Baton head", weapon, PrimitiveType.Cube, new Vector3(0, 0.5f, 0), new Vector3(0.18f, 0.65f, 0.18f), light);

 }
 }
}
