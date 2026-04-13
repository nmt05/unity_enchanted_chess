using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class sound_util {

	public static void play_sound(AudioClip clip) {
    if (clip != null && data.mem.audioSource != null) {
        data.mem.audioSource.PlayOneShot(clip);
    }
}
	
}