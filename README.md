## A BRR import tool for use with the [EarthBound Music Editor](https://github.com/PKHackers/ebmused/releases)

Usage:

Create a folder with this structure:
```
config.txt   <-- take a look at the Examples folder to see what goes in here
Flute.brr    <-- AddMusicK-compliant BRR files
Strings.brr
asdfjkl.brr
```

Drop your folder on the EXE (or give it the right path), and it spits out a CCScript file.

Compile this into a CoilSnake project, and the ROM will have the custom instruments automatically inserted and repointed for you!

Also planned - insertion of EBMusEd sequence data to overwrite song packs!