using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace cs
{
    /// <summary>
    /// System for handling game events and data, especially data for complex intereactions that needs to persist throughout the duration of the game
    /// </summary>
    public static class DataHandlerSystem
    {
        public static Dictionary<string, DataHandler> MilestoneEvents = new Dictionary<string, DataHandler>();

        public static void saveData()
        {
            foreach(KeyValuePair<string, DataHandler> keyValuePair in MilestoneEvents)
            {
                string Key = keyValuePair.Key;
                DataHandler Value = keyValuePair.Value;

            }
        }

        public static void loadData()
        {
            //story event
            string newKey = "chapter.1.scene.1.event=introduction.occured.bool";
            DataHandlerBool newValue = new DataHandlerBool(false);
            MilestoneEvents.Add(newKey, newValue);

            //puzzle trigger event
            newKey = "house.floor.topFloor.event=labarinthPuzzle.firstButton.bool";
            newValue = new DataHandlerBool(false);
            MilestoneEvents.Add(newKey, newValue);

            //active area effect
            newKey = "gameState.fieldEffect=currentlyActiveEffect.string";
            DataHandlerString newerValue = new DataHandlerString("Snow");
            MilestoneEvents.Add(newKey, newerValue);
        }

        public static void exampleFunction()
        {
            if (!MilestoneEvents.ContainsKey("FirstEvent"))
            {
                MilestoneEvents.Add("FirstEvent", new DataHandlerInt(null));
            }
            DataHandler test = MilestoneEvents["FirstEvent"];
            string eventType = test.GetMilestoneEventInformationType();
            if (eventType == "int?")
            {
                DataHandlerInt milestoneEvent = (DataHandlerInt)test;
                if (milestoneEvent.eventInformation != null)
                {
                    int someInt = (int)milestoneEvent.eventInformation;
                }
            }

            if (!MilestoneEvents.ContainsKey("SecondEvent"))
            {
                MilestoneEvents.Add("SecondEvent", new DataHandlerString(""));
            }
            DataHandler test2 = MilestoneEvents["SecondEvent"];
            string eventType2 = test.GetMilestoneEventInformationType();
            if (eventType2 == "string")
            {
                DataHandlerString milestoneEvent = (DataHandlerString)test;
                if (milestoneEvent.eventInformation != null)
                {
                    string someString = milestoneEvent.eventInformation;
                }
            }

        }

        public class DataHandler
        {
            public virtual string GetMilestoneEventInformationType()
            {
                return "";
            }
        }

        public class DataHandlerBool : DataHandler
        {

            string milestoneEventInformationType = "bool?";
            public bool? eventInformation = null;
            public DataHandlerBool(bool? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(bool? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            bool? getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }

        public class DataHandlerInt : DataHandler
        {

            string milestoneEventInformationType = "int?";
            public int? eventInformation = null;
            public DataHandlerInt(int? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(int? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            int? getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }

        public class DataHandlerString : DataHandler
        {

            string milestoneEventInformationType = "string";
            public string eventInformation = "";
            public DataHandlerString(string eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(string eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            string getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }

        public class DataHandlerChar : DataHandler
        {

            string milestoneEventInformationType = "char?";
            public char? eventInformation = null;
            public DataHandlerChar(char? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(char? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            char? getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }

        public class DataHandlerFloat : DataHandler
        {

            string milestoneEventInformationType = "float?";
            public float? eventInformation = null;
            public DataHandlerFloat(float? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(float? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            float? getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }

        public class DataHandlerDouble : DataHandler
        {

            string milestoneEventInformationType = "double?";
            public double? eventInformation = null;
            public DataHandlerDouble(double? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            void setEventInformation(double? eventInformation)
            {
                this.eventInformation = eventInformation;
            }

            double? getEventInformation()
            {
                return eventInformation;
            }

            public override string GetMilestoneEventInformationType()
            {
                return milestoneEventInformationType;
            }
        }
    }


    
    
}
