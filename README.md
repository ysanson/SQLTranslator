# Welcome to SQLTranslator

## What is it?

SQLTranslator is a simple tool to convert SQL dumps from one database to another.

## What are the limitations?

Currently, this application only works for translating from MySQL 2.2 to PostgreSQL 11. It only converts table creation, row insertion and drop statements. 
It can also create enumerations.

## How can I use it?

This application is written in .NET Core 2. This environment must be installed to run this application. To run it, just go into the root folder and type `dotnet build` in a terminal. 

