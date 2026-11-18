using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Collider))]
public class AmbientZone : MonoBehaviour
{
    private AudioSource audioSource;
    [Tooltip("Velocidad con la que el sonido hace un fundido (fade) al entrar o salir")]
    public float fadeSpeed = 2f;
    private float targetVolume = 0f;
    private float maxVolume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume; // Guarda el volumen configurado por inspector
        audioSource.volume = 0f;        // Empieza apagado

        // Asegurarse de que el collider sea un trigger
        GetComponent<Collider>().isTrigger = true;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        // Realiza un fundido (fade in / fade out) suave del volumen
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu jugador tenga la etiqueta "Player"
        {
            targetVolume = maxVolume; // Activa el sonido suavemente
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetVolume = 0f; // Apaga el sonido suavemente al salir
        }
    }
}