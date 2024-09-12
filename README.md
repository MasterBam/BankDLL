# BankDLL

### Structure:
Data Tier:
- BankLib => the library for the database methods
- BankServer => the data server

Business Tier:
- BankBusinessTier => the project of the business tier server

Clients:
- Delegate client => BankClientDelegate project (task 2)
- Async client => BankAsyncClient project (task 3)

### Logging (Task 4):
Log methods in the BusinessTier, interface implementation (BusinessServer.cs)
Log file in the initial directory of BankDLL (same directory as README.md file)
Not accessible for clients.

When logging begins, it overwrites the previous execution of the solution start-up and starts a fresh empty log entry file.

Tracks:
- Location => where it occurred
- Error => what type of error occurred
- Date
- Time

Different custom faults includes:
- IndexOutOfRange => when user enters an index beyond the scope of entries
- InvalidIndexError => when user enters illegal chars for index (leading zero entry/!number entry)
- InvalidSearch => when user enters chars besides alphabet to search via lastname
- SearchNotFound => when the user's entered name is not found in database

Other generic exceptions include:
- ArgumentNullException => when entered parameters passes through a null value


### Startup Configuration:
Currently when executed it will concurrently run both servers (data + business) and both clients.