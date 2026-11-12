using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
namespace PFACombat
{
 public static class ArenaNavigation
 {
 public static NavMeshSurface Build(Transform geometry)
 {
 NavMeshSurface surface;
            surface = geometry.gameObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            surface.overrideVoxelSize = true;
            surface.voxelSize = 0.1f;
            Physics.SyncTransforms();
            surface.BuildNavMesh();
return surface;
 }
 }
}
