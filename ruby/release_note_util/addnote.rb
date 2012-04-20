# This uses the clipboard gem: `gem install clipboard`, `gem install ffi`

require 'fileutils'
require 'clipboard'

note = gets.chomp
if note.empty?
	puts 'Cancelled'
	exit
end

original_filename = './docs/release_notes.txt'
temp_filename = original_filename + '.tmp'
old_filename = original_filename + '.old'

FileUtils.rm temp_filename if File.exists? temp_filename
FileUtils.rm old_filename if File.exists? old_filename
FileUtils.cp original_filename, old_filename

notes = File.open(old_filename, 'r').lines

File.open(temp_filename, 'w') do |temp_file|
	notes.take(3).each do |l|
		temp_file.puts l
	end

	temp_file.puts '- ' + note

	notes.each do |l|
		temp_file.puts l
	end
end

FileUtils.rm original_filename
FileUtils.mv temp_filename, original_filename

Clipboard.copy note

puts 'Added note and copied to clipboard'