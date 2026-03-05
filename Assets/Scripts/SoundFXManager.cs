using UnityEngine;

public class SoundFXManager : MonoBehaviour {

    public static SoundFXManager instance;

    //public AudioSource CrystalCollect;

    [SerializeField] private AudioSource soundFXObject;

    //******use this below to play sound clip in any given GameObject's script
    //HEADER //[SerializeField] private AudioClip **NameAudioFile;
    //plug audio clip.MP3 into public holder in Unity
    //PLAY IN SCRIPT BODY //SoundFXManager.instance.PlaySoundFXClip(**NameAudioFile, transform, 1f);
    //1.f is spatial sound 2D

    private void Awake() {
        //CrystalCollect.Play();

        if (instance == null) {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

}
