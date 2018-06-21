using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cs
{

    public enum MusicState { main, mob, boss, off, on };

    public class MusicManager : MonoBehaviour
    {
        public static MusicState musicState = MusicState.main;
        public float fadeSpeed = .02f;
        public AudioSource mainMusic;
        public AudioSource mob;
        public AudioSource boss;
        private bool error = false;
        private MusicState prevState;
        // Use this for initialization



        void Start()
        {
            if (!mainMusic || !mob || !boss)
            {
                Debug.LogWarning("One or more audio source has not been initialized. Please double check that.");
                error = true;
            }
            else
            {
                //set the music variable's initial states
                prevState = MusicState.main;
                mainMusic.volume = 1;
                mob.volume = boss.volume = 0;
                mainMusic.loop = mob.loop = boss.loop = true;
                //start music
                mainMusic.Play();
                mob.Play();
                boss.Play();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(!error)
            {
                if (musicState != prevState)
                {
                    StopCoroutine("ChangeMusic");
                    StartCoroutine("ChangeMusic");
                    prevState = musicState;
                }
            }

        }

        IEnumerator ChangeMusic()
        {
            if (musicState == MusicState.off)
            {
                while (mainMusic.volume > 1 || mob.volume > 1 || boss.volume > 1)
                {
                    mainMusic.volume = Mathf.Max(0.0f, mainMusic.volume - (fadeSpeed*Time.deltaTime));
                    boss.volume = Mathf.Max(0.0f, boss.volume - (fadeSpeed * Time.deltaTime));
                    mob.volume = Mathf.Max(0.0f, mob.volume - (fadeSpeed * Time.deltaTime));
                    yield return null;
                }
            }
            else if (musicState == MusicState.main)
            {
                while (mainMusic.volume < 1 || mob.volume > 0 || boss.volume > 0)
                {
                    mainMusic.volume = Mathf.Min(1.0f, mainMusic.volume + (fadeSpeed * Time.deltaTime));
                    boss.volume = Mathf.Min(1.0f, boss.volume - (fadeSpeed * Time.deltaTime));
                    mob.volume = Mathf.Min(1.0f, mob.volume - (fadeSpeed * Time.deltaTime));
                    yield return null;
                }
            }
            else if (musicState == MusicState.mob)
            {
                while (mob.volume < 1 || boss.volume > 0)
                {
                    mainMusic.volume = Mathf.Min(1.0f, mainMusic.volume + (fadeSpeed * Time.deltaTime));
                    boss.volume = Mathf.Min(1.0f, boss.volume - (fadeSpeed * Time.deltaTime));
                    mob.volume = Mathf.Min(1.0f, mob.volume + (fadeSpeed * Time.deltaTime));
                    yield return null;
                }
            }
            else if (musicState == MusicState.boss)
            {
                while (mainMusic.volume < 1 || mob.volume < 1 || boss.volume < 1)
                {
                    mainMusic.volume = Mathf.Min(1.0f, mainMusic.volume + (fadeSpeed * Time.deltaTime));
                    boss.volume = Mathf.Min(1.0f, boss.volume + (fadeSpeed * Time.deltaTime));
                    mob.volume = Mathf.Min(1.0f, mob.volume + (fadeSpeed * Time.deltaTime));
                    yield return null;
                }
            }
        }
    }

}
