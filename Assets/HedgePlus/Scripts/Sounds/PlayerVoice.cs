using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Voice", menuName = "Hedge+/Voice Asset", order = -1)]
public class PlayerVoice : ScriptableObject
{
    [System.Serializable] public class VoiceSet
    {
        public string Name;
        [Tooltip ("Will play a random clip if there is more than one given")] public AudioClip[] Clips;
        [Tooltip ("How likely this clip is going to play. Higher value means less probable.")] [Range(0, 10)] public int Probability;
    }

    public VoiceSet[] VoiceSets;

    public AudioClip GetVoiceClip(string Tag)
    {
        AudioClip tmp = null;
        VoiceSet voice = new VoiceSet();
        for (int i = 0; i < VoiceSets.Length; i++)
        {
            if (VoiceSets[i].Name == Tag)
            {
                voice = VoiceSets[i];
            }
        }
        if (voice.Clips.Length > 1)
        {
            int WillPlay = Random.Range(0, voice.Probability + 1);
            if (WillPlay == voice.Probability)
            {
                int RNG = Random.Range(0, voice.Clips.Length);
                tmp = voice.Clips[RNG];
            }
            else
            {
                tmp = null;
            }
        } else if (voice.Clips.Length == 1)
        {
            tmp = voice.Clips[0];
        }
        return tmp;
    }
}
