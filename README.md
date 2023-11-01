# JobManager

Welcome to the JobManager manual!

Database Schema
The database consists of two tables:
1. Job - which stores all the jobs created by the user to run as background jobs. The table consists of fields such as 'Name' which is used to store the job endpoint, and 'Interval' which tells the application the interval that the job should execute.
2. WeatherForecast - this table stores the returned data after a user has registered the WeatherForecast API as a background job. The application runs the job and stores the data and in the correct format in the database.

Application Instrictions
1. To build the application, run the (docker-compose build) command in the working directory.
2. To spin up the solution, run the (docker-compose up) command. Wait for a few seconds, you should now be able to open up your browser and head to http://localhost:8080/swagger and see your API running smoothly.