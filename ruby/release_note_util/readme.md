Use it by running `addnote.rb` in the console, then type the release note and press enter. If you don't enter anything and just press enter it cancels the add.

Release notes file is hard coded to `./docs/release_notes.txt` and it makes a backup called `release_notes.txt.old`. It writes the note after writing the first 3 lines (existing header - title, blank line then version in dev ;-) )