using UnityEngine;
using UnityEngine.AI;
namespace PFACombat
{
    public class MeleeEnemy : MonoBehaviour
    {
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
