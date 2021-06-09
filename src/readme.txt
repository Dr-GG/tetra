Tetra (mulTimEdia daTe aRrAnger) is a small .NET Core console application that moves/copies multimedia files into date categorised folders.

For example, it will move the following files into the following folders with the default settings:

IMG_20201105_141850.jpg => 202101 - Jan 2021
IMG_20201105_143335.jpg => 202101 - Jan 2021
IMG_20200705_113031.jpg => 202007 - Jul 2020
IMG_20200708_182228.jpg => 202007 - Jul 2020
IMG_20200223_155931.jpg => 202002 - Feb 2020
IMG_20200224_163037.jpg => 202002 - Feb 2020

It determines what date it belongs to by using the following simple algorithm:

1. Does the filename contain one of the following timestamp patterns:
	a) yyyyMMdd - for example 20200101
	b) yyyy-MM-dd - for example 2020-01-01
	c) yyyy_MM_dd - for example 2020_01_01

   If it does contain one of the above patterns, that is the tentative timestamp of the file. 
   If the tentative timestamp falls between the minimum and maximum allowed date taken properties, then it is the timestamp of the file.
   If it does not contain one of the above patterns, or it does not fall between the minimum and maximmum allowed date taken properties, it proceeds to step 2.

2. If the file has the 'date taken' EXIF property, it uses the EXIF property.
   If the file has no 'date taken' EXIT property, it uses the creation date of the file.

   See https://en.wikipedia.org/wiki/Exif for more technical specifications of EXIF.

The execution behaviour of Tetra can be adjusted in the appsettings.json file. 
The following properties can be adjusted in the appsettings.json file

{
  Launch: {
    MinimuAllowedDateTaken: "2000/01/01"     // The minimum allowed date for deriving 
    MaximumThreadSize: 4,                    // Specify the number of concurrent threads to run on moving multimedia files.
    MoveFiles: true,                         // true if multimedia files should be moved, otherwise false to indicate it should be copied only.
    ErrorLogPath: "\\",                      // The path to output any error logs to.
    Destination: "{your-destination-path}",  // The path to move/copy multimedia files to.
    DestinationPattern: "yyyyMM - MMM yyyy", // The ISO 8601 format to classify output folders to.
    IncludeSubFolders: true,                 // true if sub-folders should be included when moving/copying files.
    Sources: [                               // The collection of folders to copy/move multimedia files from.
      D:\\Backup\\FileArranger\\Test
    ],
    FilterSourceFileExtensions: [            // The extension multimedia files to be used. Removing this property will include all files.
      .jpg,
      .jpeg,
      .png,
      .avi,
      .mp4
    ]
  },

  Notification: {
    ProcessRefreshInterval: 50               // The number of multimedia files to be processed before indicating progress in the console.
  }
}