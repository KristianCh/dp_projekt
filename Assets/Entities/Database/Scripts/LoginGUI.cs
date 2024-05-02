using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour {
	public DatabaseHandler _sqlHolder;

	private void OnGUI(){
#if UNITY_EDITOR
		if (_sqlHolder == null) return;
		GUI.Label (new Rect (10, 10, 300, 30), "SQL Connection state："+_sqlHolder.GetConnectionState ());
#endif
	}
}
