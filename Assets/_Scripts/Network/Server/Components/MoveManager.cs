using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Server.Components
{
    public class MoveManager : MonoBehaviour
    {
        public MoveTestComponent[] players;

        private void Awake()
        {
            players = GameObject.FindObjectsOfType<MoveTestComponent>();   
        }

    }
}
