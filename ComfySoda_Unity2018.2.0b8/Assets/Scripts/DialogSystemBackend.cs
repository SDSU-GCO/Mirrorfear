using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CustomExtensions;
using System.Text.RegularExpressions;

namespace cs
{
    public static class Dialoguehandler
    {
        public static List<Parser.DialogueStructure> getSceneData(string cutscene)
        {
            StreamReader inputStream = new StreamReader("DialogFiles/"+cutscene+".txt");

            List<Parser.DialogueStructure> Script = new List<Parser.DialogueStructure>();
            while (!inputStream.EndOfStream)
            {
                Parser.DialogueStructure tempDialogueStructure;
                string speaker = inputStream.ReadLine();

                if(speaker!="")
                {
                    switch (speaker)
                    {
                        case "Child":
                            tempDialogueStructure.speaker = Parser.Speaker.PLAYER_NAME;
                            break;
                        case "Mirror Child":
                        case "MirrorChild":
                            tempDialogueStructure.speaker = Parser.Speaker.ENEMY_NAME;
                            break;
                        case "Blank":
                            tempDialogueStructure.speaker = Parser.Speaker.BLANK;
                            break;
                        default:
                            tempDialogueStructure.speaker = Parser.Speaker.ERROR;
                            Debug.Log("Error in dialog files: DialogFiles/"+cutscene+".txt");
                            break;
                    }


                    if (!inputStream.EndOfStream)
                    {
                        string dialogueLine = "";
                        while(dialogueLine=="" && !inputStream.EndOfStream)
                            dialogueLine = inputStream.ReadLine();

                        tempDialogueStructure.dialogue = Parser.parseLine(dialogueLine);
                        Script.Add(tempDialogueStructure);
                    }
                }
            }



            switch (Script[0].speaker)
            {
                //determine speaker and portraits
                case Parser.Speaker.PLAYER_NAME:
                    break;
                default:
                    break;
            }
            if(Script[0].dialogue[0].character != null)
            {
                //get character to print
                //char temp = (char) Script[0].dialogue[0].character;
            }
            if (Script[0].dialogue[0].action != null)
            {
                //perform any actions/function calls
                switch (Script[0].dialogue[0].action)
                {
                    case Parser.Action.RING_BELL:
                        break;
                    default:
                        Debug.Log("Error in dialog!");
                        break;
                }
            }

            return Script;
        }

    }

    public static class Parser
    {
        public enum Speaker { PLAYER_NAME, ENEMY_NAME, BLANK, ERROR };
        public enum Action { RING_BELL, ERROR };

        public struct ActionCharacter
        {
            public char? character;
            public Action? action;
        }

        public struct DialogueStructure
        {
            public List<ActionCharacter> dialogue;
            public Speaker speaker;
        }

        public static List<ActionCharacter> parseLine(string lineToParse)
        {
            //handle the escaped Characters
            List<ActionCharacter> dialogueLine = new List<ActionCharacter>();

            for (int i = 0; i < lineToParse.Length; i++)
            {
                ActionCharacter tempActionCharacter;
                tempActionCharacter.action = null;
                tempActionCharacter.character = null;
            

                if (lineToParse[i] == '\\')
                {
                    if (lineToParse[i + 1] == '\\')
                    {
                        tempActionCharacter.character = '\\';
                        i++;
                    }
                    else if (lineToParse.containsSubstringAtPosition("RING_BELL", i + 1))
                    {
                        tempActionCharacter.action = Action.RING_BELL;
                        i += "RING_BELL".Length;
                    }
                    else if (lineToParse.containsSubstringAtPosition("Player Name", i + 1))
                    {
                        replaceSubStringAtPosition(ref lineToParse, "Player Name", "Comfy Soda Boy", i + 1);
                    }
                }
                else
                {
                    tempActionCharacter.character = lineToParse[i];
                    dialogueLine.Add(tempActionCharacter);
                }
            }
            return dialogueLine;
        }

        static void replaceSubStringAtPosition(ref string completeString, string substringToReplace, string replacementString, int position)
        {
            string temp="";
            for(int i = 0; i<position; i++)
            {
                temp += completeString[i];
            }
            temp += replacementString;

            position += substringToReplace.Length;
            for (int i = position; i<completeString.Length ; i++)
            {
                temp += completeString[i];
            }
        }
    }
}

namespace CustomExtensions
{
    //Extension methods must be defined in a static class
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static bool containsSubstringAtPosition(this string value, string substring, int position)
        {
            string tempString = "";
            for (int i = position; i < value.Length && i < substring.Length; i++)
            {
                tempString += value[i];
            }

            bool returnValue;
            if (tempString == substring)
            {
                returnValue = true;
            }
            else
            {
                returnValue = false;
            }

            return returnValue;
        }
    }
}