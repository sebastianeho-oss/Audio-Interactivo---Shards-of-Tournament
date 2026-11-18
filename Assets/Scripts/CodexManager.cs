using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class CodexManager : MonoBehaviour
{
    public static CodexManager Instance;

    [Header("UI References")]
    public GameObject codexPanel;
    public TMP_Text textComponent;
    public Button closeButton;

    [Header("HUD Counter Reference")]
    public TMP_Text counterHUDText;

    [Header("Pages Text & Audio Lists")]
    [TextArea(5, 15)]
    public List<string> pageTexts = new List<string>();
    public List<AudioClip> pageAudios = new List<AudioClip>();

    [Header("Audio Configuration")]
    [Tooltip("Arrastra el Audio Mixer principal (MainAudioMixer)")]
    public AudioMixer audioMixer;

    [Tooltip("El grupo 'Page' de tu Audio Mixer para que la voz salga por ahí")]
    public AudioMixerGroup pageAudioGroup;

    [Header("Snapshots")]
    [Tooltip("Snapshot al que debe volver al cerrar (ej. tu estado normal o de gameplay)")]
    public AudioMixerSnapshot defaultSnapshot;

    [Tooltip("Snapshot cuando el códice está abierto (música y efectos bajos)")]
    public AudioMixerSnapshot codexOpenSnapshot;

    [Tooltip("Tiempo de transición suave en segundos")]
    public float transitionTime = 0.25f;

    private AudioSource pageAudioSource;
    private int collectedPagesCount = 0;

    public int CollectedPagesCount => collectedPagesCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (codexPanel != null)
            codexPanel.SetActive(false);

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCodex);
        }

        // Configurar automáticamente el AudioSource y asignarle el grupo 'Page' de tu mixer
        pageAudioSource = GetComponent<AudioSource>();
        if (pageAudioSource == null)
        {
            pageAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (pageAudioGroup != null)
        {
            pageAudioSource.outputAudioMixerGroup = pageAudioGroup;
        }

        UpdateHUDCounter();
    }

    public void PageCollected()
    {
        collectedPagesCount++;
        UpdateHUDCounter();
    }

    private void UpdateHUDCounter()
    {
        if (counterHUDText != null)
        {
            int totalPages = pageTexts != null ? pageTexts.Count : 0;
            counterHUDText.text = $"Paginas: {collectedPagesCount} / {totalPages}";
        }
    }

    public bool HasCollectedAllPages(int totalRequired)
    {
        return collectedPagesCount >= totalRequired;
    }

    public void OpenCodex(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pageTexts.Count) return;

        // Mostrar texto
        if (textComponent != null)
        {
            textComponent.text = pageTexts[pageIndex];
        }

        // Reproducir audio de la página (solo una vez)
        if (pageAudioSource != null && pageAudios != null && pageIndex < pageAudios.Count)
        {
            AudioClip clipToPlay = pageAudios[pageIndex];
            if (clipToPlay != null)
            {
                pageAudioSource.Stop();
                pageAudioSource.clip = clipToPlay;
                pageAudioSource.loop = false;
                pageAudioSource.Play();
            }
        }

        // Activar el Snapshot del Códice en el Mixer (baja música/SFX)
        if (codexOpenSnapshot != null)
        {
            codexOpenSnapshot.TransitionTo(transitionTime);
        }

        if (codexPanel != null)
        {
            codexPanel.SetActive(true);
        }

        if (FindAnyObjectByType<GameManager>() != null)
        {
            FindAnyObjectByType<GameManager>().OpenCodexGame();
        }
    }

    public void CloseCodex()
    {
        // Interrumpir el audio de la página inmediatamente
        if (pageAudioSource != null && pageAudioSource.isPlaying)
        {
            pageAudioSource.Stop();
        }

        // Regresar al Snapshot predeterminado (ej. Rest State o Battle State)
        if (defaultSnapshot != null)
        {
            defaultSnapshot.TransitionTo(transitionTime);
        }

        if (codexPanel != null)
        {
            codexPanel.SetActive(false);
        }

        if (FindAnyObjectByType<GameManager>() != null)
        {
            FindAnyObjectByType<GameManager>().CloseCodexGame();
        }
    }
}