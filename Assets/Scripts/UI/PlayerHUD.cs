using UnityEngine;
using UnityEngine.UI;
namespace PFACombat
{
 public class PlayerHUD : MonoBehaviour
 {
 Texture2D edgeTexture;
 Sprite edgeSprite;
        Image Panel(Transform parent, string name, Color color)
        {
            GameObject item = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            item.transform.SetParent(parent, false);
            Image image = item.GetComponent<Image>();
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }
        public void Build(Transform owner, out Image vignette, out GameObject deathPanel, out Text healthLabel)
        {
            GameObject hud = new GameObject("Player HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            hud.transform.SetParent(owner, false);
            hud.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            hud.GetComponent<Canvas>().sortingOrder = 10;
            CanvasScaler scaler = hud.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            vignette = Panel(hud.transform, "Damage - red screen edges", Color.clear);
            edgeTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            edgeTexture.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < 128; y++)
                for (int x = 0; x < 128; x++)
                {
                    float edge = Mathf.Max(Mathf.Abs(x / 127f * 2 - 1), Mathf.Abs(y / 127f * 2 - 1));
                    edgeTexture.SetPixel(x, y, new Color(1, 1, 1, Mathf.SmoothStep(0, 1, Mathf.InverseLerp(0.45f, 1, edge))));
                }
            edgeTexture.Apply();
            edgeSprite = Sprite.Create(edgeTexture, new Rect(0, 0, 128, 128), Vector2.one * 0.5f);
            vignette.sprite = edgeSprite;
            Text crosshair = ArenaVisuals.Label(hud.transform, "Crosshair", "+", 28, Color.white);
            crosshair.rectTransform.sizeDelta = new Vector2(40, 40);
            healthLabel = ArenaVisuals.Label(hud.transform, "Health", "", 28, Color.white);
            healthLabel.rectTransform.anchorMin = new Vector2(0, 0);
            healthLabel.rectTransform.anchorMax = new Vector2(0, 0);
            healthLabel.rectTransform.anchoredPosition = new Vector2(150, 55);
            healthLabel.rectTransform.sizeDelta = new Vector2(240, 60);
            Text controls = ArenaVisuals.Label(hud.transform, "Controls", "ZQSD : bouger   |   Shift : courir   |   Espace : sauter   |   Clic gauche : tirer   |   R : recharger", 22, Color.white);
            controls.rectTransform.anchorMin = new Vector2(0.5f, 0);
            controls.rectTransform.anchorMax = new Vector2(0.5f, 0);
            controls.rectTransform.anchoredPosition = new Vector2(0, 24);
            controls.rectTransform.sizeDelta = new Vector2(1500, 45);
            deathPanel = Panel(hud.transform, "Death Canvas", new Color(0.03f, 0, 0, 0.82f)).gameObject;
            Text title = ArenaVisuals.Label(deathPanel.transform, "You are dead", "You are dead", 90, new Color(1, 0.15f, 0.15f));
            title.rectTransform.sizeDelta = new Vector2(1000, 150);
            Text subtitle = ArenaVisuals.Label(deathPanel.transform, "Respawn", "Réapparition dans 3 secondes", 30, Color.white);
            subtitle.rectTransform.anchoredPosition = new Vector2(0, -100);
            subtitle.rectTransform.sizeDelta = new Vector2(900, 70);
            deathPanel.SetActive(false);
        }

 void OnDestroy() { if(edgeSprite != null) Destroy(edgeSprite); if(edgeTexture != null) Destroy(edgeTexture); }
 }
}
