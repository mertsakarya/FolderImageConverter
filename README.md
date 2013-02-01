FolderImageConverter
==============================

This application is a command line utility that uses "http://imageresizing.net" to apply a filter 
to a set of images in specified folder.

> fic.exe

Convert Images in folder and sub folders
----------------------------------------
Copyright (C) 2008 - 2013 - Mert Sakarya
----------------------------------------

Command line options:
  -i, --input=VALUE          Path for the folder to modify images.

  -o, --output[=VALUE]       Output folder.
                               Default:
                               [P:\GIT\FolderImageConverter\FolderImageConverte-
                               r\bin\Debug]

  -f, --format[=VALUE]       File format of output image.
                               Default: jpg, Possble values jpg|png|gif

  -m, --mask[=VALUE]         File mask for input folder.
                               Default: *.*
  -s, --settings=VALUE       Query options are described in
                               http://imageresizing.net/docs/reference
                               Do not add format setting. Specify it with
                               extension parameter on command line.
                               Example:
                               settings="width=320&height=240&quality=85&crop="
  -v, --verbose              Verbose output.
  -h, -?, --help