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