RunSqlRun
=========

Runner of sql, slayer of dragons.

I got tired of opening bloated applications for running sqls on databases, or at least running installations for clients to get the console utilities.

The tests that I needed to write for my other project - Tumblelog (https://github.com/fallenprogrammr/tumblelog/) where I show an end to end testing scenario including setting up the database, etc required something lightweight that could be integrated which will execute .sql files / sql commands without a lot of ceremony.

The model I strived for this application was like a set of pluggable runners (1 for each vendor) which can be loaded and executed at will, these runners will be low ceremony (no ceremony = awesome) to setup, ideally these should run by doing a xcopy deploy.

The vendor runner plugins are based on the IDatabaseRunner interface, which resides in the Runner assembly, the runner project also has a Core class having utlity functions like extracting sql queries from files and loading vendor runner assemblies dynamically.

I also have a sample console application in which the vendor dlls can be loaded and used to run sql / .sql files from the command line.

The ConsoleTests application will demonstrate the usage within an application code.

The solution has Informix, Oracle, SqlServer and SqlServerCE runners.

The InformixBinaries directory contains the files required for Informix client to run without an explicit installation.

You can copy the structure in the InformixBinaries directory to any directory and set it up from that location by executing SetupClient.cmd file.

1) cd to the directory where the InformixBinaries structure is copied to / you want the InformixBinaries to exist, these can be commonly shared between other instances using this method of connecting to Informix.
2) execute SetupClient.cmd file.
    The cmd file will add a set of registry entries (administrator privileges required).
    It sets a path entry to the location it is running from.
    It sets an environment variable to point to the location its running from.
3) The en_us\ibm.data.informix.xml is required in your application to translate message strings. For different countries + languages, include the respective xml.
4) msg\en_us in the informixbinaries directory also follows the same pattern as above, replace with your respective language directory.


The oracle client driver setup is notorious and frustrating at times, even if the .net development libraries can be xcopy deployed, the client can still be painful to manage.

Scavenging through the various efforts of others who have shared their findings, 
it all comes to the following set of dlls required in addition to the reference to the Oracle.DataAccess library.

You can replace the files with your respective version, only the version number should be changing in the names.

oci.dll
ociw32.dll
orannzsbb11.dll
oraocci11.dll
oraociei11.dll
oraociicus11.dll
OraOps11w.dll

The oraocieixx.dll file is very large, its about 120MB for the v11 release, surpassing the github limit of 100MB for files.

So, the binaries now exist in a zip - OracleBinaries.


SqlServer and SqlServerCE support is built into the .net framework, requiring no extra assemblies / dlls.
