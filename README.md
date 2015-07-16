# ResXModifier
Helper program in C# to modify C# ResX file

This program is a simple helper tool to modifier .resx file in C#. The idea is to be able to simply add, update and/or remove entries in a .resx file.

How does it work:
 1. Compile ResXModifier.exe using the Visual Studio 2010 solution ResXModifier.sln

 2. Create an XML configuration file, to configure entries to add, update and remove. An example can be found in Resources/ConfigurationFile.xml

 3. Use ResXModifier.exe to modify a .resx file: ResXModifier <resXFileToModify.resx> <ConfigurationFile.xml> <modifiedResXFile>
