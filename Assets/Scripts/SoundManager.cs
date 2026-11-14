using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.4f;
    public bool isMuted = false;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private AudioClip coinClip;
    private AudioClip keyClip;
    private AudioClip chestClip;
    private AudioClip jumpClip;
    private AudioClip altarClip;
    private AudioClip portalClip;
    private AudioClip trapClip;
    private AudioClip victoryClip;
    private AudioClip interactClip;
    private AudioClip footstepClip;
    private AudioClip waterSplashClip;
    private AudioClip hurtClip;
    private AudioClip levelCompleteClip;
    private AudioClip gameOverClip;

    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        GenerateProceduralClips();
    }

    private void Start()
    {
        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        if (sfxSource != null)
            sfxSource.volume = isMuted ? 0f : sfxVolume;

        if (musicSource != null)
            musicSource.volume = isMuted ? 0f : musicVolume;
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateVolumes();
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol);
        UpdateVolumes();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = Mathf.Clamp01(vol);
        UpdateVolumes();
    }

    public void PlaySound(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
    {
        if (clip == null || isMuted || sfxSource == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null || isMuted) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeScale);
    }

    public void PlayCoinSound() => PlaySound(coinClip, 0.7f, Random.Range(0.95f, 1.15f));
    public void PlayKeySound() => PlaySound(keyClip, 0.9f);
    public void PlayChestSound() => PlaySound(chestClip, 1f);
    public void PlayJumpSound() => PlaySound(jumpClip, 0.5f, Random.Range(0.9f, 1.1f));
    public void PlayAltarSound() => PlaySound(altarClip, 0.85f);
    public void PlayPortalSound() => PlaySound(portalClip, 0.8f);
    public void PlayTrapSound() => PlaySound(trapClip, 0.9f);
    public void PlayVictorySound() => PlaySound(victoryClip, 1f);
    public void PlayInteractSound() => PlaySound(interactClip, 0.6f);
    public void PlayFootstep() => PlaySound(footstepClip, 0.25f, Random.Range(0.85f, 1.15f));
    public void PlayWaterSplash() => PlaySound(waterSplashClip, 1f, Random.Range(0.9f, 1.1f));
    public void PlayHurt() => PlaySound(hurtClip, 0.9f);
    public void PlayLevelComplete() => PlaySound(levelCompleteClip, 1f);
    public void PlayGameOver() => PlaySound(gameOverClip, 1f);

    public void PlayMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip targetClip = null;
        if (sceneName.Contains("1") || sceneName.ToLower().Contains("island") || sceneName.ToLower().Contains("main"))
        {
            if (!musicClips.TryGetValue("Island", out targetClip))
            {
                targetClip = CreateProceduralAmbientTrack(220f, 330f, 440f, 16f);
                musicClips["Island"] = targetClip;
            }
        }
        else if (sceneName.Contains("2") || sceneName.ToLower().Contains("temple") || sceneName.ToLower().Contains("dungeon"))
        {
            if (!musicClips.TryGetValue("Temple", out targetClip))
            {
                targetClip = CreateProceduralAmbientTrack(164.81f, 220f, 261.63f, 18f); // E minor mysterious vibe
                musicClips["Temple"] = targetClip;
            }
        }
        else
        {
            if (!musicClips.TryGetValue("Vault", out targetClip))
            {
                targetClip = CreateProceduralAmbientTrack(261.63f, 329.63f, 392f, 14f); // C major royal vibe
                musicClips["Vault"] = targetClip;
            }
        }

        if (musicSource.clip != targetClip)
        {
            musicSource.clip = targetClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void GenerateProceduralClips()
    {
        coinClip = CreateToneSequence(new float[] { 987.77f, 1318.51f }, new float[] { 0.08f, 0.16f }, Waveform.Sine);
        keyClip = CreateToneSequence(new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }, new float[] { 0.09f, 0.09f, 0.09f, 0.28f }, Waveform.Triangle);
        chestClip = CreateToneSequence(new float[] { 220f, 293.66f, 370f, 440f, 587.33f }, new float[] { 0.12f, 0.12f, 0.15f, 0.18f, 0.45f }, Waveform.SquareHarmonic);
        jumpClip = CreateToneSweep(250f, 600f, 0.14f, Waveform.Sine);
        altarClip = CreateToneSequence(new float[] { 330f, 440f, 554.37f, 659.25f }, new float[] { 0.15f, 0.15f, 0.18f, 0.4f }, Waveform.Sine);
        portalClip = CreateToneSweep(200f, 800f, 0.7f, Waveform.Triangle);
        trapClip = CreateToneSequence(new float[] { 140f, 90f }, new float[] { 0.1f, 0.2f }, Waveform.Noise);
        victoryClip = CreateToneSequence(
            new float[] { 523.25f, 523.25f, 523.25f, 659.25f, 783.99f, 1046.50f },
            new float[] { 0.12f, 0.12f, 0.12f, 0.2f, 0.25f, 0.6f },
            Waveform.SquareHarmonic
        );
        interactClip = CreateToneSequence(new float[] { 600f, 800f }, new float[] { 0.05f, 0.08f }, Waveform.Sine);
        footstepClip = CreateToneSweep(120f, 70f, 0.07f, Waveform.Noise);

        // Water splash: rapid white noise burst followed by low frequency bubbling resonance
        waterSplashClip = CreateToneSweep(800f, 150f, 0.45f, Waveform.Noise);

        // Hurt: abrupt low impact punch
        hurtClip = CreateToneSequence(new float[] { 180f, 90f }, new float[] { 0.08f, 0.16f }, Waveform.SquareHarmonic);

        // Level Complete: triumphant rising major fanfare
        levelCompleteClip = CreateToneSequence(
            new float[] { 523.25f, 659.25f, 783.99f, 1046.50f },
            new float[] { 0.12f, 0.12f, 0.15f, 0.5f },
            Waveform.SquareHarmonic
        );

        // Game Over: melancholy descending minor motif
        gameOverClip = CreateToneSequence(
            new float[] { 440f, 415.30f, 370f, 329.63f },
            new float[] { 0.25f, 0.25f, 0.3f, 0.7f },
            Waveform.Triangle
        );
    }

    private enum Waveform { Sine, Triangle, SquareHarmonic, Noise }

    private AudioClip CreateToneSequence(float[] freqs, float[] durations, Waveform form)
    {
        int sampleRate = 44100;
        float totalDuration = 0f;
        for (int i = 0; i < durations.Length; i++) totalDuration += durations[i];

        int totalSamples = Mathf.CeilToInt(sampleRate * totalDuration);
        float[] samples = new float[totalSamples];

        int sampleOffset = 0;
        for (int i = 0; i < freqs.Length; i++)
        {
            float freq = freqs[i];
            float dur = durations[i];
            int count = Mathf.CeilToInt(sampleRate * dur);

            for (int s = 0; s < count && (sampleOffset + s) < totalSamples; s++)
            {
                float t = (float)s / sampleRate;
                float envelope = Mathf.Clamp01(1f - (float)s / count);
                // Soft attack
                if (s < sampleRate * 0.01f) envelope *= (s / (sampleRate * 0.01f));

                float val = 0f;
                float phase = 2f * Mathf.PI * freq * t;
                switch (form)
                {
                    case Waveform.Sine:
                        val = Mathf.Sin(phase);
                        break;
                    case Waveform.Triangle:
                        val = Mathf.PingPong(t * freq * 2f, 1f) * 2f - 1f;
                        break;
                    case Waveform.SquareHarmonic:
                        val = Mathf.Sin(phase) + 0.35f * Mathf.Sin(phase * 2f) + 0.15f * Mathf.Sin(phase * 3f);
                        break;
                    case Waveform.Noise:
                        val = Random.Range(-1f, 1f) * Mathf.Exp(-t * 20f);
                        break;
                }
                samples[sampleOffset + s] = val * envelope * 0.7f;
            }
            sampleOffset += count;
        }

        AudioClip clip = AudioClip.Create("ProceduralSFX", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateToneSweep(float startFreq, float endFreq, float duration, Waveform form)
    {
        int sampleRate = 44100;
        int totalSamples = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int s = 0; s < totalSamples; s++)
        {
            float t = (float)s / sampleRate;
            float progress = (float)s / totalSamples;
            float currentFreq = Mathf.Lerp(startFreq, endFreq, progress);
            float envelope = Mathf.Sin(progress * Mathf.PI);

            float val = 0f;
            if (form == Waveform.Noise)
            {
                val = Random.Range(-1f, 1f);
            }
            else
            {
                val = Mathf.Sin(2f * Mathf.PI * currentFreq * t);
            }
            samples[s] = val * envelope * 0.6f;
        }

        AudioClip clip = AudioClip.Create("ProceduralSweep", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateProceduralAmbientTrack(float f1, float f2, float f3, float duration)
    {
        int sampleRate = 22050; // low rate fine for calm ambient chord drone
        int totalSamples = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int s = 0; s < totalSamples; s++)
        {
            float t = (float)s / sampleRate;
            float lfo1 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * 0.15f * t);
            float lfo2 = 0.5f + 0.5f * Mathf.Sin(2f * Mathf.PI * 0.08f * t);

            float tone1 = Mathf.Sin(2f * Mathf.PI * f1 * t) * 0.25f;
            float tone2 = Mathf.Sin(2f * Mathf.PI * f2 * t) * 0.2f * lfo1;
            float tone3 = Mathf.Sin(2f * Mathf.PI * f3 * t) * 0.15f * lfo2;

            // subtle gentle fade at ends for clean loop
            float loopFade = 1f;
            float edge = sampleRate * 0.5f;
            if (s < edge) loopFade = s / edge;
            else if (s > totalSamples - edge) loopFade = (totalSamples - s) / edge;

            samples[s] = (tone1 + tone2 + tone3) * loopFade * 0.35f;
        }

        AudioClip clip = AudioClip.Create("AmbientLoop", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
