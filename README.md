# Flare-Music-Player

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Author](https://img.shields.io/badge/Author-I--SpiTfire--U-181717?logo=github)](https://github.com/I-SpiTfire-U)

> A basic music player that runs in the terminal and has the ability to display thumbnails in Kitty.

![Flare-Music-Player-Example](res/README/Flare-Music-Player.png)

## Building and Running

**1.** Clone the Repository

```sh
git clone https://github.com/I-SpiTfire-U/Flare-Music-Player
cd Flare-Music-Player
```

**2.** Build and Run

```sh
dotnet build
dotnet run
```

**3.** Publish a Standalone Binary

Select the runtime ID for whichever platform you are building for, a list of available runtime ID's can be found here: [RID Catalog](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)

```sh
dotnet publish -c Release -r <RID> --self-contained -o out
```
