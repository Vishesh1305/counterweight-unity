using System.Collections.Generic;
using UnityEngine;

// Tests a thin box at the player's feet against solid layers.
namespace Counterweight.Player
{
    public class GroundCheck : MonoBehaviour
    {
        private const int MaxResults = 8;

        [SerializeField] private Collider2D bodyCollider;
        [SerializeField] private LayerMask groundLayers;
        [Tooltip("Box width as a fraction of the body width. Below 1 so walls are not mistaken for ground.")]
        [SerializeField, Range(0.1f, 1f)] private float widthRatio = 0.9f;
        [SerializeField, Min(0.01f)] private float checkHeight = 0.1f;

        private readonly List<Collider2D> results = new List<Collider2D>(MaxResults);
        private ContactFilter2D filter;

        public bool IsGrounded { get; private set; }


        public Rigidbody2D GroundBody { get; private set; } 
        private void Awake()
        {
            if (bodyCollider == null)
            {
                Debug.LogError($"{name}: GroundCheck has no Body Collider assigned.", this);
                enabled = false;
                return;
            }

            filter = new ContactFilter2D();
            filter.SetLayerMask(groundLayers);
            filter.useTriggers = false;
        }


        public void Refresh()
        {
            GroundBody = null; // Phase 6

            if (!enabled)
            {
                IsGrounded = false;
                return;
            }

            GetCheckBox(out Vector2 center, out Vector2 size);
            int count = Physics2D.OverlapBox(center, size, 0f, filter, results);

            IsGrounded = false;
            for (int i = 0; i < count; i++)
            {

                if (results[i] != bodyCollider)
                {
                    IsGrounded = true;
                    GroundBody = FindMovingGround(i, count); 
                    return;
                }
            }
        }


        private Rigidbody2D FindMovingGround(int startIndex, int count) 
        { 
            for (int i = startIndex; i < count; i++) 
            { 
                if (results[i] == bodyCollider) 
                    continue; 

                Rigidbody2D body = results[i].attachedRigidbody; 
                if (body != null && body.bodyType != RigidbodyType2D.Static) 
                    return body; 
            } 

            return null; 
        } 

        private void GetCheckBox(out Vector2 center, out Vector2 size)
        {
            Bounds bounds = bodyCollider.bounds;
            center = new Vector2(bounds.center.x, bounds.min.y);
            size = new Vector2(bounds.size.x * widthRatio, checkHeight);
        }

        private void OnDrawGizmosSelected()
        {
            if (bodyCollider == null)
                return;

            GetCheckBox(out Vector2 center, out Vector2 size);
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(center, size);
        }
    }
}