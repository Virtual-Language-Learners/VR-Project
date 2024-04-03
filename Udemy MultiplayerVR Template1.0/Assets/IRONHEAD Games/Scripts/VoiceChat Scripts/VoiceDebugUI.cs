using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.PUN; // Make sure this namespace still correctly references the Photon Voice components
using Photon.Pun;
using Photon.Voice.Unity;
using System;

public class VoiceDebugUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI voiceState;

    private PunVoiceClient punVoiceClient; // Updated reference

    private void Awake()
    {
        this.punVoiceClient = PunVoiceClient.Instance; // Updated to use PunVoiceClient
    }

    private void OnEnable()
    {
        this.punVoiceClient.Client.StateChanged += this.VoiceClientStateChanged;
    }

    private void OnDisable()
    {
        if (this.punVoiceClient != null) // Added null check to avoid errors in the editor
        {
            this.punVoiceClient.Client.StateChanged -= this.VoiceClientStateChanged;
        }
    }

    private void Update()
    {
        if (this.punVoiceClient == null)
        {
            this.punVoiceClient = PunVoiceClient.Instance; // Updated to use PunVoiceClient
        }
    }

    private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
    {
        this.UpdateUiBasedOnVoiceState(toState);
    }

    private void UpdateUiBasedOnVoiceState(Photon.Realtime.ClientState voiceClientState)
    {
        this.voiceState.text = string.Format("PunVoiceClient: {0}", voiceClientState); // Updated text to match new class name
        if (voiceClientState == Photon.Realtime.ClientState.Joined)
        {
            voiceState.gameObject.SetActive(false);
        }
    }
}
