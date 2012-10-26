require 'fileutils'
require 'win32ole'

class MigrateDB
	PathToMigrations = 'upsql/'
	MigrationsFilePattern = "*.sql"
	DefaultDatabase = 'default_database_name'
	DefaultServer = '.\\SQLEXPRESS'

	def execute()
		puts 'MigrateDB (CC BY-SA 3.0) Software by Ben Pty Ltd - swxben.com'
		puts ''
		self.getConfigurationFromUser()
		puts ''
		
		puts 'Running bootstrap'
		self.doMigration("#{PathToMigrations}bootstrap.sql")
		
		puts "Initial migration level: #{self.getCurrentMigrationLevel()}"
		
		puts 'Starting migrations'
		pattern = PathToMigrations + MigrationsFilePattern
		migrationLevel = self.getCurrentMigrationLevel
		Dir[pattern].select{|f| not f['_'].nil?}.sort.each do |f|		
			baseName = File.basename(f)
			fileLevel = baseName.split('_')[0].to_i
			description = baseName.gsub('_', ' ').split('.')[0]
			
			if fileLevel > migrationLevel
				puts "Applying migration #{fileLevel}: #{description}"
				self.doMigration(f)
				migrationLevel = self.getCurrentMigrationLevel
			end
		end		
		puts 'Completed migrations'
		
		puts 'Current migration level: ' + self.getCurrentMigrationLevel().to_s
	end
	
	def getConfigurationFromUser()
		print "Server [#{DefaultServer}]: "
		@server = $stdin.gets.chomp
		@server = DefaultServer if @server == ''
		
		print "Database [#{DefaultDatabase}]: "
		@database = $stdin.gets.chomp
		@database = DefaultDatabase if @database == ''
		
		print 'Username [sa]: '
		@username = $stdin.gets.chomp
		@username = 'sa' if @username == ''
		
		print 'Password []: '
		@password = $stdin.gets.chomp
	end
	
	def doMigration(filename)
		cmd = "osql -S #{@server} -U #{@username} -P #{@password} -d #{@database} -i \"#{filename}\""
		#puts cmd
		puts `#{cmd}`
	end
	
	def getCurrentMigrationLevel()
		data = self.executeQuery("SELECT ISNULL(MAX(Migration), -1) FROM DBMigrations")
		return -1 if data.empty?
		return data.first.first
	end
	
	def executeQuery(sql)
		connectionString = "Provider=SQLOLEDB.1;Data Source=#{@server};Initial Catalog=#{@database};User ID=#{@username};Password=#{@password}"
 		con = WIN32OLE.new('ADODB.Connection');
		con.Open(connectionString)

		recordset = WIN32OLE.new('ADODB.Recordset')
		recordset.open(sql, con)
		
		data = []

		begin
			recordset.MoveFirst()
			data = recordset.GetRows

			recordset.Close
			con.Close
		rescue
			data = []
		end
		
		return data
	end
end

MigrateDB.new.execute()


