using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IfCodeDo : MonoBehaviour {

	[SerializeField] string codeHash = "67-0B-14-72-8A-D9-90-2A-EC-BA-32-E2-2F-A4-F6-BD";//default code is "000000"
	[SerializeField] UnityEvent onPass;
	[SerializeField] UnityEvent onFail;
	[SerializeField] bool verbose = false;
	
	public void Check(string check){
		string checkHash = Hashing.Md5(check);
		if(checkHash == codeHash){
			if(verbose) print("Both hashes are identical: " + checkHash);
			onPass.Invoke();
		}else{
			if(verbose) print(checkHash + " is not " + codeHash);
			onFail.Invoke();
		}
	}
	
}
