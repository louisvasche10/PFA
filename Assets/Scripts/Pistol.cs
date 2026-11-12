using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
namespace PFACombat
{
    public class Pistol : MonoBehaviour
    {
        [SerializeField] Transform view;
        [SerializeField] Material darkMaterial;
        [SerializeField] Material blueMaterial;
        [SerializeField] Material lightMaterial;
        [SerializeField] float shotInterval = 0.22f;
        [SerializeField] float reloadDuration = 1.2f;
        [SerializeField] int magazineSize = 6;
        Transform model;
        Transform muzzle;
        Text ammoLabel;
        PlayerHealth health;
        Vector3 restPosition;
        Quaternion restRotation;
        int ammunition;
        float nextShot;
        float recoil;
        bool reloading;
        void Start()
        {
            health = GetComponent<PlayerHealth>();
            model = new GameObject("Pistol - 6 rounds").transform;
            model.SetParent(view, false);
            restPosition = new Vector3(0.28f, -0.24f, 0.5f);
            restRotation = Quaternion.Euler(0, -5, 0);
            model.localPosition = restPosition;
            model.localRotation = restRotation;
            ArenaVisuals.Part("Slide", model, PrimitiveType.Cube, new Vector3(0, 0, 0), new Vector3(0.11f, 0.12f, 0.38f), blueMaterial);
            ArenaVisuals.Part("Frame", model, PrimitiveType.Cube, new Vector3(0, -0.08f, -0.01f), new Vector3(0.1f, 0.07f, 0.3f), darkMaterial);
            GameObject grip = ArenaVisuals.Part("Grip", model, PrimitiveType.Cube, new Vector3(0, -0.17f, -0.1f), new Vector3(0.1f, 0.23f, 0.13f), darkMaterial);
            grip.transform.localRotation = Quaternion.Euler(-12, 0, 0);
            GameObject barrel = ArenaVisuals.Part("Barrel", model, PrimitiveType.Cylinder, new Vector3(0, 0, 0.22f), new Vector3(0.06f, 0.06f, 0.06f), darkMaterial);
            barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);
            ArenaVisuals.Part("Front sight", model, PrimitiveType.Cube, new Vector3(0, 0.075f, 0.12f), new Vector3(0.025f, 0.035f, 0.035f), lightMaterial);
            ArenaVisuals.Part("Rear sight", model, PrimitiveType.Cube, new Vector3(0, 0.075f, -0.15f), new Vector3(0.06f, 0.025f, 0.025f), darkMaterial);
            muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(model, false);
            muzzle.localPosition = new Vector3(0, 0, 0.3f);
            GameObject display = new GameObject("Ammo - World Space Canvas", typeof(RectTransform), typeof(Canvas));
            display.transform.SetParent(model, false);
            display.transform.localPosition = new Vector3(0, 0.15f, -0.06f);
            display.transform.localScale = Vector3.one * 0.001f;
            Canvas canvas = display.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = view.GetComponent<Camera>();
            canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 65);
            Image background = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
            background.transform.SetParent(display.transform, false);
            background.rectTransform.sizeDelta = new Vector2(180, 65);
            background.color = new Color(0.015f, 0.025f, 0.04f, 0.95f);
            background.raycastTarget = false;
            ammoLabel = ArenaVisuals.Label(display.transform, "Ammo count", "6/6", 42, Color.cyan);
            ammoLabel.rectTransform.sizeDelta = new Vector2(180, 65);
            ResetWeapon();
        }
        void Update()
        {
            recoil = Mathf.MoveTowards(recoil, 0, Time.deltaTime * 7f);
            model.localPosition = restPosition + new Vector3(0, -0.025f * recoil, -0.075f * recoil);
            model.localRotation = restRotation * Quaternion.Euler(-12 * recoil, 0, 0);
            if (health.IsDead || !GetComponent<FirstPersonController>().IsControlling || !Application.isFocused) return;
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame && !reloading && ammunition < magazineSize)
                StartCoroutine(Reload());
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !reloading && ammunition > 0 && Time.time >= nextShot)
                Shoot();
        }
        void Shoot()
        {
            ammunition--;
            nextShot = Time.time + shotInterval;
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
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "Pistol bullet";
            bullet.layer = 2;
            Destroy(bullet.GetComponent<Collider>());
            bullet.transform.position = origin;
            bullet.transform.localScale = Vector3.one * 0.09f;
            bullet.GetComponent<Renderer>().sharedMaterial = lightMaterial;
            bullet.AddComponent<PistolBullet>().Initialize(aimPoint - origin);
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
            ammoLabel.text = ammunition + "/" + magazineSize;
            ammoLabel.color = ammunition == 0 ? new Color(1f, 0.25f, 0.2f) : Color.cyan;
        }
        public void ResetWeapon()
        {
            StopAllCoroutines();
            ammunition = magazineSize;
            reloading = false;
            recoil = 0;
            nextShot = Time.time + 0.2f;
            if (ammoLabel != null) UpdateCounter();
        }
    }
}
