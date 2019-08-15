using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server 
{
    public class RadiusTools : MonoBehaviour
    {
        public float radius;
        public Transform _transform;
        public Transform target;
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_transform.position, radius);
        }
        [EasyButtons.Button]
        public void CheackRange()
        {
            Debug.Log("[C] Range: " + GetRange(_transform.position, target.position, (ushort)radius));
        }
        private bool GetRange(Vector3 current, Vector3 target, ushort range)
        { 
            if (GetRange(current.x, target.x, range) && GetRange((int)current.z, target.z, range) && GetRange(current.z, target.z, range)) return true;
            else return false;
        }
        private bool GetRange(float num, float target, float range)
        {
            if (num <= target + range && num >= target - range) return true;
            else return false;
        }
    }
}
