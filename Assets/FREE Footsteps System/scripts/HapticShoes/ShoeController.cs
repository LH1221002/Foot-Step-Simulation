using System;
using System.Text;
using UnityEngine;

namespace HapticShoes
{
    /// <summary>
    /// ShoeController
    /// 
    /// This is the class on which methods are to be called when wanting to communicate with the shoe device
    /// </summary>
    
    public class ShoeController : MonoBehaviour
    {
        [Tooltip("IP Adress of the Shoe")]
        public string IPAddress = "192.168.0.106";                  // The shoe needs to be connected to the local hotspot of the computer

        private Action<int, int> callback;
        private ServerHandler shoeInstance;

        private bool alreadyReceivingData = false;

        public enum Material { WindowsStartupSound, Wood, Snow };   // The material which should be simulated (concrete configurations of each material are set on the shoe, 
                                                                    // change of the material ==> change of the soundclip (and i.e. for snow: no vibration when reducing the pressure again (snow is not elastic like wood))

        private int strength = 0;                                   // how much pressure needs to be applied before the vibration starts ( 0-255 )
        private Material material = Material.Wood;          
        private int volume = 0;                                     // amplitude ( 0-127 )
        private int layers = 0;                                     // amount of layers which are used to create compliance ( 2-16 , usually 15 is used)

        public void Start()
        {
            Connect(IPAddress);
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

        /// <summary>
        /// This method needs to be called to send new values to the shoe
        /// 
        /// The sent values will be saved on the shoe device until new ones will be send.
        /// Calling this method will not cause the shoe to vibrate, it only sets the values of the vibration played when applying enough pressure on the sensor.
        /// </summary>
        /// <param name="strength"> "Threshold" 0-255 </param> 
        /// <param name="material"> The material which should be simulated (see above) </param> 
        /// <param name="volume"> "Amplitude" 0-127 </param>   
        /// <param name="layers"> 2-16 </param>   
        public void SendToShoe(int strength, Material material = Material.Wood, int volume = 127, int layers = 15)
        {
            if (ToggleMapAndSetup.UseStaticVibration)
            {
                SendStaticToShoe(volume);
                return;
            }
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
                    //Debug.Log("Sending: "+command.ToString());
                    shoeInstance.SendMessage(command.ToString());
                }
            }
        }

        // This method is used if you want to use the static vibration, which is only modfiable in the amplitude.
        // The static vibration will not be adjusted to the pressure sensor. It is static and will be played as soon as pressure is detected.
        public void SendStaticToShoe(int volume)
        {
            //Meh
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

        /// <summary>
        /// This method is used to let the shoes vibrate manually.
        /// The duration of the vibration is very short, so this method is used to be called in an update function like OnCollisionStay.
        /// </summary>
        /// <param name="volum"> volume/amplitude </param> 
        public void SendVibrationToShoe(int volum = 127)
        {
            int clamppedVolume = Mathf.Clamp(volum, 0, 127);

            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("8," + clamppedVolume.ToString() + ",0\n");
            }

        }

        /// <summary>
        /// This method is used to let the shoes vibrate very strong for ~1s.
        /// </summary>
        public void SendExplodeToShoe()
        {

            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("7,0\n");
            }

        }

        /// <summary>
        /// Intializes the calibration of the weight of the user.
        /// </summary>
        /// <returns> false if the shoe is not connected yet </returns>
        public bool CalibrateMax()
        {
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                shoeInstance.SendMessage("1,0,\n");
                return true;
            }
            else return false;
        }

        public void CalibrateMin()
        {
            //Not needed anymore
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
            if (shoeInstance != null && shoeInstance.IsConnected)
            {
                if (!alreadyReceivingData)
                {
                    alreadyReceivingData = true;
                    ToggleDataTransfer();
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