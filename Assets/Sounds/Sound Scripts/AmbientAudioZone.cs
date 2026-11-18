using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(Collider))]
public class AmbientAudioZone : MonoBehaviour
{
    [Header("Configuración de Zona de Audio")]
    [Tooltip("El snapshot único para cuando estás dentro de la zona (música atenuada)")]
    [SerializeField] private AudioMixerSnapshot zoneSnapshot;

    [Tooltip("Tiempo de transición suave en segundos")]
    [SerializeField] private float transitionTime = 1.0f;

    private void Start()
    {
        // Asegurarse de que el collider funcione como Trigger automáticamente
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (zoneSnapshot != null)
            {
                zoneSnapshot.TransitionTo(transitionTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el WaveSpawner en la escena para consultar su estado actual
            WaveSpawner spawner = FindAnyObjectByType<WaveSpawner>();
            if (spawner != null)
            {
                // Si la ola está activa, restauramos la música de batalla; si no, la de descanso
                if (spawner.IsWaveActive)
                {
                    spawner.PlayBattleMusic();
                }
                else
                {
                    spawner.PlayRestMusic();
                }
            }
            else
            {
                Debug.LogWarning("AmbientAudioZone: No se encontró ningún WaveSpawner en la escena al salir de la zona.");
            }
        }
    }
}