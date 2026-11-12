using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace PFACombat
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Références de la caméra et des matériaux")]
        [SerializeField] Transform view;
        [SerializeField] Material darkMaterial;
        [SerializeField] Material blueMaterial;
        [SerializeField] Material lightMaterial;
        [Header("Tir et chargeur")]
        [SerializeField] float shotInterval = 0.22f;
        [SerializeField] float reloadDuration = 1.2f;
        [SerializeField] int magazineSize = 6;
        Transform model;
        Transform muzzle;
        Text ammoLabel;
        PlayerHealth health;
        FirstPersonController movement;
        Vector3 restPosition;
        Quaternion restRotation;
        int ammunition;
        float nextShot;
        float recoil;
        bool reloading;
        bool shotgun;
        readonly int[] savedAmmo = { 6, 6 };
        [Header("Fusil à pompe")]
        [SerializeField] int pelletCount = 8;
        [SerializeField] float spreadDegrees = 6f;
        [SerializeField] float shotgunInterval = 0.8f;
        void Start()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<FirstPersonController>();
            WeaponPresentation.Build(view, darkMaterial, blueMaterial, lightMaterial, out model, out muzzle, out ammoLabel, out restPosition, out restRotation);
            ResetWeapon();
        }
        void Update()
        {
            recoil = Mathf.MoveTowards(recoil, 0, Time.deltaTime * 7f);
            WeaponRecoil.Apply(model, restPosition, restRotation, recoil);
            if (health.IsDead || !movement.IsControlling || !Application.isFocused) return;
            if (Mouse.current != null && Mathf.Abs(Mouse.current.scroll.ReadValue().y) > 0.01f)
            {
                savedAmmo[shotgun ? 1 : 0] = ammunition;
                StopAllCoroutines();
                reloading = false;
                shotgun = !shotgun;
                ammunition = savedAmmo[shotgun ? 1 : 0];
                WeaponPresentation.SetShotgun(model, shotgun, darkMaterial, blueMaterial);
                muzzle.localPosition = new Vector3(0, 0, shotgun ? 0.82f : 0.3f);
                model.name = shotgun ? "Shotgun" : "Pistol";
                nextShot = Time.time + 0.25f;
                UpdateCounter();
            }
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && !reloading && ammunition < magazineSize)
                StartCoroutine(Reload());
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !reloading && ammunition > 0 && Time.time >= nextShot)
                Shoot();
        }
        void Shoot()
        {
            ammunition--;
            nextShot = Time.time + (shotgun ? shotgunInterval : shotInterval);
            recoil = 1;
            UpdateCounter();
            Vector3 aimPoint = view.position + view.forward * 100f;
            if (Physics.Raycast(view.position, view.forward, out RaycastHit aim, 100f, ~(1 << 2), QueryTriggerInteraction.Ignore)) aimPoint = aim.point;
            Vector3 origin = muzzle.position;
            if (Physics.Linecast(view.position, origin, out RaycastHit obstruction, ~(1 << 2), QueryTriggerInteraction.Ignore))
            {
                MeleeEnemy blockedEnemy = obstruction.collider.GetComponentInParent<MeleeEnemy>();
                if (blockedEnemy != null) blockedEnemy.TakeDamage();
                return;
            }
            int count = shotgun ? pelletCount : 1;
            for (int i = 0; i < count; i++)
            {
                Vector3 direction = (aimPoint - origin).normalized;
                if (shotgun)
                {
                    Vector2 spread = Random.insideUnitCircle * Mathf.Tan(spreadDegrees * Mathf.Deg2Rad);
                    direction = (direction + view.right * spread.x + view.up * spread.y).normalized;
                }
                ProjectileEmitter.Fire(origin, direction, lightMaterial);
            }
        }

        IEnumerator Reload()
        {
            reloading = true;
            ammoLabel.color = Color.yellow;
            yield return new WaitForSeconds(reloadDuration);
            ammunition = magazineSize;
            reloading = false;
            UpdateCounter();
        }
        void UpdateCounter()
        {
            AmmoDisplay.Refresh(ammoLabel, ammunition, magazineSize);
        }
        public void ResetWeapon()
        {
            StopAllCoroutines();
            ammunition = magazineSize;
            savedAmmo[0] = savedAmmo[1] = magazineSize;
            reloading = false;
            recoil = 0;
            nextShot = Time.time + 0.2f;
            if (ammoLabel != null) UpdateCounter();
        }
    }
}
