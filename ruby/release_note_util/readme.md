# Write to a release note file from the command line


## Usage

Use it by running `addnote.rb` in the console, then type the release note and press enter. If 
you don't enter anything and just press enter it cancels the add.

The entered note is copied to the clipboard, for your hg/git commit message pleasure. See also our AutoHotKey script for enabling `ctrl-v` to paste directly to cmd/powershell.


## Release note file

Release notes file is hard coded to `./docs/release_notes.txt` and it makes a backup called `release_notes.txt.old` before rewriting the original file. It writes the note after writing the first 3 lines (existing header - title, blank line then version in dev ;-) )


## Dependencies

[Clipboard](http://rubygems.org/gems/clipboard): `gem install clipboard`

Clipboard depends on FFI (`gem install ffi`) which requires the DevKit.


## Caveats

Only tested in Windows.