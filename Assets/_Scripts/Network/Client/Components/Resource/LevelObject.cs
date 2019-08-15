using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components
{
    public enum EObjectLevel
    {
        small, medium,large
    }
    public class LevelObject : MonoBehaviour
    {
        public ushort id; 
        public EObjectLevel mask;
        public Transform _transform;
        public ushort idRegion;

        [EasyButtons.Button]
        public void GetRegion()
        {
            RegionComponents regionManager = FindObjectOfType<RegionComponents>();
            _transform = transform;
            foreach (var item in regionManager.regions)
            {
                if(GetRange(item.position,_transform.position,regionManager.RangeRegion / 2))
                {
                    idRegion = item.id;
                    return;
                }
            }
        }
        private bool GetRange(Vector3 current, Vector3 target, int range)
        {
            if (GetRange(current.x, target.x, range) && GetRange((int)current.z, target.z, range)) return true;
            else return false;
        }
        private bool GetRange(float num, float target, float range)
        {
            if (num <= target + range && num >= target - range) return true;
            else return false;
        }
    }  
}
