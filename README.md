## A BRR import tool for use with the [EarthBound Music Editor](https://github.com/PKHackers/ebmused/releases)

Usage:

Create a folder with this structure:
```
instruments.txt   <-- contains AddMusicK-style instrument definitions
Flute.brr
Strings.brr
asdfjkl.brr
```

Run the tool and point it to this folder, and it spits out a CCScript file.

Compile this into a CoilSnake project, and the ROM will have the custom instruments automatically inserted and repointed for you!
