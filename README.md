# Memory Card 2025 (MemCard2025)

## Disclaimer

The Memory Card 2025 (MemCard2025) applications are designed for use within a private home local network environment.

To simplify its operation, The Memory Card Web Server and Viewer (MemCard2025WebViewer) uses HTTP instead of HTTPS, transferring data in plain text.

If users choose to expose the applications or their content to the Internet, they are solely responsible for implementing appropriate security measures to protect their privacy and any potential losses resulting from such exposure.

## Quick Start Guide
1. Launch `MemCard2025DesktopViewer.exe`.
2. Navigate to "Display Mode" and select "Auto Cycle".
3. The demo cards will display repeatedly.

## About the Project

This tool is designed to support cognitive and memory training, offering a valuable resource for extensive learning.
Its primary goal is to assist in preventing or slowing the progression of dementia. However, its medical efficacy has not been scientifically proven.

To customize your experience, you can adjust the "DisplayMode_AutoCycle_interval" parameter in the MemCard-resource\config\memcard2025.cfg file.

- The default interval is set to 8 seconds, suitable for the anti-dementia demonstration.
- For faster-paced applications, such as vocabulary flashcards, users can set it to 2 seconds or another preferred value.

## Applications Included
MemCard2025 consists of three applications:
1. **Memory Card Creator** (`MemCard2025Creator`): For creating and managing memory cards.
2. **Memory Card Desktop Viewer** (`MemCard2025DesktopViewer`): For viewing and interacting with memory cards on a desktop.
3. **Memory Card Web Server/Viewer** (`MemCard2025WebViewer`): For accessing memory cards via a web interface.

## Key Concepts
1. Memory cards are organized into categories.
2. Each category can hold up to 9999 cards.
3. The "Shuffle Cards" operation works within a single category.
4. Users can customize the card list and add media manually or use `MemCard2025Creator`.
5. Directory structure of the desktop applications:

   ```
   C:\MemCard2025
   │---MemCard2025Creator.exe
   │---MemCard2025DesktopViewer.exe
   ├───MemCard-backup          # Export backup zip packages here
   └───MemCard-resource        # Root directory of MemCard resource files
       ├───bgm                # Background music files
       ├───config             # Configuration files
       ├───media              # Root directory of card media
       │   ├───audio          # Card audio files (reading subtitles)
       │   ├───images         # Card image files
       │   └───subtitles      # Card subtitle text files
       └───runtime            # Runtime temporary files
   ```

## Adding Memory Cards
1. Assign a unique card name in the format:
   `<Category>-<SubNumber>-<Alias>`
   
   Example:
   ```
   food-0001-cake
   ```
   Note:    1 Alias is optional;
            2 "Add a Batch of Cards" in MemCard2025Creator uses the original image file name as both Alias and Subtitle, and generate readings of the Subtitles as card audio files.
   
2. Add the card unique name to the card list file:
   ```
   C:\MemCard2025\MemCard-resource\config\card-list.txt
   ```
3. Rename the corresponding image (.JPG), audio (.MP3), and subtitle (.TXT) files with the same unique card name and copy them to their respective media directories:
   - `C:\MemCard2025\MemCard-resource\media\images`
   - `C:\MemCard2025\MemCard-resource\media\audio`
   - `C:\MemCard2025\MemCard-resource\media\subtitles`
4. Restart the desktop viewer to view the new cards.

## Using the Desktop Viewer
- **Display Mode**: Browse through memory cards.
- **Auto Cycle**: Automatically cycle through cards repeatedly.
- **Challenge Mode**: Test your memory and get a percentage score for cards you remember.
- **Shuffle Cards**: Randomize the order of cards.
- **Load Default Sequence**: Reset the card order to the default sequence.

## Using the Web Viewer
- **Directory structure of the web applications:**
```
C:\MemCard2025\MemCard2025WebViewer\
├───htmlmemcard		# doc root of web site
│   ├───css
│   ├───icon
│   ├───js
│   ├───MemCard-resource	# the same directory as desktop viewer
│   │   ├───bgm
│   │   ├───config
│   │   ├───media
│   │   │   ├───audio
│   │   │   ├───images
│   │   │   ├───images0
│   │   │   └───subtitles
│   │   └───runtime
│   └───subpages
└───server			# start the web server from here
```
- **Users can start a Node.js server and visit the LAN web site from any devices in the same LAN.**
Install Node.js and verify installation by
```
node -v
```
install express: 
```
npm init -y
npm install express
```

- **Open a command prompt and change directory to the server dirctory:**
```
cd C:\MemCard2025\MemCard2025WebViewer\server
node C:\MemCard2025\MemCard2025WebViewer\server\server.js
```

- **Open a browser and navigate to the URI displayed in the command prompt.**
```
http://<LAN IP address>:54321
```


## Playing Background Music
1. Copy music files to:
   ```
   C:\MemCard2025\MemCard-resource\bgm\
   ```
2. Add paths to the music files in the BGM list:
   ```
   C:\MemCard2025\MemCard-resource\config\BGM-list.txt
   ```
3. Restart the desktop viewer and enable the "Background Music" checkbox to play the music in a loop.

**Note**: Adjust the volume to a comfortable level to avoid hearing issues.

## MemCard2025 configuration
You may edit the configuration file C:\MemCard2025\MemCard-resource\config\memcard2025.cfg by notepad or any other plain text editor.
- **Default content:**
```
DisplayMode_AutoCycle_interval=8                # 8 seconds interval when Auto Cycle display
BGM_list=MemCard-resource\config\BGM-list.txt   # BackGround Music play list file
```


