# RainDrop
A youtube-dl wrapper - was originally a standalone program, but youtube-dl did so much more I ended up just wrapping it.

RainDrop is a simple Youtube/Soundcloud Audio Downloader written in C# by DarkTussin
This version wraps Youtube-DL providing additional functionality.
To Use: 

1. Select a "Bucket" (Folder) to save the "RainDrops" (Audio Files) to.
2. Enter Content in the top Textbox:
   
   a. Enter a Youtube/Soundcloud Link (contains /watch?v=)
   b. Enter a Playlist Link (contains /playlist?list=)
   c. Enter a song/playlist search (Example: pink floyd comfortably numb or ukf dubstep playlist 2014)

3. Click "Make it Rain" (Download)  to start downloading the files.

(Note: If a playlist is detected, it will automatically pull the links)
Raindrops that "Failed to Condense" (Failed to Download) will "Evaporate" (Close)

To Cancel a Download simply close the associated RainDrop.
To Cancel all Downloads simply close the main form.
(If songs are downloading, it will abort them - click again to close all the way).

All songs are downloaded with the highest quality level available.
Songs with HQ Resolutions will typically contain higher quality audio.
All songs that are downloaded will be automatically downloaded in the highest quality audio format available.
