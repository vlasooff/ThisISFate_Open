 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Core
{

    public class Console  : MonoBehaviour 
    {
        public bool isOpen;
        public void FixedUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {

                if (isOpen)
                {
                    isOpen = false;
                }
                else isOpen = true;
            }
        } 
        public void OnGUI()
        {
            if(isOpen)
            {

            }
        }
    }
}
