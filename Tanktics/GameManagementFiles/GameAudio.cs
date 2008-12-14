//prebuilt..sample code.
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Tanktics
{
    public class GameAudio
    {

        //Audio Engine Items
        private AudioEngine m_Engine;
        private WaveBank m_WaveBank;
        private SoundBank m_SoundBank;

        //New Audio Struct
        public struct audioItem
        {
            public Cue audioCue;
            public bool isPlaying;
            public bool isPaused;
        };

        //List of game audio
        public List<audioItem> m_gameAudio = new List<audioItem>();

        /// <summary>
        /// GameAudio constructor
        /// </summary>
        public GameAudio()
        {
            m_Engine = new AudioEngine("Content\\Audio\\GameSounds.xgs");
            m_WaveBank = new WaveBank(m_Engine, "Content\\Audio\\Wave Bank.xwb");
            m_SoundBank = new SoundBank(m_Engine, "Content\\Audio\\Sound Bank.xsb");
        }

        /// <summary>
        /// GameAudio destructor
        /// </summary>
        ~GameAudio()
        {
            m_SoundBank.Dispose();
            m_WaveBank.Dispose();
            m_Engine.Dispose();
        }

        /// <summary>
        /// Adds a sound using sound filename
        /// </summary>
        /// <param name="sound"></param>
        public void AddSound(String sound)
        {
            audioItem newAudio;
            newAudio.audioCue = m_SoundBank.GetCue(sound);
            newAudio.isPaused = false;
            newAudio.isPlaying = false;
            m_gameAudio.Add(newAudio);
        }

        /// <summary>
        /// Plays a sound based on sound filename
        /// </summary>
        /// <param name="sound"></param>
        public void PlaySound(String sound)
        {
            //Default variables
            int position = 0;
            bool found = false;

            //Look for sound in list
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].audioCue.Name == sound)
                {
                    //Found sprite!
                    found = true;
                    position = i;
                }
            }

            //Can't update without an old sound object
            if (found)
            {
                audioItem localvar;
                localvar.isPaused = false;
                localvar.isPlaying = true;
                localvar.audioCue = m_SoundBank.GetCue(m_gameAudio[position].audioCue.Name);

                m_gameAudio[position] = localvar;
                m_gameAudio[position].audioCue.Play();
            }
        }

        /// <summary>
        /// Pauses specified sound
        /// </summary>
        /// <param name="sound"></param>
        public void PauseSound(String sound)
        {
            //Default variables
            int position = 0;
            bool found = false;

            //Look for sound in list
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].audioCue.Name == sound)
                {
                    //Found sprite!
                    found = true;
                    position = i;
                }
            }

            //Can't update without an old sound object
            if (found && m_gameAudio[position].isPlaying)
            {
                audioItem localvar;
                localvar.isPaused = true;
                localvar.isPlaying = false;
                localvar.audioCue = m_gameAudio[position].audioCue;

                m_gameAudio[position] = localvar;

                m_gameAudio[position].audioCue.Pause();
            }
        }

        /// <summary>
        /// Resumes specified sound
        /// </summary>
        /// <param name="sound"></param>
        public void UnpauseSound(String sound)
        {
            //Default variables
            int position = 0;
            bool found = false;

            //Look for sound in list
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].audioCue.Name == sound)
                {
                    //Found sprite!
                    found = true;
                    position = i;
                }
            }

            //Can't update without an old sound object
            if (found && m_gameAudio[position].isPaused)
            {
                audioItem localvar;
                localvar.isPaused = false;
                localvar.isPlaying = true;
                localvar.audioCue = m_gameAudio[position].audioCue;

                m_gameAudio[position] = localvar;

                m_gameAudio[position].audioCue.Resume();
            }
        }

        /// <summary>
        /// Stops specified sound
        /// </summary>
        /// <param name="sound"></param>
        public void StopSound(String sound)
        {
            //Default variables
            int position = 0;
            bool found = false;

            //Look for sound in list
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].audioCue.Name == sound)
                {
                    //Found sprite!
                    found = true;
                    position = i;
                }
            }

            //Can't update without an old sound object
            if (found && m_gameAudio[position].isPlaying)
            {
                audioItem localvar;
                localvar.isPaused = false;
                localvar.isPlaying = false;
                localvar.audioCue = m_gameAudio[position].audioCue;

                m_gameAudio[position] = localvar;

                m_gameAudio[position].audioCue.Stop(AudioStopOptions.Immediate);
            }
        }

        /// <summary>
        /// Stops all playing/paused sounds
        /// </summary>
        public void StopAll()
        {
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].isPlaying || m_gameAudio[i].isPaused)
                {
                    audioItem localvar;
                    localvar.isPaused = false;
                    localvar.isPlaying = false;
                    localvar.audioCue = m_gameAudio[i].audioCue;

                    m_gameAudio[i] = localvar;

                    m_gameAudio[i].audioCue.Stop(AudioStopOptions.Immediate);
                }
            }
        }

        /// <summary>
        /// Pauses all playing sounds
        /// </summary>
        public void PauseAll()
        {
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].isPlaying)
                {
                    audioItem localvar;
                    localvar.isPaused = true;
                    localvar.isPlaying = false;
                    localvar.audioCue = m_gameAudio[i].audioCue;

                    m_gameAudio[i] = localvar;

                    m_gameAudio[i].audioCue.Pause();
                }
            }
        }

        /// <summary>
        /// Unpauses all paused sounds
        /// </summary>
        public void UnpauseAll()
        {
            for (int i = 0; i < m_gameAudio.Count; i++)
            {
                if (m_gameAudio[i].isPaused)
                {
                    audioItem localvar;
                    localvar.isPaused = false;
                    localvar.isPlaying = true;
                    localvar.audioCue = m_gameAudio[i].audioCue;

                    m_gameAudio[i] = localvar;

                    m_gameAudio[i].audioCue.Resume();
                }
            }
        }

        /// <summary>
        /// Updates Sound Engine
        /// </summary>
        public void UpdateAudio()
        {
            m_Engine.Update();
        }
    }
}
