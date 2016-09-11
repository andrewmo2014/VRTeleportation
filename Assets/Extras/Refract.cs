using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider))]
public class Refract : MonoBehaviour {

	public float indexOfRefraction;

	// Use this for initialization
	void Start () {

		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material = Resources.Load<Material>("Glass");

		tag = "Refract";

		MeshFilter mf = GetComponent<MeshFilter>();
	
		MeshCollider mc = GetComponent<MeshCollider>();
		mc.sharedMesh = mf.sharedMesh;

		gameObject.AddComponent<MeshCollider>();
		MeshCollider[] mcs = GetComponents<MeshCollider>();
		MeshCollider mcInverted = mcs[mcs.Length - 1];

		gameObject.AddComponent<ReverseNormals>();
		ReverseNormals revNorms = GetComponent<ReverseNormals>();
		revNorms.sharedMesh = mc.sharedMesh;
		revNorms.mc = mcInverted;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
