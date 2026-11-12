using UnityEngine;
using UnityEngine.AI;
namespace PFACombat
{
    public class MeleeEnemy : MonoBehaviour
    {
        [Header("Paramètres réglables dans Unity")]
        [SerializeField] int health = 3;
        [SerializeField] int damage = 20;
        [SerializeField] float attackRange = 1.65f;
        [SerializeField] float attackInterval = 1f;
        ArenaGame arena;
        PlayerHealth target;
        NavMeshAgent agent;
        Transform weapon;
        readonly GameObject[] healthMarks = new GameObject[3];
        Vector3 weaponRest;
        float nextPath;
        float nextAttack;
        float swingTime;
        bool dead;
        public void Initialize(ArenaGame game, PlayerHealth player, Material red, Material dark, Material light)
        {
            arena = game;
            target = player;
            agent = GetComponent<NavMeshAgent>();
            EnemyPresentation.Build(transform, red, dark, light, healthMarks, out weapon, out weaponRest);
            nextAttack = Time.time + 1f;
        }
        void Update()
        {
            if (dead || target == null || !agent.isOnNavMesh) return;
            if (target.IsDead)
            {
                agent.isStopped = true;
                return;
            }
            agent.isStopped = false;
            if (Time.time >= nextPath)
            {
                nextPath = Time.time + 0.15f;
                if (NavMesh.SamplePosition(target.transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
            Vector3 difference = target.transform.position - transform.position;
            bool close = new Vector2(difference.x, difference.z).magnitude <= attackRange && Mathf.Abs(difference.y) < 1.2f;
            if (close)
            {
                Vector3 direction = new Vector3(difference.x, 0, difference.z);
                if (direction.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(direction);
                Vector3 from = transform.position + Vector3.up * 1.2f;
                Vector3 to = target.transform.position + Vector3.up * 1.2f;
                bool blocked = Physics.Linecast(from, to, ~(1 << 2), QueryTriggerInteraction.Ignore);
                if (!blocked && Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    swingTime = 0.3f;
                    target.TakeDamage(damage);
                }
            }
            swingTime = Mathf.Max(0, swingTime - Time.deltaTime);
            float swing = Mathf.Sin(swingTime / 0.3f * Mathf.PI);
            weapon.localRotation = Quaternion.Euler(-85f * swing, 0, 0);
            weapon.localPosition = weaponRest + Vector3.forward * (swing * 0.35f);
        }
        public void TakeDamage()
        {
            if (dead) return;
            health--;
            if (health >= 0 && health < healthMarks.Length) healthMarks[health].SetActive(false);
            if (health > 0) return;
            dead = true;
            agent.isStopped = true;
            GetComponent<Collider>().enabled = false;
            arena.EnemyKilled(this);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
