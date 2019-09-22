using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings0ld : MonoBehaviour{

	[SerializeField] Text lockText;
	[SerializeField] Text autocalibrateText;

	ConnectionDisplay currentDisplay = null;

	private void OnEnable() {
		currentDisplay = null;
		if(ConnectionsDisplayer.Instance.Handles.Count > 0) {
			foreach(ConnectionsDisplayer.DisplayedConnectionHandle handle in ConnectionsDisplayer.Instance.Handles) {
				if(handle.connection.paired) {
					currentDisplay = handle.display;
				}
			}

			if(currentDisplay) {
				//handle lock text
				if(currentDisplay.Connection.lockedId == "free") {
					lockText.text = "Lock Device";
				} else {
					lockText.text = "Unlock Device";
				}
				autocalibrateText.text = "(no autocalibration data available)";
			}
		}
	}

	public void ClickLock() {
		if(currentDisplay) {
			currentDisplay.OnClickLock();
			lockText.text = "Processing... " + currentDisplay.Connection.lockedId == "free" ? "Locking" : "Unlocking";
			StartCoroutine(waitThenShowLockStatus());
		}
	}

	IEnumerator waitThenShowLockStatus() {
		yield return new WaitForSeconds(2);
		if(currentDisplay) {
			if(currentDisplay.Connection.lockedId == "free") {
				lockText.text = "Lock Device";
			} else {
				lockText.text = "Unlock Device";
			}
		}
	}

	public void ClickAutocalibrateStart() {
		if(currentDisplay) {
			currentDisplay.OnClickAutocalibration();
		}
	}

	public void ClickAutocalibrationStop() {
		if(currentDisplay)
			currentDisplay.OnClickStopAutocalibration();
	}

	public void ClickAutocalibrationReset() {
		if(currentDisplay)
			currentDisplay.OnClickResetAutocalibration();
	}

	private void Update() {
		if(currentDisplay.LastAutocalibrationCommand == 1) {
			autocalibrateText.text = "Drift detected: " + string.Format("{0:0.#}°/s", currentDisplay.LastAutocalibrationDrift);
		} else {
			autocalibrateText.text = "(no autocalibration data available)";
		}
	}

}
