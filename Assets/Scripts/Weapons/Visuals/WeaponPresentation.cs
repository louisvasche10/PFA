using UnityEngine;
using UnityEngine.UI;
namespace PFACombat
{
 public static class WeaponPresentation
 {
 public static void Build(Transform view, Material darkMaterial, Material blueMaterial, Material lightMaterial, out Transform model, out Transform muzzle, out Text ammoLabel, out Vector3 restPosition, out Quaternion restRotation)
 {
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

 }
 public static void SetShotgun(Transform model, bool shotgun, Material dark, Material blue)
 {
     // Les pièces supplémentaires forment un canon long, une pompe et une crosse.
     Transform parts = model.Find("Shotgun parts");
     if (parts == null)
     {
         parts = new GameObject("Shotgun parts").transform;
         parts.SetParent(model, false);
         GameObject barrel = ArenaVisuals.Part("Long barrel", parts, PrimitiveType.Cylinder, new Vector3(0, 0, 0.48f), new Vector3(0.07f, 0.3f, 0.07f), dark);
         barrel.transform.localRotation = Quaternion.Euler(90, 0, 0);
         ArenaVisuals.Part("Pump grip", parts, PrimitiveType.Cube, new Vector3(0, -0.07f, 0.4f), new Vector3(0.13f, 0.12f, 0.25f), blue);
         ArenaVisuals.Part("Stock", parts, PrimitiveType.Cube, new Vector3(0, -0.07f, -0.3f), new Vector3(0.12f, 0.15f, 0.25f), dark);
     }
     parts.gameObject.SetActive(shotgun);
 }
 }
}
