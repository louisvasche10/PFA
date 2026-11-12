using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace PFACombat
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Paramètres réglables dans Unity")]
        [SerializeField] int maxHealth = 100;
        [SerializeField] float respawnDelay = 3f;
        public bool IsDead { get; private set; }
        public int CurrentHealth { get; private set; }
        ArenaGame arena;
        FirstPersonController movement;
        WeaponController pistol;
        PlayerHUD hud;
        Image vignette;
        GameObject deathPanel;
        Text healthLabel;
        float flash;
        public void Initialize(ArenaGame game)
        {
            arena = game;
            movement = GetComponent<FirstPersonController>();
            pistol = GetComponent<WeaponController>();
            CurrentHealth = maxHealth;
            hud = gameObject.AddComponent<PlayerHUD>();
            hud.Build(transform, out vignette, out deathPanel, out healthLabel);
            UpdateHUD();
        }
        void Update()
        {
            if (arena == null) return;
            if (!IsDead && transform.position.y < -15) TakeDamage(maxHealth);
            flash = Mathf.MoveTowards(flash, 0, Time.deltaTime * 2f);
            if (vignette != null)
            {
                float missingHealth = 1f - (float)CurrentHealth / maxHealth;
                vignette.color = new Color(0.9f, 0, 0, Mathf.Clamp01(missingHealth * 0.9f + flash * 0.3f));
            }
        }
        public void TakeDamage(int amount)
        {
            if (IsDead || arena == null) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            flash = 1;
            UpdateHUD();
            if (CurrentHealth == 0) StartCoroutine(Die());
        }
        IEnumerator Die()
        {
            IsDead = true;
            movement.enabled = false;
            deathPanel.SetActive(true);
            yield return new WaitForSeconds(respawnDelay);
            movement.ResetPlayer();
            CurrentHealth = maxHealth;
            IsDead = false;
            flash = 0;
            pistol.ResetWeapon();
            deathPanel.SetActive(false);
            UpdateHUD();
            arena.ResetEncounter();
            movement.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void UpdateHUD()
        {
            healthLabel.text = "PV " + CurrentHealth + "/" + maxHealth;
        }
    }
}
