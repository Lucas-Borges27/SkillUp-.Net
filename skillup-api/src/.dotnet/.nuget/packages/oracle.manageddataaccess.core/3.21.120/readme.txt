Oracle.ManagedDataAccess.Core NuGet Package 3.21.120 README
===========================================================
Release Notes: Oracle Data Provider for .NET Core

September 2023

This provider supports .NET 6 & .NET 7.


Bug Fixes since Oracle.ManagedDataAccess.Core NuGet Package 3.21.110
====================================================================
Bug 32214059 - WRONG DATA RETURNED WITH ILFS=-1 WHEN USING CALLING ORACLEDATAREADER.GETCHARS TWICE
Bug 34248388 - BULKCOPY: ROWSCOPIED RESETS TO ZERO EVERY TIME NOTIFYAFTER OCCURS
Bug 34873260 - WEB APPLICATION CREATING MORE INACTVIE SESSION IN 19C
Bug 35206617 - SHOWING TIME ZONE DIFF WHILE FETCHING GMT DATA
Bug 35530163 - NULLREFERENCEEXCEPTION WHEN LONG RUNNING QUERIES ARE EXECUTED
Bug 35535322 - CQN WITH TWO ORACLEDEPENDENCY OBJECTS CAUSES QUERY NOTIFITICATIONS TO BE SENT ONLY FOR ONE OF THEM
Bug 35606793 - PERFORMANCE ISSUES WHEN USING "USER ID=/" IN THE CONNECTION STRING
Bug 35658128 - ORACLEBULKCOPY FAILS WITH ORA-20022: RELEASE LOCK FAILED
Bug 35715019 - USING SOURCE_ROUTE LEADS TO INABILITY TO FIND TARGET HOST
Bug 35761459 - DEPENDENCY ON SYSTEM.DIRECTORYSERVICES.PROTOCOLS 6.0.1 CAUSES LOADING ISSUES
Bug 35778668 - HANG OBSERVED WHEN CALLING ORACLECONNECTION.STATE WHILE SQLS ARE EXECUTED USING THE SAME CONNECTION


Known Issues and Limitations
============================
1) BindToDirectory throws NullReferenceException on Linux when LdapConnection AuthType is Anonymous

https://github.com/dotnet/runtime/issues/61683

This issue is observed when using System.DirectoryServices.Protocols, version 6.0.0.
To workaround the issue, use System.DirectoryServices.Protocols, version 5.0.1.

 Copyright (c) 2021, 2023, Oracle and/or its affiliates. 
