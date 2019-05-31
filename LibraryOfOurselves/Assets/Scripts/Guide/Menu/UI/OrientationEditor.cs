using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationEditor : MonoBehaviour{

	[SerializeField] GameObject singleOrientationEditorPrefab;

	List<Vector4> orientations;

	public void ResetOrientations(bool addAnEmpty) {
		//Erase everything
		for(int i = 0; i<transform.childCount-1; ++i) {
			Destroy(transform.GetChild(i).gameObject);
		}
		orientations = new List<Vector4>();
		if(addAnEmpty) AddOrientation();
	}

	public void AddOrientation() {
		AddOrientation(Vector4.zero);
	}

	public void AddOrientation(Vector4 orientation) {
		Transform lastChild = transform.GetChild(transform.childCount - 1);
		SingleOrientationEditor soe = Instantiate(singleOrientationEditorPrefab, transform).GetComponent<SingleOrientationEditor>();
		lastChild.SetAsLastSibling();

		soe.DeltaAngle = orientation;
	}

	public List<Vector4> GetValues() {
		List<Vector4> ret = new List<Vector4>();
		for(int i = 0; i < transform.childCount - 1; ++i) {
			SingleOrientationEditor soe = transform.GetChild(i).GetComponent<SingleOrientationEditor>();
			if(soe) {
				if(soe.DeltaAngle != Vector4.zero) {
					ret.Add(soe.DeltaAngle);
				}
			}
		}
		return ret;
	}

}
