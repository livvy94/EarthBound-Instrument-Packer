# EBMusEd tutorial ideas:

## Notes
Note lengths: [length in ticks] [optional staccato & volume descriptor]

Example: [18]

Example: [18 7F] <-- sets the note length to 18, and sets the release time and note volume (separate from channel volume!) to full

Example: [18 0F] <-- sets the note length to 18, and sets the release time to 0 (makes it really staccato) and sets the volume to full

(Show an example of the volume stuff used to create variation)

Tip - Start with 06, then compress as you go with PK Piano

To insert notes, either use the keyboard or PK Piano.

C9 - Rest

C8 - Continue previous note

### Subroutines
Creating subroutines

Syntax - *0,2 means subroutine 0, repeat it 2 times

Make sure each channel ends on the same tick, otherwise it glitches up in-game!

Don't use Ctrl-S - It saves, then thinks you're typing S and inserts a note

## Patterns
Add patterns

Insert patterns

Delete patterns - WATCH OUT

Repeat & Repeat Position

## Effects

Everything here can be found in the Code List

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

[FA instrument] - Set first drum instrument (should get its own video)

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

[F4 finetune] - Set finetune

[FB ?? ??] - Unknown

[FC] - Mute channel (debug code, not implemented)

[FD] - Fast-forward on (debug code, not implemented)

[FE] - Fast-forward off (debug code, not implemented)

## The Set First Drum effect
[FA instrument] - Set first drum

This effect controls what instruments the CA-DF notes are.

These are special notes that automatically set the instrument and play a C-4.

(Show an example)

## Exporting and re-importing your songs

File -> Export Song

Don't forget to put what instrument packs you used in the filename!

File -> Import Song

Make sure the SPC Packs tab stuff and the BGM list tab stuff is correct

## SPC Packs tab

oh jeez

## Custom Instruments
[Pinci's tutorial](https://www.smwcentral.net/?p=viewthread&t=101960)

How to use EBInstPack
