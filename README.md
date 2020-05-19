## A BRR import tool for use with the [EarthBound Music Editor](https://github.com/PKHackers/ebmused/releases)

(Eventual) Usage:

Create a folder with this structure:
```
instruments.txt   <-- contains AddMusicK-style instrument definitions
Flute.brr
Strings.brr
asdfjkl.brr
```

Run the tool and point it to this folder, and it spits out a .bin file.

You can manually insert this into your ROM, or use CCScript to automatically repoint the data.
