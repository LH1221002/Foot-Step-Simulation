using System;
using System.Text;
using UnityEngine;

namespace HapticShoes
{
    public class ShoeController : MonoBehaviour
    {
        [Tooltip("IP Adress of the Shoe")]
        public string IPAddress = "192.168.0.106";

        private Action<int, int> callback;
        private ServerHandler shoeInstance;

        private bool alreadyReceivingData = false;

        public enum Material { WindowsStartupSound, Wood, Snow };

        private int strength = 0;
        private Material material = Material.Wood;
        private int volume = 0;
        private int layers = 0;

        
        public void Start()
        {
            Connect(IPAddress);

            //ToggleDataTransfer();
            //SendToShoe(100, Material.Wood, 70, 16);
            //ReceiveData((a,b)=> { Debug.Log(a + " " + b); });
        }

        public void Connect(string ipAddress = null)
        {
            if (ipAddress != null)
                IPAddress = ipAddress;

            try
            {
                shoeInstance?.Dispose();

                shoeInstance = new ServerHandler();

                shoeInstance.ConnectToServer(IPAddress, 80);

                alreadyReceivingData = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                shoeInstance?.Dispose();
                shoeInstance = null;
                Debug.LogError("Couldn't connect to Shoe!");
            }
        }

        public void Disconnect()
        {
            shoeInstance?.Dispose();
        }

        public void SendToShoe(int strength, Material material = Material.Wood, int volume = 127, int layers = 15)
        {
            int clamppedStrength = Mathf.Clamp(strength, 0, 255);
            int clamppedVolume = Mathf.Clamp(volume, 0, 127);
            int clamppedLayers = Mathf.Clamp(layers, 2, 16);

            bool dataChanged = false;

            if (clamppedStrength != this.strength)
            {
                dataChanged = true;
                this.strength = clamppedStrength;
            }
            if (clamppedVolume != this.volume)
            {
                dataChanged = true;
                this.volume = clamppedVolume;
            }
            if (clamppedLayers != this.layers)
            {
                dataChanged = true;
                this.layers = clamppedLayers;
            }
            if (material != this.material)
            {
                dataChanged = true;
                this.material = material;
            }


            if (dataChanged)
            {
                StringBuilder command = new StringBuilder();
                command.Append("0,");
                command.Append(((int)this.material).ToString());
                command.Append(",");
                command.Append(this.strength.ToString());
                command.Append(",");
                command.Append(this.layers.ToString());
                command.Append(",");
                command.Append(this.volume.ToString());
                command.Append("\n");


                if (shoeInstance != null && shoeInstance.IsConnected)
                {
                    Debug.Log("Sending: "+command.ToString());
                    shoeInstance.SendMessage(command.ToString());
                }
            }
        }

        public void SendStaticToShoe(int volume)
        {
            //Meh
            return;
            int clamppedVolume = Mathf.Clamp(volume, 0, 127);

            bool dataChanged = false;

            if (volume != this.volume)
            {
                dataChanged = true;
                this.volume = volume;
            }

            if (dataChanged)
            {
                StringBuilder command = new StringBuilder();
                command.Append("6,");
                command.Append(((int)this.volume).ToString());
                command.Append("\n");


                if (shoeInstance != null && shoeInstance.IsConnected)
                {
                    shoeInstance.SendMessage(command.ToString());
                }
            }
        }

        public void CalibrateMax()
        {
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("1,0,\n");
            }
        }

        public void CalibrateMin()
        {
            //brauchen wir aktuell nicht mehr
            return;
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("4,20,0\n");
            }
        }

        public void ToggleDataTransfer()
        {
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("4,20,0\n");
            }
        }

        public void ReceiveData(Action<int, int> action)
        {
            callback += action;
            //print("1");
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                //print("2");
                if (!alreadyReceivingData)
                {
                    //print("3");
                    alreadyReceivingData = true;
                    shoeInstance.SendMessage("4,20,0\n");
                    shoeInstance.ServerResponse += ProcessData;
                    shoeInstance.StartListening();
                }
            }
        }

        private void ProcessData(object sender, EventArgs e)
        {
            
            Dispatcher.RunOnMainThread(() =>
            {                
                var shoeMessage = sender.ToString();
                
                //Debug.Log(shoeMessage);
                var s = shoeMessage.Split(';');

                var currentPressureLevel = int.Parse(s[1]);
                var adjustedPressureLevel = int.Parse(s[2]);


                callback.Invoke(currentPressureLevel, adjustedPressureLevel);
            });
        }

        void OnApplicationQuit()
        {
            Debug.Log("Removing Shoe");
            shoeInstance?.Dispose();
        }

    }
}