using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }
    private bool HoverActive = false;

    private float HoverTimer = 0.0f;
    private float HoverTimeLimit = 0.25f;
    
    [SerializeField]
    private UnityEvent<GameObject> OnObjectHover;

    [SerializeField]
    private Material OnHoverActiveMaterial;

    [SerializeField]
    private Material OnHoverInactiveMaterial;

    [SerializeField]
    private string InteractableCMD;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (InteractableCMD == "pause") HoverTimeLimit = 1.0f;
        if (InteractableCMD == "e") HoverTimeLimit = 0.7f;//mode interactable

    }

    private void Update()
    {
        if (IsHovered)
        {
            OnObjectHover?.Invoke(gameObject);
            
            if (!HoverActive)
            {
                HoverActive= true;
                HoverTimer= Time.time;
            }
            else if(Time.time - HoverTimer > HoverTimeLimit)
            {
                HoverAction();
                HoverActive= false;
            }

         meshRenderer.material = OnHoverActiveMaterial;
        }
        else
        {
            if (InteractableCMD == "pause")
            {
                if(PowerChairClient.Instance.pauseActive) meshRenderer.material = OnHoverActiveMaterial;
                else meshRenderer.material = OnHoverInactiveMaterial;
            }
            else meshRenderer.material = OnHoverInactiveMaterial;
            HoverActive = false;
        }
    }

    private void HoverAction()
    {
        //write code here to send command to mcu
        //PowerChairClient.Instance.SendCommandTest(InteractableCMD);
        if(InteractableCMD == "pause") PowerChairClient.Instance.pauseActive = !PowerChairClient.Instance.pauseActive;
        else if (!PowerChairClient.Instance.pauseActive)
        {
            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //IPAddress broadcast = IPAddress.Parse("192.168.86.28");

            byte[] sendbuf = Encoding.ASCII.GetBytes(InteractableCMD);
            //IPEndPoint ep = new IPEndPoint(broadcast, 4444);

            PowerChairClient.Instance.getSock().SendTo(sendbuf, PowerChairClient.Instance.getEP());
         
        }
    

    }
}
