using Mirror;
using Adrenak.UniMic;
using UnityEngine;

/// <summary>
/// Captura el audio del micrófono del jugador local y publica eventos de ruido al bus.
/// Solo se ejecuta en el cliente local (isLocalPlayer) — cada máquina lee su propio micrófono.
///
/// Se pega al prefab del Player.
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public sealed class MicrophoneNoiseSource : NetworkBehaviour
{
    [Header("Microphone")]
    [SerializeField, Tooltip("Frecuencia de muestreo. 44100 Hz es estándar de calidad CD.")]
    private int sampleRate = 44100;

    [SerializeField, Tooltip("Duración del buffer del micrófono en segundos.")]
    private int bufferLengthSeconds = 1;

    [Header("Noise Detection")]
    [SerializeField, Range(0f, 1f), Tooltip("Volumen mínimo para considerarse ruido (0-1). " +
        "Por debajo de esto, no se publica evento.")]
    private float noiseThreshold = 0.05f;

    [SerializeField, Tooltip("Cada cuántos segundos se analiza el volumen y se publica.")]
    private float analysisInterval = 0.1f;

    [SerializeField, Tooltip("Cuántos samples se analizan por chequeo. Más = más preciso pero más lento.")]
    private int samplesToAnalyze = 1024;

    [Header("Debug")]
    [SerializeField, Tooltip("Muestra en consola el volumen actual del micrófono.")]
    private bool showDebugLogs = false;

    [Header("Debug Controls")]
    [SerializeField, Tooltip("Tecla para toggle del detector de ruido (solo debug).")]
    private UnityEngine.InputSystem.Key muteToggleKey = UnityEngine.InputSystem.Key.M;

    [SerializeField, Tooltip("Estado inicial del mute.")]
    private bool isMuted = false;

    private AudioClip microphoneClip;
    private string microphoneDeviceName;
    private float lastAnalysisTime;
    private float[] audioSamples;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Solo el jugador local lee su propio micrófono.
        InitializeMicrophone();
    }

    private void InitializeMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("[MicrophoneNoiseSource] No se detectó ningún micrófono.");
            return;
        }

        // Usa el micrófono por defecto del sistema.
        microphoneDeviceName = Microphone.devices[0];

        // Empieza a grabar. El "loop=true" hace que el buffer se sobrescriba en bucle.
        microphoneClip = Microphone.Start(
            deviceName: microphoneDeviceName,
            loop: true,
            lengthSec: bufferLengthSeconds,
            frequency: sampleRate
        );

        audioSamples = new float[samplesToAnalyze];

        Debug.Log($"[MicrophoneNoiseSource] Micrófono iniciado: {microphoneDeviceName} " +
                  $"a {sampleRate} Hz");
    }

    private void StopMicrophone()
    {
        if (!string.IsNullOrEmpty(microphoneDeviceName))
        {
            Microphone.End(microphoneDeviceName);
            Debug.Log($"[MicrophoneNoiseSource] Micrófono detenido: {microphoneDeviceName}");
        }
    }
    private void Update()
    {
        // Solo el jugador local analiza su propio micrófono.
        if (!isLocalPlayer || microphoneClip == null)
        {
            return;
        }

        // Toggle de mute con tecla configurable.
        HandleMuteToggle();

        // Si está muteado, no analiza ni publica eventos.
        if (isMuted)
        {
            return;
        }

        // Solo analiza cada X segundos, no cada frame (optimización).
        if (Time.time - lastAnalysisTime < analysisInterval)
        {
            return;
        }

        lastAnalysisTime = Time.time;
        AnalyzeMicrophone();
    }
    private void HandleMuteToggle()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard[muteToggleKey].wasPressedThisFrame)
        {
            isMuted = !isMuted;

            // Mutea el detector de ruido (evita eventos).
            SetOwnMicrophoneMute(isMuted);

            // Mutea también el voice chat.
            if (EOSVoiceManager.Instance != null)
            {
                if (isMuted) EOSVoiceManager.Instance.Mute();
                else EOSVoiceManager.Instance.Unmute();
            }

            Debug.Log($"[MicrophoneNoiseSource] 🎙️ Mute detector: {(isMuted ? "ON" : "OFF")}");
        }
    }
    private void SetOwnMicrophoneMute(bool mute)
    {
        if (mute)
        {
            // Detiene tu propio Microphone de Unity (el que usas para el RMS).
            if (Microphone.IsRecording(microphoneDeviceName))
            {
                Microphone.End(microphoneDeviceName);
            }
        }
        else
        {
            // Reanuda la captura de tu micrófono.
            if (!Microphone.IsRecording(microphoneDeviceName))
            {
                microphoneClip = Microphone.Start(
                    deviceName: microphoneDeviceName,
                    loop: true,
                    lengthSec: bufferLengthSeconds,
                    frequency: sampleRate
                );
            }
        }
    }

    private void AnalyzeMicrophone()
    {
        // Obtiene la posición actual del "cabezal de grabación" en el buffer.
        int currentPosition = Microphone.GetPosition(microphoneDeviceName);

        // Calcula desde qué posición leer los últimos samples grabados.
        int startPosition = currentPosition - samplesToAnalyze;
        if (startPosition < 0)
        {
            // Aún no hay suficientes samples grabados.
            return;
        }

        // Lee los últimos samples del buffer.
        microphoneClip.GetData(audioSamples, startPosition);

        float rms = CalculateRMS(audioSamples);

        /* if (showDebugLogs)
        {
            Debug.Log($"[MicrophoneNoiseSource] RMS actual: {rms:F4} (umbral: {noiseThreshold:F4})");
        } */

        // Si el volumen pasa el umbral, publica un evento de ruido.
        if (rms >= noiseThreshold)
        {
            PublishNoiseEvent(rms);
        }
    }

    /// <summary>
    /// Calcula el RMS de un array de samples.
    /// RMS = raíz cuadrada del promedio de los cuadrados de cada sample.
    /// Da un valor entre 0 (silencio) y 1 aprox (máximo).
    /// </summary>
    private static float CalculateRMS(float[] samples)
    {
        float sumOfSquares = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            sumOfSquares += samples[i] * samples[i];
        }

        float mean = sumOfSquares / samples.Length;
        return Mathf.Sqrt(mean);
    }

    private void PublishNoiseEvent(float rms)
    {
        // Normaliza el RMS a intensidad 0-1 (con un factor de amplificación
        // porque el RMS típico está entre 0.05 y 0.3, no llega a 1).
        float intensity = Mathf.Clamp01(rms * 3f);

        NoiseEvent noiseEvent = new NoiseEvent(
            worldPosition: transform.position,
            intensity: intensity,
            source: NoiseSource.Voice,
            sourcePlayerNetId: netId
        );
        NoiseEventBus.Publish(noiseEvent);

        // El cliente le manda al servidor su ruido.
        // El servidor se encargará de publicarlo al bus y replicarlo si es necesario.
        CmdReportNoise(noiseEvent.worldPosition, intensity, noiseEvent.source);
    }

    /// <summary>
    /// [Command] Se ejecuta en el SERVIDOR cuando un cliente reporta que hizo ruido.
    /// Solo el jugador local (dueño del Player) puede llamarlo.
    /// </summary>
    [Command]
    private void CmdReportNoise(Vector3 position, float intensity, NoiseSource source)
    {
        Debug.Log($"[Server] 📨 🟢  Mensaje recibido del cliente. " +
                  $"Player netId: {netId}, " +
                  $"connectionId: {connectionToClient?.connectionId}, " +
                  $"intensidad: {intensity:F2}");

        // Publica en el bus del servidor para que las criaturas escuchen.
        NoiseEvent noiseEvent = new NoiseEvent(
            worldPosition: position,
            intensity: intensity,
            source: source,
            sourcePlayerNetId: netId
        );
        NoiseEventBus.Publish(noiseEvent);

        // Replica a todos los clientes REMOTOS (no al owner ni al server).
        // isServer=true en el host significa que ya publicamos arriba,
        // así que no queremos que el RPC lo vuelva a hacer en el host.
        RpcNotifyNoise(position, intensity, source);
    }

    /// <summary>
    /// [ClientRpc] Se ejecuta en clientes REMOTOS (ni el owner, ni el host-server).
    /// Sirve para HUDs remotos, efectos visuales, o cualquier cosa que necesite ver todos los ruidos.
    /// </summary>
    [ClientRpc(includeOwner = false)]
    private void RpcNotifyNoise(Vector3 position, float intensity, NoiseSource source)
    {
        // Si esta máquina también es el server (host), el evento ya se publicó
        // en CmdReportNoise. Evitamos duplicarlo.
        if (isServer)
        {
            return;
        }

        NoiseEvent noiseEvent = new NoiseEvent(
            worldPosition: position,
            intensity: intensity,
            source: source,
            sourcePlayerNetId: netId
        );
        NoiseEventBus.Publish(noiseEvent);
    }
}
