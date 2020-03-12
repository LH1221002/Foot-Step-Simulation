// - AUTHOR : Pavel Cristian.
// - WHERE SHOULD BE ATTACHED : This script should be attached on the main root of the character, 
//	 on the GameObject the Rigidbody / CharacterController script is attached.
// - PURPOSE OF THE SCRIPT : The purpose of this script is to gather data from the ground below the character and use the
//   data to find a user-defined sound for the type of ground found.

// DISCLAIMER : THIS SCRIPT CAN BE USED IN ANY WAY, MENTIONING MY WORK WILL BE GREATLY APPRECIATED BUT NOT REQUIRED.

using HapticShoes;
using System.Collections;
using UnityEngine;

namespace Footsteps
{

    public enum TriggeredBy
    {
        COLLISION_DETECTION,    // The footstep sound will be played when the physical foot collides with the ground.
        TRAVELED_DISTANCE       // The footstep sound will be played after the character has traveled a certain distance
    }

    public enum ControllerType
    {
        RIGIDBODY,
        CHARACTER_CONTROLLER
    }

    public partial class CharacterFootsteps : MonoBehaviour
    {

        [Tooltip("The method of triggering footsteps.")]
        [SerializeField] TriggeredBy triggeredBy;

        [Tooltip("This is used to determine what distance has to be traveled in order to play the footstep sound.")]
        [SerializeField] float distanceBetweenSteps = 1.8f;

        [Tooltip("To know how much the character moved, a reference to a rigidbody / character controller is needed.")]
        [SerializeField] ControllerType controllerType;
        [SerializeField] Rigidbody characterRigidbody;
        [SerializeField] CharacterController characterController;

        [Tooltip("Use only one audioSource?.")]
        [SerializeField] bool oneAudioSource;

        [Tooltip("The audioSource if you only want one.")]
        [SerializeField] AudioSource audioSource;

        [Tooltip("2 audioSources: the left audioSource")]
        [SerializeField] AudioSource audioSourceLeft;
        [Tooltip("2 audioSources: the right audioSource")]
        [SerializeField] AudioSource audioSourceRight;

        // Random volume between this limits
        [SerializeField] float minVolume = 0.3f;
        [SerializeField] float maxVolume = 0.5f;

        [Tooltip("If this is enabled, you can see how far the script will check for ground, and the radius of the check.")]
        [SerializeField] bool debugMode = true;

        [Tooltip("How high, relative to the character's pivot point the start of the ray is.")]
        [SerializeField] float groundCheckHeight = 0.5f;

        [Tooltip("What is the radius of the ray.")]
        [SerializeField] float groundCheckRadius = 0.5f;

        [Tooltip("How far the ray is casted.")]
        [SerializeField] float groundCheckDistance = 0.3f;

        [Tooltip("What are the layers that should be taken into account when checking for ground.")]
        [SerializeField] LayerMask groundLayers;

        [SerializeField] int amountOfBombs = 5; //set -1 if you want to use placed bombs
        [SerializeField] bool showBombs;
        [SerializeField] GameObject bombPrefab;
        [SerializeField] bool useTimer;
        [SerializeField] int seconds = 25;

        private int width = 10;
        private int height = 16;

        Transform thisTransform;
        RaycastHit currentGroundInfo;
        float stepCycleProgress;
        float lastPlayTime;
        bool previouslyGrounded;
        bool isGrounded;
        private DiamondScore diamondScore;

        Vector2[] bombs;
        GameObject[] bombIndicators;
        Vector2[] goals;
        GameObject[] goalIndicators;
        private int currenti;

        void Awake()
        {
            diamondScore = GameObject.FindGameObjectWithTag("DiamondScore").GetComponent<DiamondScore>();
            if (groundLayers.value == 0)
            {
                groundLayers = 1;
            }

            thisTransform = transform;
            string errorMessage = "";

            if (!audioSource) errorMessage = "No audio source assigned in the inspector, footsteps cannot be played";
            else if (triggeredBy == TriggeredBy.TRAVELED_DISTANCE && !characterRigidbody && !characterController) errorMessage = "Please assign a Rigidbody or CharacterController component in the inspector, footsteps cannot be played";
            else if (!FindObjectOfType<SurfaceManager>()) errorMessage = "Please create a Footstep Database, otherwise footsteps cannot be played, you can create a database" +
                                                                         " by clicking 'FootstepsCreator' in the main menu";

            if (errorMessage != "")
            {
                Debug.LogError(errorMessage);
                enabled = false;
            }
            ActivateGoals();
        }

        public void StartGame(bool b)
        {
            if (!this.gameObject.activeSelf) return;
            ActivateGoals();
            ActivateBombs(b);
        }

        public void ActivateGoals()
        {
            if (!this.gameObject.activeSelf) return;
            currenti = 0;
            goalIndicators = GameObject.FindGameObjectsWithTag("Goal");
            goals = new Vector2[goalIndicators.Length];
            for (int i = 0; i < goalIndicators.Length; i++)
            {
                goals[i] = new Vector2(goalIndicators[i].transform.localPosition.x, goalIndicators[i].transform.localPosition.z);
                goalIndicators[i].SetActive(i == currenti);
            }
        }

        public void ActivateBombs(bool b)
        {
            if (!this.gameObject.activeSelf) return;
            if (amountOfBombs < 0)
            {
                bombIndicators = GameObject.FindGameObjectsWithTag("BombIndicator");
                bombs = new Vector2[bombIndicators.Length];
                for (int i = 0; i < bombIndicators.Length; i++)
                {
                    bombs[i] = new Vector2(bombIndicators[i].transform.localPosition.x, bombIndicators[i].transform.localPosition.z);
                    bombIndicators[i].SetActive(showBombs || b);
                }
            }
            else
            {
                bombs = new Vector2[amountOfBombs];
                for (int i = 0; i < amountOfBombs; i++)
                {
                    bombs[i] = (new Vector2(Random.Range(0, width) + (float)0.5, Random.Range(2, height) + (float)0.5));
                }
                if (showBombs)
                {
                    for (int i = 0; i < bombs.Length; i++)
                    {
                        Instantiate(bombPrefab).transform.localPosition = new Vector3(bombs[i].x, 0, bombs[i].y);
                    }
                }
            }
        }

        public void ClearBombs()
        {
            if (!this.gameObject.activeSelf) return;
            bombIndicators = new GameObject[0];
            bombs = new Vector2[0];
        }

        //void Update()
        //{
        //    CheckGround();

        //    if (triggeredBy == TriggeredBy.TRAVELED_DISTANCE)
        //    {
        //        float speed = (characterController ? characterController.velocity : characterRigidbody.velocity).magnitude;

        //        if (isGrounded)
        //        {
        //            // Advance the step cycle only if the character is grounded.
        //            AdvanceStepCycle(speed * Time.deltaTime);
        //        }
        //    }
        //}

        public enum audioDirection
        {
            left,
            right,
            both
        }

        private void playSound(AudioClip clip, float volume, audioDirection direction)
        {
            if (oneAudioSource)
            {
                audioSource.PlayOneShot(clip, volume);
            }
            else
            {
                switch (direction)
                {
                    case audioDirection.left: audioSourceLeft.PlayOneShot(clip, volume); break;
                    case audioDirection.right: audioSourceRight.PlayOneShot(clip, volume); break;
                    case audioDirection.both: audioSourceLeft.PlayOneShot(clip, volume); audioSourceRight.PlayOneShot(clip, volume); break;
                    default: print("CharacterFootsteps: playSound: direction is null"); break;
                }
            }
        }

        public VibrationData TrySetFootstep(bool b, Vector2 coordinates, RaycastHit col)
        {
            currentGroundInfo = col;
            isGrounded = true;
            audioDirection direction = b ? audioDirection.left : audioDirection.right;
            if (isGrounded)
            {
                return SetFootstep(direction, coordinates);
            }
            return null;
        }

        public void TryPlayFootstep(bool b, Vector2 coordinates, float pressure)
        {
            audioDirection direction = b ? audioDirection.left : audioDirection.right;
            if (isGrounded)
            {
                PlayFootstep(direction, coordinates, pressure);
            }
        }

        void PlayLandSound()
        {
            playSound(SurfaceManager.singleton.GetFootstep(currentGroundInfo.collider, currentGroundInfo.point), 1, audioDirection.both);
        }

        void AdvanceStepCycle(float increment)
        {
            stepCycleProgress += increment;

            if (stepCycleProgress > distanceBetweenSteps)
            {
                stepCycleProgress = 0f;
                SetFootstep(audioDirection.both, new Vector2(-1, -1));
            }
        }

        private bool called = false;
        private int oldGroundId = -1;
        private int oldDistance = -1;
        VibrationData SetFootstep(audioDirection direction, Vector2 coordinates)
        {
            /////////////
            int groundId = SurfaceManager.singleton.GetSurfaceIndex(currentGroundInfo.collider, currentGroundInfo.point);

            int newDistance = 0;

            int strength = 200; //0-200
            int layers = 2;     //2-16
            int amplitude = 0;  //0-127
                                /////////////

            //print("ghuer: "+ groundId + " : " + currentGroundInfo.transform.GetComponent<MeshRenderer>().material.name) ;

            float distance = getDistanceToBombs(coordinates).Left;

            if (distance < 1)
            {
                newDistance = 0;
                strength = 0;
                layers = 15;
                amplitude = 127;
            }
            else if (distance < 2)
            {
                newDistance = 1;
                strength = 20;
                layers = 15;
                amplitude = 127;
            }
            else if (distance < 3 || bombs==null)
            {
                newDistance = 2;
                strength = 58;
                layers = 15;
                amplitude = 45;
            }
            else if (distance < 4)
            {
                newDistance = 3;
                strength = 80;
                layers = 15;
                amplitude = 20;
            }
            //else if (distance < 5)
            //{
            //    newDistance = 4;
            //    strength = 160;
            //    layers = 15;
            //    amplitude = 50;
            //}
            else
            {
                newDistance = -1;
                strength = 90;
                layers = 16;
                amplitude = 2;
            }

            if (newDistance != oldDistance || oldGroundId != groundId)
            {
                oldDistance = newDistance;
                groundId = oldGroundId;
                //print($"Strength {strength} Layers {layers} Amplitude {amplitude}");
                //////////////////
                //sendToShoe(strength, layers, amplitude);
                //////////////////
            }

            return new VibrationData { Strength = strength, Layers = layers, Volume = amplitude, Material = groundId == 5 ? ShoeController.Material.Snow : ShoeController.Material.Wood };
        }

        void PlayFootstep(audioDirection direction, Vector2 coordinates, float pressure)
        {
            //print("playing footstep");
            AudioClip randomFootstep = SurfaceManager.singleton.GetFootstep(currentGroundInfo.collider, currentGroundInfo.point);
            float volume;
            minVolume = (float)0.8;
            maxVolume = 60;
            if (coordinates.x == -1)
                volume = Random.Range(minVolume, maxVolume);
            else
            {
                Pair<float, GameObject> nearestBomb = getDistanceToBombs(coordinates);
                float distance = nearestBomb.Left;
                //print(distance);
                if (distance < 1)
                {
                    volume = maxVolume;
                    explode(nearestBomb.Right);
                }
                else if (distance < 2)
                {
                    volume = 32;
                    //if (pressure > 0.8) explode(nearestBomb.Right);
                }
                else if (distance < 3)
                {
                    volume = 18;
                }
                else if (distance < 4)
                {
                    volume = 10;
                }
                else if (distance < 5)
                {
                    volume = 4;
                }
                else
                {
                    volume = 0.2f;
                }
                //print(distance + " : " + volume);
            }

            if (randomFootstep)
            {
                playSound(randomFootstep, volume, direction);
            }

            if (currenti >= goals.Length) return;
            //print(coordinates +" : "+ goals[currenti]);
            if (Vector2.Distance(coordinates, goals[currenti]) <0.6f)
            {
                //Show progress or smth
                if (currenti >= goalIndicators.Length) return;
                if (diamondScore.IncrementScore())
                {
                    currenti = 0;
                    goalIndicators.Initialize();    //clear the list
                    goals.Initialize();
                }
                goalIndicators[currenti].SetActive(false);
                currenti++;
                if (currenti == goalIndicators.Length)
                {
                    //End
                    print("Everything done");
                }
                else
                {
                    goalIndicators[currenti].SetActive(true);
                }
            }
        }

        private void explode(GameObject bomb)
        {
            //this.GetComponent<Rigidbody>().AddExplosionForce(10, this.transform.position, 5, 3);
            bomb.SetActive(true);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Shoe"))
            {
                if(go.TryGetComponent<ControllerVibration>(out ControllerVibration cv))
                {
                    cv.HardPulse();
                }
            }
            // DO SOMETHING
            //if (called == false)
            //{
            //    called = true;
            //    StartCoroutine(ExampleCoroutine());
            //}
        }

        private Pair<float,GameObject> getDistanceToBombs(Vector2 coordinates)
        {
            float distance = 10;
            int currentMaxI = -1;
            if (bombs!=null)
            {
                for (int i = 0; i < bombs.Length; i++)
                {
                    //distance = Mathf.Min(Vector2.Distance(coordinates, bombs[i]), distance);
                    float currentDistance = Vector2.Distance(coordinates, bombs[i]);
                    if (currentDistance <= distance){
                        distance = currentDistance;
                        currentMaxI = i;
                    }
                }
            }
            return new Pair<float, GameObject>() { Left = distance, Right = currentMaxI >= 0 ? bombIndicators[currentMaxI] : null};
        }

        IEnumerator ExampleCoroutine()
        {
            print("Piep Piep Piep...");
            yield return new WaitForSeconds((float)0.5);

            GameObject.FindGameObjectWithTag("TopDownController").GetComponent<Rigidbody>().AddExplosionForce(100000, GameObject.FindGameObjectWithTag("TopDownController").transform.position, 5, 4);
            called = false;
        }

        void OnDrawGizmos()
        {
            if (debugMode)
            {
                Gizmos.DrawWireSphere(transform.position + Vector3.up * groundCheckHeight, groundCheckRadius);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + Vector3.up * groundCheckHeight, Vector3.down * (groundCheckDistance + groundCheckRadius));
            }
        }

        void CheckGround()
        {
            previouslyGrounded = isGrounded;
            Ray ray = new Ray(thisTransform.position + Vector3.up * groundCheckHeight, Vector3.down);

            if (Physics.SphereCast(ray, groundCheckRadius, out currentGroundInfo, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            if (!previouslyGrounded && isGrounded)
            {
                PlayLandSound();
            }
            // print(isGrounded);
        }
    }
}
