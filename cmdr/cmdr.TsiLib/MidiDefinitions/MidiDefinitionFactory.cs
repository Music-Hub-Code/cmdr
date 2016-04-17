﻿using System;
using System.Collections.Generic;
using System.Linq;
using cmdr.TsiLib.Enums;
using cmdr.TsiLib.MidiDefinitions.Base;
using cmdr.TsiLib.Utils;

namespace cmdr.TsiLib.MidiDefinitions
{
    public static class MidiDefinitionFactory
    {
        public static Dictionary<string, AMidiDefinition> CreateGenericMidiInDefinitions()
        {
            return createGenericMidiDefinitions(MappingType.In);
        }

        public static Dictionary<string, AMidiDefinition> CreateGenericMidiOutDefinitions()
        {
            return createGenericMidiDefinitions(MappingType.Out);
        }

        public static Dictionary<string, AMidiDefinition> GetInDefinitionsFromTsi(string tsiFile)
        {
            return getDefinitionsFromTsi(MappingType.In, tsiFile);
        }

        public static Dictionary<string, AMidiDefinition> GetOutDefinitionsFromTsi(string tsiFile)
        {
            return getDefinitionsFromTsi(MappingType.Out, tsiFile);
        }


        private static Dictionary<string, AMidiDefinition> createGenericMidiDefinitions(MappingType type)
        {
            Dictionary<string, AMidiDefinition> result = new Dictionary<string,AMidiDefinition>();

            KeyConverter keyConverter = new KeyConverter();

            int numChannels = 16;
            int num = 128;
            string note;
            for (int i = 1; i <= numChannels; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    note = String.Format("Ch{0:00}.CC.{1:000}", i, j);
                    result.Add(note, new GenericMidiDefinition(type, note));
                }

                for (int j = 0; j < num; j++)
                {
                    note = String.Format("Ch{0:00}.Note.{1}", i, keyConverter.GetKeyTextIPN(j));
                    result.Add(note, new GenericMidiDefinition(type, note));
                }

                note = String.Format("Ch{0:00}.PitchBend", i);
                result.Add(note, new GenericMidiDefinition(type, note));
            }
            return result;
        }

        private static Dictionary<string, AMidiDefinition> getDefinitionsFromTsi(MappingType type, string tsiFile)
        {
            Dictionary<string, AMidiDefinition> result = new Dictionary<string,AMidiDefinition>();

            try
            {
                TsiFile file = TsiFile.Load(String.Empty, tsiFile);
                if (type == MappingType.In)
                {
                    result = file.Devices.SelectMany(dev =>
                        dev.RawDevice.Data.MidiDefinitions.In.Definitions.Select(def =>
                            AMidiDefinition.Parse(dev.TypeStr, MappingType.In, def))
                    ).DistinctBy(m => m.Note).ToDictionary(k => k.Note, k => k);
                }
                else
                {
                    result
                        = file.Devices.SelectMany(dev =>
                        dev.RawDevice.Data.MidiDefinitions.Out.Definitions.Select(def =>
                            AMidiDefinition.Parse(dev.TypeStr, MappingType.Out, def))
                    ).DistinctBy(m => m.Note).ToDictionary(k => k.Note, k => k);
                }
            }
            catch (Exception)
            {
                
            }

            return result;
        }
    }
}
