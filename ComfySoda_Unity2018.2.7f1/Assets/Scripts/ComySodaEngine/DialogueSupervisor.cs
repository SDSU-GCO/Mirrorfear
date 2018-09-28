using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using CustomExtensions;
using System.Text.RegularExpressions;

namespace cs
{
    public static class DialogueSupervisor
    {


        public static List<Parser.SentenceStructure> getSceneData(string cutscene)
        {
            StreamReader inputStream = new StreamReader("DialogueFiles/" + cutscene + ".txt");

            List<Parser.SentenceStructure> Script = new List<Parser.SentenceStructure>();
            while (!inputStream.EndOfStream)
            {
                Parser.SentenceStructure tempDialogueStructure;
                string speaker = inputStream.ReadLine();

                if(speaker!="")
                {
                    tempDialogueStructure.speaker = speaker;

                    if (!inputStream.EndOfStream)
                    {
                        string dialogueLine = "";
                        while(dialogueLine=="" && !inputStream.EndOfStream)
                            dialogueLine = inputStream.ReadLine();

                        tempDialogueStructure.actionCharachterlist = Parser.parseLine(dialogueLine);
                        Script.Add(tempDialogueStructure);
                    }
                }
            }
            return Script;
        }

        public static class Parser
        {
            #region DialogueStructures
            public struct ActionCharacter
            {
                public char? character;
                public string action;
            }

            public struct SentenceStructure
            {
                public List<ActionCharacter> actionCharachterlist;
                public string speaker;
            }
            #endregion

            public static List<ActionCharacter> parseLine(string lineToParse)
            {
                //create a line to return
                List<ActionCharacter> dialogueLine = new List<ActionCharacter>();

                for (int i = 0; i < lineToParse.Length; i++)
                {
                    ActionCharacter tempActionCharacter;
                    tempActionCharacter.action = "";
                    tempActionCharacter.character = null;
                    
                    //handle the escaped Characters
                    if (lineToParse[i] == '\\')
                    {
                        #region Handle Special Escape Strings
                        if (lineToParse[i + 1] == '\\')
                        {
                            tempActionCharacter.character = '\\';
                            i++;
                        }
                        else if (lineToParse.containsSubstringAtPosition("DYNAMIC_ACTION: ", i + 1))
                        {
                            tempActionCharacter.action = "DYNAMIC_ACTION: ";
                            i += ("DYNAMIC_ACTION: ".Length);

                            i++;
                            while (lineToParse[i] != '\\' && lineToParse[i + 1] != 'n')
                            {
                                if (lineToParse[i] == '\\' && lineToParse[i + 1] == '\\')
                                {
                                    tempActionCharacter.action += lineToParse[i];
                                    i++;
                                }
                                i++;
                            }
                            i++;
                        }
                        else
                        {
                            if(lineToParse.containsSubstringAtPosition("Player Name", i + 1))
                                lineToParse.replaceSubStringAtPosition("Player Name", "Timmy", i + 1);
                        }
                        #endregion
                    }
                    else
                    {
                        tempActionCharacter.character = lineToParse[i];
                        dialogueLine.Add(tempActionCharacter);
                    }
                }
                return dialogueLine;
            }
        }
    }
}

//extend string functionality
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
            //trim cuts off trailing whitespace
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string stringToStripWhiteSpaceFrom)
        {
            //passes in the string, the substring to remove(@"\s+" represents substrings of whitespace), and the replacement string
            return Regex.Replace(stringToStripWhiteSpaceFrom, @"\s+", " ");
        }

        public static bool containsSubstringAtPosition(this string stringToCheckIn, string substring, int position)
        {
            string tempString = "";
            for (int i = position; i < stringToCheckIn.Length && i < substring.Length; i++)
            {
                tempString += stringToCheckIn[i];
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

        public static void replaceSubStringAtPosition(this string completeString, string substringToReplace, string replacementString, int position)
        {
            string temp = "";
            for (int i = 0; i < position; i++)
            {
                temp += completeString[i];
            }
            temp += replacementString;

            position += substringToReplace.Length;
            for (int i = position; i < completeString.Length; i++)
            {
                temp += completeString[i];
            }
        }
    }
}