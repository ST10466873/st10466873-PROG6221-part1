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

        private void DisplayAsciiArt()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   _____       _               _____ecurity
  / ____|     | |             |  _ \       | |
 | |    _   _ | |__   ___ _ __| |_) | ___  | |_ 
 | |   | | | || '_ \ / _ \ '__|  _ < / _ \ | __|
 | |___| |_| || |_) |  __/ |  | |_) | (_) || |_ 
  \_____\__, ||_.__/ \___|_|  |____/ \___/  \__|
         __/ |                                  
        |___/                                   
            ");
            Console.ResetColor();
        }

        private void GreetUser()
        {
            PrintWithTypingEffect("--------------------------------------------------", ConsoleColor.DarkGray);
            PrintWithTypingEffect("Welcome to the Cybersecurity Awareness Bot!", ConsoleColor.Green);
            PrintWithTypingEffect("--------------------------------------------------", ConsoleColor.DarkGray);

            Console.Write("\nTo get started, please enter your name: ");
            userName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "User";
            }

            PrintWithTypingEffect($"\nHello, {userName}! I'm here to help you stay safe online.", ConsoleColor.Cyan);
        }