using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {
        private readonly bool collisionInitialized = false;
        private bool forceRefreshCollidersData;

        /// <summary>
        ///     Initial operations for handling collisions
        /// </summary>
        private void PhysicsUpdate()
        {
            if (!UseCollisions) return;

            if (!collisionInitialized) InitColliders();
            else RefreshCollidersDataList();

            // Letting every tail segment check only enabled colliders by game object
            CollidersDataToCheck.Clear();

            for (var i = 0; i < IncludedCollidersData.Count; i++)
            {
                if (IncludedCollidersData[i].Collider == null)
                {
                    forceRefreshCollidersData = true;
                    break;
                }

                if (IncludedCollidersData[i].Collider.gameObject.activeInHierarchy)
                {
                    IncludedCollidersData[i].RefreshColliderData();
                    CollidersDataToCheck.Add(IncludedCollidersData[i]);
                }
            }
        }


        private void PostPhysics()
        {
            if (UseCollisions == false) return;
            if (MovementMusclesCollision && MovementMuscles > 0.989f) return;

            Bones[Bones.Count - 1].CollisionHelperVector = Bones[Bones.Count - 1].transform.position;

            var b = Bones[0];
            while (b != null)
            {
                b.CollisionHelperVector = b.transform.position;
                if (b.EnableCollisions) PushIfSegmentInsideCollider(b, ref b.CollisionHelperVector);
                b = b.Child;
            }

            b = Bones[0];
            while (b.Child != null)
            {
                b.transform.rotation =
                    Quaternion.FromToRotation(b.transform.TransformDirection(b.Child.transform.localPosition),
                        (b.Child.CollisionHelperVector - b.CollisionHelperVector).normalized) * b.transform.rotation;
                b = b.Child;
            }
        }


        #region Gizmos

#if UNITY_EDITOR

        private void _Gizmos_DrawColliders()
        {
            if (_editor_SelCategory != EStimulationMode.Collisions) return;

            if (UseCollisions)
            {
                var c = Gizmos.color;
                Color sphColor;
                float al = Application.isPlaying ? al = 0.265f : 0.2f;

                sphColor = new Color(1f, 0f, 1f, al);
                Gizmos.color = sphColor;

                // Procedural Positions
                sphColor = new Color(.2f, 1f, .2f, 1f);
                var usphColor = new Color(.2f, 1f, .2f, .15f);

                for (var i = 0; i < Bones.Count; i++)
                {
                    Gizmos.color = Bones[i].EnableCollisions ? sphColor : usphColor;
                    var pos = Bones[i].transform.TransformPoint(OffsetAllColliders);
                    Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(i));
                    Gizmos.color = sphColor;
                }

                if (Bones.Count > 0)
                    if (GetLastTransform() != null)
                        if (!Application.isPlaying)
                        {
                            var pos = GetLastTransform().TransformPoint(HelperOffset);
                            Gizmos.DrawWireSphere(pos, GetColliderSphereRadiusFor(Bones.Count - 1));
                        }

                Gizmos.color = new Color(.2f, 1f, .2f, 0.22f);
                for (var i = 0; i < IncludedColliders.Count; i++)
                    if (IncludedColliders[i] != null)
                        Gizmos.DrawLine(transform.position, IncludedColliders[i].transform.position);

                Gizmos.color = c;
            }
        }

#endif

        #endregion


        /// <summary>
        ///     Pushing spine segment from detected collider
        /// </summary>
        public void PushIfSegmentInsideCollider(Bone bone, ref Vector3 targetPoint)
        {
            var offset = bone.transform.TransformVector(OffsetAllColliders);

            if (!DetailedCollision)
            {
                for (var i = 0; i < CollidersDataToCheck.Count; i++)
                    if (CollidersDataToCheck[i].PushIfInside(ref targetPoint, bone.GetCollisionRadiusScaled(), offset))
                        return;
            }
            else
            {
                for (var i = 0; i < CollidersDataToCheck.Count; i++)
                    CollidersDataToCheck[i].PushIfInside(ref targetPoint, bone.GetCollisionRadiusScaled(), offset);
            }
        }


        #region Colliders Management

        /// <summary>
        ///     Refreshing colliders data for included colliders
        /// </summary>
        public void RefreshCollidersDataList()
        {
            if (IncludedColliders.Count != IncludedCollidersData.Count || forceRefreshCollidersData)
            {
                IncludedCollidersData.Clear();

                for (var i = IncludedColliders.Count - 1; i >= 0; i--)
                {
                    if (IncludedColliders[i] == null)
                    {
                        IncludedColliders.RemoveAt(i);
                        continue;
                    }

                    var colData = FImp_ColliderData_Base.GetColliderDataFor(IncludedColliders[i]);
                    IncludedCollidersData.Add(colData);
                }

                forceRefreshCollidersData = false;
            }
        }

        /// <summary>
        ///     Calculating automatically scale for colliders, which will be automatically assigned after initialization
        /// </summary>
        protected float GetColliderSphereRadiusFor(int i)
        {
            var step = 0f;
            if (Bones.Count > 1) step = 1f / (Bones.Count - 1);
            return 0.5f * CollidersScaleMul * CollidersScaleCurve.Evaluate(step * i);
        }


        /// <summary>
        ///     Adding collider to included colliders list
        /// </summary>
        public void AddCollider(Collider collider)
        {
            if (IncludedColliders.Contains(collider)) return;
            IncludedColliders.Add(collider);
        }


        /// <summary>
        ///     Initializing collider helper list
        /// </summary>
        private void InitColliders()
        {
            CollidersDataToCheck = new List<FImp_ColliderData_Base>();

            for (var i = 0; i < Bones.Count; i++)
                Bones[i].CollisionRadius = GetColliderSphereRadiusFor(i);

            IncludedCollidersData = new List<FImp_ColliderData_Base>();
            RefreshCollidersDataList();
        }

        /// <summary>
        ///     Checking if colliders list don't have duplicates
        /// </summary>
        public void CheckForColliderDuplicates()
        {
            for (var i = 0; i < IncludedColliders.Count; i++)
            {
                var col = IncludedColliders[i];
                var count = IncludedColliders.Count(o => o == col);

                if (count > 1)
                {
                    IncludedColliders.RemoveAll(o => o == col);
                    IncludedColliders.Add(col);
                }
            }
        }

        #endregion
    }
}