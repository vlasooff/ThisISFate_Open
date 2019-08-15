using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Community.Client.Components 
{
    public class InteractionPlayer : MonoBehaviour
    {
        private Camera m_camera;
        private RaycastHit HitInfo;
        [SerializeField]
        private Interactable buffer;
        UISlider uiSlider;
        bool isActivMenu;
        [SerializeField]
        int scroll;
        float scrooler;
        float useTime = 0;

        [SerializeField]
        private KeyCode KeyUse = KeyCode.F; 
        public void Start()
        {
            uiSlider = ClientManager.manager.UVManager.slider;
            m_camera = Camera.main;
        }

        public void FixedUpdate()
        {
            if (Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out HitInfo, 10.0f))
            {
                Collider collider = HitInfo.collider;
                if (collider.CompareTag("Interact"))
                {
                    OnMenu(collider.GetComponent<Interactable>());
                    ScrollMenu();
                    if (Input.GetKeyDown(KeyUse) && (Time.time - useTime) > 1)
                    {
                        uiSlider.text.fontSize = 14;
                        buffer.Execute(scroll);
                        useTime = Time.time;
                    }
                    else
                        uiSlider.text.fontSize = 18;
                }
                else OffMenu();
            }
            else
            {
                OffMenu();
            }
 
        }
        private void ScrollMenu()
        {

            scrooler += Input.GetAxis("Mouse ScrollWheel") * 2;
            scroll = (int)Mathf.Clamp(scrooler, 0, uiSlider.texts.Length);
            if(uiSlider.text != uiSlider.texts[scroll])
            {
                SelectedMenu();
            }
        }
        private void SelectedMenu()
        {
            uiSlider.texts[scroll].color = uiSlider.selectedColor;
            uiSlider.text.color = uiSlider.DefaultColor;
            uiSlider.text = uiSlider.texts[scroll];
        }
        private void OnMenu(Interactable newInteract)
        {
            if(buffer != newInteract)
            {
                isActivMenu = true;
                buffer = newInteract;
                for (int i = 0; i < uiSlider.texts.Length; i++)
                {
                    if(i > buffer.sliderTexts.Length -1)
                    {
                        uiSlider.texts[i].text = "";
                    }
                    else
                        uiSlider.texts[i].text = buffer.sliderTexts[i].Text;

                }
                scroll = buffer.sliderTexts.Length;
                uiSlider.text = uiSlider.texts[0];
                uiSlider.menu.gameObject.SetActive(isActivMenu);
            }
            
        }
        private void OffMenu()
        {
            isActivMenu = false;
            uiSlider.menu.gameObject.SetActive(isActivMenu);
            buffer = null;
        }
    }
}
