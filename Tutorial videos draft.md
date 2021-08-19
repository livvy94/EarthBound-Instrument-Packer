# EBMusEd tutorial ideas:
## Welcome!
Hello! This tutorial series will come to you in multiple parts in this playlist. Click here to go to it!

As time goes on, I'd like to swap in new versions of the videos in here if need be. I'm imagining someone in the comments asking a good question, or me from the future realizing that something could have been worded in a way that's easier to understand.

Well, I hope you enjoy!


## How Audio on the SNES works
The SNES has eight channels, which means that eight voices can be playing at once, including sound effects which game engines usually reserve for the last two.

The samples are small, carefully-made waveforms, compressed into BRR files, and loaded into audio memory.

The sound engine, pattern data, and the echo buffer shares the same block of memory, and if you use too much echo, it eats your instruments and sounds like this.

Making BRRs will be covered in a future video.

## Note Lengths
One of the worst hurdles to learning EBMusEd is understanding the concept of note lengths.

In trackers, everything is the same length, and in MIDI you just don't worry about stuff like that.

Here, you have to specify note lengths. It's not too bad once you get used to it though.

The format of a note length command is `[number of ticks] [optional release time & volume descriptor]`

One example is `[18]`

Another example is `[18 7F]`. This sets the note length to 18, and sets the release time and note volume (which is a separate thing from channel volume!) to full

Another example is `[18 0F]`. This sets the note length to 18, and sets the release time to 0 (makes it really staccato) and sets the volume to full.

Changing the optional number can make things sound a little more human, or interesting.

(example)

A big part of what I do is starting with a small value like 06, then compressing as I go.

To compress note lengths, add them up with a hex calculator. Every windows computer comes with one, press start and type Calculator. Then click the menu and choose Programmer mode.

To compress these four notes, multiply the length of 06 by four, to get 18!

(continue with the rest of the channel)

As you can see, once you set the note length, each note afterwards will play at that length.

## Inserting Notes

To insert notes, either use the keyboard or PK Piano.

(examples of both)

PK Piano is a basic tool I made that gives you auditory feedback and copies the hex code into your clipboard so you can paste it in.

You can also use a MIDI keyboard if you own one.

C9 - Rest

C8 - Continue previous note

Don't use Ctrl-S - It saves, then thinks you're typing S and inserts a note. Oops!

### Subroutines
To create a subroutine, (show the menu dropdown)

Here's an example of the syntax you use when referring to subroutines in the box: *0,2 

This means "repeat subroutine 0 two times"

This is extremely useful for compressing drum stuff that repeats a lot, or repeated melodies.

(make an echo channel)

Make sure each channel ends on the same tick, otherwise it glitches up in-game!

Whenever I have to deal with this problem, I usually copy all of the notes that are in the subroutine, paste them here, and just manually shorten the last note so it doesn't go over.

## Patterns
The `Add` button adds a new pattern.

The `Insert` button inserts a duplicate of the currently-selected pattern.

The `Delete` button deletes all instances of the currently-selected pattern - WATCH OUT!

- This is not how these kinds of programs behave, and it's easy to accidentally lose a lot of work if you don't save often (which of course you should).

`Repeat` is the number of times you want the song to repeat. `255` is the value you usually want, it repeats infinitely.

`Repeat position` is the frame you want it to jump back to. Some of EarthBound's songs like to set global volume and instruments and such in an extremely short first frame, and then jump back to the second frame. The reason for this is that in-game, you can slowly fade out the volume (like at the start of a cutscene), and if one of these fades is happening and it hits the global volume command, it cancels out the fade, but the song still abruptly stops.

Make sure each channel in a pattern is exactly the same length, like I said in the subroutines video.

## Effects

Everything here can be found in the Code List. I reccomend making your own code list with the effects you use the most!

### Common effects
[E7 tempo] - Set tempo

[E5 volume] - Set Global Volume

[E0 XX] - Set current instrument

[E1 XX] - Panning

[ED volume] - Set channel volume

[E3 start speed range] - Vibrato

[E4] - Vibrato Off

[EB start speed range] - Tremolo

[EC] - Tremolo off

[FA instrument] - Set first drum instrument (this one probably deserves its own video)

### Portamento effects
[F1 start length range] - Portamento on (note -> note+range)

[F2 start length range] - Portamento on (note-range -> note)

[F3] - Portamento off

[F9 start length note] - Note-based portamento

### Delay effects
[F5 channels lvol rvol] - Delay 1

[F7 delay feedback filter] - Delay 2

[F6] - Delay off

### Sliding effects
[EE time volume] - Slide channel volume

[E2 time panning] - Slide channel panning

[E8 time tempo] - Slide tempo

[F8 time lvol rvol] - Slide echo volume

[E6 time volume] - Slide global volume

### Uncommon effects
[EA transpose] - Channel transpose

[E9 transpose] - Global transpose

[F4 finetune] - Set finetune - only useful for BRRs where you messed up the tuning by a tiny amount and don't want to go back and fix

[FB ?? ??] - Unused. It takes those two numbers and doesn't do anything.

[FC] - Mute channel (debug code, not implemented)

[FD] - Fast-forward on (debug code, not implemented)

[FE] - Fast-forward off (debug code, not implemented)

(show an example of each of these commands working)

## The Set First Drum effect
[FA instrument] - Set first drum

This effect controls what instruments the CA-DF notes are.

These are special notes that automatically set the instrument and play a C-4.

(Show an example)

## Exporting and re-importing your songs

File -> Export Song

- This saves a project file - the raw data that gets saved in the game
- Don't forget to put what instrument packs you used in the filename! It's a pain to import note data saved like this and then realize you've forgotten what what instruments go with it.

File -> Import Song

Make sure the SPC Packs tab stuff and the BGM list tab stuff is correct

## SPC Packs tab

oh jeez

## Making Your Own BRRs
- Decide what you want to record, and record it in your favorite sound recorder
- Save it as a WAV file
- Go into OpenMPT and load the WAV file as a sample
- Make sure it's tuned to C if it's pitched (not percussive)
- Downsample it to 32000 hz or 16000 hz, whatever the lowest is that you can get away with without sounding like garbage.
- Set loop points
- Save it as a new WAV file
- Convert this WAV file to a BRR file
  - Open the [C700 VST](http://picopicose.com/software.html) and drop the WAV file into it
  - Make sure it doesn't sound glitchy
  - Click the Save Sample button
  - Alternately, you can use [BRRTools](https://www.smwcentral.net/?p=section&a=details&id=17670), but I've never used it before and you have to put the loop point in the command line arguments
  

## Making Custom Instrument Packs
[Pinci's tutorial](https://www.smwcentral.net/?p=viewthread&t=101960)

How to use EBInstPack

1. Make custom instrument pack CCS files
   - Get all the BRRs you want
      - convert WAVs or drag over from a SMWCentral dump
   - Make your config.txt
      - tune with BRR player and Audacity's sine wave generator
      - alternately, tune with the sample playing back in OpenMPT
      - alternately, a lot of SMWCentral folks have the decency to include the configuration info in the BRR packs they make
1. Compile the CoilSnake project, resulting in a Composition ROM. Copy this somewhere, and make another copy of it and rename it Reference.smc.
1. Open the Composition ROM in EBMusEd and make the songs, then export each of them as .ebm files
1. Import the .ebm files into the Base ROM. They're not going to sound right until...
1. You compile the CoilSnake project again. The base ROM contains the note data, and the CCS files contain the instrument packs. It should work...

Obstacles:
If you use the ROM CoilSnake outputs as the ROM you're making a song in, then it'll get wiped when you compile the project again. This is why it's important to copy it somewhere else, so the composition stuff happens in its own seperate ROM.