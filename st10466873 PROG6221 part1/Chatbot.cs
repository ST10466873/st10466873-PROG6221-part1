using System;
using System.Collections.Generic;
using System.Text;

namespace st10466873_PROG6221_part1
{
    public class Chatbot
    {
        private string userName;

        public void Start()
        {
            PlayVoiceGreeting();
            DisplayAsciiArt();
            GreetUser();
            ChatLoop();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.PlaySync();
            }
            catch (Exception)
            {
                PrintWithTypingEffect("[System: Audio file 'greeting.wav' not found. Please ensure it is in the correct directory.]\n", ConsoleColor.Red);
            }
        }