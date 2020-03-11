using UnityEngine;
using UnityEngine.UI;

namespace Footsteps {

	public class DemoUI : MonoBehaviour {

		[SerializeField] readonly GameObject topDownController;
		[SerializeField] readonly GameObject topDownCamera;
		[SerializeField] readonly GameObject firstPersonController;


		public void ActivateTopDown() {
			if(!topDownController.activeSelf) topDownController.transform.position = firstPersonController.transform.position;

			firstPersonController.SetActive(false);
			topDownController.SetActive(true);
			topDownCamera.SetActive(true);
		}

		public void ActivateFirstPerson() {
			if(!firstPersonController.activeSelf) firstPersonController.transform.position = topDownController.transform.position;

			firstPersonController.SetActive(true);
			topDownController.SetActive(false);
			topDownCamera.SetActive(false);
		}
	}
}
