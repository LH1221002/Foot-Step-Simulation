using UnityEngine;
using System.Collections;
using HapticShoes;

namespace Footsteps {

	[RequireComponent(typeof(Collider), typeof(Rigidbody))]
	public class FootstepTrigger : MonoBehaviour {

		Collider thisCollider;
		CharacterFootsteps footsteps;
		public bool useShoeDeviceDate = false;

        private ShoeController sc;

        private bool iAmLeft;


        void Awake() {

            if (useShoeDeviceDate)
            {
                this.GetComponent<Collider>().enabled = false;
            }
            else
            {
                this.GetComponent<Collider>().enabled = !(true==false);
                SetCollisions();
            }

        }
		IEnumerator Start() {

            yield return new WaitForSeconds(2);
            sc = GetComponent<ShoeController>();

            sc.ReceiveData((int raw, int scaled) => {
                handlePressure(scaled / 255f);
            });

            if (this.gameObject.tag == "FootStepTriggerL")
            {
                iAmLeft = true;
            }
            else
            {
                iAmLeft = false;
            }

			thisCollider = GetComponent<Collider>();
			footsteps = GetComponentInParent<CharacterFootsteps>();
			Rigidbody thisRigidbody = GetComponent<Rigidbody>();

			if(thisCollider) {
				thisCollider.isTrigger = true;
				SetCollisions();
			}

			if(thisRigidbody) thisRigidbody.isKinematic = true;

			string errorMessage = "";

			if(!footsteps) errorMessage = "No 'CharacterFootsteps' script found as a parent, this footstep trigger will not work";
			else if(!thisCollider) errorMessage = "Please attach a collider marked as a trigger to this gameobject, this footstep trigger will not work";
			else if(!thisRigidbody) errorMessage = "Please attach a rigidbody to this gameobject, this footstep trigger will not work";

			if(errorMessage != "") {
				Debug.LogError(errorMessage);
				//enabled = false;

				yield break;
			}
		}


		private void Update()
		{
			if (useShoeDeviceDate)
			{
				if (Physics.Raycast(this.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit))
				{
					//offsetDistance = hit.distance;
					//Debug.DrawLine(transform.position, hit.point, Color.cyan, 2, false);
					VibrationData vd = updatePosition(hit.collider);
                    if (vd == null) return;
                    if(sc) sc.SendToShoe(vd.Strength, vd.Material, vd.Volume, vd.Layers);
                }
                else
                {
                    if (sc) sc.SendToShoe(255);


                }

			}
		}

		private VibrationData updatePosition(Collider other) {
			if(footsteps) {
                return footsteps.TrySetFootstep(iAmLeft, other.transform.localPosition);// new Vector2(other.transform.localPosition.x, other.transform.localPosition.y));
            }
            return null;
		}

		public void handlePressure(float pressure)
		{
			if (useShoeDeviceDate)
			{
				if (Physics.Raycast(this.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit))
				{
					if (footsteps)
					{
						footsteps.TryPlayFootstep(iAmLeft, hit.collider.transform.localPosition, pressure);

						GameObject.FindGameObjectWithTag("TestCube").GetComponent<Rigidbody>().AddExplosionForce(600, hit.collider.transform.position, 4, 6);
					}
				}
			}
		}

		void SetCollisions() {
			if(!footsteps) return;

			Collider[] allColliders = footsteps.GetComponentsInChildren<Collider>();

			foreach(var collider in allColliders) {
				if(collider != GetComponent<Collider>()) {
					Physics.IgnoreCollision(thisCollider, collider);
				}
			}
		}
	}
}
