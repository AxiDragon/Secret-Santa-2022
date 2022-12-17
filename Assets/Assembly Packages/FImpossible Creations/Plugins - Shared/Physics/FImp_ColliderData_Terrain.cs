using UnityEngine;

namespace FIMSpace
{
    public class FImp_ColliderData_Terrain : FImp_ColliderData_Base
    {
        public FImp_ColliderData_Terrain(TerrainCollider collider)
        {
            Collider = collider;
            Transform = collider.transform;
            TerrCollider = collider;
            ColliderType = EFColliderType.Terrain;
            TerrainComponent = collider.GetComponent<Terrain>();
        }

        public TerrainCollider TerrCollider { get; }
        public Terrain TerrainComponent { get; }

        public override bool PushIfInside(ref Vector3 segmentPosition, float segmentRadius, Vector3 segmentOffset)
        {
            // Checking if segment is inside box shape of terrain
            if (segmentPosition.x + segmentRadius < TerrainComponent.GetPosition().x - segmentRadius ||
                segmentPosition.x > TerrainComponent.GetPosition().x + TerrainComponent.terrainData.size.x ||
                segmentPosition.z + segmentRadius < TerrainComponent.GetPosition().z - segmentRadius ||
                segmentPosition.z > TerrainComponent.GetPosition().z + TerrainComponent.terrainData.size.z)
                return false;

            var offsettedPosition = segmentPosition + segmentOffset;
            var terrPoint = offsettedPosition;
            terrPoint.y = TerrCollider.transform.position.y + TerrainComponent.SampleHeight(offsettedPosition);


            var hitToPointDist = (offsettedPosition - terrPoint).magnitude;
            var underMul = 1f;

            if (offsettedPosition.y < terrPoint.y)
                underMul = 4f;
            else if (offsettedPosition.y + segmentRadius * 2f < terrPoint.y) underMul = 8f;

            if (hitToPointDist < segmentRadius * underMul)
            {
                var toNormal = terrPoint - offsettedPosition;

                Vector3 pushNormal;
                if (underMul > 1f) pushNormal = toNormal + toNormal.normalized * segmentRadius;
                else pushNormal = toNormal - toNormal.normalized * segmentRadius;
                segmentPosition = segmentPosition + pushNormal;

                return true;
            }

            return false;
        }

        public static void PushOutFromTerrain(TerrainCollider terrainCollider, float segmentRadius, ref Vector3 point)
        {
            var terrain = terrainCollider.GetComponent<Terrain>();

            var rayOrigin = point;
            rayOrigin.y = terrainCollider.transform.position.y + terrain.SampleHeight(point) + segmentRadius;

            var ray = new Ray(rayOrigin, Vector3.down);

            RaycastHit hit;
            if (terrainCollider.Raycast(ray, out hit, segmentRadius * 2f))
            {
                var hitToPointDist = (point - hit.point).magnitude;

                var underMul = 1f;
                if (hit.point.y > point.y + segmentRadius * 0.9f)
                    underMul = 8f;
                else if (hit.point.y > point.y) underMul = 4f;

                if (hitToPointDist < segmentRadius * underMul)
                {
                    var toNormal = hit.point - point;
                    Vector3 pushNormal;

                    if (underMul > 1f) pushNormal = toNormal + toNormal.normalized * segmentRadius;
                    else pushNormal = toNormal - toNormal.normalized * segmentRadius;
                    point = point + pushNormal;
                }
            }
        }
    }
}