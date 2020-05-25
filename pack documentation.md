# EarthBound N-SPC Instrument Pack Documentation

### Sample Pointer Table HEADER (4 bytes)

>Info about the range of bytes that contain info about the samples

- Size (how much space is allocated for pointers)
- RAM Offset

### Sample Pointer Table

>Info about the samples. This is the stuff you can see in EBMusEd's Instruments tab.

- Offset & Loop Point. Make as many of these as you want as long as it fits in the above header.
- Both values are two hex numbers long, so each entry should be `[XX XX YY YY]`
- `FF` padding as needed

### BRR Dump Header
>Info about the range of bytes where the BRR waveform data resides. This is the stuff in the SPC Packs tab.

>Usually `95B0` for packs that get paired with `Pack 05`

- Size (how big the block of samples is)
- Offset (Where the block is)

### BRR Dump
- Each BRR, one after another. Note down the offsets

>Delete the first byte if you're using AddMusicK-compatible BRRs, it's the loop point.

### Instrument Configuration Table Header
>Displayed in EBMusEd's Instrument tab
- Size
- Offset

### Instrument Configuration Table
- Instrument Number (this is `1A` if you're going to pair it with `Pack 05`)
- ADSR Value 1 [Decay and Attack]
- ADSR Value 2 [Sustain and Release]
- Gain
- Tuning (Sub)
- Tuning (Multiplier)

>Check this [tutorial](https://www.smwcentral.net/?p=viewthread&t=92575&page=1&pid=1502895#p1502895) on SMWCentral for more info about getting the tuning values for a BRR file.