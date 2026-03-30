using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CADGscript : MonoBehaviour
{
	public AudioSource clackSFX;
	public AudioSource harpSFX;
	private float volumeSFX;
	private int hitCounter;


	private void Start()
	{
		volumeSFX = 1.0f;
		hitCounter = 0;
	}


	private void Update()
	{
		if (this.GetComponent<BoxCollider>().enabled == false)
		{
			StartCoroutine(StartDelay());
		}

		StartCoroutine(EndDelay());

		StartCoroutine(LoadLevel());

	}

	void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag("Clack"))
		{
			clackSFX.Play();
			volumeSFX = volumeSFX - 0.25f;
			clackSFX.volume = volumeSFX;
			hitCounter++;
		}

	}

	private IEnumerator LoadLevel()
	{
		yield return new WaitForSeconds(5);
		//SceneManager.LoadScene(1); // load level by name
		SceneManager.LoadScene("Scenes/MainMenu");
	}

	private IEnumerator EndDelay()
	{
		yield return new WaitForSeconds(1.27f);
		Camera.main.orthographic = true;
	}

	private IEnumerator StartDelay()
	{
		yield return new WaitForSeconds(.25f);
		this.GetComponent<BoxCollider>().enabled = true;

		yield return new WaitForSeconds(.1f);
		harpSFX.Play();
	}

}
