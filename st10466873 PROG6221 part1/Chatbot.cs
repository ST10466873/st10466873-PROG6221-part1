using System;
using System.Collections.Generic;
using System.Text;
using System.Media;

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

        private void ChatLoop()
        {
            bool isRunning = true;

            PrintWithTypingEffect("\nYou can ask me questions about password safety, phishing, or safe browsing.", ConsoleColor.White);
            PrintWithTypingEffect("(Type 'exit' or 'quit' to leave the chat)\n", ConsoleColor.DarkGray);

            while (isRunning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n[{userName}]: ");
                Console.ResetColor();

                string input = Console.ReadLine()?.Trim().ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    PrintWithTypingEffect("CyberBot: Please type a question or prompt so I can help you.", ConsoleColor.Red);
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[CyberBot]: ");
                Console.ResetColor();

                switch (input)
                {
                    case "how are you?":
                        PrintWithTypingEffect($"I'm functioning optimally, {userName}! Ready to tackle cyber threats.", ConsoleColor.White);
                        break;
                    case "what's your purpose?":
                    case "what is your purpose?":
                        PrintWithTypingEffect("My purpose is to educate citizens on identifying and mitigating cyber threats like phishing and identity theft.", ConsoleColor.White);
                        break;
                    case "what can i ask you about?":
                        PrintWithTypingEffect("You can ask me for tips on password safety, how to spot phishing emails, and safe browsing habits.", ConsoleColor.White);
                        break;
                    case "password safety":
                    case "passwords":
                        PrintWithTypingEffect("Always use strong, unique passwords for different accounts. Consider using a passphrase and enabling Two-Factor Authentication (2FA).", ConsoleColor.White);
                        break;
                    case "phishing":
                        PrintWithTypingEffect("Phishing scams often try to create a false sense of urgency. Never click on suspicious links or download unexpected attachments.", ConsoleColor.White);
                        break;
                    case "safe browsing":
                        PrintWithTypingEffect("Ensure the websites you visit use HTTPS. Be cautious when using public Wi-Fi, and keep your browser updated to patch security vulnerabilities.", ConsoleColor.White);
                        break;
                    case "exit":
                    case "quit":
                        PrintWithTypingEffect($"Stay safe out there in the digital world, {userName}. Goodbye!", ConsoleColor.Green);
                        isRunning = false;
                        break;
                    default:
                        PrintWithTypingEffect("I didn't quite understand that. Could you rephrase? Try asking about 'phishing', 'password safety', or 'what can I ask you about?'.", ConsoleColor.White);
                        break;
                }
            }
        }

        private void PrintWithTypingEffect(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(15);
            }
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
