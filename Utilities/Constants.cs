// MemCard2025
// MIT License
// Copyright (c) 2025 Raymond Lou Independent Developer
// See LICENSE file for full license information.

// Utilities/Constants.cs

using System;
using System.IO;


namespace MemCard2025Creator.Utilities
{
    public static class Constants
    {
        public static class Paths
        {
            public static readonly string BASE_DIRECTORY = AppDomain.CurrentDomain.BaseDirectory;
            // public const string Resource_Root = "/MemCard-resource/";
            public static readonly string Resource_Root = BASE_DIRECTORY + "MemCard-resource/";
            public static readonly string Backup_Root = BASE_DIRECTORY + "MemCard-backup/";
            public static readonly string Media_Root = Resource_Root + "media/";
            public static readonly string Image_Directory = Media_Root + "images/";
            public static readonly string Audio_Directory = Media_Root + "audio/";
            public static readonly string Subtitle_Directory = Media_Root + "subtitles/";

            // Specific directories
            public static readonly string CONFIG_FOLDER = Path.Combine(Resource_Root, "config");
            public static readonly string RUNTIME_FOLDER = Path.Combine(Resource_Root, "runtime");

            public static readonly string MEMCARD_CONFIG = Path.Combine(CONFIG_FOLDER, "memcard2025.cfg");
            public static readonly string CARD_LIST_FILE = Path.Combine(CONFIG_FOLDER, "card-list.txt");

        }

        public static class FileExtensions
        {
            public const string IMAGE = ".JPG";
            public const string SUBTITLE = ".txt";
            public const string AUDIO = ".mp3";
        }


    }
}
