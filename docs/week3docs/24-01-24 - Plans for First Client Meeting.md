# Requirements From Document
Each requirement is specified as one from the set of FURPS+.
- Create database to store contacts - F
- Add new contacts through csv file - F
- Edit contacts based on csv file - F
- Add single record to database - F
- Edit single record in database - F
- Search functionality - F
- Locally installed or cloud - +
- Uses SQL relational database - +

# Current Problem for Client
Somebody currently has to manually merge/update csv files to manage large list of contacts. This takes a long time and is inefficient.

### One Sentence Design Statement
Database solution for simplifying contact management.

# List of Questions
Questions are numbered, but they do not have to be asked in that order. Numbers are just there so that when responses are written down in the middle of a meeting, they can be mapped to the original question.

1. What is the main file type used for storing contacts? (.xslx, .csv...)
3. Contacts stored in single file or multiple files?
4. App will have button to upload csv file?
5. Should database records be exportable as a csv?
6. What info are you currently storing?
7. Is there sensitive health info being stored?
8. Are there any preferences for separation of data? (employee/contact/whatever)
9. Should there be different levels of authorization for data access? (Read/write/edit/delete)
10. Will csv be the default way to upload data to the database, or will the workflow change when the app is released?
11. If the app is installed locally, will there be an IT person/team to roll it out?
12. If the app needs to be extended/is not finished at the end of the semester, who will be taking over work on it?
	1. Do they have technology preferences?
13. Is there currently any sort of database for contacts?
14. How many people will be using this application? (employees in SM)
15. Will the app need to work across multiple computers?
16. Does SM have multiple offices?
	1. Does each location share a common set of contacts or an individual list?
17. Does SM already have a server running on prem?
18. What 'level' of employee will be using the app? (i.e. Secretary/Manager/whatever)
19. Are there currently any solutions for backing up the contacts list?
	1. If there are, can we piggyback on whatever is being used?
20. Should the app be able to extend or change the data that is being stored in the database? (e.g. Change 'Phone' record to 'Home phone' and 'Mobile')
21. Would it be possible to supply us with the list of contacts? If not the whole list, then a single redacted record should be enough.
22. Can we setup a scheduled weekly meeting time instead of scheduling each week