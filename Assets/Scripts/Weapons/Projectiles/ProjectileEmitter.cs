using UnityEngine;
namespace PFACombat
{
    public static class ProjectileEmitter
    {
        public static void Fire(Vector3 origin, Vector3 direction, Material material)
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "Projectile";
            bullet.layer = 2;
            Object.Destroy(bullet.GetComponent<Collider>());
            bullet.transform.position = origin;
            bullet.transform.localScale = Vector3.one * 0.09f;
            bullet.GetComponent<Renderer>().sharedMaterial = material;
            bullet.AddComponent<PistolBullet>().Initialize(direction);
        }
    }
}
