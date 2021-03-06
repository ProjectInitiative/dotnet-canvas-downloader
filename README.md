# Canvas Downloader

## Purpose

To download as much course data as possible before losing access to my university's Canvas site.

## Requirements

  - A Canvas personal access token: new access tokens can be generated under account > settings > scroll to bottom > + New Access Token
  - [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) is required to build from source only.

## Build from source

1. Clone this repository


> NOTE: In my case, the University of Texas `CANVAS_LMS_SERVER` URL would be formatted as follows: `https://utexas.instructure.com`

1. Install all of the required Python dependencies by running the following commands:

```bash
cd <DIR-OF-REPO>
```

## Usage

To run the program, run the following:

```bash
./canvas-downloader.exe
```

## Additional Information

> DISCLAIMER: **Read your University's Terms of Service before using.** 
> **I HAVE YET TO IMPLEMENT PROPER API RATE LIMITING! RUN AT YOUR OWN RISK!**
> Since the program does not make any parallel API calls, I have not experienced any API quota locks while running two instances of the program one two different computers from my university, **BUT** there is still a risk that you lock yourself out of your account and will need to contact IT!

This script is a quick and dirty implementation focusing on trying to get as much data as possible before losing access to the university Canvas site. If the program crashes or you stop it prematurely, the program should pick back up where it left off. 

### Platforms tested

  - Windows 10
  - Ubuntu 20.04


### Futurework

Currently functionality:
  - Downloads all files uploaded by professors and TAs 
  - Retains original file structure and names

Future functionality (not guaranteed):
  - Download all modules files and content
  - Retains module structure
  - Download quiz content (HTML)
  - API rate limiting
  - Fully interactive CLI application to guide the user through process
  - Cross platform port to compiled language so no "coding" experience is required


## Special Thanks

Andrew Gu