using UnityEngine;

namespace SystemBreach.Map
{
    // Defines the fixed path enemies follow. Place waypoint child GameObjects in order.
    public class WaypointPath : MonoBehaviour
    {
        public static WaypointPath Instance { get; private set; }

        [SerializeField] private Transform[] waypoints;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // Auto-collect children if not set manually
            if (waypoints == null || waypoints.Length == 0)
            {
                waypoints = new Transform[transform.childCount];
                for (int i = 0; i < transform.childCount; i++)
                    waypoints[i] = transform.GetChild(i);
            }
        }

        public Transform[] Waypoints => waypoints;

        public Vector3 GetWaypointPosition(int index)
        {
            if (index < 0 || index >= waypoints.Length) return Vector3.zero;
            return waypoints[index].position;
        }

        public int WaypointCount => waypoints?.Length ?? 0;

        void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length < 2) return;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] && waypoints[i + 1])
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            Gizmos.color = Color.green;
            if (waypoints[0]) Gizmos.DrawSphere(waypoints[0].position, 0.2f);
            Gizmos.color = Color.red;
            if (waypoints[^1]) Gizmos.DrawSphere(waypoints[^1].position, 0.2f);
        }
    }
}
