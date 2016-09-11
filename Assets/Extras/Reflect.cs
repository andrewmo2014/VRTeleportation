using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class Reflect : MonoBehaviour {

	public int fidelity = 9;

	// Use this for initialization
	void Start () {

		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material = Resources.Load<Material>("Mirror");

		tag = "Reflect";

		gameObject.AddComponent<MirrorReflection>();
		MirrorReflection reflection = GetComponent<MirrorReflection>();
		reflection.m_TextureSize = (int)Mathf.Pow(2,fidelity);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
