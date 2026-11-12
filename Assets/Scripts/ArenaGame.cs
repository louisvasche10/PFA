using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
namespace PFACombat
{
    public class ArenaGame : MonoBehaviour
    {
        [SerializeField] Transform levelGeometry;
        [SerializeField] PlayerHealth player;
        [SerializeField] Material redMaterial;
        [SerializeField] Material darkMaterial;
        [SerializeField] Material lightMaterial;
        [SerializeField] float enemySpeed = 4f;
        NavMeshSurface surface;
        MeleeEnemy enemy;
        readonly Vector3[] spawnPoints = {
            new Vector3(-17, 0, 14), new Vector3(17, 0, 14), new Vector3(-17, 0, -14),
            new Vector3(17, 0, -14), new Vector3(-10, 3, 8), new Vector3(10, 3, 8), new Vector3(0, 6, 16)
        };
        void Start()
        {
            surface = levelGeometry.gameObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true;
            surface.voxelSize = 0.1f;
            Physics.SyncTransforms();
            surface.BuildNavMesh();
            player.Initialize(this);
            SpawnEnemy();
        }
        public void EnemyKilled(MeleeEnemy killed)
        {
            if (enemy != killed) return;
            enemy = null;
            Invoke(nameof(SpawnEnemy), 1f);
        }
        void SpawnEnemy()
        {
            if (enemy != null || player.IsDead) return;
            int start = Random.Range(0, spawnPoints.Length);
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Vector3 point = spawnPoints[(i + start) % spawnPoints.Length];
                if (Vector3.Distance(point, player.transform.position) < 9f) continue;
                if (!NavMesh.SamplePosition(point, out NavMeshHit hit, 1.5f, NavMesh.AllAreas)) continue;
                NavMeshPath path = new NavMeshPath();
                if (!NavMesh.SamplePosition(player.transform.position, out NavMeshHit target, 3f, NavMesh.AllAreas)) continue;
                if (!NavMesh.CalculatePath(hit.position, target.position, NavMesh.AllAreas, path) || path.status != NavMeshPathStatus.PathComplete) continue;
                GameObject body = new GameObject("Enemy - Red melee guard");
                body.transform.position = hit.position;
                CapsuleCollider collider = body.AddComponent<CapsuleCollider>();
                collider.center = new Vector3(0, 1f, 0);
                collider.height = 2f;
                collider.radius = 0.4f;
                NavMeshAgent agent = body.AddComponent<NavMeshAgent>();
                agent.radius = 0.4f;
                agent.height = 2f;
                agent.speed = enemySpeed;
                agent.acceleration = 20f;
                agent.angularSpeed = 540f;
                agent.stoppingDistance = 1.15f;
                enemy = body.AddComponent<MeleeEnemy>();
                enemy.Initialize(this, player, redMaterial, darkMaterial, lightMaterial);
                return;
            }
            Invoke(nameof(SpawnEnemy), 1f);
        }
        public void ResetEncounter()
        {
            CancelInvoke(nameof(SpawnEnemy));
            if (enemy != null)
            {
                enemy.gameObject.SetActive(false);
                Destroy(enemy.gameObject);
            }
            enemy = null;
            foreach (PistolBullet bullet in FindObjectsByType<PistolBullet>(FindObjectsSortMode.None))
                Destroy(bullet.gameObject);
            SpawnEnemy();
        }
        void OnDestroy()
        {
            CancelInvoke();
            if (surface != null) surface.RemoveData();
        }
    }
}
