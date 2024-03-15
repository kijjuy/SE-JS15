### Unsorted Notes
- CEO/Co-Founder: Darlene
- Ron not here, had another meeting
- Main contact: David, technical, program coordinator, knows basics of SQL and database
- 1000-2000 contacts per merge, takes hours
- Since COVID contacts change very frequently
- <b>Weekly meeting time will be Thursdays at noon</b>
- Database is preferred to be cloud hosted for security and backup reasons
- Frontend application is preferred to be locally installed (Note from Brandon: I think it will be much easier for it to be a web based interface. This eliminates the installation process. Can talk to client about it as an option)
- There must be an option for analysis of the data, meaning there should be graphs and charts based on the data. Unclear requirement. Questions to get more insight:
	- Does this mean that for each search term there should be a graph of some sort of data for all of the records? e.g. If searching by organization, show graph of age of contacts?
	- Should this be on all data, or should the graphing be dynamic when a search is made on the data?
	- What actual data will be represented in these graphs?

### Numbered Questions
The numbers are here to map back to the planned questions document.

1. Files are mostly in Excel files, some in csv. Should be able to handle both
2. There is a master list of contacts, but there are some auxiliary lists. They should have some way to distinguish between them.
	1. Contacts from auxiliary lists should be somehow tagged to be searchable
3. Yes, csv and xslx files should be uploadable.
4. Should be able to export database entries as csv/excel file
5. List of known columns in database:
	1. Required:
		- <b>Mainiling List </b>
		- Organization
		- first name
		- last name
		- combined name
		- title
		- address
		- city
		- province
		- postal code
		- phone #
		- home category
	1. Optional (nullable):
		- \# of beds
		- fax #
6. No sensitive health information being stored
7. Not really preferences for separating data into different tables, will talk more about it in further design
8. Yes, have levels of authorizations. 1 admin who can do anything to data, everyone else just a user who can view, maybe edit and add.
9. The main way to upload will be bulk, single record add will be used sparingly.
10. *Forgot to ask*
11. David will be maintaining the project after it has been deployed
12.  No existing database
13. Unspecified, but multiple users. One admin, and the rest lower privileged.
14. Yes, will need to work across multiple computers. Should also work for some people who work from home.
15. SM has a single office.
16. No currently running server
17. N/A
18. *Question scrapped*
19. *Forgot to ask, might be better to leave until later anyways. Also wants to be cloud based, so backup is taken care of*
20. N/A
21. They will provide a sample of data in a coming email
22. Do you want to be able to extend the data being stored (change the schema of the database)? Yes, it would be beneficial.

