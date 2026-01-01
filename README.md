## Sammi Duckrace
Quick Programm Overview
<img width="4807" height="1355" alt="grafik" src="https://github.com/user-attachments/assets/e9759d9e-e955-49e0-a721-e2f77d1293ba" />

### Intro
This is a Program that utilises Spout2 to stream a viewport to OBS or any other reciver. 

#### Licenses
It utilises this wrapper https://github.com/you-win/spout-gd and of course Spout 2 https://github.com/leadedge/Spout2. The C# Bindings were generated with "CSharp-Wrapper-Generator-for-GDExtension" at https://github.com/Delsin-Yu/CSharp-Wrapper-Generator-for-GDExtension.
The Spout2 & Spout GD Binaries are checked into this project, their licenses apply to them as appropiate, copys of their code licenses are present in the license folder. Names and Images are for testing convenience and held by their respective license holder, only code written by me or by contributers is licensed under [WTFPL v2](https://github.com/Nickname863/DuckRace-Sammi/blob/master/Licenses/DuckRace/License.md).
### Setup

1. Install The Deck from the SAMMI Folder in SAMMI.
2. Copy "duckRaceParticipants.ini" to a location of your choice and adjust the path in the SAMMI Deck.
3. Copy ALL contents of the "Executable" Folder to a location of your choice and adjust the Path in the Deck.
4. Open The "!race" Button. Open OBS.
5. Ensure that the Sammi local API is enabled in the Sammi Settings.

<img width="922" height="577" alt="grafik" src="https://github.com/user-attachments/assets/477b5006-2f72-423e-8741-f403fbceebb7" />

6. Configure The Scenes in SAMMI to your linking. And run the button.
7. DuckRace.exe will start and minimize itself into the system Tray (you might need to open and close "DuckRace.exe" yourself once because of windows security)
8. If You want to close the application select quit in the System Tray

<img width="227" height="84" alt="grafik" src="https://github.com/user-attachments/assets/d72832ef-f0c4-4ccf-a7dc-e18ba0449539" />

9. Alternatively maximise the application by left clickin the Icon in the system Tray. hold down the Shift Key and click on the X, without shift the application will only minimize.

### How to Build

Download the .Net Build of Godot 

<img width="715" height="389" alt="grafik" src="https://github.com/user-attachments/assets/b8d9bd25-c485-4eb7-9251-5a9052cf5b84" />

Import the Project into Godot (You might need to install the .net SDK, Godot will inform you)

<img width="304" height="146" alt="grafik" src="https://github.com/user-attachments/assets/99c30763-2c55-4ae5-a942-65a43e27bc69" />

Select "Export..."

<img width="394" height="417" alt="grafik" src="https://github.com/user-attachments/assets/261981f2-7426-4a88-84e7-b9712b2d8924" />

Select the WindowsDeploy Preset and press the "Export Project" Button, you will be prompted to save the binaries into a folder.

Done


### How to work with the Project.

#### Use an External IDE
Godots .Net Ediotor support is quite sub-par. Because of taht a lot of People use external IDE for development.
There are plenty to choose from, notable examples are Jetbrains Rider, Visual Studio 20xx or, if you only want a code editor Visual Studio Code.

In my Guide i will focus on Visual Studio 20xx Community Edition because i utilised it for this for this project.

#### Installation

Go to [Community Edition](https://aka.ms/vs/17/release/vs_community.exe) and download the Community Edition. You can also find downloads here https://learn.microsoft.com/en-us/visualstudio/releases/2022/release-history
We use an outdated version because of Issues with Godot integration. It doesnt really matter.

In The installer you mostly only need the .Net Desktop Development Package (sadly the Installer refuses to speak english to me)

<img width="1114" height="659" alt="grafik" src="https://github.com/user-attachments/assets/aab5ef48-9412-44ce-a47a-0814393f3500" />


The next step is up to you, but you can uninstal copilot in the Components Tab if you wish to 

<img width="757" height="266" alt="grafik" src="https://github.com/user-attachments/assets/edce4c9f-2f8e-41a0-83b5-7df062e89bb2" />

On Startig you will be propted to log in. You should skip this, if you use your Microsoft Account to log in and do not have a Profile picture it might leak your names Initials (they will be in the upper right corner when VS is running).

Important last optional step, Visual Studio will nag you with code suggestions, turn them off they are bad.

<img width="923" height="726" alt="grafik" src="https://github.com/user-attachments/assets/90e46c84-3579-43a1-b642-1b64bd2be425" />

#### Setting up external editor.

To open C# Scripts in another editor, Change the Editor Setting For the External Editor. (For Example to Visual Studio)
<img width="961" height="758" alt="grafik" src="https://github.com/user-attachments/assets/b48808a2-ab43-46cf-90bd-1eb1ccf9f9e7" />


#### Setting Up C# Debugging

To Set up the Debugging with Visual Studio, which can be extremely useful you will need to edit the launchprofile
![Animationgresd](https://github.com/user-attachments/assets/41d554c6-608e-49fd-a427-e7907ad356fa)

Everything should be preconfigured, however you have to set the Executable to your Install of the Godot Game engine.
Additionally oyu have to adjust the Project Path to whereverf your godot project file is located (the folder you importet)


<img width="865" height="307" alt="grafik" src="https://github.com/user-attachments/assets/9af6977e-638f-441b-b187-e24c1b0f42c3" />



