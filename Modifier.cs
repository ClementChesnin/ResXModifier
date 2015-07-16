using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Resources;
using System.Xml.Linq;

namespace ResXModifier
{
    internal class Modifier
    {
        private OrderedDictionary _resourceEntries;

        public Modifier()
        {
            _resourceEntries = new OrderedDictionary();
        }

        public void loadResXFileToModify(string resXFileToModify)
        {
            _resourceEntries = new OrderedDictionary();

            // Get existing resources.
            using (var reader = new ResXResourceReader(resXFileToModify))
            {
                if (reader != null)
                {
                    reader.UseResXDataNodes = true;
                    foreach (DictionaryEntry entry in reader)
                    {
                        var dnode = (ResXDataNode)entry.Value;
                        _resourceEntries.Add(dnode.Name, dnode);
                    }
                }
            }
        }

        public bool tryModify(string configurationFile)
        {
            Dictionary<string, string> valuesToModify;
            Dictionary<string, string> valuesToAdd;
            List<string> valuesToRemove;
            if (tryParseConfigurationFile(
                    configurationFile,
                    out valuesToModify,
                    out valuesToAdd,
                    out valuesToRemove))
            {
                if (!tryModifyValues(valuesToModify))
                {
                    return false;
                }
                if (!tryAddValues(valuesToAdd))
                {
                    return false;
                }
                if (!tryRemoveValues(valuesToRemove))
                {
                    return false;
                }
            }
            else
            {
                // Error.
                return false;
            }

            return true;
        }

        public void writeModifiedResX(string modifiedResXFile)
        {
            using (var resourceWriter = new ResXResourceWriter(modifiedResXFile))
            {
                foreach (DictionaryEntry resourceEntry in _resourceEntries)
                {
                    var dnode = (ResXDataNode)resourceEntry.Value;
                    resourceWriter.AddResource(dnode);
                }
                resourceWriter.Generate();
            }
        }

        private bool tryParseConfigurationFile(
            string configurationFile,
            out Dictionary<string, string> valuesToModify,
            out Dictionary<string, string> valuesToAdd,
            out List<string> valuesToRemove)
        {
            var xdoc = XDocument.Load(configurationFile);
            populateValuesToModify(xdoc, out valuesToModify);
            populateValuesToAdd(xdoc, out valuesToAdd);
            populateValuesToRemove(xdoc, out valuesToRemove);

            return true;
        }

        private void populateValuesToModify(XDocument xdoc, out Dictionary<string, string> valuesToModify)
        {
            valuesToModify = new Dictionary<string, string>();

            foreach (var modifyNodes in xdoc.Root.Elements("Modify"))
            {
                var keyAttribute = modifyNodes.Attribute("Key");
                var valueAttribute = modifyNodes.Attribute("Value");
                if ((null != keyAttribute) && (null != valueAttribute))
                {
                    valuesToModify.Add(keyAttribute.Value, valueAttribute.Value);
                }
            }
        }

        private void populateValuesToAdd(XDocument xdoc, out Dictionary<string, string> valuesToAdd)
        {
            valuesToAdd = new Dictionary<string, string>();

            foreach (var addNodes in xdoc.Root.Elements("Add"))
            {
                var keyAttribute = addNodes.Attribute("Key");
                var valueAttribute = addNodes.Attribute("Value");
                if ((null != keyAttribute) && (null != valueAttribute))
                {
                    valuesToAdd.Add(keyAttribute.Value, valueAttribute.Value);
                }
            }
        }

        private void populateValuesToRemove(XDocument xdoc, out List<string> valuesToRemove)
        {
            valuesToRemove = new List<string>();

            foreach (var removeNodes in xdoc.Root.Elements("Remove"))
            {
                var keyAttribute = removeNodes.Attribute("Key");
                if (null != keyAttribute)
                {
                    valuesToRemove.Add(keyAttribute.Value);
                }
            }
        }

        private bool tryModifyValues(Dictionary<string, string> valuesToModify)
        {
            foreach (var modifiedValueCouple in valuesToModify)
            {
                if (_resourceEntries.Contains(modifiedValueCouple.Key))
                {
                    var originalDnode = (ResXDataNode)_resourceEntries[modifiedValueCouple.Key];
                    var newDnode = new ResXDataNode(modifiedValueCouple.Key, modifiedValueCouple.Value);
                    newDnode.Comment = originalDnode.Comment;
                    _resourceEntries[modifiedValueCouple.Key] = newDnode;
                }
                else
                {
                    Console.WriteLine("Key to modify {0} not found", modifiedValueCouple.Key);
                    return false;
                }
            }

            return true;
        }

        private bool tryAddValues(Dictionary<string, string> valuesToAdd)
        {
            foreach (var modifiedValueCouple in valuesToAdd)
            {
                if (_resourceEntries.Contains(modifiedValueCouple.Key))
                {
                    Console.WriteLine("Key to add {0} already exist", modifiedValueCouple.Key);
                    return false;
                }
                else
                {
                    _resourceEntries.Add(modifiedValueCouple.Key, new ResXDataNode(modifiedValueCouple.Key, modifiedValueCouple.Value));
                }
            }

            return true;
        }

        private bool tryRemoveValues(List<string> valuesToRemove)
        {
            foreach (var valueToRemove in valuesToRemove)
            {
                if (_resourceEntries.Contains(valueToRemove))
                {
                    _resourceEntries.Remove(valueToRemove);
                }
                else
                {
                    Console.WriteLine("Key to remove {0} not found", valueToRemove);
                    return false;
                }
            }

            return true;
        }
    }
}
