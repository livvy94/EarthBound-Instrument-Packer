# EarthBound N-SPC Instrument Pack Documentation

The following is the format used for all SPC Transfer Blocks:
- Size in bytes of the data
- ARAM offset to load it into

In human-readable form, this is "Copy the next [size] bytes into [ARAM offset]." The data itself follows immediately afterwards.


Here's an example Sample Pointer Table (see below):
```
"[04 00]" //Take the next 4 bytes
"[68 6C]" //And load them into ARAM offset 6C68
"[B0 95 B9 95]" //The data
```

ARAM locations at a glance:
```
4800 - Note Data/EBM file contents
6C68 - Sample Pointer Table
6E9C - Instrument Configuration Table
95B0 to FFFF (or wherever the echo starts) - Samples
```

* * *

### SPC Transfer Block - Sample Pointer Table
COMMAND: Copy the next [size] bytes into ARAM offset `6C68`

DATA: Sample Pointer Table entries go here.

- ARAM offset of the start of the sample
- ARAM offset to loop back to (aka loop point, but relative)

This is information about the samples' locations in ARAM. This is the info you can see in EBMusEd's Instruments tab under "Samples." Each entry here corresponds with each Instrument Metadata entry below. If you want a duplicate instrument with the same sample, but different metadata, you set them to the same offsets here.


Both of these values are two hex numbers long, so each entry should be `[XX XX YY YY]` - Start and Loop

* * *

### SPC Transfer Block - Samples
COMMAND: Copy the next [size] bytes into ARAM offset `95B0` (where `Pack 05` ends)

DATA: Insert the BRR files, one after another here.

In AddMusicK-compatible files, the first two bytes are the loop point, which doesn't go here.

* * *

### SPC Transfer Block - Instrument Configuration Table
COMMAND: Copy the next [size] bytes into ARAM offset `6E9C`

DATA: Insert the Instrument Configuration Table entries here.
- Instrument Index (this starts at `1A` if you're going to pair it with `Pack 05`)
- ADSR Value 1 [Decay and Attack]
- ADSR Value 2 [Sustain and Release]
- Gain
- Tuning (Sub)
- Tuning (Multiplier)

Also known as "patches." Check the AddMusicK [hex command reference](https://bin.smwcentral.net/u/1743/hex_command_reference.html) file for an ADSR value generator, and check [this tutorial](https://www.smwcentral.net/?p=viewthread&t=92575&page=1&pid=1502895#p1502895) on SMWCentral for how to get the tuning values with BRR Player.

* * *

### End of Transfer - [00 00]