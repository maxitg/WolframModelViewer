# Wolfram Model Viewer

This repository is a Unity app which allows one to view graphs corresponding to states of the
[Wolfram Model](https://www.wolframphysics.org).

It's currently in the early stages of development.
The initial version here was developed by a Wolfram engineer (not me).
We would like to make further progress on it, but we don't quite have the resources to do it ourselves, so we are 
open-sourcing it in a hope someone would like to contribute.

The original version has a dependency on
[Modern UI Pack](https://assetstore.unity.com/packages/tools/gui/modern-ui-pack-150824), which we cannot open-source,
and thus it was removed from the project.
You can see some error messages because of that.
We will need to replace it with another open-source asset.

## Getting Started

To run the app, open it from the Unity Hub, then select `Wolfram -> Set iOS AR Build`, open the "Game" tab, and click
play.

The graph from `Resouces/Models/graph.xml` will load.
To load another one, generate another XML file by running the following in Wolfram Language:

```wl
Export[".../WolframModelViewer/Assets/Resources/Models/graph.xml", Graph[...], "GraphML”]
```

You can rotate the graph by clicking and dragging, and zoom in/out by holding the right mouse button and dragging.
You can also drag individual vertices around.

## Configure Unity for source control

With your project open in the Unity editor:

Open the editor settings window: "Edit > Project Settings > Editor"

Make .meta files visible to avoid broken object references: "Version Control / Mode: Visible Meta Files"

Use plain text serialization to avoid unresolvable merge conflicts: "Asset Serialization / Mode: Force Text"

Save your changes: "File > Save Project"

This will affect the following lines in your editor settings file:

```
ProjectSettings/EditorSettings.asset

m_ExternalVersionControlSupport: Visible Meta Files

m_SerializationMode: 2
```
